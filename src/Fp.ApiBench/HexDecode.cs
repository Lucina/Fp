using BenchmarkDotNet.Attributes;

namespace Fp.ApiBench;

public class HexDecode
{
    private const int AllCount = 4 * 1024 * 1024;
    private readonly TestPair _pair;

    public HexDecode()
    {
        byte[] data = new byte[AllCount];
        new Random(42069).NextBytes(data);
        string strRepr = Convert.ToHexString(data);
        _pair = new TestPair(strRepr, data);
    }

    [Params(128 / 8, 258 / 8, 4 * 1024, 256 * 1024, 4 * 1024 * 1024)]
    public int MessageSize { get; set; }

    [Benchmark]
    public void Convert_FromHexString() => Convert.FromHexString(_pair.Key.AsSpan(0, MessageSize * 2));

    [Benchmark]
    public void DecodePtrValidated() => DecodeHexPtr(_pair.Key.AsSpan(0, MessageSize * 2));

    [Benchmark]
    public void DecodePtrNonValidated() => DecodeHexPtr(_pair.Key.AsSpan(0, MessageSize * 2), false);

    [Benchmark]
    public void DecodeSpanValidated() => DecodeHexSpan(_pair.Key.AsSpan(0, MessageSize * 2));

    [Benchmark]
    public void DecodeSpanNonValidated() => DecodeHexSpan(_pair.Key.AsSpan(0, MessageSize * 2), false);

    public static byte[] DecodeHexPtr(ReadOnlySpan<char> hex, bool validate = true)
    {
        int byteCount = GetHexByteCount(hex, out int start);
        byte[] res = new byte[byteCount];
        if (validate) DecodeHexPtrValidatedInternal(start, hex, res, byteCount);
        else DecodeHexPtrNonValidatedInternal(start, hex, res, byteCount);
        return res;
    }

    public static void DecodeHexPtr(ReadOnlySpan<char> hex, Span<byte> result, bool validate = true)
    {
        int byteCount = GetHexByteCount(hex, out int start);
        if (result.Length < byteCount)
            throw new ArgumentException($"Result buffer is too short ({result.Length} bytes) to contain result of length {byteCount} bytes", nameof(result));
        if (validate) DecodeHexPtrValidatedInternal(start, hex, result, byteCount);
        else DecodeHexPtrNonValidatedInternal(start, hex, result, byteCount);
    }

    public static byte[] DecodeHexSpan(ReadOnlySpan<char> hex, bool validate = true)
    {
        int byteCount = GetHexByteCount(hex, out int start);
        byte[] res = new byte[byteCount];
        if (validate) DecodeHexSpanValidatedInternal(start, hex, res);
        else DecodeHexSpanNonValidatedInternal(start, hex, res);
        return res;
    }

    public static void DecodeHexSpan(ReadOnlySpan<char> hex, Span<byte> result, bool validate = true)
    {
        int byteCount = GetHexByteCount(hex, out int start);
        if (result.Length < byteCount)
            throw new ArgumentException($"Result buffer is too short ({result.Length} bytes) to contain result of length {byteCount} bytes", nameof(result));
        if (validate) DecodeHexSpanValidatedInternal(start, hex, result);
        else DecodeHexSpanNonValidatedInternal(start, hex, result);
    }

    public static int GetHexByteCount(ReadOnlySpan<char> hex, out int start)
    {
        int len = hex.Length;
        if (len == 0)
        {
            start = 0;
            return 0;
        }
        if (len % 2 != 0) throw new ArgumentException($"Hex string has length {len}, must be even", nameof(hex));
        start = len >= 2 && hex[0] == '0' && char.ToLowerInvariant(hex[1]) == 'x' ? 2 : 0;
        return (len - start) >> 1;
    }

