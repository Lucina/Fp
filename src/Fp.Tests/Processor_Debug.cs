using System;
using System.Linq;
using System.Text;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Debug : ProcessorTestBase
{
    [Test]
    public void MemClear_Clears()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        P.MemLabel(data, pattern);
        Assert.That(P.MemAnnotations.Count, Is.Not.EqualTo(0));
        P.MemClear();
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(0));
    }

    [Test]
    public void MemLabel_PosDebugTrue_HasAssignment()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        P.MemLabel(data, 0, 1);
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(1));
        var kv = P.MemAnnotations.Single();
        Assert.That(kv.Key, Is.EqualTo((ReadOnlyMemory<byte>)data.AsMemory()));
        Assert.That(kv.Value.Count, Is.EqualTo(1));
        Assert.That(kv.Value.Values[0].Offset, Is.EqualTo(0));
        Assert.That(kv.Value.Values[0].Length, Is.EqualTo(1));
    }

    [Test]
    public void MemLabel_PatternDebugTrue_HasAssignment()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        var results = P.MemLabel(data, pattern);
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(1));
        var kv = P.MemAnnotations.Single();
        Assert.That(kv.Key, Is.EqualTo((ReadOnlyMemory<byte>)data.AsMemory()));
        Assert.That(kv.Value.Count, Is.EqualTo(2));
        Assert.That(kv.Value.Values[0].Offset, Is.EqualTo(4));
        Assert.That(kv.Value.Values[1].Offset, Is.EqualTo(7));
    }

    [Test]
    public void MemLabel_PosDebugFalse_HasNoAssignment()
    {
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        P.MemLabel(data, 0, 1);
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(0));
    }

    [Test]
    public void MemLabel_PatternDebugFalse_HasNoAssignment()
    {
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        var results = P.MemLabel(data, pattern);
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(0));
    }

    [Test]
    public void MemLabel_PosDebugTrueAndSameOffset_NoRepeatAssignment()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        P.MemLabel(data, 0, 1, "a");
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(1));
        var kv = P.MemAnnotations.Single();
        Assert.That(kv.Key, Is.EqualTo((ReadOnlyMemory<byte>)data.AsMemory()));
        Assert.That(kv.Value.Count, Is.EqualTo(1));
        Assert.That(kv.Value[0].Label, Is.EqualTo("a"));
        P.MemLabel(data, 0, 1, "b");
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(1));
        var kv2 = P.MemAnnotations.Single();
        Assert.That(kv2.Key, Is.EqualTo((ReadOnlyMemory<byte>)data.AsMemory()));
        Assert.That(kv2.Value.Count, Is.EqualTo(1));
        Assert.That(kv.Value[0].Label, Is.EqualTo("a"));
    }

    [Test]
    public void MemLabel_PatternDebugTrueAndSameOffset_NoRepeatAssignment()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        P.MemLabel(data, new[] { (byte)'h' }, "a");
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(1));
        var kv = P.MemAnnotations.Single();
        Assert.That(kv.Key, Is.EqualTo((ReadOnlyMemory<byte>)data.AsMemory()));
        Assert.That(kv.Value.Count, Is.EqualTo(1));
        Assert.That(kv.Value[0].Label, Is.EqualTo("a"));
        P.MemLabel(data, new[] { (byte)'h' }, "b");
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(1));
        var kv2 = P.MemAnnotations.Single();
        Assert.That(kv2.Key, Is.EqualTo((ReadOnlyMemory<byte>)data.AsMemory()));
        Assert.That(kv2.Value.Count, Is.EqualTo(1));
        Assert.That(kv.Value[0].Label, Is.EqualTo("a"));
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
        var kv = P.MemAnnotations.Single();
        Assert.That(kv.Key, Is.EqualTo((ReadOnlyMemory<byte>)data.AsMemory()));
        Assert.That(kv.Value.Count, Is.EqualTo(2));
        Assert.That(kv.Value.Values[0].Offset, Is.EqualTo(6));
        Assert.That(kv.Value.Values[0].Length, Is.EqualTo(6));
        Assert.That(kv.Value.Values[1].Offset, Is.EqualTo(19));
        Assert.That(kv.Value.Values[1].Length, Is.EqualTo(8));
    }

    [Test]
    public void MemPrint_Plain_FormatMatches()
    {
        string expected = @"
0x0000000000 00 01 02 03
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.That(sbl.GetContent(), Is.EqualTo(expected));
    }

    [Test]
    public void MemLabel_PatternAnnotatedWhenDebugFalse_PrintNoAnnotation()
    {
        string expected = @"
0x0000000000 00 01 02 03
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, new byte[] { 0 }, "label1");
        P.Debug = true;
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.That(sbl.GetContent(), Is.EqualTo(expected));
    }

    [Test]
    public void MemLabel_PosAnnotatedWhenDebugFalse_PrintNoAnnotation()
    {
        string expected = @"
0x0000000000 00 01 02 03
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0, 4, "label1");
        P.Debug = true;
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.That(sbl.GetContent(), Is.EqualTo(expected));
    }

    [Test]
    public void MemPrint_Annotated_FormatMatches()
    {
        string expected = @"
0x0000000000 00 01 02 03 label1
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0, 4, "label1");
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.That(sbl.GetContent(), Is.EqualTo(expected));
    }

    [Test]
    public void MemPrint_AnnotatedLongerThanMax16_LengthTrim()
    {
        string expected = @"
0x0000000000 00 01 02 03 0123456789abcdef
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0, 4, "0123456789abcdefg");
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.That(sbl.GetContent(), Is.EqualTo(expected));
    }

    [Test]
    public void MemPrint_OverlapDifferentStart_CarryOver()
    {
        string expected = @"
0x0000000000 00 01 02 03 label1
0x0000000004 04 05 06 07 label2
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0, 2, "label1");
        P.MemLabel(data, 1, 1, "label2");
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.That(sbl.GetContent(), Is.EqualTo(expected));
    }

    [Test]
    public void MemPrint_DebugFalse_Noop()
    {
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.That(sbl.GetContent(), Is.EqualTo(""));
    }

    [Test]
    public void MemPrint_AnnotatedMultipleOnSameLine_CarryOver()
    {
        string expected = @"
0x0000000000 00 01 02 03 label1
0x0000000004 04 05 06 07 label2
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10         ^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0, 2, "label1");
        P.MemLabel(data, 2, 2, "label2");
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.That(sbl.GetContent(), Is.EqualTo(expected));
    }

    [Test]
    public void MemPrint_AnnotatedMultipleOnSameLineAfterEnd_CarryOver()
    {
        string expected = @"
0x0000000000 00 01 02 03
0x0000000004 04 05 06 07
0x0000000008 08 09 0A 0B
0x000000000C 0C 0D 0E 0F
0x0000000010 10 11       label1
                         label2^

".TrimStart('\r', '\n').Replace("\r", "").Replace("^", "");
        P.Debug = true;
        byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };
        var sbl = new StringBuilderLog { Delimiter = "\n" };
        P.LogWriter = sbl;
        P.MemLabel(data, 0x10, 1, "label1");
        P.MemLabel(data, 0x11, 1, "label2");
        P.MemPrint(data, space: true, pow2Modulus: false, displayWidth: 41);
        Assert.That(sbl.GetContent(), Is.EqualTo(expected));
    }

    // TODO
}
