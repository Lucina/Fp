using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using static Fp.FsProcessor;

namespace Fp
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public partial class FsProcessor
    {
        #region Filter utilities

        /// <summary>
        /// Checks if a file exists in the same folder as current file.
        /// </summary>
        /// <param name="sibling">Filename to check.</param>
        /// <returns>True if file with provided name exists next to current file.</returns>
        public bool HasSibling(string sibling) =>
            (FileSystem ?? throw new InvalidOperationException())
            .FileExists(Path.Combine(InputDirectory, sibling));

        /// <summary>
        /// Checks if a file exists in the same folder as specified path.
        /// </summary>
        /// <param name="path">Main path.</param>
        /// <param name="sibling">Filename to check.</param>
        /// <returns>True if file with provided name exists next to current file.</returns>
        public bool PathHasSibling(string path, string sibling) =>
            (FileSystem ?? throw new InvalidOperationException())
            .FileExists(Path.Combine(Path.GetDirectoryName(path) ?? "/", sibling));

        /// <summary>
        /// Checks if current file has any one of the given extensions.
        /// </summary>
        /// <param name="extensions">File extensions to check.</param>
        /// <returns>True if any extension matches.</returns>
        public bool HasExtension(params string[] extensions) =>
            PathHasExtension(InputFile ?? throw new InvalidOperationException(), extensions);

        #endregion
    }


    // ReSharper disable InconsistentNaming
    public partial class Scripting
    {
        #region Filter

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="source">Source to read.</param>v
        /// <param name="offset">Source offset.</param>
        /// <param name="text">Value to check for.</param>
        /// <returns>True if found.</returns>
        public static bool magic(this byte[] source, ReadOnlySpan<byte> text, int offset = 0) =>
            Processor.HasMagic(source, text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="source">Source to read.</param>v
        /// <param name="offset">Source offset.</param>
        /// <param name="text">Value to check for.</param>
        /// <returns>True if found.</returns>
        public static bool magic(this byte[] source, string text, int offset = 0) =>
            Processor.HasMagic(source, text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="source">Source to read.</param>v
        /// <param name="offset">Source offset.</param>
        /// <param name="text">Value to check for.</param>
        /// <returns>True if found.</returns>
        public static bool magic(this Memory<byte> source, ReadOnlySpan<byte> text, int offset = 0) =>
            Processor.HasMagic(source.Span, text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="source">Source to read.</param>v
        /// <param name="offset">Source offset.</param>
        /// <param name="text">Value to check for.</param>
        /// <returns>True if found.</returns>
        public static bool magic(this Memory<byte> source, string text, int offset = 0) =>
            Processor.HasMagic(source.Span, text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="source">Source to read.</param>v
        /// <param name="offset">Source offset.</param>
        /// <param name="text">Value to check for.</param>
        /// <returns>True if found.</returns>
        public static bool magic(this ReadOnlyMemory<byte> source, ReadOnlySpan<byte> text, int offset = 0) =>
            Processor.HasMagic(source.Span, text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="source">Source to read.</param>v
        /// <param name="offset">Source offset.</param>
        /// <param name="text">Value to check for.</param>
        /// <returns>True if found.</returns>
        public static bool magic(this ReadOnlyMemory<byte> source, string text, int offset = 0) =>
            Processor.HasMagic(source.Span, text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="source">Source to read.</param>v
        /// <param name="offset">Source offset.</param>
        /// <param name="text">Value to check for.</param>
        /// <returns>True if found.</returns>
        public static bool magic(this Span<byte> source, ReadOnlySpan<byte> text, int offset = 0) =>
            Processor.HasMagic(source, text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="source">Source to read.</param>v
        /// <param name="offset">Source offset.</param>
        /// <param name="text">Value to check for.</param>
        /// <returns>True if found.</returns>
        public static bool magic(this Span<byte> source, string text, int offset = 0) =>
            Processor.HasMagic(source, text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="source">Source to read.</param>v
        /// <param name="offset">Source offset.</param>
        /// <param name="text">Value to check for.</param>
        /// <returns>True if found.</returns>
        public static bool magic(this ReadOnlySpan<byte> source, ReadOnlySpan<byte> text, int offset = 0) =>
            Processor.HasMagic(source, text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="source">Source to read.</param>v
        /// <param name="offset">Source offset.</param>
        /// <param name="text">Value to check for.</param>
        /// <returns>True if found.</returns>
        public static bool magic(this ReadOnlySpan<byte> source, string text, int offset = 0) =>
            Processor.HasMagic(source, text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="text">Value to check for.</param>
        /// <param name="offset">Source offset.</param>
        /// <returns>True if found.</returns>
        public static bool magic(ReadOnlySpan<byte> text, long offset = 0) =>
            Current.HasMagic(text, offset);

        /// <summary>
        /// Checks for identifier.
        /// </summary>
        /// <param name="text">Value to check for.</param>
        /// <param name="offset">Source offset.</param>
        /// <returns>True if found.</returns>
        public static bool magic(string text, long offset = 0) =>
            Current.HasMagic(text, offset);

        #endregion
    }
    // ReSharper restore InconsistentNaming
}
