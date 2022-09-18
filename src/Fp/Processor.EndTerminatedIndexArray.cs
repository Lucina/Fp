using System;
using System.IO;
using Fp.Helpers;

namespace Fp;

public partial class Processor
{
    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="buffer">Source buffer.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <typeparam name="T">Index type.</typeparam>
    /// <returns>Segments.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static OffsetSegment<T>[] GetEndTerminatedIndexArray<T>(ReadOnlySpan<T> buffer, out T end)
#if NET7_0_OR_GREATER
        where T : System.Numerics.INumber<T>
#else
        where T : IComparable<T>
#endif
    {
        if (buffer.IsEmpty) throw new ArgumentException(); // >= 1
        if (buffer.Length == 1)
        {
            end = buffer[0];
            return Array.Empty<OffsetSegment<T>>();
        }
        OffsetSegment<T>[] result = new OffsetSegment<T>[buffer.Length - 1];
        T value = buffer[0];
        for (int i = 0; i < result.Length; i++)
        {
            T next = buffer[i + 1];
            if (value.CompareTo(next) > 0) throw new InvalidDataException($"Expected smaller end index for element {i} compared to starting index {value} (got {next})");
            result[i] = new OffsetSegment<T>(value, next);
            value = next;
        }
        end = value;
        return result;
    }

    /// <summary>
    /// Gets an end-terminated index array from the specified buffer.
    /// </summary>
    /// <param name="buffer">Source buffer.</param>
    /// <param name="transform">Transform to apply to input values to get final indices.</param>
    /// <param name="end">End position (the first position beyond the bounds of the target region the array refers to).</param>
    /// <typeparam name="T">Source index type.</typeparam>
    /// <typeparam name="TElement">Output index type.</typeparam>
    /// <returns>Segments.</returns>
    /// <exception cref="ArgumentException">Thrown for an empty buffer.</exception>
    /// <exception cref="InvalidDataException">Thrown for an invalid index value (each successive element must compare greater than or equal to the previous element).</exception>
    public static OffsetSegment<TElement>[] GetEndTerminatedIndexArray<T, TElement>(ReadOnlySpan<T> buffer, Func<T, TElement> transform, out TElement end)
#if NET7_0_OR_GREATER
        where TElement : System.Numerics.INumber<TElement>
#else
        where TElement : IComparable<TElement>
#endif
    {
        if (buffer.IsEmpty) throw new ArgumentException(); // >= 1
        if (buffer.Length == 1)
        {
            end = transform(buffer[0]);
            return Array.Empty<OffsetSegment<TElement>>();
        }
        OffsetSegment<TElement>[] result = new OffsetSegment<TElement>[buffer.Length - 1];
        TElement value = transform(buffer[0]);
        for (int i = 0; i < result.Length; i++)
        {
            TElement next = transform(buffer[i + 1]);
            if (value.CompareTo(next) > 0) throw new InvalidDataException($"Expected smaller end index for element {i} compared to starting index {value} (got {next})");
            result[i] = new OffsetSegment<TElement>(value, next);
            value = next;
        }
        end = value;
        return result;
    }

    // TODO IEnumerable<T> variants
}