    private static unsafe void DecodeHexPtrValidatedInternal(int start, ReadOnlySpan<char> hex, Span<byte> target, int byteCount)
    {
        fixed (char* buf = hex)
        {
            fixed (byte* res = target)
            {
                char* rBuf = buf + start;
                byte* rRes = res;
                for (int i = 0; i < byteCount; i++)
                {
                    byte v;
                    char c = *rBuf++;
                    if (c > 0x66) throw new ArgumentException($"Illegal character {c} at position {rBuf - buf - 1}", nameof(hex));
                    if (c > 0x60) v = (byte)((c + 9) << 4);
                    else if (c > 0x46) throw new ArgumentException($"Illegal character {c} at position {rBuf - buf - 1}", nameof(hex));
                    else if (c > 0x40) v = (byte)((c + 9) << 4);
                    else if (c > 0x39) throw new ArgumentException($"Illegal character {c} at position {rBuf - buf - 1}", nameof(hex));
                    else if (c > 0x2F) v = (byte)(c << 4);
                    else throw new ArgumentException($"Illegal character {c} at position {rBuf - buf - 1}", nameof(hex));
                    c = *rBuf++;
                    if (c > 0x66) throw new ArgumentException($"Illegal character {c} at position {rBuf - buf - 1}", nameof(hex));
                    if (c > 0x60) v |= (byte)((c + 9) & 0xf);
                    else if (c > 0x46) throw new ArgumentException($"Illegal character {c} at position {rBuf - buf - 1}", nameof(hex));
                    else if (c > 0x40) v |= (byte)((c + 9) & 0xf);
                    else if (c > 0x39) throw new ArgumentException($"Illegal character {c} at position {rBuf - buf - 1}", nameof(hex));
                    else if (c > 0x2F) v |= (byte)(c & 0xf);
                    else throw new ArgumentException($"Illegal character {c} at position {rBuf - buf - 1}", nameof(hex));
                    *rRes = v;
                    rRes++;
                }
            }
        }
    }

    private static unsafe void DecodeHexPtrNonValidatedInternal(int start, ReadOnlySpan<char> hex, Span<byte> target, int byteCount)
    {
        fixed (char* buf = hex)
        {
            fixed (byte* res = target)
            {
                char* rBuf = buf + start;
                byte* rRes = res;
                for (int i = 0; i < byteCount; i++)
                {
                    byte v;
                    char c = *rBuf++;
                    v = c < 0x3A ? (byte)(c << 4) : (byte)((c + 9) << 4);
                    c = *rBuf++;
                    v |= c < 0x3A ? (byte)(c & 0xf) : (byte)((c + 9) & 0xf);
                    *rRes = v;
                    rRes++;
                }
            }
        }
    }

    private static void DecodeHexSpanValidatedInternal(int start, ReadOnlySpan<char> hex, Span<byte> target)
    {
        for (int i = start, j = 0; i + 1 < hex.Length; i += 2, j++)
        {
            char c = hex[i];
            if (c > 0x66) throw new ArgumentException($"Illegal character {c} at position {i}", nameof(hex));
            if (c > 0x60) target[j] = (byte)((c + 9) << 4);
            else if (c > 0x46) throw new ArgumentException($"Illegal character {c} at position {i}", nameof(hex));
            else if (c > 0x40) target[j] = (byte)((c + 9) << 4);
            else if (c > 0x39) throw new ArgumentException($"Illegal character {c} at position {i}", nameof(hex));
            else if (c > 0x2F) target[j] = (byte)(c << 4);
            else throw new ArgumentException($"Illegal character {c} at position {i}", nameof(hex));
            c = hex[i + 1];
            if (c > 0x66) throw new ArgumentException($"Illegal character {c} at position {i + 1}", nameof(hex));
            if (c > 0x60) target[j] += (byte)((c + 9) & 0xf);
            else if (c > 0x46) throw new ArgumentException($"Illegal character {c} at position {i + 1}", nameof(hex));
            else if (c > 0x40) target[j] += (byte)((c + 9) & 0xf);
            else if (c > 0x39) throw new ArgumentException($"Illegal character {c} at position {i + 1}", nameof(hex));
            else if (c > 0x2F) target[j] += (byte)(c & 0xf);
            else throw new ArgumentException($"Illegal character {c} at position {i + 1}", nameof(hex));
        }
    }

    private static void DecodeHexSpanNonValidatedInternal(int start, ReadOnlySpan<char> hex, Span<byte> target)
    {
        for (int i = start, j = 0; i + 1 < hex.Length; i += 2, j++)
        {
            char c = hex[i];
            target[j] = c < 0x3A ? (byte)(c << 4) : (byte)((c + 9) << 4);
            c = hex[i + 1];
            target[j] += c < 0x3A ? (byte)(c & 0xf) : (byte)((c + 9) & 0xf);
        }
    }

