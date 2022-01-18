using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fp;

internal class SegmentedFileSystemSource : FileSystemSource,
    IEnumerable<(string path, Stream stream)>
{
    private readonly FileSystemSource _source;
    private readonly Dictionary<string, Stream> _outputEntries;
    private readonly HashSet<string> _dirs;
    private readonly bool _proxyWrites;

    internal SegmentedFileSystemSource(FileSystemSource source, bool proxyWrites,
        IEnumerable<BufferData<byte>>? existingEntries = null)
    {
        _source = source;
        _proxyWrites = proxyWrites;
        _outputEntries = new Dictionary<string, Stream>();
        _dirs = new HashSet<string>();
        if (existingEntries == null) return;
        foreach (var existingEntry in existingEntries)
        {
            string path = existingEntry.BasePath.NormalizeAndStripWindowsDrive();
            _dirs.Add(Path.GetDirectoryName(path) ?? Path.GetFullPath("/"));
            _outputEntries.Add(path, new MStream(existingEntry.Buffer));
        }
    }

    protected override Stream OpenReadImpl(string path, FileMode fileMode = FileMode.Open,
        FileShare fileShare = FileShare.Delete | FileShare.None | FileShare.Read | FileShare.ReadWrite |
                              FileShare.Write)
    {
        path = NormalizePath(path);
        if (_source.FileExists(path))
            return _source.OpenRead(path, fileMode, fileShare);
        if (_outputEntries.TryGetValue(path, out var stream))
            return stream;
        throw new FileNotFoundException();
    }

    public override Stream OpenWrite(string path, FileMode fileMode = FileMode.Create,
        FileShare fileShare = FileShare.Delete | FileShare.None | FileShare.Read | FileShare.ReadWrite |
                              FileShare.Write)
    {
        if (_proxyWrites) return _source.OpenWrite(path, fileMode, fileShare);
        path = NormalizePath(path);
        MemoryStream stream = new();
        _outputEntries.Add(path, stream);
        _dirs.Add(Path.GetDirectoryName(path) ?? Path.GetFullPath("/"));
        return stream;
    }

    public override IEnumerable<string> EnumerateFiles(string path)
        => _source.EnumerateFiles(NormalizePath(path));

    public override IEnumerable<string> EnumerateDirectories(string path)
        => _source.EnumerateDirectories(NormalizePath(path));

    public override bool CreateDirectory(string path)
    {
        _dirs.Add(NormalizePath(path));
        return true;
    }

    public override bool FileExists(string path)
    {
        path = NormalizePath(path);
        return _source.FileExists(path) || _outputEntries.ContainsKey(path);
    }

    public override bool DirectoryExists(string path)
    {
        path = NormalizePath(path);
        return _source.DirectoryExists(path) || _dirs.Contains(path);
    }

    public IEnumerator<(string path, Stream stream)> GetEnumerator()
    {
        return _outputEntries.Select(kvp => (kvp.Key, kvp.Value)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string NormalizePath(string path) => path.NormalizeAndStripWindowsDrive();
}
