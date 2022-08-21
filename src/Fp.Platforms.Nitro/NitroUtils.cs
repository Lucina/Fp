using System;
using System.IO;
using DSDecmp.Formats.Nitro;

namespace Fp.Platforms.Nitro;

/// <summary>
/// Nitro utilities (powered by <see cref="DSDecmp"/>).
/// </summary>
public static class NitroUtils
{
    #region Auto-detection

    /// <summary>
    /// Decompresses nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro(this ReadOnlyMemory<byte> source, int capacity = 0)
    {
        MemoryStream ms = new(capacity);
        GetNitroFormat(source).Decompress(source.Stream(), source.Length, ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Decompresses nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro(this Memory<byte> source, int capacity = 0)
    {
        MemoryStream ms = new(capacity);
        GetNitroFormat(source).Decompress(source.Stream(), source.Length, ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Decompresses nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro(this byte[] source, int capacity = 0)
    {
        MemoryStream ms = new(capacity);
        GetNitroFormat(source).Decompress(source.Stream(), source.Length, ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Decompresses nitro format from stream.
    /// </summary>
    /// <param name="source">Source stream.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro(this Stream source, int capacity = 0)
    {
        if (!source.CanSeek) throw new InvalidOperationException();
        MemoryStream ms = new(capacity);
        GetNitroFormat(source).Decompress(source, source.Length - source.Position, ms);
        return ms.ToArray();
    }

    #endregion

    #region Detection

    /// <summary>
    /// Detects nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <returns>Nitro format.</returns>
    /// <exception cref="InvalidOperationException">Thrown when buffer is empty.</exception>
    /// <exception cref="NotSupportedException">Thrown when unsupported magic value is read.</exception>
    public static NitroCFormat GetNitroFormat(this ReadOnlyMemory<byte> source)
    {
        if (source.Length == 0) throw new InvalidOperationException("Buffer empty");
        return GetNitroFormat(source.Span[0]);
    }

    /// <summary>
    /// Detects nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <returns>Nitro format.</returns>
    /// <exception cref="InvalidOperationException">Thrown when buffer is empty.</exception>
    /// <exception cref="NotSupportedException">Thrown when unsupported magic value is read.</exception>
    public static NitroCFormat GetNitroFormat(this Memory<byte> source) =>
        GetNitroFormat((ReadOnlyMemory<byte>)source);

    /// <summary>
    /// Detects nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <returns>Nitro format.</returns>
    /// <exception cref="InvalidOperationException">Thrown when buffer is empty.</exception>
    /// <exception cref="NotSupportedException">Thrown when unsupported magic value is read.</exception>
    public static NitroCFormat GetNitroFormat(this byte[] source) => GetNitroFormat((ReadOnlyMemory<byte>)source);

    /// <summary>
    /// Detects nitro format from stream.
    /// </summary>
    /// <param name="source">Source stream.</param>
    /// <returns>Nitro format.</returns>
    /// <exception cref="InvalidOperationException">Thrown when buffer is empty.</exception>
    /// <exception cref="NotSupportedException">Thrown when unsupported magic value is read.</exception>
    public static NitroCFormat GetNitroFormat(this Stream source)
    {
        if (!source.CanSeek) throw new InvalidOperationException();
        int v = source.ReadByte();
        source.Seek(-1, SeekOrigin.Current);
        return GetNitroFormat(v);
    }

    /// <summary>
    /// Detects nitro format from magic value.
    /// </summary>
    /// <param name="v">Magic value.</param>
    /// <returns>Nitro format.</returns>
    /// <exception cref="InvalidOperationException">Thrown when input is -1.</exception>
    /// <exception cref="NotSupportedException">Thrown when unsupported magic value is provided.</exception>
    public static NitroCFormat GetNitroFormat(int v)
    {
        return v switch
        {
            -1 => throw new InvalidOperationException("Stream empty"),
            0x00 => One<NullCompression>.Value,
            0x10 => One<LZ10>.Value,
            0x11 => One<LZ11>.Value,
            0x24 => One<Huffman4>.Value,
            0x28 => One<Huffman8>.Value,
            0x30 => One<RLE>.Value,
            _ => Unknown<NitroCFormat>.FromKey(v)
        };
    }

    #endregion

    #region Format object

    /// <summary>
    /// Decompresses nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="format">Compression format.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro(this ReadOnlyMemory<byte> source, NitroCFormat format, int capacity = 0)
        => DeNitro(source.Stream(), format, capacity);

    /// <summary>
    /// Decompresses nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="format">Compression format.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro(this Memory<byte> source, NitroCFormat format, int capacity = 0)
        => DeNitro(source.Stream(), format, capacity);

    /// <summary>
    /// Decompresses nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="format">Compression format.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro(this byte[] source, NitroCFormat format, int capacity = 0)
        => DeNitro(source.Stream(), format, capacity);

    /// <summary>
    /// Decompresses nitro format from stream.
    /// </summary>
    /// <param name="source">Source stream.</param>
    /// <param name="format">Compression format.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro(this Stream source, NitroCFormat format, int capacity = 0)
    {
        if (!source.CanSeek) throw new InvalidOperationException();
        MemoryStream ms = new(capacity);
        format.Decompress(source, source.Length - source.Position, ms);
        return ms.ToArray();
    }

    #endregion

    #region Generic

    /// <summary>
    /// Decompresses nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <typeparam name="T">Compression format.</typeparam>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro<T>(this ReadOnlyMemory<byte> source, int capacity = 0)
        where T : NitroCFormat, new()
        => DeNitro<T>(source.Stream(), capacity);

    /// <summary>
    /// Decompresses nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <typeparam name="T">Compression format.</typeparam>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro<T>(this Memory<byte> source, int capacity = 0) where T : NitroCFormat, new()
        => DeNitro<T>(source.Stream(), capacity);

    /// <summary>
    /// Decompresses nitro format from buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <typeparam name="T">Compression format.</typeparam>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro<T>(this byte[] source, int capacity = 0) where T : NitroCFormat, new()
        => DeNitro<T>(source.Stream(), capacity);

    /// <summary>
    /// Decompresses nitro format from stream.
    /// </summary>
    /// <param name="source">Source stream.</param>
    /// <param name="capacity">Initial output capacity.</param>
    /// <typeparam name="T">Compression format.</typeparam>
    /// <returns>Result buffer.</returns>
    public static byte[] DeNitro<T>(this Stream source, int capacity = 0) where T : NitroCFormat, new()
    {
        if (!source.CanSeek) throw new InvalidOperationException();
        MemoryStream ms = new(capacity);
        One<T>.Value.Decompress(source, source.Length - source.Position, ms);
        return ms.ToArray();
    }

    #endregion

    #region Extensions

    /// <summary>
    /// Detects nitro file extension (e.g. .nsbmd, .nsbtx) from fourcc (e.g. BMD0, BTX0).
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="defaultValue">Default extension.</param>
    /// <returns>Nitro file extension.</returns>
    public static string GetNitroExtension(this ReadOnlyMemory<byte> source, string defaultValue = ".dat") =>
        // https://wiki.vg-resource.com/Nitro_Files
        Processor.ReadUtf8String(source.Span, out _, out _, 4) switch
        {
            "BMD0" => ".nsbmd",
            "BTX0" => ".nsbtx",
            "BCA0" => ".nsbca",
            "BTP0" => ".nsbtp",
            "BTA0" => ".nsbta",
            "BMA0" => ".nsbma",
            "BVA0" => ".nsbva",
            _ => defaultValue
        };

    /// <summary>
    /// Detects nitro file extension (e.g. .nsbmd, .nsbtx) from fourcc (e.g. BMD0, BTX0).
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="defaultValue">Default extension.</param>
    /// <returns>Nitro file extension.</returns>
    public static string GetNitroExtension(this Memory<byte> source, string defaultValue = ".dat") =>
        GetNitroExtension((ReadOnlyMemory<byte>)source, defaultValue);

    /// <summary>
    /// Detects nitro file extension (e.g. .nsbmd, .nsbtx) from fourcc (e.g. BMD0, BTX0).
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="defaultValue">Default extension.</param>
    /// <returns>Nitro file extension.</returns>
    public static string GetNitroExtension(this byte[] source, string defaultValue = ".dat") =>
        GetNitroExtension((ReadOnlyMemory<byte>)source, defaultValue);

    #endregion
}
