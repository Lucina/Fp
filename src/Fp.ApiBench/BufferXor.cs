using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace Fp.ApiBench;

public class BufferXor
{
    private const int AllCount = 4 * 1024 * 1024;
    private readonly byte[] _baseBuffer;
    private readonly byte[] _xorBuffer;

    public BufferXor()
    {
        _baseBuffer = new byte[AllCount];
        _xorBuffer = new byte[AllCount];
        var r = new Random(42069);
        r.NextBytes(_baseBuffer);
        r.NextBytes(_xorBuffer);
    }

    public IEnumerable<(int message, int key)> Sizes => new[]
    {
        (128 / 8, 128 / 8), //
        (4 * 1024, 128 / 8), //
        (4 * 1024, 256 / 8), //
        (4 * 1024, 4 * 1024), //
        (1024 * 1024, 128 / 8), //
        (1024 * 1024, 256 / 8), //
        (1024 * 1024, 4 * 1024), //
        (4 * 1024 * 1024, 128 / 8), //
        (4 * 1024 * 1024, 256 / 8), //
        (4 * 1024 * 1024, 4 * 1024) //
    };

    [ParamsSource(nameof(Sizes))]
    public (int message, int key) Size { get; set; }

    [Benchmark(Baseline = true)]
    public void XorRepeat_Fallback() => ApplyXorFallback(_baseBuffer.AsSpan(0, Size.message), _xorBuffer.AsSpan(0, Size.key), SequenceBehaviour.Repeat);

    [Benchmark]
    public void XorRepeat_Vectorized() => ApplyXorVectorized(_baseBuffer.AsSpan(0, Size.message), _xorBuffer.AsSpan(0, Size.key), SequenceBehaviour.Repeat);

    internal static void ApplyXorVectorized(Span<byte> span, ReadOnlySpan<byte> pattern, SequenceBehaviour sequenceBehaviour = SequenceBehaviour.Repeat)
    {
        if (!Vector.IsHardwareAccelerated) throw new PlatformNotSupportedException();
        if (span.IsEmpty || pattern.IsEmpty) return;
        switch (sequenceBehaviour)
        {
            case SequenceBehaviour.Truncate:
                {
                    if (pattern.Length < span.Length)
                    {
                        int index = 0;
                        while (index + Vector<byte>.Count <= pattern.Length)
                        {
                            Span<byte> targetMemory = span[index..];
                            Vector<byte> sourceVec = new(targetMemory);
                            sourceVec = Vector.Xor(sourceVec, new Vector<byte>(pattern[index..]));
                            sourceVec.CopyTo(targetMemory);
                            index += Vector<byte>.Count;
                        }
                        for (int i = index; i < pattern.Length; i++)
                            span[i] ^= pattern[i];
                    }
                    else
                    {
                        int index = 0;
                        while (index + Vector<byte>.Count <= span.Length)
                        {
                            Span<byte> targetMemory = span[index..];
                            Vector<byte> sourceVec = new(targetMemory);
                            sourceVec = Vector.Xor(sourceVec, new Vector<byte>(pattern[index..]));
                            sourceVec.CopyTo(targetMemory);
                            index += Vector<byte>.Count;
                        }
                        for (int i = index; i < span.Length; i++)
                            span[i] ^= pattern[i];
                    }
                    break;
                }
            case SequenceBehaviour.Repeat:
                {
                    Span<byte> segment = span;
                    while (true)
                    {
                        if (pattern.Length < segment.Length)
                        {
                            int index = 0;
                            while (index + Vector<byte>.Count <= pattern.Length)
                            {
                                Span<byte> targetMemory = segment[index..];
                                Vector<byte> sourceVec = new(targetMemory);
                                sourceVec = Vector.Xor(sourceVec, new Vector<byte>(pattern[index..]));
                                sourceVec.CopyTo(targetMemory);
                                index += Vector<byte>.Count;
                            }
                            for (int i = index; i < pattern.Length; i++)
                                segment[i] ^= pattern[i];
                            segment = segment[pattern.Length..];
                        }
                        else
                        {
                            int index = 0;
                            while (index + Vector<byte>.Count <= segment.Length)
                            {
                                Span<byte> targetMemory = segment[index..];
                                Vector<byte> sourceVec = new(targetMemory);
                                sourceVec = Vector.Xor(sourceVec, new Vector<byte>(pattern[index..]));
                                sourceVec.CopyTo(targetMemory);
                                index += Vector<byte>.Count;
                            }
                            for (int i = index; i < segment.Length; i++)
                                segment[i] ^= pattern[i];
                            break;
                        }
                    }
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(sequenceBehaviour), sequenceBehaviour, null);
        }
    }

