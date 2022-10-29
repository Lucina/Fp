using System.IO;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Strings : ProcessorTestBase
{
    private const string Message = "pontoon";
    private const string MessageBOM = "\ufeffpontoon";
    private static readonly byte[] Message_UTF8 = { 0x70, 0x6F, 0x6E, 0x74, 0x6F, 0x6F, 0x6E };
    private static readonly byte[] Message_UTF8Null = { 0x70, 0x6F, 0x6E, 0x74, 0x6F, 0x6F, 0x6E, 0x00 };
    private static readonly byte[] Message_UTF16LE = { 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00 };
    private static readonly byte[] Message_UTF16LENull = { 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00, 0x00 };
    private static readonly byte[] Message_UTF16BE = { 0x00, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E };
    private static readonly byte[] Message_UTF16BENull = { 0x00, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00 };
    private static readonly byte[] Message_UTF16LEBOM = { 0xFF, 0xFE, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00 };
    private static readonly byte[] Message_UTF16LEBOMNull = { 0xFF, 0xFE, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00, 0x00 };
    private static readonly byte[] Message_UTF16BEBOM = { 0xFE, 0xFF, 0x00, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E };
    private static readonly byte[] Message_UTF16BEBOMNull = { 0xFE, 0xFF, 0x00, 0x70, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x74, 0x00, 0x6F, 0x00, 0x6F, 0x00, 0x6E, 0x00, 0x00 };

    [Test]
    public void WriteUtf8String_Basic_Correct()
    {
        MemoryStream ms = new();
        P.WriteUtf8String(Message, false, ms);
        Assert.That(ms.ToArray(), Is.EqualTo(Message_UTF8));
        ms.SetLength(0);
        P.WriteUtf8String(Message, true, ms);
        Assert.That(ms.ToArray(), Is.EqualTo(Message_UTF8Null));
    }

    [Test]
    public void ReadUtf8String_Basic_Correct()
    {
        MemoryStream ms = new();
        ms.Write(Message_UTF8);
        ms.Position = 0;
        Assert.That(P.ReadUtf8String(ms, out int read, out int numBytes), Is.EqualTo(Message));
        Assert.That(read, Is.EqualTo(Message_UTF8.Length));
        Assert.That(numBytes, Is.EqualTo(Message_UTF8.Length));
        ms.SetLength(0);
        ms.Write(Message_UTF8Null);
        ms.Position = 0;
        Assert.That(P.ReadUtf8String(ms, out read, out numBytes), Is.EqualTo(Message));
        Assert.That(read, Is.EqualTo(Message_UTF8Null.Length));
        Assert.That(numBytes, Is.EqualTo(Message_UTF8.Length));
    }

    [Test]
    public void WriteUtf16String_BasicLE_Correct()
    {
        MemoryStream ms = new();
        P.WriteUtf16String(Message, false, false, false, ms);
        Assert.That(ms.ToArray(), Is.EqualTo(Message_UTF16LE));
        ms.SetLength(0);
        P.WriteUtf16String(Message, true, false, false, ms);
        Assert.That(ms.ToArray(), Is.EqualTo(Message_UTF16LENull));
    }

    [Test]
    public void ReadUtf16String_BasicLE_Correct()
    {
        MemoryStream ms = new();
        ms.Write(Message_UTF16LE);
        ms.Position = 0;
        Assert.That(P.ReadUtf16String(ms, out int read, out int numBytes), Is.EqualTo(Message));
        Assert.That(read, Is.EqualTo(Message_UTF16LE.Length));
        Assert.That(numBytes, Is.EqualTo(Message_UTF16LE.Length));
        ms.SetLength(0);
        ms.Write(Message_UTF16LENull);
        ms.Position = 0;
        Assert.That(P.ReadUtf16String(ms, out read, out numBytes), Is.EqualTo(Message));
        Assert.That(read, Is.EqualTo(Message_UTF16LENull.Length));
        Assert.That(numBytes, Is.EqualTo(Message_UTF16LE.Length));
    }

    [Test]
    public void WriteUtf16String_BasicBE_Correct()
    {
        MemoryStream ms = new();
        P.WriteUtf16String(Message, false, true, false, ms);
        Assert.That(ms.ToArray(), Is.EqualTo(Message_UTF16BE));
        ms.SetLength(0);
        P.WriteUtf16String(Message, true, true, false, ms);
        Assert.That(ms.ToArray(), Is.EqualTo(Message_UTF16BENull));
    }

    [Test]
    public void ReadUtf16String_BasicBE_Correct()
    {
        MemoryStream ms = new();
        ms.Write(Message_UTF16BE);
        ms.Position = 0;
        Assert.That(P.ReadUtf16String(ms, out int read, out int numBytes), Is.EqualTo(Message));
        Assert.That(read, Is.EqualTo(Message_UTF16BE.Length));
        Assert.That(numBytes, Is.EqualTo(Message_UTF16BE.Length));
        ms.SetLength(0);
        ms.Write(Message_UTF16BENull);
        ms.Position = 0;
        Assert.That(P.ReadUtf16String(ms, out read, out numBytes), Is.EqualTo(Message));
        Assert.That(read, Is.EqualTo(Message_UTF16BENull.Length));
        Assert.That(numBytes, Is.EqualTo(Message_UTF16BE.Length));
    }

    [Test]
    public void WriteUtf16String_LEBOM_Correct()
    {
        MemoryStream ms = new();
        P.WriteUtf16String(Message, false, false, true, ms);
        Assert.That(ms.ToArray(), Is.EqualTo(Message_UTF16LEBOM));
        ms.SetLength(0);
        P.WriteUtf16String(Message, true, false, true, ms);
        Assert.That(ms.ToArray(), Is.EqualTo(Message_UTF16LEBOMNull));
    }

    [Test]
    public void ReadUtf16String_LEBOM_Correct()
    {
        MemoryStream ms = new();
        ms.Write(Message_UTF16LEBOM);
        ms.Position = 0;
        Assert.That(P.ReadUtf16String(ms, out int read, out int numBytes), Is.EqualTo(MessageBOM));
        Assert.That(read, Is.EqualTo(Message_UTF16LEBOM.Length));
        Assert.That(numBytes, Is.EqualTo(Message_UTF16LEBOM.Length));
        ms.SetLength(0);
        ms.Write(Message_UTF16LEBOMNull);
        ms.Position = 0;
        Assert.That(P.ReadUtf16String(ms, out read, out numBytes), Is.EqualTo(MessageBOM));
        Assert.That(read, Is.EqualTo(Message_UTF16LEBOMNull.Length));
        Assert.That(numBytes, Is.EqualTo(Message_UTF16LEBOM.Length));
    }

    [Test]
    public void WriteUtf16String_BEBOM_Correct()
    {
        MemoryStream ms = new();
        P.WriteUtf16String(Message, false, true, true, ms);
        Assert.That(ms.ToArray(), Is.EqualTo(Message_UTF16BEBOM));
        ms.SetLength(0);
        P.WriteUtf16String(Message, true, true, true, ms);
        Assert.That(ms.ToArray(), Is.EqualTo(Message_UTF16BEBOMNull));
    }

    [Test]
    public void ReadUtf16String_BEBOM_Correct()
    {
        MemoryStream ms = new();
        ms.Write(Message_UTF16BEBOM);
        ms.Position = 0;
        Assert.That(P.ReadUtf16String(ms, out int read, out int numBytes), Is.EqualTo(MessageBOM));
        Assert.That(read, Is.EqualTo(Message_UTF16BEBOM.Length));
        Assert.That(numBytes, Is.EqualTo(Message_UTF16BEBOM.Length));
        ms.SetLength(0);
        ms.Write(Message_UTF16BEBOMNull);
        ms.Position = 0;
        Assert.That(P.ReadUtf16String(ms, out read, out numBytes), Is.EqualTo(MessageBOM));
        Assert.That(read, Is.EqualTo(Message_UTF16BEBOMNull.Length));
        Assert.That(numBytes, Is.EqualTo(Message_UTF16BEBOM.Length));
    }

    // TODO
}
