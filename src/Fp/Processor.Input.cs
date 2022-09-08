using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Fp
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    public partial class Processor
    {
        #region Stream read utilities

        /// <summary>
        /// Gets a read-optimized stream from an existing stream, disposing the passed stream if it is replaced.
        /// </summary>
        /// <param name="stream">Stream to read.</param>
        /// <returns>Equivalent stream optimized for reading.</returns>
        public Stream GetReadingStream(Stream stream)
        {
            if (Preload && (stream is not MemoryStream alreadyMs || !alreadyMs.TryGetBuffer(out _)))
            {
                MemoryStream ms = new(new byte[stream.Length]);
                stream.CopyTo(ms);
                stream.Dispose();
                stream = ms;
            }

            if (stream is FileStream)
                stream = new MultiBufferStream(stream, true);
            return stream;
        }

        /// <summary>
        /// Gets a seekable stream (closes base stream if replaced).
        /// </summary>
        /// <param name="stream">Base stream.</param>
        /// <returns>Seekable stream.</returns>
        /// <remarks>
        /// This method conditionally creates a seekable stream from a non-seekable stream by copying the
        /// stream's contents to a new <see cref="MemoryStream"/> instance. The returned object is either
        /// this newly created stream or the passed argument <paramref name="stream"/> if it was already seekable.
        /// </remarks>
        public static Stream GetSeekableStream(Stream stream)
        {
            if (stream.CanSeek)
            {
                return stream;
            }

            MemoryStream ms;
            try
            {
                long length = stream.Length;
                ms = length > int.MaxValue ? new MemoryStream() : new MemoryStream(new byte[length]);
            }
            catch
            {
                ms = new MemoryStream();
            }

            stream.CopyTo(ms);
            stream.Close();
            ms.Position = 0;
            stream = ms;
            return stream;
        }

        /// <summary>
        /// Skips data in stream.
        /// </summary>
        /// <param name="bytes">Number of bytes to skip.</param>
        /// <param name="stream">Stream to operate on.</param>
        /// <returns>New position in stream.</returns>
        public static long Skip(long bytes, Stream stream)
            => stream.Seek(bytes, SeekOrigin.Current);

        /// <summary>
        /// Skips data in current file's input stream.
        /// </summary>
        /// <param name="bytes">Number of bytes to skip.</param>
        /// <returns>New position in stream.</returns>
        public long Skip(long bytes)
            => (_inputStream ?? throw new InvalidOperationException()).Seek(bytes, SeekOrigin.Current);

        internal static int ReadBaseArray(Stream stream, byte[] array, int offset, int length, bool lenient)
        {
            int left = length, read, tot = 0;
            do
            {
                read = stream.Read(array, offset + tot, left);
                left -= read;
                tot += read;
            } while (left > 0 && read != 0);

            if (left > 0 && read == 0 && !lenient)
            {
                throw new IOException(
                    $"Failed to read required number of bytes! 0x{read:X} read, 0x{left:X} left, 0x{stream.Position:X} end position");
            }

            return tot;
        }

        internal static int ReadBaseSpan(Stream stream, Span<byte> span, bool lenient)
        {
            int left = span.Length, read, tot = 0;
            do
            {
                read = stream.Read(span[tot..]);
                left -= read;
                tot += read;
            } while (left > 0 && read != 0);

            if (left > 0 && !lenient)
            {
                throw new IOException(
                    $"Failed to read required number of bytes! 0x{read:X} read, 0x{left:X} left, 0x{stream.Position:X} end position");
            }

            return tot;
        }

        #region Stream to span

        /// <summary>
        /// Reads data from stream, optionally replacing reference to provided span to prevent copy when reading from <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use provided span.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        public static int Read(Stream stream, ref Span<byte> span, bool lenient = true, bool forceNew = false)
        {
            if (forceNew || stream is not MemoryStream ms || !ms.TryGetBuffer(out ArraySegment<byte> buf))
            {
                return ReadBaseSpan(stream, span, lenient);
            }

            try
            {
                return (span = buf.AsSpan((int)ms.Position, span.Length)).Length;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                throw new IOException("Failed to convert span from memory stream", exception);
            }
        }

        /// <summary>
        /// Reads data from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        public static int Read(Stream stream, int length, out Span<byte> span, bool lenient = true,
            bool forceNew = false)
        {
            if (!forceNew && stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> buf))
            {
                try
                {
                    return (span = buf.AsSpan((int)stream.Position, length)).Length;
                }
                catch (ArgumentOutOfRangeException exception)
                {
                    throw new IOException("Failed to convert span from memory stream", exception);
                }
            }

            span = new Span<byte>(new byte[length]);
            return ReadBaseSpan(stream, span, lenient);
        }

        /// <summary>
        /// Reads data from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        public static int Read(Stream stream, Span<byte> span, bool lenient = true)
        {
            if (stream is not MemoryStream ms || !ms.TryGetBuffer(out ArraySegment<byte> buf))
            {
                return ReadBaseSpan(stream, span, lenient);
            }

            try
            {
                buf.AsSpan((int)ms.Position, span.Length).CopyTo(span);
                return span.Length;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                throw new IOException("Failed to convert span from memory stream", exception);
            }
        }

        #endregion

        #region Implicit input stream to span

        /// <summary>
        /// Reads data from current file's input stream, optionally replacing reference to provided span to prevent copy when reading from <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use provided span.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        public int Read(ref Span<byte> span, bool lenient = true, bool forceNew = false)
            => Read(_inputStream ?? throw new InvalidOperationException(), ref span, lenient, forceNew);

        /// <summary>
        /// Reads data from current file's input stream.
        /// </summary>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        public int Read(int length, out Span<byte> span, bool lenient = true, bool forceNew = false)
            => Read(_inputStream ?? throw new InvalidOperationException(), length, out span, lenient, forceNew);

        /// <summary>
        /// Reads data from current file's input stream.
        /// </summary>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        public int Read(Span<byte> span, bool lenient = true)
            => Read(_inputStream ?? throw new InvalidOperationException(), span, lenient);

        #endregion

        #region Offset stream to span

        /// <summary>
        /// Reads data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        public static int Read(Stream stream, long offset, Span<byte> span, bool lenient = true)
        {
            long position = stream.Position;
            try
            {
                stream.Position = offset;
                int count = Read(stream, span, lenient);
                return count;
            }
            finally
            {
                stream.Position = position;
            }
        }

        /// <summary>
        /// Reads data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use provided span.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        public static int Read(Stream stream, long offset, ref Span<byte> span, bool lenient = true,
            bool forceNew = false)
        {
            long position = stream.Position;
            try
            {
                stream.Position = offset;
                int count = Read(stream, ref span, lenient, forceNew);
                return count;
            }
            finally
            {
                stream.Position = position;
            }
        }

        /// <summary>
        /// Reads data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        public static int Read(Stream stream, long offset, int length, out Span<byte> span, bool lenient = true,
            bool forceNew = false)
        {
            long position = stream.Position;
            try
            {
                stream.Position = offset;
                int count = Read(stream, length, out span, lenient, forceNew);
                return count;
            }
            finally
            {
                stream.Position = position;
            }
        }

        #endregion

        #region Offset implicit input stream to span

        /// <summary>
        /// Reads data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        public int Read(long offset, Span<byte> span, bool lenient = true)
            => Read(_inputStream ?? throw new InvalidOperationException(), offset, span, lenient);

        /// <summary>
        /// Reads data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use provided span.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        public int Read(long offset, ref Span<byte> span, bool lenient = true, bool forceNew = false)
            => Read(_inputStream ?? throw new InvalidOperationException(), offset, ref span, lenient, forceNew);

        /// <summary>
        /// Reads data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="length">Number of bytes to try to read.</param>
        /// <param name="span">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        public int Read(long offset, int length, out Span<byte> span, bool lenient = true, bool forceNew = false)
            => Read(_inputStream ?? throw new InvalidOperationException(), offset, length, out span, lenient, forceNew);

        #endregion

        #region Stream to byte array

        /// <summary>
        /// Reads data from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="arrayOffset">Offset in array to write to.</param>
        /// <param name="arrayLength">Length to write.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        public static int Read(Stream stream, byte[] array, int arrayOffset, int arrayLength, bool lenient = true) =>
            ReadBaseArray(stream, array, arrayOffset, arrayLength, lenient);


        /// <summary>
        /// Reads data from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        public static int Read(Stream stream, byte[] array, bool lenient = true)
            => Read(stream, array, 0, array.Length, lenient);

        #endregion

        #region Implicit input stream to byte array

        /// <summary>
        /// Reads data from current file's input stream.
        /// </summary>
        /// <param name="array">Target to copy to.</param>
        /// <param name="arrayOffset">Offset in array to write to.</param>
        /// <param name="arrayLength">Length to write.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        public int Read(byte[] array, int arrayOffset, int arrayLength, bool lenient = true)
            => Read(_inputStream ?? throw new InvalidOperationException(), array, arrayOffset, arrayLength, lenient);


        /// <summary>
        /// Reads data from current file's input stream.
        /// </summary>
        /// <param name="array">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        public int Read(byte[] array, bool lenient = true)
            => Read(_inputStream ?? throw new InvalidOperationException(), array, 0, array.Length, lenient);

        #endregion

        #region Offset stream to byte array

        /// <summary>
        /// Reads data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="arrayOffset">Offset in array to write to.</param>
        /// <param name="arrayLength">Length to write.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        public static int Read(Stream stream, long offset, byte[] array, int arrayOffset, int arrayLength,
            bool lenient = true)
        {
            long position = stream.Position;
            try
            {
                stream.Position = offset;
                int count = Read(stream, array, arrayOffset, arrayLength, lenient);
                return count;
            }
            finally
            {
                stream.Position = position;
            }
        }

        /// <summary>
        /// Reads data from stream at the specified offset.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <paramref name="stream"/> is restored on completion.</remarks>
        public static int Read(Stream stream, long offset, byte[] array, bool lenient = true)
            => Read(stream, offset, array, 0, array.Length, lenient);

        #endregion

        #region Offset implicit input stream to byte array

        /// <summary>
        /// Reads data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="arrayOffset">Offset in array to write to.</param>
        /// <param name="arrayLength">Length to write.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        public int Read(long offset, byte[] array, int arrayOffset, int arrayLength, bool lenient = true)
            => Read(_inputStream ?? throw new InvalidOperationException(), offset, array, arrayOffset, arrayLength,
                lenient);

        /// <summary>
        /// Reads data from current file's input stream at the specified offset.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <param name="array">Target to copy to.</param>
        /// <param name="lenient">If false, throws when failed to fill target.</param>
        /// <returns>Number of bytes read.</returns>
        /// <exception cref="IOException">Thrown when <paramref name="lenient"/> is false
        /// and stream cannot provide enough data to fill target.</exception>
        /// <remarks>Original position of <see cref="InputStream"/> is restored on completion.</remarks>
        public int Read(long offset, byte[] array, bool lenient = true)
            => Read(offset, array, 0, array.Length, lenient);

        #endregion

        /// <summary>
        /// Gets byte array from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Array with file contents.</returns>
        public static byte[] GetArray(Stream stream, bool forceNew = false)
        {
            if (!stream.CanSeek)
                throw new NotSupportedException("Getting memory from non-seekable stream is unsupported");
            switch (stream)
            {
                case MStream mes:
                    return mes.GetMemory().ToArray();
                case MemoryStream ms when !forceNew:
                    return ms.Capacity == ms.Length && ms.TryGetBuffer(out _) ? ms.GetBuffer() : ms.ToArray();
                default:
                    stream.Position = 0;
                    try
                    {
                        byte[] arr = new byte[stream.Length];
                        Read(stream, arr, false);
                        return arr;
                    }
                    catch (Exception)
                    {
                        // Fallback to MemoryStream copy
                        stream.Position = 0;
                        MemoryStream ms2 = new();
                        stream.CopyTo(ms2);
                        return ms2.ToArray();
                    }
            }
        }

        /// <summary>
        /// Loads newly allocated byte array from input stream.
        /// </summary>
        /// <returns>Array with file contents.</returns>
        public byte[] Load()
            => GetArray(_inputStream ?? throw new InvalidOperationException(), true);

        /// <summary>
        /// Gets byte array from input stream.
        /// </summary>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Array with file contents.</returns>
        public byte[] GetArray(bool forceNew = false)
            => GetArray(_inputStream ?? throw new InvalidOperationException(), forceNew);

        /// <summary>
        /// Gets byte array from stream.
        /// </summary>
        /// <param name="offset">Offset in stream.</param>
        /// <param name="length">Length of segment.</param>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Array with file contents.</returns>
        /// <exception cref="NotSupportedException">Thrown if the stream is not seekable.</exception>
        public static byte[] GetArray(int offset, int length, Stream stream, bool forceNew = false)
        {
            if (!stream.CanSeek)
                throw new NotSupportedException("Getting memory from non-seekable stream is unsupported");
            switch (stream)
            {
                case MStream mes:
                    return mes.GetMemory().Slice(offset, length).ToArray();
                case MemoryStream ms when !forceNew:
                    return offset == 0 && ms.Length == length && ms.Capacity == ms.Length && ms.TryGetBuffer(out _)
                        ? ms.GetBuffer()
                        : ms.ToArray();
                default:
                    if (offset + length > stream.Length)
                        throw new IOException("Target range exceeds stream bounds");
                    stream.Position = offset;
                    try
                    {
                        byte[] arr = new byte[length];
                        Read(stream, arr, false);
                        return arr;
                    }
                    catch (Exception)
                    {
                        // Fallback to MemoryStream copy
                        stream.Position = offset;
                        MemoryStream ms2 = new();
                        new SStream(stream, length).CopyTo(ms2);
                        return ms2.ToArray();
                    }
            }
        }

        /// <summary>
        /// Gets byte array from input stream.
        /// </summary>
        /// <param name="offset">Offset in stream.</param>
        /// <param name="length">Length of segment.</param>
        /// <param name="forceNew">Force use newly allocated buffer.</param>
        /// <returns>Array with file contents.</returns>
        public byte[] GetArray(int offset, int length, bool forceNew = false)
            => GetArray(offset, length, _inputStream ?? throw new InvalidOperationException(), forceNew);

        /// <summary>
        /// Dumps remaining content from stream to byte array.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <returns>Array with file contents.</returns>
        public static byte[] Dump(Stream stream, int maxLength = int.MaxValue)
        {
            MemoryStream ms2 = new();
            new SStream(stream, maxLength, false).CopyTo(ms2);
            return ms2.ToArray();
        }

        /// <summary>
        /// Dumps remaining content from input stream to byte array.
        /// </summary>
        /// <param name="maxLength">Maximum length.</param>
        /// <returns>Array with file contents.</returns>
        public byte[] Dump(int maxLength = int.MaxValue)
            => Dump(_inputStream ?? throw new InvalidOperationException(), maxLength);

        /// <summary>
        /// Gets read-only memory from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Array with file contents.</returns>
        /// <remarks>Non-allocating requisition of memory from <see cref="MemoryStream"/> and <see cref="MStream"/> is supported.</remarks>
        public static ReadOnlyMemory<byte> GetMemory(Stream stream)
        {
            if (!stream.CanSeek)
                throw new NotSupportedException("Getting memory from non-seekable stream is unsupported");
            switch (stream)
            {
                case MStream mes:
                    return mes.GetMemory();
                case MemoryStream ms when ms.TryGetBuffer(out ArraySegment<byte> buffer):
                    return buffer;
                default:
                    stream.Position = 0;
                    try
                    {
                        byte[] arr = new byte[stream.Length];
                        Read(stream, arr, false);
                        return arr;
                    }
                    catch (Exception)
                    {
                        // Fallback to MemoryStream copy
                        stream.Position = 0;
                        MemoryStream ms2 = new();
                        stream.CopyTo(ms2);
                        return ms2.GetBuffer().AsMemory(0, (int)ms2.Length);
                    }
            }
        }

        /// <summary>
        /// Gets read-only memory from input stream.
        /// </summary>
        /// <returns>Array with file contents.</returns>
        /// <remarks>Non-allocating requisition of memory from <see cref="MemoryStream"/> and <see cref="MStream"/> is supported.</remarks>
        public ReadOnlyMemory<byte> GetMemory()
            => GetMemory(_inputStream ?? throw new InvalidOperationException());

        /// <summary>
        /// Gets read-only memory from stream.
        /// </summary>
        /// <param name="offset">Offset in stream.</param>
        /// <param name="length">Length of segment.</param>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Array with file contents.</returns>
        /// <remarks>Non-allocating requisition of memory from <see cref="MemoryStream"/> and <see cref="MStream"/> is supported.</remarks>
        public static ReadOnlyMemory<byte> GetMemory(long offset, int length, Stream stream)
        {
            if (!stream.CanSeek)
                throw new NotSupportedException("Getting memory from non-seekable stream is unsupported");
            switch (stream)
            {
                case MStream mes:
                    return mes.GetMemory().Slice((int)offset, length);
                case MemoryStream ms when ms.TryGetBuffer(out ArraySegment<byte> buffer):
                    return buffer.AsMemory((int)offset, length);
                default:
                    if (offset + length > stream.Length)
                        throw new IOException("Target range exceeds stream bounds");
                    stream.Position = offset;
                    try
                    {
                        byte[] arr = new byte[length];
                        Read(stream, arr, false);
                        return arr;
                    }
                    catch (Exception)
                    {
                        // Fallback to MemoryStream copy
                        stream.Position = offset;
                        MemoryStream ms2 = new();
                        new SStream(stream, length).CopyTo(ms2);
                        return ms2.GetBuffer().AsMemory(0, length);
                    }
            }
        }

        /// <summary>
        /// Gets read-only memory from input stream.
        /// </summary>
        /// <param name="offset">Offset in stream.</param>
        /// <param name="length">Length of segment.</param>
        /// <returns>Array with file contents.</returns>
        /// <remarks>Non-allocating requisition of memory from <see cref="MemoryStream"/> and <see cref="MStream"/> is supported.</remarks>
        public ReadOnlyMemory<byte> GetMemory(long offset, int length)
            => GetMemory(offset, length, _inputStream ?? throw new InvalidOperationException());

        /// <summary>
        /// Ensures a file is loaded / initialized with nonzero length.
        /// </summary>
        /// <param name="target">Target memory variable.</param>
        /// <param name="init">Initialization flag.</param>
        /// <param name="openDelegate">Delegate for opening file (stream will be disposed).</param>
        /// <param name="storeDelegate">Delegate for getting data.</param>
        /// <returns>true if file is loaded.</returns>
        public bool EnsureFile(ref Memory<byte> target, ref bool init, Func<Stream> openDelegate,
            Func<Stream, Memory<byte>> storeDelegate)
        {
            if (init) return target.Length != 0;
            init = true;
            try
            {
                using var fs = openDelegate();
                target = storeDelegate(fs);
                return true;
            }
            catch (Exception e)
            {
                LogFail($"Failed to load file: {e}");
                target = Memory<byte>.Empty;
                return false;
            }
        }

        #endregion
    }

    public partial class FpUtil
    {
        /// <summary>
        /// Dumps byte array from a stream.
        /// </summary>
        /// <param name="stream">Stream to dump.</param>
        /// <param name="maxLength">Maximum input length.</param>
        /// <returns>Byte array.</returns>
        public static byte[] Dump(this Stream stream, int maxLength = int.MaxValue) =>
            Processor.Dump(stream, maxLength);

        /// <summary>
        /// Creates stream from array.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Stream.</returns>
        public static Stream Stream(this byte[] source) => new MStream(source);

        /// <summary>
        /// Creates stream from memory.
        /// </summary>
        /// <param name="source">Source memory.</param>
        /// <returns>Stream.</returns>
        public static Stream Stream(this Memory<byte> source) => new MStream(source);

        /// <summary>
        /// Creates stream from memory.
        /// </summary>
        /// <param name="source">Source memory.</param>
        /// <returns>Stream.</returns>
        public static Stream Stream(this ReadOnlyMemory<byte> source) => new MStream(source);

        /// <summary>
        /// Gets seekable stream (closes base stream if replaced).
        /// </summary>
        /// <param name="stream">Base stream.</param>
        /// <returns>Seekable stream.</returns>
        /// <remarks>
        /// This method conditionally creates a seekable stream from a non-seekable stream by copying the
        /// stream's contents to a new <see cref="MemoryStream"/> instance. The returned object is either
        /// this newly created stream or the passed argument <paramref name="stream"/> if it was already seekable.
        /// </remarks>
        public static Stream Seekable(this Stream stream) => Processor.GetSeekableStream(stream);
    }
}
