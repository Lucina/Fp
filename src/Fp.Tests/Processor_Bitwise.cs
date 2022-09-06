using System;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Bitwise : ProcessorTestBase
{
    private const byte XorByte = 0xd5;

    [Test]
    public void SingleByteApplyXorArm_LargeBuffer_MatchesExpected()
    {
        if (!AdvSimd.IsSupported) Assert.Ignore("AdvSimd intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyXorAdvSimd(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorArm_Misaligned_MatchesExpected()
    {
        if (!AdvSimd.IsSupported) Assert.Ignore("AdvSimd intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = new byte[1097].AsSpan()[14..];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorAdvSimd(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorSse2_LargeBuffer_MatchesExpected()
    {
        if (!Sse2.IsSupported) Assert.Ignore("Sse2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyXorSse2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorSse2_Misaligned_MatchesExpected()
    {
        if (!Sse2.IsSupported) Assert.Ignore("Sse2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = new byte[1097].AsSpan()[14..];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorSse2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorAvx2_LargeBuffer_MatchesExpected()
    {
        if (!Avx2.IsSupported) Assert.Ignore("Avx2 intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyXorAvx2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorAvx2_Misaligned_MatchesExpected()
    {
        if (!Avx2.IsSupported) Assert.Ignore("Avx2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = new byte[1097].AsSpan()[14..];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorAvx2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyXor_SmallBufferTruncate_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[1843];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Truncate);
        ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Truncate);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyXor_LargeBufferTruncate_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[53];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Truncate);
        ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Truncate);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyXor_SmallBufferRepeat_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[91];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[1843];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Repeat);
        ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Repeat);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void BufferApplyXor_LargeBufferRepeat_MatchesExpected()
    {
        if (!Vector.IsHardwareAccelerated) Assert.Ignore("Hardware vector acceleration not supported");
        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> xorArr = new byte[53];
        Random.Shared.NextBytes(xorArr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        ApplyXorVectorized(arr, xorArr, SequenceBehaviour.Repeat);
        ApplyXorFallback(arr2, xorArr, SequenceBehaviour.Repeat);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

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

    // TODO
}
