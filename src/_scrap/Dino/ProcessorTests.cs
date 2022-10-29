using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Fp.Tests.Dino;

public class ProcessorTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestSearch()
    {
        byte[] arr = Encoding.UTF8.GetBytes("word");
        const int count = 10;
        var ms = new MemoryStream();
        for (int i = 0; i < count; i++)
            ms.Write(arr, 0, arr.Length);
        int next = 0;
        int mCount = 0;
        foreach (long match in Processor.Match(ms, 0, ms.Length, arr.AsMemory(), 0, arr.Length))
        {
            Assert.AreEqual(next, match);
            next += arr.Length;
            mCount++;
        }

        Assert.AreEqual(count, mCount);

        next = 0;
        mCount = 0;
        byte[] msArr = ms.ToArray();
        foreach (long match in Processor.Match(msArr, arr))
        {
            Assert.AreEqual(next, match);
            next += arr.Length;
            mCount++;
        }

        Assert.AreEqual(count, mCount);
    }

    [Test]
    public void TestJoin()
    {
        Assert.AreEqual("/A/B/C/D/", Processor.Join(false, "/A", "/B", "C/", "/D/"));
        Assert.AreEqual("/A/B", Processor.Join(false, "/A", "B"));
        Assert.AreEqual("/A/B", Processor.Join(false, "/A", "", "", "B"));
        Assert.AreEqual(@"/Source/Path/\x/", Processor.Join(false, "/Source", "/Path/", @"\x/"));
        Assert.AreEqual(@"C:\A\B\C\D\E", Processor.Join(true, @"C:\A", @"\B", @"C\", @"\D\", "E"));
        Assert.AreEqual(@"C:\A\B", Processor.Join(true, @"C:\A", "B"));
        Assert.AreEqual(@"C:\A/B", Processor.Join(true, @"C:\A", "/B"));
    }

}
