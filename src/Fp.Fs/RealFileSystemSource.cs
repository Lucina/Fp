using System.Collections.Generic;
using System.IO;

namespace Fp.Fs;

/// <summary>
/// Filesystem provider using System.IO APIs.
/// </summary>
public class RealFileSystemSource : FileSystemSource
{
    /// <inheritdoc />
    protected override Stream OpenReadImpl(string path, FileMode fileMode = FileMode.Open,
        FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        => new FileStream(path, fileMode, FileAccess.Read, fileShare);

    /// <inheritdoc />
    public override Stream OpenWrite(string path, FileMode fileMode = FileMode.Create,
        FileShare fileShare = FileShare.ReadWrite | FileShare.Delete)
        => new FileStream(path, fileMode, FileAccess.Write, fileShare);

    /// <inheritdoc />
    public override IEnumerable<string> EnumerateFiles(string path)
        => Directory.EnumerateFiles(path);

    /// <inheritdoc />
    public override IEnumerable<string> EnumerateDirectories(string path)
        => Directory.EnumerateDirectories(path);

    /// <inheritdoc />
    public override bool CreateDirectory(string path)
        => Directory.CreateDirectory(path).Exists;

    /// <inheritdoc />
    public override bool FileExists(string path)
        => File.Exists(path);

    /// <inheritdoc />
    public override bool DirectoryExists(string path)
        => Directory.Exists(path);

    /// <inheritdoc />
    public override string NormalizePath(string path) => Path.GetFullPath(path);
}
