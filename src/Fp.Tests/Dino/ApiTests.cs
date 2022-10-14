using System;
using System.IO;
using Fp.Fs;
using Fp.Plus.Ciphers;
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
        mbs.LargeReadOverride = false;
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

    [Test]
    public void TestBlowfish()
    {
        byte[] data;
        byte[] tmp = File.ReadAllBytes("Watch_Dogs2020-4-3-0-57-53.png");
        data = new byte[Processor.GetPaddedLength(tmp.Length, CipherPaddingMode.Zero, 8)];
        tmp.AsSpan().CopyTo(data);

        byte[] dataEnc = new byte[data.Length];
        Buffer.BlockCopy(data, 0, dataEnc, 0, data.Length);
        Blowfish bf = new();
        byte[] ptkey = Processor.DecodeHex("1010ffff");
        bf.SetBlankIv();
        bf.SetKey(ptkey);
        bf.EncryptCbc(dataEnc);
        bf.SetBlankIv();
        bf.SetKey(ptkey);
        bf.DecryptCbc(dataEnc);
        Assert.IsTrue(data.AsSpan().SequenceEqual(dataEnc));
        bf.SetKey(ptkey);
        bf.EncryptEcb(dataEnc);
        bf.SetKey(ptkey);
        bf.DecryptEcb(dataEnc);
        Assert.IsTrue(data.AsSpan().SequenceEqual(dataEnc));
    }
}
