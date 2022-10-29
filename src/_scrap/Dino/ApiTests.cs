using System;
using System.IO;
using Fp.Fs;
using NUnit.Framework;

namespace Fp.Tests.Dino;

public class ApiTests
{
    [Test]
    public void TestMultiBufferStream()
    {
        // Test a bunch of random location reads
        Random r = new();
        byte[] a = new byte[4096];
        r.NextBytes(a);
        MemoryStream ms = new(a);
        MultiBufferStream mbs = new(ms, true, 8, 128);
        mbs.LargeReadOverrideThreshold = int.MaxValue;
        byte[] temp = new byte[256];
        for (int i = 0; i < 128; i++)
        {
            int position = 16 * (r.Next() % 256);
            mbs.Position = position;
            int read = mbs.Read(temp, 0, 256);
            //Console.WriteLine($"{i} {position} {read}");
            Assert.AreEqual(new ArraySegment<byte>(a, position, read), new ArraySegment<byte>(temp, 0, read));
        }

        // Test full read
        mbs.Position = 0;
        MemoryStream ms2 = new();
        mbs.CopyTo(ms2);
        ms2.TryGetBuffer(out ArraySegment<byte> ms2B);
        Assert.AreEqual(new ArraySegment<byte>(a), ms2B);
    }

    [Test]
    public void TestNumberCast()
    {
        object number1 = 1;
        Assert.AreEqual(1, FpUtil.CastNumber<object, byte>(number1));
        Assert.AreEqual(3841, FpUtil.CastNumber<uint, short>(3841));
        Assert.AreEqual(17, FpUtil.CastNumber<object, uint>("17"));
        Assert.AreEqual(1.0f, FpUtil.CastNumber<object, float>("1.0"), 0.001f);
    }

    [Test]
    public void TestScriptingDetection()
    {
        FsProcessor.FsInstance.ShieldUp();
        try
        {
            byte[] data = "RIFF\0\0\0\0WAVE".Ascii();
            string extension = data._WAV().___(".N");
            Assert.AreEqual(".wav", extension);
            byte[] data2 = "RIFZ\0\0\0\0WAVE".Ascii();
            string extension2 = data2._WAV().___(".N");
            Assert.AreEqual(".N", extension2);
        }
        finally
        {
            FsProcessor.ShieldDown();
        }
    }
}
