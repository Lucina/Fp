using NUnit.Framework;

namespace Fp.Tests.Utility;

public class ProcessorTestBase
{
    protected Processor P;

    [SetUp]
    public void SetUp()
    {
        P = new Processor();
    }

    [TearDown]
    public void TearDown()
    {
        P.Dispose();
        P = null;
    }
}
