using System;
using System.IO;

namespace Fp
{
    /// <summary>
    /// Represents a path element linked to parent nodes.
    /// </summary>
    public record FpPath(string Name, FpPath? Previous = null)
    {
        /// <summary>
        /// Gets a child path with the specified name or sub-path.
        /// </summary>
        /// <param name="name">Child path.</param>
        /// <returns>Combined path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
        public FpPath Add(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name == "") return this;
            if (Path.GetDirectoryName(name) == null) return new FpPath(name, this);
            return GetFromString(name)! with { Previous = this };
        }

        /// <summary>
        /// Gets this instance with a changed extension.
        /// </summary>
        /// <param name="extension">New extension.</param>
        /// <returns>Path with replaced extension.</returns>
        public FpPath ChangeExtension(string extension) =>
            this with { Name = Path.ChangeExtension(Name, extension) };

        /*public string AsJoined() => Join(PlatformSupportBackSlash, AsArray());
            public string AsJoined(bool supportBackSlash) => Join(supportBackSlash, AsArray());*/

        /// <summary>
        /// Gets this path as a string using <see cref="Path.Combine(string[])"/>.
        /// </summary>
        /// <returns>This path as as string.</returns>
        public string AsCombined() => Path.Combine(AsArray());

        /// <summary>
        /// Gets this path's elements as an array.
        /// </summary>
        /// <returns>Path element array.</returns>
        public string[] AsArray()
        {
            int len = 1;
            FpPath? p = this;
            while ((p = p.Previous) != null) len++;
            string[] res = new string[len];
            FpPath p2 = this;
            for (int i = len - 1; i >= 0; i--, p2 = p2.Previous!)
                res[i] = p2.Name;
            return res;
        }

        /// <summary>
        /// Gets a path from a string.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Path.</returns>
        public static FpPath? GetFromString(string? value) =>
            string.IsNullOrEmpty(value)
                ? null
                : new FpPath(
#if NET6_0_OR_GREATER
                    value,
#else
                    value!,
#endif
                    GetFromString(Path.GetDirectoryName(value)));

        /// <summary>
        /// Gets a path from a string.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Path.</returns>
        public static implicit operator FpPath?(string? value) => GetFromString(value);

        /// <summary>
        /// Concatenates path elements.
        /// </summary>
        /// <param name="left">Parent element.</param>
        /// <param name="right">Child element.</param>
        /// <returns>Concatenated path.</returns>
        public static FpPath operator /(FpPath left, string right) => left.Add(right);

        /// <summary>
        /// Gets a string representation of this path.
        /// </summary>
        /// <returns>Converted path.</returns>
        public override string ToString() => AsCombined();
    }
}
