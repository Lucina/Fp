using System;
using System.Diagnostics.CodeAnalysis;
#if NET6_0_OR_GREATER
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
#endif

namespace Fp;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class Processor
{
    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    public static void ApplyXor(Span<byte> span, byte value)
    {
#if NET6_0_OR_GREATER
        if (Avx2.IsSupported)
            ApplyXorAvx2(span, value);
        else if (Sse2.IsSupported)
            ApplyXorSse2(span, value);
        else if (AdvSimd.IsSupported)
            ApplyXorAdvSimd(span, value);
        else
            ApplyXorFallback(span, value);
#else
        ApplyXorFallback(span, value);
#endif
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    /// <param name="behaviour">Key behaviour.</param>
    public static void ApplyXor(Span<byte> span, ReadOnlySpan<byte> value, SequenceBehaviour behaviour)
    {
#if NET6_0_OR_GREATER
        // TODO intrinsics
        ApplyXorFallback(span, value, behaviour);
#else
        ApplyXorFallback(span, value, behaviour);
#endif
    }

#if NET6_0_OR_GREATER

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
    /// <param name="behaviour">Key behaviour.</param>
    public static void ApplyXorFallback(Span<byte> span, ReadOnlySpan<byte> value, SequenceBehaviour behaviour)
    {
        int i = 0, sl = span.Length, kl = value.Length;
        for (; i < sl && i < kl; i++) span[i] ^= value[i];
        if (behaviour == SequenceBehaviour.Truncate) return;
        for (; i < sl; i++) span[i] ^= value[i % kl];
    }
}