    internal static void ApplyXorFallback(Span<byte> span, ReadOnlySpan<byte> pattern, SequenceBehaviour sequenceBehaviour = SequenceBehaviour.Repeat)
    {
        if (span.IsEmpty || pattern.IsEmpty) return;
        switch (sequenceBehaviour)
        {
            case SequenceBehaviour.Truncate:
                {
                    if (pattern.Length < span.Length)
                        for (int i = 0; i < pattern.Length; i++)
                            span[i] ^= pattern[i];
                    else
                        for (int i = 0; i < span.Length; i++)
                            span[i] ^= pattern[i];
                    break;
                }
            case SequenceBehaviour.Repeat:
                {
                    Span<byte> segment = span;
                    while (true)
                    {
                        if (pattern.Length < segment.Length)
                        {
                            for (int i = 0; i < pattern.Length; i++)
                                segment[i] ^= pattern[i];
                            segment = segment[pattern.Length..];
                        }
                        else
                        {
                            for (int i = 0; i < segment.Length; i++)
                                segment[i] ^= pattern[i];
                            break;
                        }
                    }
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(sequenceBehaviour), sequenceBehaviour, null);
        }
    }

    public enum SequenceBehaviour
    {
        /// <summary>
        /// Truncate sequence.
        /// </summary>
        Truncate,

