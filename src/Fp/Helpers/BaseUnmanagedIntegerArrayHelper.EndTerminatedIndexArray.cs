using System;
using System.IO;

namespace Fp.Helpers;

public abstract partial record BaseUnmanagedIntegerArrayHelper<T>
{
    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <returns>Segments.</returns>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(byte[] source, int offset, int count)
    {
        return ProcessEndTerminatedIndexArray(this[source, offset, count]);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(byte[] source, int offset, int count, Func<T, TElement> transform)
#if NET7_0_OR_GREATER
        where TElement : System.Numerics.INumber<TElement>
#endif
    {
        return ProcessEndTerminatedIndexArray(this[source, offset, count], transform);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <returns>Segments.</returns>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(Memory<byte> source, int offset, int count)
    {
        return ProcessEndTerminatedIndexArray(this[source, offset, count]);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(Memory<byte> source, int offset, int count, Func<T, TElement> transform)
#if NET7_0_OR_GREATER
        where TElement : System.Numerics.INumber<TElement>
#endif
    {
        return ProcessEndTerminatedIndexArray(this[source, offset, count], transform);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <returns>Segments.</returns>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(Span<byte> source, int offset, int count)
    {
        return ProcessEndTerminatedIndexArray(this[source, offset, count]);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(Span<byte> source, int offset, int count, Func<T, TElement> transform)
#if NET7_0_OR_GREATER
        where TElement : System.Numerics.INumber<TElement>
#endif
    {
        return ProcessEndTerminatedIndexArray(this[source, offset, count], transform);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <returns>Segments.</returns>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(ReadOnlyMemory<byte> source, int offset, int count)
    {
        return ProcessEndTerminatedIndexArray(this[source, offset, count]);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(ReadOnlyMemory<byte> source, int offset, int count, Func<T, TElement> transform)
#if NET7_0_OR_GREATER
        where TElement : System.Numerics.INumber<TElement>
#endif
    {
        return ProcessEndTerminatedIndexArray(this[source, offset, count], transform);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <returns>Segments.</returns>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(ReadOnlySpan<byte> source, int offset, int count)
    {
        return ProcessEndTerminatedIndexArray(this[source, offset, count]);
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="source">Source buffer.</param>
    /// <param name="offset">Byte offset in buffer.</param>
    /// <param name="count">Number of indices in buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(ReadOnlySpan<byte> source, int offset, int count, Func<T, TElement> transform)
#if NET7_0_OR_GREATER
        where TElement : System.Numerics.INumber<TElement>
#endif
    {
        return ProcessEndTerminatedIndexArray(this[source, offset, count], transform);
    }

    /// <summary>
    /// Gets an end-terminated index array from <see cref="Helper.InputStream"/>.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of indices in stream.</param>
    /// <returns>Segments.</returns>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(long offset, int count)
    {
        return ProcessEndTerminatedIndexArray(this[offset, count]);
    }

    /// <summary>
    /// Gets an end-terminated index array from <see cref="Helper.InputStream"/>.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of indices in stream.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(long offset, int count, Func<T, TElement> transform)
#if NET7_0_OR_GREATER
        where TElement : System.Numerics.INumber<TElement>
#endif
    {
        return ProcessEndTerminatedIndexArray(this[offset, count], transform);
    }

    /// <summary>
    /// Gets an end-terminated index array from a stream.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of indices in stream.</param>
    /// <param name="stream">Source stream.</param>
    /// <returns>Segments.</returns>
    public OffsetSegment<T>[] GetEndTerminatedIndexArray(long offset, int count, Stream stream)
    {
        return ProcessEndTerminatedIndexArray(this[offset, count, stream]);
    }

    /// <summary>
    /// Gets an end-terminated index array from a stream.
    /// </summary>
    /// <param name="offset">Byte offset in stream.</param>
    /// <param name="count">Number of indices in stream.</param>
    /// <param name="stream">Source stream.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <returns>Segments.</returns>
    /// <typeparam name="TElement">Output index type.</typeparam>
    public OffsetSegment<TElement>[] GetEndTerminatedIndexArray<TElement>(long offset, int count, Stream stream, Func<T, TElement> transform)
#if NET7_0_OR_GREATER
        where TElement : System.Numerics.INumber<TElement>
#endif
    {
        return ProcessEndTerminatedIndexArray(this[offset, count, stream], transform);
    }

    private static OffsetSegment<T>[] ProcessEndTerminatedIndexArray(ReadOnlySpan<T> buffer)
    {
        if (buffer.Length == 0) throw new ArgumentException();
        if (buffer.Length == 1) return Array.Empty<OffsetSegment<T>>();
        OffsetSegment<T>[] result = new OffsetSegment<T>[buffer.Length - 1];
        T value = buffer[0];
        for (int i = 0; i < result.Length; i++)
        {
            T next = buffer[i + 1];
            result[i] = new OffsetSegment<T>(value, next);
            value = next;
        }
        return result;
    }

    private static OffsetSegment<TElement>[] ProcessEndTerminatedIndexArray<TElement>(ReadOnlySpan<T> buffer, Func<T, TElement> transform)
#if NET7_0_OR_GREATER
        where TElement : System.Numerics.INumber<TElement>
#endif
    {
        if (buffer.Length == 0) throw new ArgumentException();
        if (buffer.Length == 1) return Array.Empty<OffsetSegment<TElement>>();
        OffsetSegment<TElement>[] result = new OffsetSegment<TElement>[buffer.Length - 1];
        TElement value = transform(buffer[0]);
        for (int i = 0; i < result.Length; i++)
        {
            TElement next = transform(buffer[i + 1]);
            result[i] = new OffsetSegment<TElement>(value, next);
            value = next;
        }
        return result;
    }
}
