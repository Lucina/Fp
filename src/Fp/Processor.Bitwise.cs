using System;
using System.Diagnostics.CodeAnalysis;
#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
#endif

namespace Fp;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class Processor
{
    #region Bitwise utilities

    #region Base functions

    /// <summary>
    /// Applies AND to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">AND value.</param>
    public static void ApplyAnd(Span<byte> span, byte value)
    {
#if NET6_0_OR_GREATER
        if (Avx2.IsSupported)
            ApplyAndAvx2(span, value);
        else if (Sse2.IsSupported)
            ApplyAndSse2(span, value);
        else if (AdvSimd.IsSupported)
            ApplyAndArm(span, value);
        else
            ApplyAndFallback(span, value);
#else
        ApplyAndFallback(span, value);
#endif
    }

    /// <summary>
    /// Applies AND to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">AND value.</param>
    /// <param name="behaviour">Key behaviour.</param>
    public static void ApplyAnd(Span<byte> span, ReadOnlySpan<byte> value, SequenceBehaviour behaviour)
    {
#if NET6_0_OR_GREATER
        // TODO intrinsics
        ApplyAndFallback(span, value, behaviour);
#else
        ApplyAndFallback(span, value, behaviour);
#endif
    }

    /// <summary>
    /// Applies OR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">AND value.</param>
    public static void ApplyOr(Span<byte> span, byte value)
    {
#if NET6_0_OR_GREATER
        if (Avx2.IsSupported)
            ApplyOrAvx2(span, value);
        else if (Sse2.IsSupported)
            ApplyOrSse2(span, value);
        else if (AdvSimd.IsSupported)
            ApplyOrArm(span, value);
        else
            ApplyOrFallback(span, value);
#else
        ApplyOrFallback(span, value);
#endif
    }

    /// <summary>
    /// Applies OR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">AND value.</param>
    /// <param name="behaviour">Key behaviour.</param>
    public static void ApplyOr(Span<byte> span, ReadOnlySpan<byte> value, SequenceBehaviour behaviour)
    {
#if NET6_0_OR_GREATER
        // TODO intrinsics
        ApplyOrFallback(span, value, behaviour);
#else
        ApplyOrFallback(span, value, behaviour);
#endif
    }

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
            ApplyXorArm(span, value);
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

    #endregion

#if NET6_0_OR_GREATER

    #region ARM

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

