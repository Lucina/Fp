using System.Collections.Generic;
using System.IO;

namespace Fp;

/// <summary>
/// Object for I/O to some filesystem provider.
/// </summary>
public abstract class FileSystemSource
{
    static FileSystemSource()
    {
        Default = new RealFileSystemSource();
    }

    /// <summary>
    /// Set when filesystem is being accessed in parallel.
    /// </summary>
    /// <remarks>
    /// This options determines whether the provider will copy its input streams to allow unhindered seeking.
    /// </remarks>
    public bool ParallelAccess;

    /// <summary>
    /// Default filesystem provider for platform.
    /// </summary>
    public static readonly FileSystemSource Default;

    /// <summary>
    /// Normalizes a path to the underlying filesystem.
    /// </summary>
    /// <param name="path">Path to normalize.</param>
    /// <returns>Normalized path.</returns>
    public abstract string NormalizePath(string path);

    /// <summary>
    /// Gets a seekable read-only stream.
    /// </summary>
    /// <param name="path">Path to open.</param>
    /// <param name="fileMode">File open mode.</param>
    /// <param name="fileShare">File sharing mode.</param>
    /// <returns>Stream.</returns>
    public Stream OpenRead(string path, FileMode fileMode = FileMode.Open, FileShare fileShare =
        FileShare.ReadWrite | FileShare.Delete)
    {
        Stream src = Processor.GetSeekableStream(OpenReadImpl(path, fileMode, fileShare));
        if (!ParallelAccess || src is MemoryStream) return src;
        using Stream ssrc = src;
        MemoryStream ms = new(new byte[src.Length]);
        ssrc.CopyTo(ms);
        ms.Position = 0;
        return ms;
    }

    /// <summary>
    /// Gets a read-only stream.
    /// </summary>
    /// <param name="path">Path to open.</param>
    /// <param name="fileMode">File open mode.</param>
    /// <param name="fileShare">File sharing mode.</param>
    /// <returns>Stream.</returns>
    protected abstract Stream OpenReadImpl(string path, FileMode fileMode = FileMode.Open, FileShare fileShare =
        FileShare.ReadWrite | FileShare.Delete);

    /// <summary>
    /// Get a write-only stream.
    /// </summary>
    /// <param name="path">Path to open.</param>
    /// <param name="fileMode">File open mode.</param>
    /// <param name="fileShare">File sharing mode.</param>
    /// <returns>Stream.</returns>
    public abstract Stream OpenWrite(string path, FileMode fileMode = FileMode.Create,
        FileShare fileShare = FileShare.ReadWrite | FileShare.Delete);

    /// <summary>
    /// Enumerates files in a directory.
    /// </summary>
    /// <param name="path">Directory to enumerate.</param>
    /// <returns>Files in directory.</returns>
    public abstract IEnumerable<string> EnumerateFiles(string path);

    /// <summary>
    /// Enumerates directories in a directory.
    /// </summary>
    /// <param name="path">Directory to enumerate.</param>
    /// <returns>Directories in directory.</returns>
    public abstract IEnumerable<string> EnumerateDirectories(string path);

    /// <summary>
    /// Creates a directory (including parents).
    /// </summary>
    /// <param name="path">Directory to create.</param>
    /// <returns>True if succeeded.</returns>
    public abstract bool CreateDirectory(string path);

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    /// <param name="path">File path to check.</param>
    /// <returns>True if exists.</returns>
    public abstract bool FileExists(string path);


    /// <summary>
    /// Checks if a directory exists.
    /// </summary>
    /// <param name="path">Directory path to check.</param>
    /// <returns>True if exists.</returns>
    public abstract bool DirectoryExists(string path);
}
