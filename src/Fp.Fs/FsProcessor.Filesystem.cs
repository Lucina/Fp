using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Fp;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class FsProcessor
{
    #region Filesystem utilities

    /// <summary>
    /// Opens file for reading.
    /// </summary>
    /// <param name="asMain">If true, sets <see cref="Processor.InputStream"/>.</param>
    /// <param name="file">File to open, <see cref="InputFile"/> by default.</param>
    /// <returns>Created stream.</returns>
    public Stream OpenFile(bool asMain, string? file = null)
    {
        if (FileSystem == null)
        {
            throw new InvalidOperationException();
        }

        file ??= InputFile ?? throw new InvalidOperationException();
        file = Path.Combine(InputDirectory ?? throw new InvalidOperationException(), file);
        if (!FileSystem.FileExists(file))
        {
            throw new FileNotFoundException("File not found", file);
        }

        Stream stream = FileSystem.OpenRead(file);
        stream = GetReadingStream(stream);
        if (asMain) UseStream(stream);
        return stream;
    }

    /// <summary>
    /// Opens file for reading and set <see cref="Processor.InputStream"/>.
    /// </summary>
    /// <param name="file">File to open, <see cref="InputFile"/> by default.</param>
    /// <returns>Created stream.</returns>
    public Stream OpenMainFile(string? file = null)
        => OpenFile(true, file);

    /// <summary>
    /// Opens file in <see cref="InputDirectory"/> for reading.
    /// </summary>
    /// <param name="name">File to open.</param>
    /// <returns>Created stream.</returns>
    public Stream OpenFile(string name)
        => OpenFile(false, name);

    private Stream OpenOutputFileInternal(bool sub, bool asMain, string? extension = null,
        string? filename = null)
    {
        if (FileSystem == null) throw new InvalidOperationException();
        filename = sub ? GenPathSub(extension, filename) : GenPath(extension, filename);
        Stream stream = FileSystem.OpenWrite(filename);
        if (asMain) UseOutputStream(stream);
        return stream;
    }

    /// <summary>
    /// Opens file for writing.
    /// </summary>
    /// <param name="asMain">If true, sets <see cref="Processor.OutputStream"/>.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File to open, generates path by default.</param>
    /// <returns>Created stream.</returns>
    public Stream OpenOutputFile(bool asMain, string? extension = null, string? filename = null)
        => OpenOutputFileInternal(false, asMain, extension, filename);

    /// <summary>
    /// Opens file for writing and set <see cref="Processor.OutputStream"/>.
    /// </summary>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File to open, generates path by default.</param>
    /// <returns>Created stream.</returns>
    public Stream OpenOutputFile(string? extension = null, string? filename = null)
        => OpenOutputFileInternal(false, true, extension, filename);

    /// <summary>
    /// Opens file for writing under folder named by current file's name.
    /// </summary>
    /// <param name="asMain">If true, sets <see cref="Processor.OutputStream"/>.</param>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File to open, generates path by default.</param>
    /// <returns>Created stream.</returns>
    public Stream OpenOutputSubFile(bool asMain, string? extension = null, string? filename = null)
        => OpenOutputFileInternal(true, asMain, extension, filename);

    /// <summary>
    /// Opens file for writing under folder named by current file's name and set <see cref="Processor.OutputStream"/>.
    /// </summary>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File to open, generates path by default.</param>
    /// <returns>Created stream.</returns>
    public Stream OpenOutputSubFile(string? extension = null, string? filename = null)
        => OpenOutputFileInternal(true, true, extension, filename);

    /// <summary>
    /// Makes directories above path.
    /// </summary>
    /// <param name="file">Path to make parents for.</param>
    /// <exception cref="IOException">Thrown when system failed to create directories.</exception>
    public void MkParents(string file) => MkDirs(Path.GetDirectoryName(file));

    /// <summary>
    /// Makes directories up to path.
    /// </summary>
    /// <param name="dir">Path to make directories to.</param>
    /// <exception cref="IOException">Thrown when system failed to create directories.</exception>
    public void MkDirs(string? dir = null)
    {
        if (FileSystem == null)
        {
            throw new InvalidOperationException();
        }

        dir ??= OutputDirectory ?? throw new InvalidOperationException();
        if (!FileSystem.CreateDirectory(dir))
        {
            throw new IOException($"Failed to create target directory {dir}");
        }
    }

    /// <summary>
    /// Generates a path under folder named by specified main file's name.
    /// </summary>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File name.</param>
    /// <param name="mainFile">File to use for folder name and file name (if <paramref name="filename"/> not specified).</param>
    /// <param name="mkDirs">If true, create directories in filesystem.</param>
    /// <returns>Generated path.</returns>
    public string GenPathSub(string? extension = null, string? filename = null, string? mainFile = null,
        bool mkDirs = true)
    {
        if (OutputDirectory == null)
            throw new InvalidOperationException();

        mainFile ??= InputFile ?? throw new InvalidOperationException();
        filename = filename == null
            ? $"{Path.GetFileNameWithoutExtension(mainFile)}_{OutputCounter++:D8}{extension}"
            : $"{filename}{extension}";
        string path = Join(SupportBackSlash, OutputDirectory, Path.GetFileName(mainFile), filename);
        if (mkDirs)
        {
            MkParents(path);
        }

        return path;
    }

    /// <summary>
    /// Generates a path.
    /// </summary>
    /// <param name="extension">File extension.</param>
    /// <param name="filename">File name.</param>
    /// <param name="directory">Main output directory.</param>
    /// <param name="mainFile">File to use for file name (if <paramref name="filename"/> not specified).</param>
    /// <param name="mkDirs">If true, create directories in filesystem.</param>
    /// <returns>Generated path.</returns>
    public string GenPath(string? extension = null, string? filename = null, string? directory = null,
        string? mainFile = null, bool mkDirs = true)
    {
        if (OutputDirectory == null)
        {
            throw new InvalidOperationException();
        }

        mainFile ??= InputFile ?? throw new InvalidOperationException();
        filename = filename == null
            ? $"{Path.GetFileNameWithoutExtension(mainFile)}_{OutputCounter++:D8}{extension}"
            : $"{filename}{extension}";
        string path = Path.Combine(OutputDirectory, directory ?? string.Empty, filename);
        if (mkDirs)
        {
            MkParents(path);
        }

        return path;
    }

    #endregion
}