    private readonly record struct TestPair(string Key, byte[] Result);

/*
// * Summary *

BenchmarkDotNet=v0.13.1, OS=macOS Monterey 12.5.1 (21G83) [Darwin 21.6.0]
Apple M1, 1 CPU, 8 logical and 8 physical cores
.NET SDK=6.0.301
  [Host]     : .NET 6.0.6 (6.0.622.26707), Arm64 RyuJIT
  DefaultJob : .NET 6.0.6 (6.0.622.26707), Arm64 RyuJIT


|                 Method | MessageSize |             Mean |          Error |         StdDev |
|----------------------- |------------ |-----------------:|---------------:|---------------:|
|  Convert_FromHexString |          16 |         31.51 ns |       0.342 ns |       0.320 ns |
|     DecodePtrValidated |          16 |         38.20 ns |       0.310 ns |       0.290 ns |
|  DecodePtrNonValidated |          16 |         28.42 ns |       0.047 ns |       0.044 ns |
|    DecodeSpanValidated |          16 |         46.23 ns |       0.065 ns |       0.054 ns |
| DecodeSpanNonValidated |          16 |         30.98 ns |       0.044 ns |       0.034 ns |
|  Convert_FromHexString |          32 |         57.89 ns |       0.281 ns |       0.263 ns |
|     DecodePtrValidated |          32 |         69.47 ns |       0.062 ns |       0.055 ns |
|  DecodePtrNonValidated |          32 |         50.31 ns |       0.217 ns |       0.181 ns |
|    DecodeSpanValidated |          32 |         84.65 ns |       0.153 ns |       0.128 ns |
| DecodeSpanNonValidated |          32 |         54.86 ns |       0.169 ns |       0.141 ns |
|  Convert_FromHexString |        4096 |      6,692.64 ns |       4.376 ns |       3.879 ns |
|     DecodePtrValidated |        4096 |     21,169.14 ns |     419.367 ns |     392.276 ns |
|  DecodePtrNonValidated |        4096 |      6,286.89 ns |      71.182 ns |      66.583 ns |
|    DecodeSpanValidated |        4096 |     17,000.77 ns |     952.073 ns |   2,807.209 ns |
| DecodeSpanNonValidated |        4096 |      6,931.09 ns |      22.271 ns |      17.387 ns |
|  Convert_FromHexString |      262144 |    439,745.80 ns |   1,509.915 ns |   1,412.375 ns |
|     DecodePtrValidated |      262144 |  1,665,318.39 ns |   8,210.439 ns |   7,680.050 ns |
|  DecodePtrNonValidated |      262144 |  1,433,190.66 ns |   7,922.406 ns |   7,410.623 ns |
|    DecodeSpanValidated |      262144 |  1,966,295.61 ns |  24,765.899 ns |  21,954.324 ns |
| DecodeSpanNonValidated |      262144 |  1,758,831.15 ns |  10,341.931 ns |   9,673.849 ns |
|  Convert_FromHexString |     4194304 |  7,064,896.85 ns |   9,138.092 ns |   8,547.777 ns |
|     DecodePtrValidated |     4194304 | 26,630,571.72 ns | 131,118.003 ns | 116,232.694 ns |
|  DecodePtrNonValidated |     4194304 | 23,117,221.68 ns | 161,744.254 ns | 151,295.675 ns |
|    DecodeSpanValidated |     4194304 | 31,271,392.02 ns | 279,810.520 ns | 261,734.934 ns |
| DecodeSpanNonValidated |     4194304 | 28,245,757.55 ns | 154,299.789 ns | 144,332.118 ns |

// * Warnings *
MultimodalDistribution
  HexDecode.DecodeSpanValidated: Default -> It seems that the distribution is bimodal (mValue = 3.65)

// * Hints *
Outliers
  HexDecode.DecodePtrValidated: Default     -> 2 outliers were detected (38.76 ns, 38.76 ns)
  HexDecode.DecodeSpanValidated: Default    -> 2 outliers were removed (47.64 ns, 47.93 ns)
  HexDecode.DecodeSpanNonValidated: Default -> 3 outliers were removed (32.39 ns..32.97 ns)
  HexDecode.DecodePtrValidated: Default     -> 1 outlier  was  removed (71.67 ns)
  HexDecode.DecodePtrNonValidated: Default  -> 2 outliers were removed (52.42 ns, 53.28 ns)
  HexDecode.DecodeSpanValidated: Default    -> 2 outliers were removed (87.49 ns, 88.68 ns)
  HexDecode.DecodeSpanNonValidated: Default -> 2 outliers were removed (56.53 ns, 56.59 ns)
  HexDecode.Convert_FromHexString: Default  -> 1 outlier  was  removed (6.82 us)
  HexDecode.DecodeSpanNonValidated: Default -> 3 outliers were removed (7.24 us..7.32 us)
  HexDecode.Convert_FromHexString: Default  -> 1 outlier  was  detected (434.80 us)
  HexDecode.DecodePtrValidated: Default     -> 1 outlier  was  detected (1.65 ms)
  HexDecode.DecodeSpanValidated: Default    -> 1 outlier  was  removed (2.09 ms)
  HexDecode.DecodeSpanNonValidated: Default -> 1 outlier  was  detected (1.74 ms)
  HexDecode.DecodePtrValidated: Default     -> 1 outlier  was  removed (27.02 ms)

// * Legends *
  MessageSize : Value of the 'MessageSize' parameter
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  1 ns        : 1 Nanosecond (0.000000001 sec)

 */
}
