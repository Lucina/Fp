using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace Fp.ApiBench;

public class ByteXor
{
    private const int AllCount = 8 * 1024 * 1024;
    private readonly byte[] _baseBuffer;
    private const byte XorByte = 0xFD;
    private int _alignedVectorized;
    private int _aligned16;
    private int _aligned32;

    public ByteXor()
    {
        _baseBuffer = new byte[AllCount];
        var r = new Random(42069);
        r.NextBytes(_baseBuffer);
        _alignedVectorized = Vector.IsHardwareAccelerated ? Processor.GetAlignmentStart(_baseBuffer, Vector<byte>.Count) : 0;
        _aligned16 = Processor.GetAlignmentStart(_baseBuffer, 16);
        _aligned32 = Processor.GetAlignmentStart(_baseBuffer, 32);
    }

    public IEnumerable<int> Sizes => new[]
    {
        128 / 8, //
        256 / 8, //
        4 * 1024, //
        1024 * 1024, //
        4 * 1024 * 1024 //
    };

    [ParamsSource(nameof(Sizes))]
    public int Size { get; set; }

    [Benchmark(Baseline = true)]
    public void XorRepeat_Fallback() => ApplyXorFallback(_baseBuffer.AsSpan(0, Size), XorByte);

    [Benchmark]
    public void XorRepeat_FallbackPointer() => ApplyXorFallbackPointer(_baseBuffer.AsSpan(0, Size), XorByte);

    [Benchmark]
    public void XorRepeat_Vectorized() => ApplyXorVectorized(_baseBuffer.AsSpan(_alignedVectorized, Size), XorByte);

#if true
    [Benchmark]
    public void XorRepeat_AdvSimd() => ApplyXorAdvSimd(_baseBuffer.AsSpan(_aligned16, Size), XorByte);

#else
    [Benchmark]
    public void XorRepeat_Sse2() => ApplyXorSse2(_baseBuffer.AsSpan(_aligned16, Size), XorByte);

    [Benchmark]
    public void XorRepeat_Avx2() => ApplyXorAvx2(_baseBuffer.AsSpan(_aligned32, Size), XorByte);
#endif

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    public static void ApplyXorFallback(Span<byte> span, byte value)
    {
        for (int i = 0; i < span.Length; i++) span[i] ^= value;
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    public static unsafe void ApplyXorFallbackPointer(Span<byte> span, byte value)
    {
        fixed (byte* pStart = span)
        {
            ApplyXorFallbackPointer(pStart, pStart + span.Length, value);
        }
    }

    private static unsafe void ApplyXorFallbackPointer(byte* pStart, byte* pEnd, byte value)
    {
        byte* p = pStart;
        while (p < pEnd)
        {
            *p ^= value;
            p++;
        }
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    public static void ApplyXorVectorized(Span<byte> span, byte value)
    {
        if (!Vector.IsHardwareAccelerated) throw new PlatformNotSupportedException();
        int index = 0;
        Vector<byte> pattern = new(value);
        while (index + Vector<byte>.Count <= span.Length)
        {
            Vector.Xor(new Vector<byte>(span.Slice(index)), pattern).CopyTo(span.Slice(index));
            index += Vector<byte>.Count;
        }
        for (int i = index; i < span.Length; i++)
            span[i] ^= value;
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    public static unsafe void ApplyXorAdvSimd(Span<byte> span, byte value)
    {
        if (!AdvSimd.IsSupported) throw new PlatformNotSupportedException();
        const int split = 128 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] ^= value;
                i++;
            }

            if (kill1Idx == l) return;

            var src = FillVector128AdvSimd(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                AdvSimd.Store(pSource + i, AdvSimd.Xor(AdvSimd.LoadVector128(pSource + i), src));
                i += split;
            }

            while (i < span.Length)
            {
                pSource[i] ^= value;
                i++;
            }
        }
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    public static unsafe void ApplyXorSse2(Span<byte> span, byte value)
    {
        if (!Sse2.IsSupported) throw new PlatformNotSupportedException();
        const int split = 128 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] ^= value;
                i++;
            }

            if (kill1Idx == l) return;

            var src = FillVector128Sse2(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                Sse2.StoreAligned(pSource + i, Sse2.Xor(Sse2.LoadAlignedVector128(pSource + i), src));
                i += split;
            }

