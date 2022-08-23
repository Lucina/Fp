using System;

namespace Fp;

/// <summary>
/// Utility functions for format conversion.
/// </summary>
public static class DataUtil
{
    /// <summary>
    /// Copies buffer to new compact array.
    /// </summary>
    /// <param name="memory">Data to copy.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Newly allocated array with copied data.</returns>
    public static T[] CloneBuffer<T>(this ReadOnlySpan<T> memory)
    {
        T[] target = new T[memory.Length];
        memory.CopyTo(target);
        return target;
    }

    /// <summary>
    /// Copies <see cref="ArraySegment{T}"/> to new compact array.
    /// </summary>
    /// <param name="memory">Data to copy.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Newly allocated array with copied data.</returns>
    public static T[] CloneBuffer<T>(this ArraySegment<T> memory) => ((ReadOnlySpan<T>)memory).CloneBuffer();

    /// <summary>
    /// Copies buffer to new compact array.
    /// </summary>
    /// <param name="memory">Data to copy.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Newly allocated array with copied data.</returns>
    public static T[] CloneBuffer<T>(this ReadOnlyMemory<T> memory) => memory.Span.CloneBuffer();

    /// <summary>
    /// Copies buffer to new compact array.
    /// </summary>
    /// <param name="memory">Data to copy.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Newly allocated array with copied data.</returns>
    public static T[] CloneBuffer<T>(this Memory<T> memory) => ((ReadOnlySpan<T>)memory.Span).CloneBuffer();

    /// <summary>
    /// Copies buffer to new compact array.
    /// </summary>
    /// <param name="memory">Data to copy.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Newly allocated array with copied data.</returns>
    public static T[] CloneBuffer<T>(this Span<T> memory) => ((ReadOnlySpan<T>)memory).CloneBuffer();

    /// <summary>
    /// Copies buffer to new compact array.
    /// </summary>
    /// <param name="memory">Data to copy.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Newly allocated array with copied data.</returns>
    public static T[] CloneBuffer<T>(this T[] memory) => ((ReadOnlySpan<T>)memory).CloneBuffer();
}
