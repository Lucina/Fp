using System;
using System.IO;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

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
// TODO move this
public abstract record BaseUnmanagedIntegerHelper<T> : BaseUnmanagedHelper<T> where T : unmanaged
#if NET7_0_OR_GREATER
    , INumber<T>
#endif
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Skips to the next nonzero entry.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with nonzero value.</param>
    /// <param name="value">Retrieved value or 0 if no nonzero values found.</param>
    /// <returns>True if a nonzero value is found.</returns>
    public unsafe bool SkipToNonzero(Stream stream, long baseOffset, ref int index, out T value)
    {
        stream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (Processor.TryRead(stream, elementBuffer, out _))
        {
            value = this[elementBuffer, 0];
            if (value == T.Zero) index++;
            else return true;
        }
        value = T.Zero;
        return false;
    }

    /// <summary>
    /// Skips to the next nonzero entry.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="maxOffset">Maximum stream offset (exclusive including element size).</param>
    /// <param name="index">Initial index, ends at first index with nonzero value.</param>
    /// <param name="value">Retrieved value or 0 if no nonzero values found.</param>
    /// <returns>True if a nonzero value is found.</returns>
    public unsafe bool SkipToNonzero(Stream stream, long baseOffset, long maxOffset, ref int index, out T value)
    {
        long position = baseOffset + sizeof(T) * index;
        stream.Position = position;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (position + sizeof(T) <= maxOffset && Processor.TryRead(stream, elementBuffer, out _))
        {
            value = this[elementBuffer, 0];
            if (value == T.Zero)
            {
                index++;
                position += sizeof(T);
            }
            else return true;
        }
        value = T.Zero;
        return false;
    }

    /// <summary>
    /// Skips to the next nonzero entry.
    /// </summary>
    /// <param name="stream">Data source.</param>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with nonzero value.</param>
    /// <param name="maxIndex">Maximum index (exclusive).</param>
    /// <param name="value">Retrieved value or 0 if no nonzero values found.</param>
    /// <returns>True if a nonzero value is found.</returns>
    public unsafe bool SkipToNonzero(Stream stream, long baseOffset, ref int index, int maxIndex, out T value)
    {
        stream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (index < maxIndex && Processor.TryRead(stream, elementBuffer, out _))
        {
            value = this[elementBuffer, 0];
            if (value == T.Zero) index++;
            else return true;
        }
        value = T.Zero;
        return false;
    }

    /// <summary>
    /// Skips to the next nonzero entry.
    /// </summary>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with nonzero value.</param>
    /// <param name="value">Retrieved value or 0 if no nonzero values found.</param>
    /// <returns>True if a nonzero value is found.</returns>
    public unsafe bool SkipToNonzero(long baseOffset, ref int index, out T value)
    {
        if (InputStream == null) throw new InvalidOperationException();
        InputStream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (Processor.TryRead(InputStream, elementBuffer, out _))
        {
            value = this[elementBuffer, 0];
            if (value == T.Zero) index++;
            else return true;
        }
        value = T.Zero;
        return false;
    }

    /// <summary>
    /// Skips to the next nonzero entry.
    /// </summary>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="maxOffset">Maximum stream offset (exclusive including element size).</param>
    /// <param name="index">Initial index, ends at first index with nonzero value.</param>
    /// <param name="value">Retrieved value or 0 if no nonzero values found.</param>
    /// <returns>True if a nonzero value is found.</returns>
    public unsafe bool SkipToNonzero(long baseOffset, long maxOffset, ref int index, out T value)
    {
        if (InputStream == null) throw new InvalidOperationException();
        long position = baseOffset + sizeof(T) * index;
        InputStream.Position = position;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (position + sizeof(T) <= maxOffset && Processor.TryRead(InputStream, elementBuffer, out _))
        {
            value = this[elementBuffer, 0];
            if (value == T.Zero)
            {
                index++;
                position += sizeof(T);
            }
            else return true;
        }
        value = T.Zero;
        return false;
    }

    /// <summary>
    /// Skips to the next nonzero entry.
    /// </summary>
    /// <param name="baseOffset">Base offset.</param>
    /// <param name="index">Initial index, ends at first index with nonzero value.</param>
    /// <param name="maxIndex">Maximum index (exclusive).</param>
    /// <param name="value">Retrieved value or 0 if no nonzero values found.</param>
    /// <returns>True if a nonzero value is found.</returns>
    public unsafe bool SkipToNonzero(long baseOffset, ref int index, int maxIndex, out T value)
    {
        if (InputStream == null) throw new InvalidOperationException();
        InputStream.Position = baseOffset + sizeof(T) * index;
        Span<byte> elementBuffer = stackalloc byte[sizeof(T)];
        while (index < maxIndex && Processor.TryRead(InputStream, elementBuffer, out _))
        {
            value = this[elementBuffer, 0];
            if (value == T.Zero) index++;
            else return true;
        }
        value = T.Zero;
        return false;
    }
#endif
}
