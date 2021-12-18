using System.Diagnostics.CodeAnalysis;
using static Fp.FsProcessor;

namespace Fp
{
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

    public partial class Scripting
    {
        /// <summary>
        /// Loads byte array from input.
        /// </summary>
        /// <returns>Byte array.</returns>
        public static byte[] load() => Current.Load();

        /// <summary>
        /// Dumps byte array from input.
        /// </summary>
        /// <param name="maxLength">Maximum input length.</param>
        /// <returns>Byte array.</returns>
        public static byte[] dump(int maxLength = int.MaxValue) => Current.Dump(maxLength);
    }
}
