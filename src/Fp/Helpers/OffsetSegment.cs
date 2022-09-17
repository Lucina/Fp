namespace Fp.Helpers;

/// <summary>
/// Represents a segment in a buffer or stream.
/// </summary>
/// <param name="Start">Start position.</param>
/// <param name="End">End position (the first position beyond the bounds of this segment).</param>
/// <typeparam name="T">Type to use for index, usually a primitive integer type.</typeparam>
public readonly record struct OffsetSegment<T>(T Start, T End)
#if NET7_0_OR_GREATER
    where T : System.Numerics.INumber<T>
#endif
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// Length of segment.
    /// </summary>
    public T Length => End - Start;
#endif
}
