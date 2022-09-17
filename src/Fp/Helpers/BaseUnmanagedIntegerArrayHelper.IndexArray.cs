using System;
using System.IO;

namespace Fp.Helpers;

public abstract partial record BaseUnmanagedIntegerArrayHelper<T>
{
    /// <summary>
    /// Gets an index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <returns>Indices.</returns>
    public ReadOnlySpan<T> GetIndexArray(byte[] source, int offset, int count)
    {
        return this[source, offset, count];
    }

    /// <summary>
    /// Gets an index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Indices.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public TElement[] GetIndexArray<TElement>(byte[] source, int offset, int count, Func<T, TElement> transform)
    {
        return DoTransform(this[source, offset, count], transform);
    }

    /// <summary>
    /// Gets an index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <returns>Indices.</returns>
    public ReadOnlySpan<T> GetIndexArray(Memory<byte> source, int offset, int count)
    {
        return this[source, offset, count];
    }

    /// <summary>
    /// Gets an index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Indices.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public TElement[] GetIndexArray<TElement>(Memory<byte> source, int offset, int count, Func<T, TElement> transform)
    {
        return DoTransform(this[source, offset, count], transform);
    }

    /// <summary>
    /// Gets an index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <returns>Indices.</returns>
    public ReadOnlySpan<T> GetIndexArray(Span<byte> source, int offset, int count)
    {
        return this[source, offset, count];
    }

    /// <summary>
    /// Gets an index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Indices.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public TElement[] GetIndexArray<TElement>(Span<byte> source, int offset, int count, Func<T, TElement> transform)
    {
        return DoTransform(this[source, offset, count], transform);
    }

    /// <summary>
    /// Gets an index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <returns>Indices.</returns>
    public ReadOnlySpan<T> GetIndexArray(ReadOnlyMemory<byte> source, int offset, int count)
    {
        return this[source, offset, count];
    }

    /// <summary>
    /// Gets an index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Indices.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public TElement[] GetIndexArray<TElement>(ReadOnlyMemory<byte> source, int offset, int count, Func<T, TElement> transform)
    {
        return DoTransform(this[source, offset, count], transform);
    }

    /// <summary>
    /// Gets an index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <returns>Indices.</returns>
    public ReadOnlySpan<T> GetIndexArray(ReadOnlySpan<byte> source, int offset, int count)
    {
        return this[source, offset, count];
    }

    /// <summary>
    /// Gets an index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Indices.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public TElement[] GetIndexArray<TElement>(ReadOnlySpan<byte> source, int offset, int count, Func<T, TElement> transform)
    {
        return DoTransform(this[source, offset, count], transform);
    }

    /// <summary>
    /// Gets an index array from <see cref="Helper.InputStream"/>.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of indices in stream.</param>
    /// <returns>Indices.</returns>
    public T[] GetIndexArray(long offset, int count)
    {
        return this[offset, count];
    }

    /// <summary>
    /// Gets an index array from <see cref="Helper.InputStream"/>.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of indices in stream.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Indices.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public TElement[] GetIndexArray<TElement>(long offset, int count, Func<T, TElement> transform)
    {
        return DoTransform(this[offset, count], transform);
    }

    /// <summary>
    /// Gets an index array from a stream.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of indices in stream.</param>
    /// <param name="stream">Source stream.</param>
    /// <returns>Indices.</returns>
    public T[] GetIndexArray(long offset, int count, Stream stream)
    {
        return this[offset, count, stream];
    }

    /// <summary>
    /// Gets an index array from a stream.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of indices in stream.</param>
    /// <param name="stream">Source stream.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Indices.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public TElement[] GetIndexArray<TElement>(long offset, int count, Stream stream, Func<T, TElement> transform)
    {
        return DoTransform(this[offset, count, stream], transform);
    }

    private static TElement[] DoTransform<TElement>(ReadOnlySpan<T> buffer, Func<T, TElement> transform)
    {
        TElement[] result = new TElement[buffer.Length];
        for (int i = 0; i < buffer.Length; i++)
            result[i] = transform(buffer[i]);
        return result;
    }
}
