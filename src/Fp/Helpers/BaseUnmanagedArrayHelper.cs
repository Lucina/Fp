using System;
using System.IO;
using static System.Buffers.ArrayPool<byte>;

namespace Fp.Helpers;

/// <summary>
/// Base unmanaged array data helper.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public abstract unsafe record BaseUnmanagedArrayHelper<T> : Helper where T : unmanaged
{
    /// <summary>
    /// Element size in memory (contiguous read).
    /// </summary>
    public virtual int ElementSize => sizeof(T);

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="count">Element count.</param>
    public virtual ReadOnlySpan<T> this[byte[] source, int offset, int count] =>
        this[source.AsSpan(), offset, count];

    /// <summary>
    /// Writes data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    public virtual ReadOnlySpan<T> this[byte[] source, int offset]
    {
        set => this[source.AsSpan(), offset] = value;
    }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="count">Element count.</param>
    public virtual ReadOnlySpan<T> this[Memory<byte> source, int offset, int count] =>
        this[source.Span, offset, count];

    /// <summary>
    /// Writes data.
    /// </summary>
    /// <param name="offset">Offset.</param>
    /// <param name="source">Data source.</param>
    public virtual ReadOnlySpan<T> this[Memory<byte> source, int offset]
    {
        set => this[source.Span, offset] = value;
    }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="count">Element count.</param>
    public virtual ReadOnlySpan<T> this[Span<byte> source, int offset, int count] =>
        this[source.Slice(offset, count * ElementSize)];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="count">Element count.</param>
    public virtual ReadOnlySpan<T> this[ReadOnlyMemory<byte> source, int offset, int count] =>
        this[source.Span, offset, count];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    public abstract ReadOnlySpan<T> this[ReadOnlySpan<byte> source] { get; }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="offset">Offset (no seeking if -1).</param>
    /// <param name="count">Element count.</param>
    public virtual T[] this[long offset, int count] => this[offset, count, InputStream];

    /// <summary>
    /// Writes data.
    /// </summary>
    /// <param name="offset">Offset (no seeking if -1).</param>
    public virtual ReadOnlySpan<T> this[long offset]
    {
        set => this[offset, OutputStream] = value;
    }

    /// <summary>
    /// Reads / writes data.
    /// </summary>
    /// <param name="source">Data source.</param>
    public virtual ReadOnlySpan<T> this[byte[] source]
    {
        get => this[source.AsSpan()];
        set => this[source.AsSpan()] = value;
    }

    /// <summary>
    /// Reads / writes data.
    /// </summary>
    /// <param name="source">Data source.</param>
    public virtual ReadOnlySpan<T> this[Memory<byte> source]
    {
        get => this[source.Span];
        set => this[source.Span] = value;
    }

    /// <summary>
    /// Writes data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    public virtual ReadOnlySpan<T> this[Span<byte> source, int offset]
    {
        set => this[source[offset..]] = value;
    }

    /// <summary>
    /// Reads / writes data.
    /// </summary>
    /// <param name="source">Data source.</param>
    public abstract ReadOnlySpan<T> this[Span<byte> source] { get; set; }

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="offset">Offset.</param>
    /// <param name="count">Element count.</param>
    public virtual ReadOnlySpan<T> this[ReadOnlySpan<byte> source, int offset, int count] =>
        this[source.Slice(offset, count * ElementSize)];

    /// <summary>
    /// Reads data.
    /// </summary>
    /// <param name="offset">Offset (no seeking if -1).</param>
    /// <param name="count">Element count.</param>
    /// <param name="stream">Data source.</param>
    public virtual T[] this[long offset, int count, Stream stream]
    {
        get
        {
            int llenn = count * ElementSize;
            byte[] arr = Shared.Rent(llenn);
            try
            {
                if (offset != -1) Processor.Read(stream, offset, arr, 0, llenn, false);
                else Processor.Read(stream, arr, 0, llenn, false);
                return this[arr, 0, count].ToArray();
            }
            finally
            {
                Shared.Return(arr);
            }
        }
    }

    /// <summary>
    /// Writes data.
    /// </summary>
    /// <param name="offset">Offset (no seeking if -1).</param>
    /// <param name="stream">Data source.</param>
    public virtual ReadOnlySpan<T> this[long offset, Stream stream]
    {
        set
        {
            byte[] arr = new byte[value.Length * ElementSize];
            this[arr, 0] = value;
            if (offset != -1) Processor.Write(stream, offset, arr);
            else Processor.Write(stream, arr);
        }
    }
}