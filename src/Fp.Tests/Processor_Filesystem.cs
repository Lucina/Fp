using System.IO;
using Fp.Tests.Utility;
using NUnit.Framework;

namespace Fp.Tests;

public class Processor_Filesystem : ProcessorTestBase
{
    [Test]
    public void UseStream_AssignsInputStream()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.True);
    }
    [Test]
    public void UseOutputStream_AssignsOutputStream()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.OutputStream), Is.True);
    }

    // TODO
}
