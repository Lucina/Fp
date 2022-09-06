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
#if NET6_0_OR_GREATER

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

#endif

    private static int GetAlignmentStart(ulong ptr, uint alignment, int length)
    {
        return Math.Min((int)unchecked((alignment - ptr) % alignment), length);
    }

    /// <summary>
    /// Checks if a buffer contains at least one element aligned to the specified number of bytes.
    /// </summary>
    /// <param name="buffer">Buffer to check.</param>
    /// <param name="alignment">Alignment value in bytes.</param>
    /// <returns>True if buffer has at least one aligned value.</returns>
    /// <remarks>
    /// This is intended for use as a precondition for using intrinsics.
    /// </remarks>
    public static unsafe bool ContainsAtLeastOneAligned(ReadOnlySpan<byte> buffer, uint alignment)
    {
        fixed (byte* p = buffer)
        {
            return buffer.Length - GetAlignmentStart((ulong)p, alignment, buffer.Length) >= alignment;
        }
    }

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
}
