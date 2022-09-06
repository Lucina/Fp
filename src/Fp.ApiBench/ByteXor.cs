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
    public void XorRepeat_Fallback() => Processor.ApplyXorFallback(_baseBuffer.AsSpan(0, Size), XorByte);

    [Benchmark]
    public void XorRepeat_Vectorized2() => Processor.ApplyXorVectorized(_baseBuffer.AsSpan(0, Size), XorByte);

    [Benchmark]
    public void XorRepeat_AdvSimd() => Processor.ApplyXorAdvSimd(_baseBuffer.AsSpan(0, Size), XorByte);

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

 */
}
