namespace Fp.Helpers;

/// <summary>
/// Represents converted string.
/// </summary>
/// <param name="String">String value.</param>
/// <param name="ByteLength">Byte length.</param>
public readonly record struct StringData(string String, int ByteLength)
{
    /// <summary>
    /// Converts string to <see cref="StringData"/> with 0 byte length.
    /// </summary>
    /// <param name="str">String value.</param>
    /// <returns><see cref="StringData"/><see cref="StringData"/> instance.</returns>
    public static implicit operator StringData(string str) => new(str, 0);
}
