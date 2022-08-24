using System;
using System.Linq;
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
        var kv = P.MemAnnotations.First();
        Assert.That(kv.Key, Is.EqualTo((ReadOnlyMemory<byte>)data.AsMemory()));
        Assert.That(kv.Value.Count, Is.EqualTo(2));
        Assert.That(kv.Value.Values[0].Offset, Is.EqualTo(4));
        Assert.That(kv.Value.Values[1].Offset, Is.EqualTo(7));
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

    [Test]
    public void MemLabel_RepeatingSequence_HasConcatenated()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("super_ababab_drome_abababab");
        Span<byte> pattern = stackalloc byte[] { (byte)'a', (byte)'b' };
        var results = P.MemLabel(data, pattern);
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(1));
        var kv = P.MemAnnotations.First();
        Assert.That(kv.Key, Is.EqualTo((ReadOnlyMemory<byte>)data.AsMemory()));
        Assert.That(kv.Value.Count, Is.EqualTo(2));
        Assert.That(kv.Value.Values[0].Offset, Is.EqualTo(6));
        Assert.That(kv.Value.Values[0].Length, Is.EqualTo(6));
        Assert.That(kv.Value.Values[1].Offset, Is.EqualTo(19));
        Assert.That(kv.Value.Values[1].Length, Is.EqualTo(8));
    }

    // TODO
}
