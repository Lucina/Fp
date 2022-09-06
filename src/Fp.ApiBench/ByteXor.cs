using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace Fp.ApiBench;

public class ByteXor
{
    private const int AllCount = 4 * 1024 * 1024;
    private readonly byte[] _baseBuffer;
    private const byte XorByte = 0xFD;

    public ByteXor()
    {
        _baseBuffer = new byte[AllCount];
        var r = new Random(42069);
        r.NextBytes(_baseBuffer);
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
    public void XorRepeat_Vectorized2() => ApplyXorVectorized(_baseBuffer.AsSpan(0, Size), XorByte);

    /* [Benchmark]
    public void XorRepeat_AdvSimd() => ApplyXorAdvSimd(_baseBuffer.AsSpan(0, Size), XorByte); */

    [Benchmark]
    public void XorRepeat_Sse2() => ApplyXorSse2(_baseBuffer.AsSpan(0, Size), XorByte);

    [Benchmark]
    public void XorRepeat_Avx2() => ApplyXorAvx2(_baseBuffer.AsSpan(0, Size), XorByte);

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


    |                Method |    Size |             Mean |         Error |        StdDev | Ratio |
    |---------------------- |-------- |-----------------:|--------------:|--------------:|------:|
    |    XorRepeat_Fallback |      16 |         6.603 ns |     0.0796 ns |     0.0745 ns |  1.00 |
    |  XorRepeat_Vectorized |      16 |         2.373 ns |     0.0017 ns |     0.0015 ns |  0.36 |
    | XorRepeat_Vectorized2 |      16 |         2.370 ns |     0.0018 ns |     0.0017 ns |  0.36 |
    |     XorRepeat_AdvSimd |      16 |         5.948 ns |     0.0036 ns |     0.0034 ns |  0.90 |
    |                       |         |                  |               |               |       |
    |    XorRepeat_Fallback |      32 |        13.864 ns |     0.0098 ns |     0.0086 ns |  1.00 |
    |  XorRepeat_Vectorized |      32 |         3.935 ns |     0.0030 ns |     0.0028 ns |  0.28 |
    | XorRepeat_Vectorized2 |      32 |         4.082 ns |     0.0030 ns |     0.0028 ns |  0.29 |
    |     XorRepeat_AdvSimd |      32 |         6.269 ns |     0.0037 ns |     0.0031 ns |  0.45 |
    |                       |         |                  |               |               |       |
    |    XorRepeat_Fallback |    4096 |     2,327.795 ns |     4.7967 ns |     4.2522 ns |  1.00 |
    |  XorRepeat_Vectorized |    4096 |       220.716 ns |     0.1104 ns |     0.0979 ns |  0.09 |
    | XorRepeat_Vectorized2 |    4096 |       200.097 ns |     0.2119 ns |     0.1982 ns |  0.09 |
    |     XorRepeat_AdvSimd |    4096 |       106.990 ns |     0.0942 ns |     0.0835 ns |  0.05 |
    |                       |         |                  |               |               |       |
    |    XorRepeat_Fallback | 1048576 |   583,036.205 ns |   936.2426 ns |   781.8050 ns |  1.00 |
    |  XorRepeat_Vectorized | 1048576 |    55,570.818 ns |   294.4327 ns |   245.8647 ns |  0.10 |
    | XorRepeat_Vectorized2 | 1048576 |    50,651.891 ns |   128.6226 ns |   114.0206 ns |  0.09 |
    |     XorRepeat_AdvSimd | 1048576 |    30,191.375 ns |   153.7018 ns |   136.2527 ns |  0.05 |
    |                       |         |                  |               |               |       |
    |    XorRepeat_Fallback | 4194304 | 2,577,268.382 ns | 1,453.8317 ns | 1,359.9151 ns |  1.00 |
    |  XorRepeat_Vectorized | 4194304 |   266,264.798 ns |   658.3681 ns |   615.8379 ns |  0.10 |
    | XorRepeat_Vectorized2 | 4194304 |   248,525.567 ns |   512.8791 ns |   428.2773 ns |  0.10 |
    |     XorRepeat_AdvSimd | 4194304 |   132,223.074 ns |   189.9516 ns |   177.6808 ns |  0.05 |

    // * Hints *
    Outliers
      ByteXor.XorRepeat_Vectorized: Default  -> 1 outlier  was  removed (3.51 ns)
      ByteXor.XorRepeat_Fallback: Default    -> 1 outlier  was  removed (15.08 ns)
      ByteXor.XorRepeat_AdvSimd: Default     -> 2 outliers were removed (7.44 ns, 7.44 ns)
      ByteXor.XorRepeat_Fallback: Default    -> 1 outlier  was  removed, 3 outliers were detected (2.32 us, 2.32 us, 2.34 us)
      ByteXor.XorRepeat_Vectorized: Default  -> 1 outlier  was  removed (223.17 ns)
      ByteXor.XorRepeat_AdvSimd: Default     -> 1 outlier  was  removed (109.65 ns)
      ByteXor.XorRepeat_Fallback: Default    -> 2 outliers were removed (589.07 us, 596.68 us)
      ByteXor.XorRepeat_Vectorized: Default  -> 2 outliers were removed (60.01 us, 60.79 us)
      ByteXor.XorRepeat_Vectorized2: Default -> 1 outlier  was  removed (51.05 us)
      ByteXor.XorRepeat_AdvSimd: Default     -> 1 outlier  was  removed (30.65 us)
      ByteXor.XorRepeat_Vectorized2: Default -> 2 outliers were removed (249.97 us, 250.24 us)

    // * Legends *
      Size   : Value of the 'Size' parameter
      Mean   : Arithmetic mean of all measurements
      Error  : Half of 99.9% confidence interval
      StdDev : Standard deviation of all measurements
      Ratio  : Mean of the ratio distribution ([Current]/[Baseline])
      1 ns   : 1 Nanosecond (0.000000001 sec)

    // * Summary *

    BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1889 (21H2)
    Intel Core i5-9400 CPU 2.90GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
    .NET SDK=6.0.400
      [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
      DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT


    |                Method |    Size |             Mean |         Error |        StdDev | Ratio | RatioSD |
    |---------------------- |-------- |-----------------:|--------------:|--------------:|------:|--------:|
    |    XorRepeat_Fallback |      16 |         6.247 ns |     0.0306 ns |     0.0271 ns |  1.00 |    0.00 |
    | XorRepeat_Vectorized2 |      16 |        10.565 ns |     0.0460 ns |     0.0430 ns |  1.69 |    0.01 |
    |        XorRepeat_Sse2 |      16 |        11.822 ns |     0.0251 ns |     0.0222 ns |  1.89 |    0.01 |
    |        XorRepeat_Avx2 |      16 |        10.531 ns |     0.2019 ns |     0.1889 ns |  1.69 |    0.03 |
    |                       |         |                  |               |               |       |         |
    |    XorRepeat_Fallback |      32 |        13.271 ns |     0.0540 ns |     0.0479 ns |  1.00 |    0.00 |
    | XorRepeat_Vectorized2 |      32 |         3.058 ns |     0.0154 ns |     0.0136 ns |  0.23 |    0.00 |
    |        XorRepeat_Sse2 |      32 |        12.031 ns |     0.0376 ns |     0.0314 ns |  0.91 |    0.00 |
    |        XorRepeat_Avx2 |      32 |        19.294 ns |     0.0382 ns |     0.0319 ns |  1.45 |    0.00 |
    |                       |         |                  |               |               |       |         |
    |    XorRepeat_Fallback |    4096 |     1,870.117 ns |     6.5600 ns |     6.1362 ns |  1.00 |    0.00 |
    | XorRepeat_Vectorized2 |    4096 |       128.777 ns |     0.4091 ns |     0.3627 ns |  0.07 |    0.00 |
    |        XorRepeat_Sse2 |    4096 |       132.179 ns |     1.0627 ns |     0.9940 ns |  0.07 |    0.00 |
    |        XorRepeat_Avx2 |    4096 |        73.888 ns |     0.9571 ns |     0.8953 ns |  0.04 |    0.00 |
    |                       |         |                  |               |               |       |         |
    |    XorRepeat_Fallback | 1048576 |   478,066.696 ns | 1,695.6334 ns | 1,503.1348 ns |  1.00 |    0.00 |
    | XorRepeat_Vectorized2 | 1048576 |    34,121.424 ns |   195.2121 ns |   163.0109 ns |  0.07 |    0.00 |
    |        XorRepeat_Sse2 | 1048576 |    33,005.862 ns |    94.2515 ns |    88.1629 ns |  0.07 |    0.00 |
    |        XorRepeat_Avx2 | 1048576 |    24,528.846 ns |   189.1022 ns |   176.8863 ns |  0.05 |    0.00 |
    |                       |         |                  |               |               |       |         |
    |    XorRepeat_Fallback | 4194304 | 1,913,007.659 ns | 5,370.6614 ns | 4,760.9515 ns |  1.00 |    0.00 |
    | XorRepeat_Vectorized2 | 4194304 |   140,979.544 ns | 1,083.3038 ns | 1,013.3230 ns |  0.07 |    0.00 |
    |        XorRepeat_Sse2 | 4194304 |   133,483.214 ns |   550.7187 ns |   488.1978 ns |  0.07 |    0.00 |
    |        XorRepeat_Avx2 | 4194304 |   100,435.265 ns | 1,217.6075 ns | 1,138.9508 ns |  0.05 |    0.00 |

    // * Hints *
    Outliers
      ByteXor.XorRepeat_Fallback: Default    -> 1 outlier  was  removed (7.85 ns)
      ByteXor.XorRepeat_Sse2: Default        -> 1 outlier  was  removed (13.52 ns)
      ByteXor.XorRepeat_Fallback: Default    -> 1 outlier  was  removed (15.08 ns)
      ByteXor.XorRepeat_Vectorized2: Default -> 1 outlier  was  removed (4.59 ns)
      ByteXor.XorRepeat_Sse2: Default        -> 2 outliers were removed (13.67 ns, 13.85 ns)
      ByteXor.XorRepeat_Avx2: Default        -> 2 outliers were removed (20.94 ns, 20.99 ns)
      ByteXor.XorRepeat_Vectorized2: Default -> 1 outlier  was  removed (132.38 ns)
      ByteXor.XorRepeat_Fallback: Default    -> 1 outlier  was  removed (484.26 us)
      ByteXor.XorRepeat_Vectorized2: Default -> 2 outliers were removed (34.86 us, 35.60 us)
      ByteXor.XorRepeat_Fallback: Default    -> 1 outlier  was  removed (1.94 ms)
      ByteXor.XorRepeat_Sse2: Default        -> 1 outlier  was  removed (136.80 us)

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
