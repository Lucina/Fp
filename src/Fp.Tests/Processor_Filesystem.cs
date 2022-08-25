using System;
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

    [Test]
    public void CloseFile_AsMainFalse_Noop()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        P.CloseFile(false);
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
    }

    [Test]
    public void CloseFile_AsMainFalseCustomStream_ClosesAndDisposesInputStreamButDoesNotUnset()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        var ms2 = new MemoryStream();
        Assert.That(ms2.ReadByte(), Is.EqualTo(-1));
        P.CloseFile(false, ms2);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        Assert.That(() => ms2.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.True);
    }

    [Test]
    public void CloseFile_AsMainFalse_ClosesAndDisposesInputStreamAndUnsets()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        P.CloseFile(true);
        Assert.That(() => ms1.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.False);
    }

    [Test]
    public void CloseFile_AsMainFalseCustomStream_ClosesAndDisposesStreamAndUnsets()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        var ms2 = new MemoryStream();
        Assert.That(ms2.ReadByte(), Is.EqualTo(-1));
        P.CloseFile(true, ms2);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        Assert.That(() => ms2.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.False);
    }

    [Test]
    public void CloseFile_ClosesAndDisposesInputStream()
    {
        var ms1 = new MemoryStream();
        P.UseStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.InputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        P.CloseFile();
        Assert.That(() => ms1.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void CloseOutputFile_ClosesAndDisposesInputStream()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.OutputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        P.CloseOutputFile();
        Assert.That(() => ms1.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
    }

    [Test]
    public void CloseOutputFile_AsMainFalse_Noop()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.OutputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        P.CloseOutputFile(false);
        Assert.That(ReferenceEquals(ms1, P.OutputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
    }

    [Test]
    public void CloseOutputFile_AsMainFalseCustomStream_ClosesAndDisposesInputStreamButDoesNotUnset()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.OutputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        var ms2 = new MemoryStream();
        Assert.That(ms2.ReadByte(), Is.EqualTo(-1));
        P.CloseOutputFile(false, ms2);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        Assert.That(() => ms2.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
        Assert.That(ReferenceEquals(ms1, P.OutputStream), Is.True);
    }

    [Test]
    public void CloseOutputFile_AsMainFalse_ClosesAndDisposesInputStreamAndUnsets()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.OutputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        P.CloseOutputFile(true);
        Assert.That(() => ms1.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
        Assert.That(ReferenceEquals(ms1, P.OutputStream), Is.False);
    }

    [Test]
    public void CloseOutputFile_AsMainFalseCustomStream_ClosesAndDisposesStreamAndUnsets()
    {
        var ms1 = new MemoryStream();
        P.UseOutputStream(ms1);
        Assert.That(ReferenceEquals(ms1, P.OutputStream), Is.True);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        var ms2 = new MemoryStream();
        Assert.That(ms2.ReadByte(), Is.EqualTo(-1));
        P.CloseOutputFile(true, ms2);
        Assert.That(ms1.ReadByte(), Is.EqualTo(-1));
        Assert.That(() => ms2.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
        Assert.That(ReferenceEquals(ms1, P.OutputStream), Is.False);
    }

    // TODO
}
