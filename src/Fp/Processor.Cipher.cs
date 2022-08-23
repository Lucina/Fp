using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Fp;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public partial class Processor
{
    #region Decryption utilities

    /// <summary>
    /// Gets padded message length using specified padding mode.
    /// </summary>
    /// <param name="length">Input message length.</param>
    /// <param name="cipherPaddingMode">Padding mode to use.</param>
    /// <param name="blockSize">Cipher block size.</param>
    /// <returns>Padded length of message.</returns>
    public static int GetPaddedLength(int length, CipherPaddingMode cipherPaddingMode, int blockSize) =>
        cipherPaddingMode switch
        {
            CipherPaddingMode.Zero => length + (blockSize - length % blockSize),
            CipherPaddingMode.Iso_Iec_7816_4 or CipherPaddingMode.AnsiX9_23 or CipherPaddingMode.Iso10126 or CipherPaddingMode.Pkcs7 or CipherPaddingMode.Pkcs5 => length + 1 + (blockSize - (length + 1) % blockSize),
            _ => throw new ArgumentOutOfRangeException(nameof(cipherPaddingMode), cipherPaddingMode, null)
        };

    /// <summary>
    /// Gets depadded message length using specified padding mode.
    /// </summary>
    /// <param name="span">Message.</param>
    /// <param name="cipherPaddingMode">Padding mode to use.</param>
    /// <param name="validate">Apply validation, throwing an error on invalid input.</param>
    /// <returns>Depadded length of message.</returns>
    public static int GetDepaddedLength(Span<byte> span, CipherPaddingMode cipherPaddingMode, bool validate = true) =>
        cipherPaddingMode switch
        {
            CipherPaddingMode.Zero => GetDepaddedLengthZero(span),
            CipherPaddingMode.Iso_Iec_7816_4 => GetDepaddedLengthIso_Iec_7816_4(span),
            CipherPaddingMode.AnsiX9_23 => GetDepaddedLengthAnsiX9_23(span, validate),
            CipherPaddingMode.Iso10126 => GetDepaddedLengthLastByteSubtract(span),
            CipherPaddingMode.Pkcs7 or CipherPaddingMode.Pkcs5 => GetDepaddedLengthPkcs5Pkcs7(span, validate),
            _ => throw new ArgumentOutOfRangeException(nameof(cipherPaddingMode), cipherPaddingMode, null)
        };

    private static int GetDepaddedLengthZero(Span<byte> span)
    {
        for (int i = span.Length; i > 0; i--)
            if (span[i - 1] != 0)
                return i;
        return 0;
    }


    private static int GetDepaddedLengthIso_Iec_7816_4(Span<byte> span)
    {
        for (int i = span.Length - 1; i >= 0; i--)
            switch (span[i])
            {
                case 0x00:
                    break;
                case 0x80:
                    return i;
                default:
                    throw new ArgumentException($"Invalid padding byte for {nameof(CipherPaddingMode.Iso_Iec_7816_4)}, need 0x80 or 0x00 but got 0x{span[i]:X2}");
            }
        throw new ArgumentException($"Message is all null bytes and {nameof(CipherPaddingMode.Iso_Iec_7816_4)} requires 0x80 to mark beginning of padding");
    }

    private static int GetDepaddedLengthPkcs5Pkcs7(ReadOnlySpan<byte> span, bool validate)
    {
        if (span.Length == 0) return 0;
        byte l = span[^1];
        if (!validate) return span.Length - l;
        for (int i = span.Length - l; i < span.Length - 1; i++)
            if (span[i] != l)
                throw new InvalidDataException($"Invalid ANSI X9.23 padding byte at 0x{i:X8} (expected {l})");
        return span.Length - l;
    }

    private static int GetDepaddedLengthAnsiX9_23(ReadOnlySpan<byte> span, bool validate)
    {
        if (span.Length == 0) return 0;
        byte l = span[^1];
        if (!validate) return span.Length - l;
        for (int i = span.Length - l; i < span.Length - 1; i++)
            if (span[i] != 0)
                throw new InvalidDataException($"Invalid ANSI X9.23 padding byte at 0x{i:X8} (expected 0)");
        return span.Length - l;
    }

    private static int GetDepaddedLengthLastByteSubtract(ReadOnlySpan<byte> span) =>
        span.Length == 0 ? 0 : span.Length - span[^1];

    /// <summary>
    /// Creates a byte array from a hex string. Hex strings may only be prefixed with "0x".
    /// </summary>
    /// <param name="hex">Hex string to decode.</param>
    /// <param name="validate">Validate characters.</param>
    /// <returns>Array with decoded hex string.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="hex"/> has an odd length.</exception>
    public static byte[] DecodeHex(ReadOnlySpan<char> hex, bool validate = true)
    {
        int byteCount = GetHexByteCount(hex, out int start);
        byte[] res = new byte[byteCount];
        if (validate) DecodeHexPtrValidatedInternal(start, hex, res, byteCount);
        else DecodeHexPtrNonValidatedInternal(start, hex, res, byteCount);
        return res;
    }

    /// <summary>
    /// Creates a byte array from a hex string. Hex strings may only be prefixed with "0x".
    /// </summary>
    /// <param name="hex">Hex string to decode.</param>
    /// <param name="result">Decoded hex buffer.</param>
    /// <param name="validate">Validate characters.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="hex"/> has an invalid length or if <paramref name="result"/> is not long enough for result.</exception>
    public static void DecodeHex(ReadOnlySpan<char> hex, Span<byte> result, bool validate = true)
    {
        int byteCount = GetHexByteCount(hex, out int start);
        if (result.Length < byteCount)
            throw new ArgumentException($"Result buffer is too short ({result.Length} bytes) to contain result of length {byteCount} bytes", nameof(result));
        if (validate) DecodeHexPtrValidatedInternal(start, hex, result, byteCount);
        else DecodeHexPtrNonValidatedInternal(start, hex, result, byteCount);
    }

    /// <summary>
    /// Gets byte count for a hex string, stripping 0[xX] prefix.
    /// </summary>
    /// <param name="hex">Hex buffer.</param>
    /// <param name="start">Expected start of hex payload (skips ahead of 0[xX] prefix).</param>
    /// <returns>Number of bytes expected.</returns>
    /// <exception cref="ArgumentException">Thrown for invalid input length.</exception>
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

    #endregion
}
