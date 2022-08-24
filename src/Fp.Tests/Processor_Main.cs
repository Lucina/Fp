using System;
using System.IO;
using System.Text;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Main : ProcessorTestBase
{
    [Test]
    public void DefaultProperties_Correct()
    {
        Assert.That(P.Args, Is.EquivalentTo(Array.Empty<string>()));
        Assert.That(P.LogReceiver, Is.EqualTo(NullLog.Instance));
        Assert.That(P.Preload, Is.EqualTo(false));
        Assert.That(P.Debug, Is.EqualTo(false));
        Assert.That(P.Nop, Is.EqualTo(false));
    }

    [Test]
    public void Prepare_PropertiesInitialized()
    {
        string[] args = { "a", "b" };
        var sbl = new StringBuilderLog();
        var conf = new ProcessorConfiguration(args, preload: true, debug: true, nop: true, logReceiver: sbl);
        P.Prepare(conf);
        Assert.That(P.Args, Is.EquivalentTo(args));
        Assert.That(P.LogReceiver, Is.EqualTo(sbl));
        Assert.That(P.Preload, Is.EqualTo(true));
        Assert.That(P.Debug, Is.EqualTo(true));
        Assert.That(P.Nop, Is.EqualTo(true));
    }

    [Test]
    public void Initialize_InstanceWithArgs_PropertiesInitialized()
    {
        string[] args = { "a", "b" };
        var sbl = new StringBuilderLog();
        var conf = new ProcessorConfiguration(args, preload: true, debug: true, nop: true, logReceiver: sbl);
        P.Prepare(conf);
        var child = P.Initialize<Processor>(args);
        Assert.That(child.Args, Is.EquivalentTo(args));
        Assert.That(child.LogReceiver, Is.EqualTo(sbl));
        Assert.That(child.Preload, Is.EqualTo(true));
        Assert.That(child.Debug, Is.EqualTo(true));
        Assert.That(child.Nop, Is.EqualTo(true));
    }

    [Test]
    public void Initialize_StaticWithConfiguration_PropertiesInitialized()
    {
        string[] args = { "a", "b" };
        var sbl = new StringBuilderLog();
        var conf = new ProcessorConfiguration(args, preload: true, debug: true, nop: true, logReceiver: sbl);
        var child = Processor.Initialize<Processor>(conf);
        Assert.That(child.Args, Is.EquivalentTo(args));
        Assert.That(child.LogReceiver, Is.EqualTo(sbl));
        Assert.That(child.Preload, Is.EqualTo(true));
        Assert.That(child.Debug, Is.EqualTo(true));
        Assert.That(child.Nop, Is.EqualTo(true));
    }

    public void LogInfo_Works()
    {
        var sbl = new StringBuilderLog("\n");
        P.Prepare(ProcessorConfiguration.Default with { LogReceiver = sbl });
        P.LogInfo("text2impeach");
        Assert.That(sbl.GetContent(), Is.EqualTo("text2impeach\n"));
    }

    [Test]
    public void LogWarn_Works()
    {
        var sbl = new StringBuilderLog("\n");
        P.Prepare(ProcessorConfiguration.Default with { LogReceiver = sbl });
        P.LogWarn("text2impeach");
        Assert.That(sbl.GetContent(), Is.EqualTo("text2impeach\n"));
    }

    [Test]
    public void LogFail_Works()
    {
        var sbl = new StringBuilderLog("\n");
        P.Prepare(ProcessorConfiguration.Default with { LogReceiver = sbl });
        P.LogFail("text2impeach");
        Assert.That(sbl.GetContent(), Is.EqualTo("text2impeach\n"));
    }

    [Test]
    public void LogChunk_Works()
    {
        var sbl = new StringBuilderLog("\n");
        P.Prepare(ProcessorConfiguration.Default with { LogReceiver = sbl });
        P.LogChunk("text", false);
        Assert.That(sbl.GetContent(), Is.EqualTo("text"));
        P.LogChunk("2", true);
        Assert.That(sbl.GetContent(), Is.EqualTo("text2\n"));
    }

    [Test]
    public void Region_Exit_RestoresStream()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        using (_ = P.Region(0, 0))
        {
            Assert.That(ReferenceEquals(ms1, P.InputStream), Is.False);
            Assert.That(P.InputStream!, Is.Not.Null);
        }
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.True);
    }

    [Test]
    public void Region_CreatesCorrectSubStream()
    {
        var ms1 = new MemoryStream(new byte[] { 0, 1, 2, 3 });
        P.UseStream(ms1);
        using (_ = P.Region(1, 2))
        {
            var stream = P.InputStream;
            Assert.That(stream!, Is.Not.Null);
            Assert.That(stream.Length, Is.EqualTo(2));
            Assert.That(stream.ReadByte(), Is.EqualTo(1));
            Assert.That(stream.ReadByte(), Is.EqualTo(2));
            Assert.That(stream.ReadByte(), Is.EqualTo(-1));
        }
    }

    [Test]
    public void Region_CreatesCorrectAutomaticLength()
    {
        var ms1 = new MemoryStream(new byte[] { 0, 1, 2, 3 });
        P.UseStream(ms1);
        using (_ = P.Region(1))
        {
            var stream = P.InputStream;
            Assert.That(stream!, Is.Not.Null);
            Assert.That(stream.Length, Is.EqualTo(3));
            Assert.That(stream.ReadByte(), Is.EqualTo(1));
            Assert.That(stream.ReadByte(), Is.EqualTo(2));
            Assert.That(stream.ReadByte(), Is.EqualTo(3));
            Assert.That(stream.ReadByte(), Is.EqualTo(-1));
        }
    }

    [Test]
    public void Region_UnwrapsSStreamCorrectly()
    {
        var ms1 = new MemoryStream(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 });
        ms1.Position = 1;
        var ss1 = new SStream(ms1, 6);
        ss1.Position = 1;
        var ss2 = new SStream(ss1, 4);
        ss2.Position = 1;
        var ss3 = new SStream(ss2, 2);
        P.UseStream(ss3);
        using (_ = P.Region(0, 2))
        {
            Assert.That(ReferenceEquals(ms1, P.InputStream), Is.False);
            var stream = P.InputStream as SStream;
            Assert.That(stream!, Is.Not.Null);
            Assert.That(stream.Length, Is.EqualTo(2));
            Assert.That(stream.ReadByte(), Is.EqualTo(3));
            Assert.That(stream.ReadByte(), Is.EqualTo(4));
            Assert.That(stream.ReadByte(), Is.EqualTo(-1));
            stream.Position = 0;
            Assert.That(ms1.Position, Is.EqualTo(3));
        }
    }

    [Test]
    public void Cleanup_ClearsInputStream()
    {
        var input = new MemoryStream();
        P.UseStream(input);
        Assert.That(P.InputStream, Is.Not.Null);
        P.Cleanup();
        Assert.That(P.InputStream, Is.Null);
        Assert.That(() => input.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void Dispose_ClearsInputStream()
    {
        var input = new MemoryStream();
        P.UseStream(input);
        Assert.That(P.InputStream, Is.Not.Null);
        P.Dispose();
        Assert.That(P.InputStream, Is.Null);
        Assert.That(() => input.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void Cleanup_ClearsOutputStream()
    {
        var output = new MemoryStream();
        P.UseOutputStream(output);
        Assert.That(P.OutputStream, Is.Not.Null);
        P.Cleanup();
        Assert.That(P.OutputStream, Is.Null);
        Assert.That(() => output.WriteByte(0), Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void Dispose_ClearsOutputStream()
    {
        var output = new MemoryStream();
        P.UseOutputStream(output);
        Assert.That(P.OutputStream, Is.Not.Null);
        P.Dispose();
        Assert.That(P.OutputStream, Is.Null);
        Assert.That(() => output.WriteByte(0), Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void Cleanup_ClearsMemoryAnnotations()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        P.MemLabel(data, pattern);
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(1));
        P.Cleanup();
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(0));
    }

    [Test]
    public void Dispose_ClearsMemoryAnnotations()
    {
        P.Debug = true;
        byte[] data = Encoding.UTF8.GetBytes("hello world");
        Span<byte> pattern = stackalloc byte[] { (byte)'o' };
        P.MemLabel(data, pattern);
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(1));
        P.Dispose();
        Assert.That(P.MemAnnotations.Count, Is.EqualTo(0));
    }
}
