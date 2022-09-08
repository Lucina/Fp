using System;
using System.IO;

namespace Fp.Helpers;

public abstract partial record BaseUnmanagedIntegerHelper<T>
{
    /// <summary>
    /// Skips to the next entry (including current) matching the specified value.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with value <paramref name="toFind"/>.</param>
    /// <param name="toFind">Value to find.</param>
    /// <returns>True if a value is found.</returns>
    public unsafe bool SkipUntil(Stream stream, long baseOffset, ref int index, T toFind)
    {
        stream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (Processor.TryRead(stream, elementBuffer, out _))
        {
            T value = this[elementBuffer];
            if (value.Equals(toFind)) return true;
            index++;
        }
        return false;
    }

    /// <summary>
    /// Skips to the next entry (including current) matching the specified value.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="maxOffset">Maximum stream offset (exclusive including element size).</param>
    /// <param name="index">Initial index, ends at first index with value <paramref name="toFind"/>.</param>
    /// <param name="toFind">Value to find.</param>
    /// <returns>True if a value is found.</returns>
    public unsafe bool SkipUntil(Stream stream, long baseOffset, long maxOffset, ref int index, T toFind)
    {
        long position = baseOffset + sizeof(T) * index;
        stream.Position = position;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (position + sizeof(T) <= maxOffset && Processor.TryRead(stream, elementBuffer, out _))
        {
            T value = this[elementBuffer];
            if (value.Equals(toFind)) return true;
            index++;
            position += sizeof(T);
        }
        return false;
    }

    /// <summary>
    /// Skips to the next entry (including current) matching the specified value.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with value <paramref name="toFind"/>.</param>
    /// <param name="maxIndex">Maximum index (exclusive).</param>
    /// <param name="toFind">Value to find.</param>
    /// <returns>True if a value is found.</returns>
    public unsafe bool SkipUntil(Stream stream, long baseOffset, ref int index, int maxIndex, T toFind)
    {
        stream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (index < maxIndex && Processor.TryRead(stream, elementBuffer, out _))
        {
            T value = this[elementBuffer];
            if (value.Equals(toFind)) return true;
            index++;
        }
        return false;
    }

    /// <summary>
    /// Skips to the next entry (including current) matching the specified value.
    /// </summary>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with value <paramref name="toFind"/>.</param>
    /// <param name="toFind">Value to find.</param>
    /// <returns>True if a value is found.</returns>
    public unsafe bool SkipUntil(long baseOffset, ref int index, T toFind)
    {
        if (InputStream == null) throw new InvalidOperationException();
        InputStream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (Processor.TryRead(InputStream, elementBuffer, out _))
        {
            T value = this[elementBuffer];
            if (value.Equals(toFind)) return true;
            index++;
        }
        return false;
    }

    /// <summary>
    /// Skips to the next entry (including current) matching the specified value.
    /// </summary>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="maxOffset">Maximum stream offset (exclusive including element size).</param>
    /// <param name="index">Initial index, ends at first index with value <paramref name="toFind"/>.</param>
    /// <param name="toFind">Value to find.</param>
    /// <returns>True if a value is found.</returns>
    public unsafe bool SkipUntil(long baseOffset, long maxOffset, ref int index, T toFind)
    {
        if (InputStream == null) throw new InvalidOperationException();
        long position = baseOffset + sizeof(T) * index;
        InputStream.Position = position;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (position + sizeof(T) <= maxOffset && Processor.TryRead(InputStream, elementBuffer, out _))
        {
            T value = this[elementBuffer];
            if (value.Equals(toFind)) return true;
            index++;
            position += sizeof(T);
        }
        return false;
    }

    /// <summary>
    /// Skips to the next entry (including current) matching the specified value.
    /// </summary>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with value <paramref name="toFind"/>.</param>
    /// <param name="maxIndex">Maximum index (exclusive).</param>
    /// <param name="toFind">Value to find.</param>
    /// <returns>True if a value is found.</returns>
    public unsafe bool SkipUntil(long baseOffset, ref int index, int maxIndex, T toFind)
    {
        if (InputStream == null) throw new InvalidOperationException();
        InputStream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (index < maxIndex && Processor.TryRead(InputStream, elementBuffer, out _))
        {
            T value = this[elementBuffer];
            if (value.Equals(toFind)) return true;
            index++;
        }
        return false;
    }

    /// <summary>
    /// Skips to the next entry (including current) matching the specified value.
    /// </summary>
    /// <param name="span">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with value <paramref name="toFind"/>.</param>
    /// <param name="toFind">Value to find.</param>
    /// <returns>True if a value is found.</returns>
    public unsafe bool SkipUntil(ReadOnlySpan<byte> span, int baseOffset, ref int index, T toFind)
    {
        int position = baseOffset + sizeof(T) * index;
        if (position + sizeof(T) > span.Length) return false;
        span = span[position..];
        while (span.Length >= sizeof(T))
        {
            T value = this[span];
            if (value.Equals(toFind)) return true;
            index++;
            span = span[sizeof(T)..];
        }
        return false;
    }

    /// <summary>
    /// Skips to the next entry (including current) matching the specified value.
    /// </summary>
    /// <param name="span">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="maxOffset">Maximum stream offset (exclusive including element size).</param>
    /// <param name="index">Initial index, ends at first index with value <paramref name="toFind"/>.</param>
    /// <param name="toFind">Value to find.</param>
    /// <returns>True if a value is found.</returns>
    public unsafe bool SkipUntil(ReadOnlySpan<byte> span, int baseOffset, int maxOffset, ref int index, T toFind)
    {
        int position = baseOffset + sizeof(T) * index;
        if (position + sizeof(T) > span.Length) return false;
        span = span[position..];
        while (position + sizeof(T) <= maxOffset && span.Length >= sizeof(T))
        {
            T value = this[span];
            if (value.Equals(toFind)) return true;
            index++;
            position += sizeof(T);
            span = span[sizeof(T)..];
        }
        return false;
    }

    /// <summary>
    /// Skips to the next entry (including current) matching the specified value.
    /// </summary>
    /// <param name="span">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with value <paramref name="toFind"/>.</param>
    /// <param name="maxIndex">Maximum index (exclusive).</param>
    /// <param name="toFind">Value to find.</param>
    /// <returns>True if a value is found.</returns>
    public unsafe bool SkipUntil(ReadOnlySpan<byte> span, int baseOffset, ref int index, int maxIndex, T toFind)
    {
        int position = baseOffset + sizeof(T) * index;
        if (position + sizeof(T) > span.Length) return false;
        span = span[position..];
        while (index < maxIndex && span.Length >= sizeof(T))
        {
            T value = this[span];
            if (value.Equals(toFind)) return true;
            index++;
            span = span[sizeof(T)..];
        }
        return false;
    }
}
