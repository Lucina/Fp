using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Fp
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public partial class Processor
    {
        #region Filter utilities

        /// <summary>
        /// Checks if a file has any one of the given extensions.
        /// </summary>
        /// <param name="extensions">File extensions to check.</param>
        /// <param name="file">File to check.</param>
        /// <returns>True if any extension matches.</returns>
        public static bool PathHasExtension(string file, params string?[] extensions) =>
            extensions.Any(extension => extension == null
                ? !file.Contains('.')
                : file.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase));

        /// <summary>
        /// Checks if a span has a specific value at a certain offset.
        /// </summary>
        /// <param name="source">Span to read.</param>
        /// <param name="span">Value to check against.</param>
        /// <param name="offset">Position in span to check.</param>
        /// <returns>True if span region matches value.</returns>
        public static bool HasMagic(ReadOnlySpan<byte> source, ReadOnlySpan<byte> span, int offset = 0) =>
            source.Length - offset >= span.Length && span.SequenceEqual(source.Slice(offset, span.Length));

        /// <summary>
        /// Checks if a span has a specific value at a certain offset.
        /// </summary>
        /// <param name="source">Span to read.</param>
        /// <param name="array">Value to check against.</param>
        /// <param name="offset">Position in span to check.</param>
        /// <returns>True if span region matches value.</returns>
        public static bool HasMagic(ReadOnlySpan<byte> source, byte[] array, int offset = 0)
            => HasMagic(source, array.AsSpan(), offset);

        /// <summary>
        /// Checks if a span has a specific value at a certain offset.
        /// </summary>
        /// <param name="source">Span to read.</param>
        /// <param name="str">Value to check against.</param>
        /// <param name="offset">Position in span to check.</param>
        /// <returns>True if span region matches value.</returns>
        public static bool HasMagic(ReadOnlySpan<byte> source, string str, int offset = 0)
            => HasMagic(source, Ascii(str), offset);

        /// <summary>
        /// Checks if a stream has a specific value at a certain offset.
        /// </summary>
        /// <param name="stream">Stream to read.</param>
        /// <param name="span">Value to check against.</param>
        /// <param name="offset">Position in stream to check.</param>
        /// <returns>True if stream region matches value.</returns>
        public bool HasMagic(Stream stream, ReadOnlySpan<byte> span, long offset = 0)
        {
            Span<byte> span2 = stackalloc byte[span.Length];
            int read = Read(stream, offset, span2);
            return read == span.Length && span.SequenceEqual(span2);
        }

        /// <summary>
        /// Checks if a stream has a specific value at a certain offset.
        /// </summary>
        /// <param name="stream">Stream to read.</param>
        /// <param name="str">Value to check against.</param>
        /// <param name="offset">Position in stream to check.</param>
        /// <returns>True if stream region matches value.</returns>
        public bool HasMagic(Stream stream, string str, long offset = 0)
            => HasMagic(stream, Ascii(str), offset);

        /// <summary>
        /// Checks if current file's input stream has a specific value at a certain offset.
        /// </summary>
        /// <param name="span">Value to check against.</param>
        /// <param name="offset">Position in stream to check.</param>
        /// <returns>True if stream region matches value.</returns>
        public bool HasMagic(ReadOnlySpan<byte> span, long offset = 0)
            => HasMagic(_inputStream ?? throw new InvalidOperationException(), span, offset);

        /// <summary>
        /// Checks if current file's input stream has a specific value at a certain offset.
        /// </summary>
        /// <param name="array">Value to check against.</param>
        /// <param name="offset">Position in stream to check.</param>
        /// <returns>True if stream region matches value.</returns>
        public bool HasMagic(byte[] array, long offset = 0)
            => HasMagic(_inputStream ?? throw new InvalidOperationException(), array.AsSpan(), offset);

        /// <summary>
        /// Checks if current file's input stream has a specific value at a certain offset.
        /// </summary>
        /// <param name="str">Value to check against.</param>
        /// <param name="offset">Position in stream to check.</param>
        /// <returns>True if stream region matches value.</returns>
        public bool HasMagic(string str, long offset = 0)
            => HasMagic(_inputStream ?? throw new InvalidOperationException(), Ascii(str),
                offset);

        #endregion
    }
}