            while (i < span.Length)
            {
                pSource[i] ^= value;
                i++;
            }
        }
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    public static unsafe void ApplyXorAvx2(Span<byte> span, byte value)
    {
        if (!Avx2.IsSupported) throw new PlatformNotSupportedException();
        const int split = 256 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] ^= value;
                i++;
            }

            if (kill1Idx == l) return;

            var src = FillVector256Avx(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                Avx.StoreAligned(pSource + i, Avx2.Xor(Avx.LoadAlignedVector256(pSource + i), src));
                i += split;
            }

            while (i < span.Length)
            {
                pSource[i] ^= value;
                i++;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe Vector128<byte> FillVector128AdvSimd(byte value)
    {
        int* srcPtr = stackalloc int[128 / 8 / 4];
        int iValue = (value << 8) | value;
        iValue |= iValue << 16;
        srcPtr[0] = iValue;
        srcPtr[1] = iValue;
        srcPtr[2] = iValue;
        srcPtr[3] = iValue;
        return AdvSimd.LoadVector128((byte*)srcPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe Vector128<byte> FillVector128Sse2(byte value)
    {
        int* srcPtr = stackalloc int[128 / 8 / 4];
        int iValue = (value << 8) | value;
        iValue |= iValue << 16;
        srcPtr[0] = iValue;
        srcPtr[1] = iValue;
        srcPtr[2] = iValue;
        srcPtr[3] = iValue;
        return Sse2.LoadVector128((byte*)srcPtr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe Vector256<byte> FillVector256Avx(byte value)
    {
        int* srcPtr = stackalloc int[256 / 8 / 4];
        int iValue = (value << 8) | value;
        iValue |= iValue << 16;
        srcPtr[0] = iValue;
        srcPtr[1] = iValue;
        srcPtr[2] = iValue;
        srcPtr[3] = iValue;
        srcPtr[4] = iValue;
        srcPtr[5] = iValue;
        srcPtr[6] = iValue;
        srcPtr[7] = iValue;
        return Avx.LoadVector256((byte*)srcPtr);
    }

    /*
// * Summary *

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.5.1 (21G83) [Darwin 21.6.0]
Apple M1, 1 CPU, 8 logical and 8 physical cores
.NET SDK=6.0.301
  [Host]     : .NET 6.0.6 (6.0.622.26707), Arm64 RyuJIT
  DefaultJob : .NET 6.0.6 (6.0.622.26707), Arm64 RyuJIT


|                    Method |    Size |             Mean |          Error |         StdDev | Ratio | RatioSD |
|-------------------------- |-------- |-----------------:|---------------:|---------------:|------:|--------:|
|        XorRepeat_Fallback |      16 |         6.428 ns |      0.0830 ns |      0.0777 ns |  1.00 |    0.00 |
| XorRepeat_FallbackPointer |      16 |         6.222 ns |      0.0094 ns |      0.0083 ns |  0.97 |    0.01 |
|      XorRepeat_Vectorized |      16 |         2.371 ns |      0.0022 ns |      0.0021 ns |  0.37 |    0.00 |
|         XorRepeat_AdvSimd |      16 |         3.328 ns |      0.0039 ns |      0.0032 ns |  0.52 |    0.01 |
|                           |         |                  |                |                |       |         |
|        XorRepeat_Fallback |      32 |        13.874 ns |      0.0440 ns |      0.0412 ns |  1.00 |    0.00 |
| XorRepeat_FallbackPointer |      32 |        11.316 ns |      0.0085 ns |      0.0080 ns |  0.82 |    0.00 |
|      XorRepeat_Vectorized |      32 |         4.000 ns |      0.0079 ns |      0.0070 ns |  0.29 |    0.00 |
|         XorRepeat_AdvSimd |      32 |         4.419 ns |      0.0100 ns |      0.0093 ns |  0.32 |    0.00 |
|                           |         |                  |                |                |       |         |
|        XorRepeat_Fallback |    4096 |     2,333.257 ns |      1.2852 ns |      1.2022 ns |  1.00 |    0.00 |
| XorRepeat_FallbackPointer |    4096 |     1,734.677 ns |      1.0099 ns |      0.8952 ns |  0.74 |    0.00 |
|      XorRepeat_Vectorized |    4096 |       200.419 ns |      0.1469 ns |      0.1302 ns |  0.09 |    0.00 |
|         XorRepeat_AdvSimd |    4096 |       105.320 ns |      0.0654 ns |      0.0546 ns |  0.05 |    0.00 |
|                           |         |                  |                |                |       |         |
|        XorRepeat_Fallback | 1048576 |   584,385.326 ns |  3,213.3925 ns |  2,848.5887 ns |  1.00 |    0.00 |
| XorRepeat_FallbackPointer | 1048576 |   442,504.678 ns |    674.4524 ns |    597.8845 ns |  0.76 |    0.00 |
|      XorRepeat_Vectorized | 1048576 |    49,931.753 ns |     26.0271 ns |     24.3458 ns |  0.09 |    0.00 |
|         XorRepeat_AdvSimd | 1048576 |    27,751.756 ns |    547.1043 ns |    511.7617 ns |  0.05 |    0.00 |
|                           |         |                  |                |                |       |         |
|        XorRepeat_Fallback | 4194304 | 2,558,442.248 ns | 50,769.7546 ns | 64,207.4838 ns |  1.00 |    0.00 |
| XorRepeat_FallbackPointer | 4194304 | 2,111,795.885 ns | 41,974.6125 ns | 60,198.7350 ns |  0.82 |    0.04 |
|      XorRepeat_Vectorized | 4194304 |   230,685.591 ns |    381.9036 ns |    318.9068 ns |  0.09 |    0.00 |
|         XorRepeat_AdvSimd | 4194304 |   132,048.767 ns |    137.6077 ns |    121.9856 ns |  0.05 |    0.00 |

// * Hints *
Outliers
  ByteXor.XorRepeat_FallbackPointer: Default -> 1 outlier  was  removed (7.40 ns)
  ByteXor.XorRepeat_AdvSimd: Default         -> 2 outliers were removed (4.48 ns, 4.49 ns)
  ByteXor.XorRepeat_Vectorized: Default      -> 1 outlier  was  removed (5.16 ns)
  ByteXor.XorRepeat_FallbackPointer: Default -> 1 outlier  was  removed (1.74 us)
  ByteXor.XorRepeat_Vectorized: Default      -> 1 outlier  was  removed (202.80 ns)
  ByteXor.XorRepeat_AdvSimd: Default         -> 2 outliers were removed (107.72 ns, 108.56 ns)
  ByteXor.XorRepeat_Fallback: Default        -> 1 outlier  was  removed (592.26 us)
  ByteXor.XorRepeat_FallbackPointer: Default -> 1 outlier  was  removed (445.01 us)
  ByteXor.XorRepeat_Fallback: Default        -> 3 outliers were detected (2.35 ms..2.52 ms)
  ByteXor.XorRepeat_FallbackPointer: Default -> 2 outliers were detected (1.81 ms, 2.10 ms)
  ByteXor.XorRepeat_Vectorized: Default      -> 2 outliers were removed (232.84 us, 233.07 us)
  ByteXor.XorRepeat_AdvSimd: Default         -> 1 outlier  was  removed (132.44 us)

// * Legends *
  Size    : Value of the 'Size' parameter
  Mean    : Arithmetic mean of all measurements
  Error   : Half of 99.9% confidence interval
  StdDev  : Standard deviation of all measurements
  Ratio   : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD : Standard deviation of the ratio distribution ([Current]/[Baseline])
  1 ns    : 1 Nanosecond (0.000000001 sec)

// * Summary *

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1889 (21H2)
Intel Core i5-9400 CPU 2.90GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK=6.0.400
  [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT


|                Method |    Size |             Mean |         Error |        StdDev | Ratio | RatioSD |
|---------------------- |-------- |-----------------:|--------------:|--------------:|------:|--------:|
|    XorRepeat_Fallback |      16 |         6.247 ns |     0.0432 ns |     0.0383 ns |  1.00 |    0.00 |
| XorRepeat_Vectorized2 |      16 |        10.985 ns |     0.1166 ns |     0.1091 ns |  1.76 |    0.02 |
|        XorRepeat_Sse2 |      16 |         9.539 ns |     0.1150 ns |     0.1075 ns |  1.53 |    0.02 |
|        XorRepeat_Avx2 |      16 |        16.231 ns |     0.0509 ns |     0.0451 ns |  2.60 |    0.02 |
|                       |         |                  |               |               |       |         |
|    XorRepeat_Fallback |      32 |        13.912 ns |     0.0690 ns |     0.0646 ns |  1.00 |    0.00 |
| XorRepeat_Vectorized2 |      32 |         3.538 ns |     0.0195 ns |     0.0173 ns |  0.25 |    0.00 |
|        XorRepeat_Sse2 |      32 |         9.803 ns |     0.0470 ns |     0.0393 ns |  0.70 |    0.00 |
|        XorRepeat_Avx2 |      32 |        10.986 ns |     0.0126 ns |     0.0106 ns |  0.79 |    0.00 |
|                       |         |                  |               |               |       |         |
|    XorRepeat_Fallback |    4096 |     1,864.825 ns |     5.1206 ns |     4.5393 ns |  1.00 |    0.00 |
| XorRepeat_Vectorized2 |    4096 |       127.881 ns |     0.3723 ns |     0.3483 ns |  0.07 |    0.00 |
|        XorRepeat_Sse2 |    4096 |       137.984 ns |     0.4310 ns |     0.3821 ns |  0.07 |    0.00 |
|        XorRepeat_Avx2 |    4096 |        62.001 ns |     0.1815 ns |     0.1609 ns |  0.03 |    0.00 |
|                       |         |                  |               |               |       |         |
|    XorRepeat_Fallback | 1048576 |   477,130.500 ns | 2,345.6853 ns | 2,079.3890 ns |  1.00 |    0.00 |
| XorRepeat_Vectorized2 | 1048576 |    31,874.629 ns |   166.1419 ns |   155.4093 ns |  0.07 |    0.00 |
|        XorRepeat_Sse2 | 1048576 |    32,877.899 ns |   120.7607 ns |   112.9596 ns |  0.07 |    0.00 |
|        XorRepeat_Avx2 | 1048576 |    24,398.031 ns |    98.6297 ns |    82.3603 ns |  0.05 |    0.00 |
|                       |         |                  |               |               |       |         |
|    XorRepeat_Fallback | 4194304 | 1,903,779.241 ns | 2,785.7655 ns | 2,469.5086 ns |  1.00 |    0.00 |
| XorRepeat_Vectorized2 | 4194304 |   128,346.387 ns |   524.0453 ns |   437.6015 ns |  0.07 |    0.00 |
|        XorRepeat_Sse2 | 4194304 |   133,745.558 ns |   389.9702 ns |   345.6984 ns |  0.07 |    0.00 |
|        XorRepeat_Avx2 | 4194304 |    98,846.070 ns |   549.8972 ns |   429.3236 ns |  0.05 |    0.00 |

// * Hints *
Outliers
  ByteXor.XorRepeat_Fallback: Default    -> 1 outlier  was  removed (7.91 ns)
  ByteXor.XorRepeat_Avx2: Default        -> 1 outlier  was  removed (17.87 ns)
  ByteXor.XorRepeat_Vectorized2: Default -> 1 outlier  was  removed (5.09 ns)
  ByteXor.XorRepeat_Sse2: Default        -> 2 outliers were removed (11.43 ns, 11.43 ns)
  ByteXor.XorRepeat_Avx2: Default        -> 2 outliers were removed (12.53 ns, 12.54 ns)
  ByteXor.XorRepeat_Fallback: Default    -> 1 outlier  was  removed (1.89 us)
  ByteXor.XorRepeat_Sse2: Default        -> 1 outlier  was  removed (141.26 ns)
  ByteXor.XorRepeat_Avx2: Default        -> 1 outlier  was  removed (64.78 ns)
  ByteXor.XorRepeat_Fallback: Default    -> 1 outlier  was  removed (485.45 us)
  ByteXor.XorRepeat_Avx2: Default        -> 2 outliers were removed (24.69 us, 24.91 us)
  ByteXor.XorRepeat_Fallback: Default    -> 1 outlier  was  removed (1.91 ms)
  ByteXor.XorRepeat_Vectorized2: Default -> 2 outliers were removed (130.70 us, 131.15 us)
  ByteXor.XorRepeat_Sse2: Default        -> 1 outlier  was  removed (136.74 us)
  ByteXor.XorRepeat_Avx2: Default        -> 3 outliers were removed (100.82 us..101.30 us)

// * Legends *
  Size    : Value of the 'Size' parameter
  Mean    : Arithmetic mean of all measurements
  Error   : Half of 99.9% confidence interval
  StdDev  : Standard deviation of all measurements
  Ratio   : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD : Standard deviation of the ratio distribution ([Current]/[Baseline])
  1 ns    : 1 Nanosecond (0.000000001 sec)

     */
}
