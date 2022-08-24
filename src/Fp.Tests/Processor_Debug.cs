using System;
using System.Text;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Debug : ProcessorTestBase
{
    [Test]
    public void MemLabel_DebugTrue_HasAssignment()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        var results = P.MemLabel(data, pattern);
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(1));
    }

    [Test]
    public void MemLabel_DebugFalse_HasNoAssignment()
    {
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        var results = P.MemLabel(data, pattern);
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(0));
    }

    // TODO
}
