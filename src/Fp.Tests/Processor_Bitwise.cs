using System;
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
        if (!AdvSimd.IsSupported) Assert.Inconclusive("AdvSimd intrinsics not supported");

        Span<byte> arr = new byte[1097];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);


        Processor.ApplyXorArm(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorArm_Misaligned_MatchesExpected()
    {
        if (!AdvSimd.IsSupported) Assert.Inconclusive("AdvSimd intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = new byte[1097].AsSpan()[14..];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorArm(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    [Test]
    public void SingleByteApplyXorSse2_LargeBuffer_MatchesExpected()
    {
        if (!Sse2.IsSupported) Assert.Inconclusive("Sse2 intrinsics not supported");

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
        if (!Sse2.IsSupported) Assert.Inconclusive("Sse2 intrinsics not supported");

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
        if (!Avx2.IsSupported) Assert.Inconclusive("Avx2 intrinsics not supported");

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
        if (!Avx2.IsSupported) Assert.Inconclusive("Avx2 intrinsics not supported");

        // Cut somewhere in 0..31 for misalignment
        Span<byte> arr = new byte[1097].AsSpan()[14..];
        Random.Shared.NextBytes(arr);
        Span<byte> arr2 = new byte[arr.Length];
        arr.CopyTo(arr2);

        Processor.ApplyXorAvx2(arr, XorByte);
        Processor.ApplyXorFallback(arr2, XorByte);

        Assert.That(arr.SequenceEqual(arr2), Is.True);
    }

    internal static void ApplyXor(Span<byte> span, ReadOnlySpan<byte> pattern, SequenceBehaviour sequenceBehaviour = SequenceBehaviour.Repeat)
    {
        switch (sequenceBehaviour)
        {
            case SequenceBehaviour.Truncate:
                if (pattern.Length < span.Length)
                    for (int i = 0; i < pattern.Length; i++)
                        span[i] ^= pattern[i];
                else
                    for (int i = 0; i < span.Length; i++)
                        span[i] ^= pattern[i];
                break;
            case SequenceBehaviour.Repeat:
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
            default:
                throw new ArgumentOutOfRangeException(nameof(sequenceBehaviour), sequenceBehaviour, null);
        }
    }

    // TODO
}
