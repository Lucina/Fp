using System;
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
}
