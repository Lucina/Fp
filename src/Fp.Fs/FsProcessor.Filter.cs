using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Fp;

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
