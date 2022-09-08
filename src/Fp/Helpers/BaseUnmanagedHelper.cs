using System;
using System.IO;

namespace Fp.Helpers;

/// <summary>
/// Base single-unit data helper.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public abstract unsafe record BaseUnmanagedHelper<T> : BaseHelper<T> where T : unmanaged
{
    /// <inheritdoc />
    public override T this[long offset, Stream stream]
    {
        get
        {
            Span<byte> span = stackalloc byte[sizeof(T)];
            if (offset != -1) Processor.Read(stream, offset, span, false);
            else Processor.Read(stream, span, false);
            return this[span];
        }
        set
        {
            Span<byte> span = stackalloc byte[sizeof(T)];
            this[span] = value;
            if (offset != -1) Processor.Write(stream, offset, span);
            else Processor.Write(stream, span);
        }
    }
}

/// <summary>
/// Base unmanaged integer array data helper.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public abstract record BaseUnmanagedIntegerHelper<T> : BaseUnmanagedHelper<T> where T : unmanaged
{
    /// <summary>
    /// Skips to the next entry not matching the specified value.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with different value.</param>
    /// <param name="toSkip">Value to skip.</param>
    /// <param name="value">Retrieved value or 0 if no different values found.</param>
    /// <returns>True if a different value is found.</returns>
    public unsafe bool SkipWhile(Stream stream, long baseOffset, ref int index, T toSkip, out T value)
    {
        stream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (Processor.TryRead(stream, elementBuffer, out _))
        {
            value = this[elementBuffer];
            if (!value.Equals(toSkip)) return true;
            index++;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Skips to the next entry not matching the specified value.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="maxOffset">Maximum stream offset (exclusive including element size).</param>
    /// <param name="index">Initial index, ends at first index with different value.</param>
    /// <param name="toSkip">Value to skip.</param>
    /// <param name="value">Retrieved value or 0 if no different values found.</param>
    /// <returns>True if a different value is found.</returns>
    public unsafe bool SkipWhile(Stream stream, long baseOffset, long maxOffset, ref int index, T toSkip, out T value)
    {
        long position = baseOffset + sizeof(T) * index;
        stream.Position = position;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (position + sizeof(T) <= maxOffset && Processor.TryRead(stream, elementBuffer, out _))
        {
            value = this[elementBuffer];
            if (!value.Equals(toSkip)) return true;
            index++;
            position += sizeof(T);
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Skips to the next entry not matching the specified value.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with different value.</param>
    /// <param name="maxIndex">Maximum index (exclusive).</param>
    /// <param name="toSkip">Value to skip.</param>
    /// <param name="value">Retrieved value or 0 if no different values found.</param>
    /// <returns>True if a different value is found.</returns>
    public unsafe bool SkipWhile(Stream stream, long baseOffset, ref int index, int maxIndex, T toSkip, out T value)
    {
        stream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (index < maxIndex && Processor.TryRead(stream, elementBuffer, out _))
        {
            value = this[elementBuffer];
            if (!value.Equals(toSkip)) return true;
            index++;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Skips to the next entry not matching the specified value.
    /// </summary>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with different value.</param>
    /// <param name="toSkip">Value to skip.</param>
    /// <param name="value">Retrieved value or 0 if no different values found.</param>
    /// <returns>True if a different value is found.</returns>
    public unsafe bool SkipWhile(long baseOffset, ref int index, T toSkip, out T value)
    {
        if (InputStream == null) throw new InvalidOperationException();
        InputStream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (Processor.TryRead(InputStream, elementBuffer, out _))
        {
            value = this[elementBuffer];
            if (!value.Equals(toSkip)) return true;
            index++;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Skips to the next entry not matching the specified value.
    /// </summary>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="maxOffset">Maximum stream offset (exclusive including element size).</param>
    /// <param name="index">Initial index, ends at first index with different value.</param>
    /// <param name="toSkip">Value to skip.</param>
    /// <param name="value">Retrieved value or 0 if no different values found.</param>
    /// <returns>True if a different value is found.</returns>
    public unsafe bool SkipWhile(long baseOffset, long maxOffset, ref int index, T toSkip, out T value)
    {
        if (InputStream == null) throw new InvalidOperationException();
        long position = baseOffset + sizeof(T) * index;
        InputStream.Position = position;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (position + sizeof(T) <= maxOffset && Processor.TryRead(InputStream, elementBuffer, out _))
        {
            value = this[elementBuffer];
            if (!value.Equals(toSkip)) return true;
            index++;
            position += sizeof(T);
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Skips to the next entry not matching the specified value.
    /// </summary>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with different value.</param>
    /// <param name="maxIndex">Maximum index (exclusive).</param>
    /// <param name="toSkip">Value to skip.</param>
    /// <param name="value">Retrieved value or 0 if no different values found.</param>
    /// <returns>True if a different value is found.</returns>
    public unsafe bool SkipWhile(long baseOffset, ref int index, int maxIndex, T toSkip, out T value)
    {
        if (InputStream == null) throw new InvalidOperationException();
        InputStream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (index < maxIndex && Processor.TryRead(InputStream, elementBuffer, out _))
        {
            value = this[elementBuffer];
            if (!value.Equals(toSkip)) return true;
            index++;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Skips to the next entry not matching the specified value.
    /// </summary>
    /// <param name="span">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with different value.</param>
    /// <param name="toSkip">Value to skip.</param>
    /// <param name="value">Retrieved value or 0 if no different values found.</param>
    /// <returns>True if a different value is found.</returns>
    public unsafe bool SkipWhile(ReadOnlySpan<byte> span, int baseOffset, ref int index, T toSkip, out T value)
    {
        int position = baseOffset + sizeof(T) * index;
        if (position + sizeof(T) > span.Length)
        {
            value = default;
            return false;
        }
        span = span[position..];
        while (span.Length >= sizeof(T))
        {
            value = this[span];
            if (!value.Equals(toSkip)) return true;
            index++;
            span = span[sizeof(T)..];
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Skips to the next entry not matching the specified value.
    /// </summary>
    /// <param name="span">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="maxOffset">Maximum stream offset (exclusive including element size).</param>
    /// <param name="index">Initial index, ends at first index with different value.</param>
    /// <param name="toSkip">Value to skip.</param>
    /// <param name="value">Retrieved value or 0 if no different values found.</param>
    /// <returns>True if a different value is found.</returns>
    public unsafe bool SkipWhile(ReadOnlySpan<byte> span, int baseOffset, int maxOffset, ref int index, T toSkip, out T value)
    {
        int position = baseOffset + sizeof(T) * index;
        if (position + sizeof(T) > span.Length)
        {
            value = default;
            return false;
        }
        span = span[position..];
        while (position + sizeof(T) <= maxOffset && span.Length >= sizeof(T))
        {
            value = this[span];
            if (!value.Equals(toSkip)) return true;
            index++;
            position += sizeof(T);
            span = span[sizeof(T)..];
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Skips to the next entry not matching the specified value.
    /// </summary>
    /// <param name="span">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with different value.</param>
    /// <param name="maxIndex">Maximum index (exclusive).</param>
    /// <param name="toSkip">Value to skip.</param>
    /// <param name="value">Retrieved value or 0 if no different values found.</param>
    /// <returns>True if a different value is found.</returns>
    public unsafe bool SkipWhile(ReadOnlySpan<byte> span, int baseOffset, ref int index, int maxIndex, T toSkip, out T value)
    {
        int position = baseOffset + sizeof(T) * index;
        if (position + sizeof(T) > span.Length)
        {
            value = default;
            return false;
        }
        span = span[position..];
        while (index < maxIndex && span.Length >= sizeof(T))
        {
            value = this[span];
            if (!value.Equals(toSkip)) return true;
            index++;
            span = span[sizeof(T)..];
        }
        value = default;
        return false;
    }
}
