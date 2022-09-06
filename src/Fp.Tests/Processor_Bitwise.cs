using System;
using System.Runtime.InteropServices;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Bitwise : ProcessorTestBase
{
    [Test]
    public void ContainsAtLeastOneAligned_LessFromStart_False()
    {
        Span<ulong> v = stackalloc ulong[1];
        Assert.That(Processor.ContainsAtLeastOneAligned(MemoryMarshal.Cast<ulong, byte>(v)[1..], sizeof(ulong)), Is.False);
    }

    [Test]
    public void ContainsAtLeastOneAligned_LessFromEnd_False()
    {
        Span<ulong> v = stackalloc ulong[1];
        Assert.That(Processor.ContainsAtLeastOneAligned(MemoryMarshal.Cast<ulong, byte>(v)[..^1], sizeof(ulong)), Is.False);
    }

    [Test]
    public void ContainsAtLeastOneAligned_Exact_True()
    {
        Span<ulong> v = stackalloc ulong[1];
        Assert.That(Processor.ContainsAtLeastOneAligned(MemoryMarshal.Cast<ulong, byte>(v), sizeof(ulong)), Is.True);
    }

    [Test]
    public void ContainsAtLeastOneAligned_MoreFromStart_True()
    {
        Span<ulong> v = stackalloc ulong[2];
        Assert.That(Processor.ContainsAtLeastOneAligned(MemoryMarshal.Cast<ulong, byte>(v)[1..], sizeof(ulong)), Is.True);
    }

    [Test]
    public void ContainsAtLeastOneAligned_MoreFromEnd_True()
    {
        Span<ulong> v = stackalloc ulong[2];
        Assert.That(Processor.ContainsAtLeastOneAligned(MemoryMarshal.Cast<ulong, byte>(v)[..^1], sizeof(ulong)), Is.True);
    }

    // TODO
}