    /// <summary>
    /// Applies AND to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">AND value.</param>
    public static unsafe void ApplyAndArm(Span<byte> span, byte value)
    {
        const int split = 128 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            #region First part

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] &= value;
                i++;
            }

            if (kill1Idx == l) return;

            #endregion

            #region Arm

            var src = FillVector128AdvSimd(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                AdvSimd.Store(pSource + i, AdvSimd.And(AdvSimd.LoadVector128(pSource + i), src));
                i += split;
            }

            #endregion

            #region Last part

            while (i < span.Length)
            {
                pSource[i] &= value;
                i++;
            }

            #endregion
        }
    }

    /// <summary>
    /// Applies OR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">OR value.</param>
    public static unsafe void ApplyOrArm(Span<byte> span, byte value)
    {
        const int split = 128 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            #region First part

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] &= value;
                i++;
            }

            if (kill1Idx == l) return;

            #endregion

            #region Arm

            var src = FillVector128AdvSimd(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                AdvSimd.Store(pSource + i, AdvSimd.Or(AdvSimd.LoadVector128(pSource + i), src));
                i += split;
            }

            #endregion

            #region Last part

            while (i < span.Length)
            {
                pSource[i] &= value;
                i++;
            }

            #endregion
        }
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    public static unsafe void ApplyXorArm(Span<byte> span, byte value)
    {
        const int split = 128 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            #region First part

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] ^= value;
                i++;
            }

            if (kill1Idx == l) return;

            #endregion

            #region Arm

            var src = FillVector128AdvSimd(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                AdvSimd.Store(pSource + i, AdvSimd.Xor(AdvSimd.LoadVector128(pSource + i), src));
                i += split;
            }

            #endregion

            #region Last part

            while (i < span.Length)
            {
                pSource[i] ^= value;
                i++;
            }

            #endregion
        }
    }

    #endregion

    #region SSE2

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

    /// <summary>
    /// Applies AND to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">AND value.</param>
    public static unsafe void ApplyAndSse2(Span<byte> span, byte value)
    {
        const int split = 128 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            #region First part

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] &= value;
                i++;
            }

            if (kill1Idx == l) return;

            #endregion

            #region Sse2

            var src = FillVector128Sse2(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                Sse2.StoreAligned(pSource + i, Sse2.And(Sse2.LoadAlignedVector128(pSource + i), src));
                i += split;
            }

            #endregion

            #region Last part

            while (i < span.Length)
            {
                pSource[i] &= value;
                i++;
            }

            #endregion
        }
    }

    /// <summary>
    /// Applies OR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">OR value.</param>
    public static unsafe void ApplyOrSse2(Span<byte> span, byte value)
    {
        const int split = 128 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            #region First part

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] |= value;
                i++;
            }

            if (kill1Idx == l) return;

            #endregion

            #region Sse2

            var src = FillVector128Sse2(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                Sse2.StoreAligned(pSource + i, Sse2.Or(Sse2.LoadAlignedVector128(pSource + i), src));
                i += split;
            }

            #endregion

            #region Last part

            while (i < span.Length)
            {
                pSource[i] |= value;
                i++;
            }

            #endregion
        }
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    public static unsafe void ApplyXorSse2(Span<byte> span, byte value)
    {
        const int split = 128 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            #region First part

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] ^= value;
                i++;
            }

            if (kill1Idx == l) return;

            #endregion

            #region Sse2

            var src = FillVector128Sse2(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                Sse2.StoreAligned(pSource + i, Sse2.Xor(Sse2.LoadAlignedVector128(pSource + i), src));
                i += split;
            }

            #endregion

            #region Last part

            while (i < span.Length)
            {
                pSource[i] ^= value;
                i++;
            }

            #endregion
        }
    }

    #endregion

    #region AVX2

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

    /// <summary>
    /// Applies AND to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">AND value.</param>
    public static unsafe void ApplyAndAvx2(Span<byte> span, byte value)
    {
        const int split = 256 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            #region First part

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] &= value;
                i++;
            }

            if (kill1Idx == l) return;

            #endregion

            #region Avx

            var src = FillVector256Avx(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                Avx.StoreAligned(pSource + i, Avx2.And(Avx.LoadAlignedVector256(pSource + i), src));
                i += split;
            }

            #endregion

            #region Last part

            while (i < span.Length)
            {
                pSource[i] &= value;
                i++;
            }

            #endregion
        }
    }

    /// <summary>
    /// Applies OR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">OR value.</param>
    public static unsafe void ApplyOrAvx2(Span<byte> span, byte value)
    {
        const int split = 256 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            #region First part

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] |= value;
                i++;
            }

            if (kill1Idx == l) return;

            #endregion

            #region Avx

            var src = FillVector256Avx(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                Avx.StoreAligned(pSource + i, Avx2.Or(Avx.LoadAlignedVector256(pSource + i), src));
                i += split;
            }

            #endregion

            #region Last part

            while (i < span.Length)
            {
                pSource[i] |= value;
                i++;
            }

            #endregion
        }
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    public static unsafe void ApplyXorAvx2(Span<byte> span, byte value)
    {
        const int split = 256 / 8;
        fixed (byte* pSource = span)
        {
            int i = 0;
            int l = span.Length;

            #region First part

            int kill1Idx = Math.Min((int)unchecked((ulong)(split - (long)pSource) % split), l);
            while (i < kill1Idx)
            {
                pSource[i] ^= value;
                i++;
            }

            if (kill1Idx == l) return;

            #endregion

            #region Avx

            var src = FillVector256Avx(value);
            int kill2Idx = l - l % split;
            while (i < kill2Idx)
            {
                Avx.StoreAligned(pSource + i, Avx2.Xor(Avx.LoadAlignedVector256(pSource + i), src));
                i += split;
            }

            #endregion

            #region Last part

            while (i < span.Length)
            {
                pSource[i] ^= value;
                i++;
            }

            #endregion
        }
    }

    #endregion

#endif

    #region Fallback

    /// <summary>
    /// Applies AND to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">AND value.</param>
    public static void ApplyAndFallback(Span<byte> span, byte value)
    {
        for (int i = 0; i < span.Length; i++) span[i] &= value;
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    /// <param name="behaviour">Key behaviour.</param>
    public static void ApplyAndFallback(Span<byte> span, ReadOnlySpan<byte> value, SequenceBehaviour behaviour)
    {
        int i = 0, sl = span.Length, kl = value.Length;
        for (; i < sl && i < kl; i++) span[i] &= value[i];
        if (behaviour == SequenceBehaviour.Truncate) return;
        for (; i < sl; i++) span[i] &= value[i % kl];
    }

    /// <summary>
    /// Applies OR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">OR value.</param>
    public static void ApplyOrFallback(Span<byte> span, byte value)
    {
        for (int i = 0; i < span.Length; i++) span[i] |= value;
    }

    /// <summary>
    /// Applies XOR to memory.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="value">XOR value.</param>
    /// <param name="behaviour">Key behaviour.</param>
    public static void ApplyOrFallback(Span<byte> span, ReadOnlySpan<byte> value, SequenceBehaviour behaviour)
    {
        int i = 0, sl = span.Length, kl = value.Length;
        for (; i < sl && i < kl; i++) span[i] |= value[i];
        if (behaviour == SequenceBehaviour.Truncate) return;
        for (; i < sl; i++) span[i] |= value[i % kl];
    }

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

    #endregion

    /// <summary>
    /// Transform delegate.
    /// </summary>
    /// <param name="input">Input value.</param>
    /// <param name="index">Index.</param>
    public delegate byte TransformDelegate(byte input, int index);

    /// <summary>
    /// Transforms memory region.
    /// </summary>
    /// <param name="span">Memory to modify.</param>
    /// <param name="func">Transformation delegate.</param>
    public static void ApplyTransform(Span<byte> span, TransformDelegate func)
    {
        for (int i = 0; i < span.Length; i++)
            span[i] = func(span[i], i);
    }

    #endregion
}
