using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Fp;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class Processor
{
    #region Filesystem utilities

    /// <summary>
    /// Sets stream for reading.
    /// </summary>
    /// <param name="stream">Stream to use.</param>
    /// <returns>Created stream.</returns>
    public void UseStream(Stream? stream)
    {
        _inputStream?.Dispose();
        InputStream = stream;
    }

    /// <summary>
    /// Closes stream.
    /// </summary>
    /// <param name="asMain">If true, clear <see cref="InputStream"/>.</param>
    /// <param name="stream">Stream to close.</param>
    public void CloseFile(bool asMain, Stream? stream = null)
    {
        if (stream == null && !asMain) return;
        stream ??= _inputStream;
        stream?.Dispose();
        if (asMain) InputStream = null;
    }

    /// <summary>
    /// Closes stream and clear <see cref="InputStream"/>.
    /// </summary>
    /// <param name="stream">Stream to close.</param>
    public void CloseFile(Stream? stream = null)
        => CloseFile(true, stream);

    /// <summary>
    /// Sets stream for writing.
    /// </summary>
    /// <param name="stream">Stream to use.</param>
    /// <returns>Created stream.</returns>
    public void UseOutputStream(Stream? stream)
    {
        OutputStream?.Flush();
        OutputStream?.Dispose();
        OutputStream = stream;
    }

    /// <summary>
    /// Closes stream.
    /// </summary>
    /// <param name="asMain">If true, clear <see cref="OutputStream"/>.</param>
    /// <param name="stream">Stream to close.</param>
    public void CloseOutputFile(bool asMain, Stream? stream = null)
    {
        if (stream == null && !asMain) return;
        stream ??= OutputStream;
        stream?.Dispose();
        if (asMain) OutputStream = null;
    }

    /// <summary>
    /// Closes stream and clear <see cref="OutputStream"/>.
    /// </summary>
    /// <param name="stream">Stream to close.</param>
    public void CloseOutputFile(Stream? stream = null)
        => CloseOutputFile(true, stream);

    /// <summary>
    /// Generates a native path by replacing \ and / with native separators.
    /// </summary>
    /// <param name="path">Path to change.</param>
    /// <returns>Native OS compatible path.</returns>
    public static string MakeNative(string path)
        => path.Replace('\\', Path.AltDirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
    // Change to Normalize, kill ./.. traversal

    /// <summary>
    /// Creates a path from components.
    /// </summary>
    /// <param name="paths">Elements to join.</param>
    /// <returns>Path.</returns>
    /// <exception cref="Exception">Thrown if separator is encountered by itself.</exception>
    public string Join(params string[] paths)
        => Join(SupportBackSlash, paths);

    /// <summary>
    /// Creates a path from components.
    /// </summary>
    /// <param name="supportBackSlash">Whether to allow backslashes as separators.</param>
    /// <param name="paths">Elements to join.</param>
    /// <returns>Path.</returns>
    /// <exception cref="ArgumentException">Thrown if separator is encountered by itself.</exception>
    public static unsafe string Join(bool supportBackSlash, params string[] paths)
    {
        if (paths.Length < 2)
        {
            return paths.Length == 0 ? string.Empty : paths[0] ?? throw new ArgumentException("Null element");
        }

        int capacity = paths.Sum(path => (path ?? throw new ArgumentException("Null element")).Length);
        char[] buf = ArrayPool<char>.Shared.Rent(capacity + paths.Length - 1);
        try
        {
            Span<char> bufSpan = buf.AsSpan();
            int cIdx = 0;
            bool prevEndWithSeparator = false;
            foreach (string path in paths)
            {
                int pathLength = path.Length;
                if (pathLength == 0)
                {
                    continue;
                }

                ReadOnlySpan<char> span = path.AsSpan();
                char first = span[0];
                if (first == '/' || supportBackSlash && first == '\\')
                {
                    if (pathLength == 1)
                    {
                        if (cIdx == 0) bufSpan[cIdx++] = first;
                    }
                    else if (prevEndWithSeparator)
                    {
                        span = span.Slice(1, --pathLength);
                    }
                }
                else if (cIdx != 0 && !prevEndWithSeparator)
                {
                    bufSpan[cIdx++] = supportBackSlash ? '\\' : '/';
                }

                span.CopyTo(bufSpan.Slice(cIdx));
                cIdx += span.Length;
                char last = span[pathLength - 1];
                prevEndWithSeparator = last == '/' || supportBackSlash && last == '\\';
            }

            fixed (char* ptr = &bufSpan.GetPinnableReference())
            {
                return new string(ptr, 0, cIdx);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buf);
        }
    }

    #endregion
}