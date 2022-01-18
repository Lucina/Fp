using System.Diagnostics.CodeAnalysis;

namespace Fp;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public partial class FsProcessor
{
    #region Stream read utilities

    /// <summary>
    /// Loads newly allocated byte array from file relative to <see cref="InputDirectory"/>.
    /// </summary>
    /// <param name="path">Input file path.</param>
    /// <returns>Array with file contents.</returns>
    public byte[] Load(string path)
    {
        using var fs = OpenFile(path);
        return GetArray(fs, true);
    }

    #endregion
}
