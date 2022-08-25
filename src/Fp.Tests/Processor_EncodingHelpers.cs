using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_EncodingHelpers : ProcessorTestBase
{
    [Test]
    public void Construct_HelpersInitialized()
    {
        Assert.That(P.buf, Is.Not.Null);
        Assert.That(P.i1, Is.Not.Null);
        Assert.That(P.i1a, Is.Not.Null);
        Assert.That(P.u1, Is.Not.Null);
        Assert.That(P.u1a, Is.Not.Null);
        Assert.That(P.i2l, Is.Not.Null);
        Assert.That(P.i2la, Is.Not.Null);
        Assert.That(P.i4l, Is.Not.Null);
        Assert.That(P.i4la, Is.Not.Null);
        Assert.That(P.i8l, Is.Not.Null);
        Assert.That(P.i8la, Is.Not.Null);
        Assert.That(P.u2l, Is.Not.Null);
        Assert.That(P.u2la, Is.Not.Null);
        Assert.That(P.u4l, Is.Not.Null);
        Assert.That(P.u4la, Is.Not.Null);
        Assert.That(P.u8l, Is.Not.Null);
        Assert.That(P.u8la, Is.Not.Null);
        Assert.That(P.i2b, Is.Not.Null);
        Assert.That(P.i2ba, Is.Not.Null);
        Assert.That(P.i4b, Is.Not.Null);
        Assert.That(P.i4ba, Is.Not.Null);
        Assert.That(P.i8b, Is.Not.Null);
        Assert.That(P.i8ba, Is.Not.Null);
        Assert.That(P.u2b, Is.Not.Null);
        Assert.That(P.u2ba, Is.Not.Null);
        Assert.That(P.u4b, Is.Not.Null);
        Assert.That(P.u4ba, Is.Not.Null);
        Assert.That(P.u8b, Is.Not.Null);
        Assert.That(P.u8ba, Is.Not.Null);
        Assert.That(P.f2, Is.Not.Null);
        Assert.That(P.f2a, Is.Not.Null);
        Assert.That(P.f4, Is.Not.Null);
        Assert.That(P.f4a, Is.Not.Null);
        Assert.That(P.f8, Is.Not.Null);
        Assert.That(P.f8a, Is.Not.Null);
        Assert.That(P.ascii, Is.Not.Null);
        Assert.That(P.utf8, Is.Not.Null);
        Assert.That(P.utf16, Is.Not.Null);
    }
}
