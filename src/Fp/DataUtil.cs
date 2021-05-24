using System;

namespace Fp
{
    /// <summary>
    /// Utility functions for format conversion
    /// </summary>
    public static class DataUtil
    {
        /// <summary>
        /// Clone buffer to newly allocated array
        /// </summary>
        /// <param name="memory">Memory to clone</param>
        /// <returns>New array</returns>
        public static T[] CloneBuffer<T>(this ReadOnlySpan<T> memory)
        {
            T[] target = new T[memory.Length];
            memory.CopyTo(target);
            return target;
        }

        /// <summary>
        /// Copy <see cref="ArraySegment{T}"/> to new compact array
        /// </summary>
        /// <param name="memory">Data to copy</param>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Newly allocated array</returns>
        public static T[] CloneBuffer<T>(this ArraySegment<T> memory) => ((ReadOnlySpan<T>)memory).CloneBuffer();

        /// <summary>
        /// Clone buffer to newly allocated array
        /// </summary>
        /// <param name="memory">Memory to clone</param>
        /// <returns>New array</returns>
        public static T[] CloneBuffer<T>(this ReadOnlyMemory<T> memory) => memory.Span.CloneBuffer();

        /// <summary>
        /// Clone buffer to newly allocated array
        /// </summary>
        /// <param name="memory">Memory to clone</param>
        /// <returns>New array</returns>
        public static T[] CloneBuffer<T>(this Memory<T> memory) => ((ReadOnlySpan<T>)memory.Span).CloneBuffer();

        /// <summary>
        /// Clone buffer to newly allocated array
        /// </summary>
        /// <param name="memory">Memory to clone</param>
        /// <returns>New array</returns>
        public static T[] CloneBuffer<T>(this Span<T> memory) => ((ReadOnlySpan<T>)memory).CloneBuffer();

        /// <summary>
        /// Clone buffer to newly allocated array
        /// </summary>
        /// <param name="memory">Memory to clone</param>
        /// <returns>New array</returns>
        public static T[] CloneBuffer<T>(this T[] memory) => ((ReadOnlySpan<T>)memory).CloneBuffer();
    }
}
