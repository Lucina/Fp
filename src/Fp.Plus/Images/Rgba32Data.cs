using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Fp.Plus.Images;

/// <summary>
/// 32-bit RGBA data.
/// </summary>
public class Rgba32Data : BufferData<uint>
{
    /// <summary>
    /// PNG DEFLATE format.
    /// </summary>
    public static readonly Guid PngDeflate = Guid.Parse("3022E430-0364-4628-A5EE-56B0344EDCB2");

    /// <summary>
    /// JPEG format.
    /// </summary>
    public static readonly Guid Jpeg = Guid.Parse("7AE22A2F-CE7F-470B-BC58-674AC8B5EA9E");

    private static readonly PngEncoder s_pngEncoder =
        new() { CompressionLevel = PngCompressionLevel.BestCompression };

    /// <inheritdoc />
    public override Guid DefaultFormat => PngDeflate;

    /// <summary>
    /// Provides option keys for <see cref="Rgba32Data"/>.
    /// </summary>
    public static class Options
    {
        /// <summary>
        /// Jpeg quality level (int 0-100).
        /// </summary>
        public const string JpegQuality = "JpegQuality";
    }

    /// <summary>
    /// Image width.
    /// </summary>
    public readonly int Width;

    /// <summary>
    /// Image height.
    /// </summary>
    public readonly int Height;

    private bool _disposed;

    /// <summary>
    /// Creates a new instance of <see cref="Rgba32Data"/>.
    /// </summary>
    /// <param name="basePath">Base path of resource.</param>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    public Rgba32Data(string basePath, int width, int height) : base(basePath, width * height)
    {
        Dry = true;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Rgba32Data"/>.
    /// </summary>
    /// <param name="basePath">Base path of resource.</param>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <param name="memoryOwner">Owner of PCM data buffer.</param>
    /// <param name="count">Length of content.</param>
    public Rgba32Data(string basePath, int width, int height, IMemoryOwner<uint> memoryOwner,
        int? count = default) : base(basePath, memoryOwner, count)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Rgba32Data"/>.
    /// </summary>
    /// <param name="basePath">Base path of resource.</param>
    /// <param name="width">Image width.</param>
    /// <param name="height">Image height.</param>
    /// <param name="buffer">PCM data.</param>
    public Rgba32Data(string basePath, int width, int height, ReadOnlyMemory<uint> buffer) : base(basePath, buffer)
    {
        Width = width;
        Height = height;
    }

    /// <inheritdoc />
    public override bool WriteConvertedData(Stream outputStream, Guid format,
        Dictionary<object, object>? formatOptions = null)
    {
        if (Dry) throw new InvalidOperationException("Cannot convert a dry data container");
        if (_disposed) throw new ObjectDisposedException(nameof(Rgba32Data));
        if (format == PngDeflate || format == Jpeg)
        {
            Image<Rgba32> image = new(Width, Height);
            if (image.TryGetSinglePixelSpan(out Span<Rgba32> span))
                Buffer.Span.Slice(0, Width * Height).CopyTo(MemoryMarshal.Cast<Rgba32, uint>(span));
            else
                for (int y = 0; y < Height; y++)
                    Buffer.Span.Slice(Width * y, Width)
                        .CopyTo(MemoryMarshal.Cast<Rgba32, uint>(image.GetPixelRowSpan(y)));

            if (format == PngDeflate)
                image.SaveAsPng(outputStream, s_pngEncoder);
            else
            {
                var jpegEncoder = new JpegEncoder();
                if (formatOptions != null &&
                    formatOptions.TryGetValue(Options.JpegQuality, out object? jpegQuality))
                    jpegEncoder.Quality = Math.Min(100, Math.Max(0, CastNumber<object, int>(jpegQuality)));
                image.SaveAsJpeg(outputStream, jpegEncoder);
            }

            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;
        _disposed = true;
        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override object Clone()
    {
        if (Dry)
            return new Rgba32Data(BasePath, Width, Height);
        if (_disposed)
            throw new ObjectDisposedException(nameof(Rgba32Data));
        return new Rgba32Data(BasePath, Width, Height, Buffer.CloneBuffer());
    }

    /// <inheritdoc />
    public override string? GetExtension(Guid? format = null)
    {
        format ??= DefaultFormat;
        return format switch
        {
            _ when format == PngDeflate => ".png",
            _ when format == Jpeg => ".jpg",
            _ => base.GetExtension(format)
        };
    }

    /// <inheritdoc />
    public override string ToString() => $"RGBA32 {{ Path = {BasePath}, Width = {Width}, Height = {Height} }}";
}