        /// <summary>
        /// Repeat sequence.
        /// </summary>
        Repeat
    }


/*
// * Summary *

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.5.1 (21G83) [Darwin 21.6.0]
Apple M1, 1 CPU, 8 logical and 8 physical cores
.NET SDK=6.0.301
  [Host]     : .NET 6.0.6 (6.0.622.26707), Arm64 RyuJIT
  DefaultJob : .NET 6.0.6 (6.0.622.26707), Arm64 RyuJIT


|               Method |            Size |             Mean |          Error |          StdDev |           Median | Ratio |
|--------------------- |---------------- |-----------------:|---------------:|----------------:|-----------------:|------:|
|   XorRepeat_Fallback |   (1048576, 16) |   548,953.649 ns |  3,117.3401 ns |   2,915.9619 ns |   547,231.649 ns |  1.00 |
| XorRepeat_Vectorized |   (1048576, 16) |   136,947.140 ns |    955.4637 ns |     893.7414 ns |   136,341.878 ns |  0.25 |
|                      |                 |                  |                |                 |                  |       |
|   XorRepeat_Fallback |   (1048576, 32) |   526,653.413 ns |  2,278.8290 ns |   2,020.1226 ns |   526,172.526 ns |  1.00 |
| XorRepeat_Vectorized |   (1048576, 32) |   110,161.643 ns |     82.6112 ns |      73.2327 ns |   110,139.148 ns |  0.21 |
|                      |                 |                  |                |                 |                  |       |
|   XorRepeat_Fallback | (1048576, 4096) |   521,053.176 ns |  6,995.8488 ns |   6,543.9213 ns |   520,597.127 ns |  1.00 |
| XorRepeat_Vectorized | (1048576, 4096) |    91,523.452 ns |  1,258.2653 ns |   1,176.9822 ns |    91,410.772 ns |  0.18 |
|                      |                 |                  |                |                 |                  |       |
|   XorRepeat_Fallback |        (16, 16) |        12.910 ns |      0.0086 ns |       0.0081 ns |        12.908 ns |  1.00 |
| XorRepeat_Vectorized |        (16, 16) |         3.172 ns |      0.0031 ns |       0.0029 ns |         3.173 ns |  0.25 |
|                      |                 |                  |                |                 |                  |       |
|   XorRepeat_Fallback |      (4096, 16) |     2,142.983 ns |      1.2490 ns |       1.1072 ns |     2,142.887 ns |  1.00 |
| XorRepeat_Vectorized |      (4096, 16) |       537.599 ns |      0.2409 ns |       0.2253 ns |       537.607 ns |  0.25 |
|                      |                 |                  |                |                 |                  |       |
|   XorRepeat_Fallback |      (4096, 32) |     2,059.824 ns |      3.4510 ns |       3.2280 ns |     2,058.711 ns |  1.00 |
| XorRepeat_Vectorized |      (4096, 32) |       434.360 ns |      0.1392 ns |       0.1234 ns |       434.329 ns |  0.21 |
|                      |                 |                  |                |                 |                  |       |
|   XorRepeat_Fallback |    (4096, 4096) |     2,621.295 ns |      2.5414 ns |       2.2528 ns |     2,620.480 ns |  1.00 |
| XorRepeat_Vectorized |    (4096, 4096) |       364.517 ns |      0.1968 ns |       0.1841 ns |       364.529 ns |  0.14 |
|                      |                 |                  |                |                 |                  |       |
|   XorRepeat_Fallback |   (4194304, 16) | 2,291,507.944 ns | 45,796.9941 ns |  62,687.4424 ns | 2,275,237.955 ns |  1.00 |
| XorRepeat_Vectorized |   (4194304, 16) |   562,339.089 ns |  1,031.6198 ns |     964.9778 ns |   561,941.039 ns |  0.25 |
|                      |                 |                  |                |                 |                  |       |
|   XorRepeat_Fallback |   (4194304, 32) | 2,278,481.371 ns | 45,438.1204 ns | 132,545.1300 ns | 2,233,735.023 ns |  1.00 |
| XorRepeat_Vectorized |   (4194304, 32) |   464,359.663 ns |  2,474.2437 ns |   2,193.3526 ns |   463,571.777 ns |  0.21 |
|                      |                 |                  |                |                 |                  |       |
|   XorRepeat_Fallback | (4194304, 4096) | 2,094,048.770 ns | 21,672.8161 ns |  19,212.3870 ns | 2,095,944.336 ns |  1.00 |
| XorRepeat_Vectorized | (4194304, 4096) |   377,964.724 ns |    813.9735 ns |     761.3913 ns |   377,509.013 ns |  0.18 |

// * Hints *
Outliers
  BufferXor.XorRepeat_Fallback: Default   -> 1 outlier  was  removed (531.63 us)
  BufferXor.XorRepeat_Vectorized: Default -> 1 outlier  was  removed (110.35 us)
  BufferXor.XorRepeat_Fallback: Default   -> 1 outlier  was  removed (2.15 us)
  BufferXor.XorRepeat_Vectorized: Default -> 1 outlier  was  removed (436.90 ns)
  BufferXor.XorRepeat_Fallback: Default   -> 1 outlier  was  removed (2.63 us)
  BufferXor.XorRepeat_Fallback: Default   -> 3 outliers were removed (2.56 ms..2.57 ms)
  BufferXor.XorRepeat_Vectorized: Default -> 1 outlier  was  removed (473.91 us)
  BufferXor.XorRepeat_Fallback: Default   -> 1 outlier  was  removed (2.16 ms)

// * Legends *
  Size   : Value of the 'Size' parameter
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  Median : Value separating the higher half of all measurements (50th percentile)
  Ratio  : Mean of the ratio distribution ([Current]/[Baseline])
  1 ns   : 1 Nanosecond (0.000000001 sec)

// * Summary *

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1889 (21H2)
Intel Core i5-9400 CPU 2.90GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK=6.0.400
  [Host]     : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT
  DefaultJob : .NET 6.0.8 (6.0.822.36306), X64 RyuJIT


|               Method |            Size |            Mean |         Error |        StdDev | Ratio |
|--------------------- |---------------- |----------------:|--------------:|--------------:|------:|
|   XorRepeat_Fallback |   (1048576, 16) |   593,393.99 ns |  3,017.404 ns |  2,674.850 ns |  1.00 |
| XorRepeat_Vectorized |   (1048576, 16) |   744,088.34 ns |  6,694.594 ns |  6,262.128 ns |  1.25 |
|                      |                 |                 |               |               |       |
|   XorRepeat_Fallback |   (1048576, 32) |   563,177.14 ns |  2,379.277 ns |  2,225.577 ns |  1.00 |
| XorRepeat_Vectorized |   (1048576, 32) |    95,397.06 ns |    192.982 ns |    161.148 ns |  0.17 |
|                      |                 |                 |               |               |       |
|   XorRepeat_Fallback | (1048576, 4096) |   540,800.07 ns |  1,910.665 ns |  1,787.237 ns |  1.00 |
| XorRepeat_Vectorized | (1048576, 4096) |    60,186.24 ns |    987.324 ns |    923.544 ns |  0.11 |
|                      |                 |                 |               |               |       |
|   XorRepeat_Fallback |        (16, 16) |        12.66 ns |      0.047 ns |      0.042 ns |  1.00 |
| XorRepeat_Vectorized |        (16, 16) |        13.82 ns |      0.038 ns |      0.032 ns |  1.09 |
|                      |                 |                 |               |               |       |
|   XorRepeat_Fallback |      (4096, 16) |     2,319.73 ns |      6.851 ns |      6.408 ns |  1.00 |
| XorRepeat_Vectorized |      (4096, 16) |     2,882.61 ns |      4.953 ns |      4.391 ns |  1.24 |
|                      |                 |                 |               |               |       |
|   XorRepeat_Fallback |      (4096, 32) |     2,216.54 ns |      6.148 ns |      5.133 ns |  1.00 |
| XorRepeat_Vectorized |      (4096, 32) |       375.36 ns |      1.589 ns |      1.327 ns |  0.17 |
|                      |                 |                 |               |               |       |
|   XorRepeat_Fallback |    (4096, 4096) |     2,503.45 ns |      7.550 ns |      7.063 ns |  1.00 |
| XorRepeat_Vectorized |    (4096, 4096) |       258.03 ns |      0.658 ns |      0.550 ns |  0.10 |
|                      |                 |                 |               |               |       |
|   XorRepeat_Fallback |   (4194304, 16) | 2,374,749.67 ns |  9,392.489 ns |  7,333.038 ns |  1.00 |
| XorRepeat_Vectorized |   (4194304, 16) | 2,953,898.32 ns |  9,450.270 ns |  7,891.404 ns |  1.24 |
|                      |                 |                 |               |               |       |
|   XorRepeat_Fallback |   (4194304, 32) | 2,264,734.15 ns | 27,516.798 ns | 24,392.925 ns |  1.00 |
| XorRepeat_Vectorized |   (4194304, 32) |   383,841.61 ns |    815.531 ns |    722.947 ns |  0.17 |
|                      |                 |                 |               |               |       |
|   XorRepeat_Fallback | (4194304, 4096) | 2,163,335.21 ns |  5,278.182 ns |  4,678.971 ns |  1.00 |
| XorRepeat_Vectorized | (4194304, 4096) |   243,717.80 ns |  1,170.853 ns |  1,037.931 ns |  0.11 |

// * Hints *
Outliers
  BufferXor.XorRepeat_Fallback: Default   -> 1 outlier  was  removed (603.38 us)
  BufferXor.XorRepeat_Vectorized: Default -> 2 outliers were removed, 3 outliers were detected (95.06 us, 95.77 us, 96.25 us)
  BufferXor.XorRepeat_Fallback: Default   -> 1 outlier  was  removed (14.39 ns)
  BufferXor.XorRepeat_Vectorized: Default -> 2 outliers were removed (15.50 ns, 15.77 ns)
  BufferXor.XorRepeat_Vectorized: Default -> 1 outlier  was  removed (2.91 us)
  BufferXor.XorRepeat_Fallback: Default   -> 2 outliers were removed (2.24 us, 2.25 us)
  BufferXor.XorRepeat_Vectorized: Default -> 2 outliers were removed (388.22 ns, 390.74 ns)
  BufferXor.XorRepeat_Vectorized: Default -> 2 outliers were removed (263.22 ns, 263.37 ns)
  BufferXor.XorRepeat_Fallback: Default   -> 3 outliers were removed (2.44 ms..2.46 ms)
  BufferXor.XorRepeat_Vectorized: Default -> 2 outliers were removed (2.98 ms, 2.98 ms)
  BufferXor.XorRepeat_Fallback: Default   -> 1 outlier  was  removed (2.37 ms)
  BufferXor.XorRepeat_Vectorized: Default -> 1 outlier  was  removed (388.70 us)
  BufferXor.XorRepeat_Fallback: Default   -> 1 outlier  was  removed (2.18 ms)
  BufferXor.XorRepeat_Vectorized: Default -> 1 outlier  was  removed, 2 outliers were detected (241.47 us, 246.00 us)

// * Legends *
  Size   : Value of the 'Size' parameter
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  Ratio  : Mean of the ratio distribution ([Current]/[Baseline])
  1 ns   : 1 Nanosecond (0.000000001 sec)

 */
}
