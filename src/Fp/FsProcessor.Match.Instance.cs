using System;
using System.Collections.Generic;
using static Fp.FsProcessor;

namespace Fp
{
    // ReSharper disable InconsistentNaming
    public partial class Scripting
    {
        #region Matching

        /// <summary>
        /// Enumerates all occurrences of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="streamMaxOffset">Upper bound (exclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="matchOffset">Offset in target to start matching.</param>
        /// <param name="matchLength">Length of target.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Enumerator for matches.</returns>
        public static IEnumerable<long> match(long streamOffset, long streamMaxOffset, ReadOnlyMemory<byte> match,
            int matchOffset, int matchLength, int bufferLength = 4096)
            => Current.Match(streamOffset, streamMaxOffset, match, matchOffset, matchLength, bufferLength);

        /// <summary>
        /// Enumerates all occurrences of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="matchOffset">Offset in target to start matching.</param>
        /// <param name="matchLength">Length of target.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Enumerator for matches.</returns>
        public static IEnumerable<long> match(long streamOffset, ReadOnlyMemory<byte> match, int matchOffset,
            int matchLength,
            int bufferLength = 4096)
            => Current.Match(streamOffset, match, matchOffset, matchLength, bufferLength);

        /// <summary>
        /// Enumerates all occurrences of a pattern.
        /// </summary>
        /// <param name="match">Target to match.</param>
        /// <param name="matchOffset">Offset in target to start matching.</param>
        /// <param name="matchLength">Length of target.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Enumerator for matches.</returns>
        public static IEnumerable<long> match(ReadOnlyMemory<byte> match, int matchOffset, int matchLength,
            int bufferLength = 4096)
            => Current.Match(match, matchOffset, matchLength, bufferLength);

        /// <summary>
        /// Enumerates all occurrences of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="streamMaxOffset">Upper bound (exclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Enumerator for matches.</returns>
        public static IEnumerable<long> match(long streamOffset, long streamMaxOffset, byte[] match,
            int bufferLength = 4096)
            => Current.Match(streamOffset, streamMaxOffset, match, bufferLength);

        /// <summary>
        /// Enumerates all occurrences of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Enumerator for matches.</returns>
        public static IEnumerable<long> match(long streamOffset, ReadOnlyMemory<byte> match, int bufferLength = 4096)
            => Current.Match(streamOffset, match, bufferLength);

        /// <summary>
        /// Enumerates all occurrences of a pattern.
        /// </summary>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Enumerator for matches.</returns>
        public static IEnumerable<long> match(ReadOnlyMemory<byte> match, int bufferLength = 4096)
            => Current.Match(match, bufferLength);

        /// <summary>
        /// Enumerates all occurrences of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="streamMaxOffset">Upper bound (exclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Enumerator for matches.</returns>
        public static IEnumerable<long> match(long streamOffset, long streamMaxOffset, string match,
            int bufferLength = 4096)
            => Current.Match(streamOffset, streamMaxOffset, match, bufferLength);

        /// <summary>
        /// Enumerates all occurrences of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Enumerator for matches.</returns>
        public static IEnumerable<long> match(long streamOffset, string match, int bufferLength = 4096)
            => Current.Match(streamOffset, match, bufferLength);

        /// <summary>
        /// Enumerates all occurrences of a pattern.
        /// </summary>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Enumerator for matches.</returns>
        public static IEnumerable<long> match(string match, int bufferLength = 4096)
            => Current.Match(match, bufferLength);

        /// <summary>
        /// Finds first occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="streamMaxOffset">Upper bound (exclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="matchOffset">Offset in target to start matching.</param>
        /// <param name="matchLength">Length of target.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of first match or -1 if no match found.</returns>
        public static long matchFirst(long streamOffset, long streamMaxOffset, ReadOnlyMemory<byte> match,
            int matchOffset,
            int matchLength, int bufferLength = 4096)
            => Current.MatchFirst(streamOffset, streamMaxOffset, match, matchOffset, matchLength, bufferLength);

        /// <summary>
        /// Finds first occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="matchOffset">Offset in target to start matching.</param>
        /// <param name="matchLength">Length of target.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of first match or -1 if no match found.</returns>
        public static long matchFirst(long streamOffset, ReadOnlyMemory<byte> match, int matchOffset, int matchLength,
            int bufferLength = 4096)
            => Current.MatchFirst(streamOffset, match, matchOffset, matchLength, bufferLength);

        /// <summary>
        /// Finds first occurrence of a pattern.
        /// </summary>
        /// <param name="match">Target to match.</param>
        /// <param name="matchOffset">Offset in target to start matching.</param>
        /// <param name="matchLength">Length of target.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of first match or -1 if no match found.</returns>
        public static long matchFirst(ReadOnlyMemory<byte> match, int matchOffset, int matchLength,
            int bufferLength = 4096)
            => Current.MatchFirst(match, matchOffset, matchLength, bufferLength);

        /// <summary>
        /// Finds first occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="streamMaxOffset">Upper bound (exclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of first match or -1 if no match found.</returns>
        public static long matchFirst(long streamOffset, long streamMaxOffset, ReadOnlyMemory<byte> match,
            int bufferLength = 4096)
            => Current.MatchFirst(streamOffset, streamMaxOffset, match, bufferLength);

        /// <summary>
        /// Finds first occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of first match or -1 if no match found.</returns>
        public static long matchFirst(long streamOffset, ReadOnlyMemory<byte> match, int bufferLength = 4096)
            => Current.MatchFirst(streamOffset, match, bufferLength);

        /// <summary>
        /// Finds first occurrence of a pattern.
        /// </summary>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of first match or -1 if no match found.</returns>
        public static long matchFirst(ReadOnlyMemory<byte> match, int bufferLength = 4096)
            => Current.MatchFirst(match, bufferLength);

        /// <summary>
        /// Finds first occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="streamMaxOffset">Upper bound (exclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of first match or -1 if no match found.</returns>
        public static long matchFirst(long streamOffset, long streamMaxOffset, string match,
            int bufferLength = 4096)
            => Current.MatchFirst(streamOffset, streamMaxOffset, match, bufferLength);

        /// <summary>
        /// Finds first occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of first match or -1 if no match found.</returns>
        public static long matchFirst(long streamOffset, string match, int bufferLength = 4096)
            => Current.MatchFirst(streamOffset, match, bufferLength);

        /// <summary>
        /// Finds first occurrence of a pattern.
        /// </summary>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of first match or -1 if no match found.</returns>
        public static long matchFirst(string match, int bufferLength = 4096)
            => Current.MatchFirst(match, bufferLength);

        /// <summary>
        /// Finds last occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="streamMaxOffset">Upper bound (exclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="matchOffset">Offset in target to start matching.</param>
        /// <param name="matchLength">Length of target.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of last match or -1 if no match found.</returns>
        public static long matchLast(long streamOffset, long streamMaxOffset, ReadOnlyMemory<byte> match,
            int matchOffset,
            int matchLength, int bufferLength = 4096)
            => Current.MatchLast(streamOffset, streamMaxOffset, match, matchOffset, matchLength, bufferLength);

        /// <summary>
        /// Finds last occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="matchOffset">Offset in target to start matching.</param>
        /// <param name="matchLength">Length of target.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of last match or -1 if no match found.</returns>
        public static long matchLast(long streamOffset, ReadOnlyMemory<byte> match, int matchOffset, int matchLength,
            int bufferLength = 4096)
            => Current.MatchLast(streamOffset, match, matchOffset, matchLength, bufferLength);

        /// <summary>
        /// Finds last occurrence of a pattern.
        /// </summary>
        /// <param name="match">Target to match.</param>
        /// <param name="matchOffset">Offset in target to start matching.</param>
        /// <param name="matchLength">Length of target.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of last match or -1 if no match found.</returns>
        public static long matchLast(ReadOnlyMemory<byte> match, int matchOffset, int matchLength,
            int bufferLength = 4096)
            => Current.MatchLast(match, matchOffset, matchLength, bufferLength);

        /// <summary>
        /// Finds last occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="streamMaxOffset">Upper bound (exclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of last match or -1 if no match found.</returns>
        public static long matchLast(long streamOffset, long streamMaxOffset, ReadOnlyMemory<byte> match,
            int bufferLength = 4096)
            => Current.MatchLast(streamOffset, streamMaxOffset, match, bufferLength);

        /// <summary>
        /// Finds last occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of last match or -1 if no match found.</returns>
        public static long matchLast(long streamOffset, ReadOnlyMemory<byte> match, int bufferLength = 4096)
            => Current.MatchLast(streamOffset, match, bufferLength);

        /// <summary>
        /// Finds last occurrence of a pattern.
        /// </summary>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of last match or -1 if no match found.</returns>
        public static long matchLast(ReadOnlyMemory<byte> match, int bufferLength = 4096)
            => Current.MatchLast(match, bufferLength);

        /// <summary>
        /// Finds last occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="streamMaxOffset">Upper bound (exclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of last match or -1 if no match found.</returns>
        public static long matchLast(long streamOffset, long streamMaxOffset, string match,
            int bufferLength = 4096)
            => Current.MatchLast(streamOffset, streamMaxOffset, match, bufferLength);

        /// <summary>
        /// Finds last occurrence of a pattern.
        /// </summary>
        /// <param name="streamOffset">Lower bound (inclusive) of stream positions to search.</param>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of last match or -1 if no match found.</returns>
        public static long matchLast(long streamOffset, string match, int bufferLength = 4096)
            => Current.MatchLast(streamOffset, match, bufferLength);

        /// <summary>
        /// Finds last occurrence of a pattern.
        /// </summary>
        /// <param name="match">Target to match.</param>
        /// <param name="bufferLength">Minimum buffer length.</param>
        /// <returns>Position of last match or -1 if no match found.</returns>
        public static long matchLast(string match, int bufferLength = 4096)
            => Current.MatchLast(match, bufferLength);

        #endregion
    }
    // ReSharper restore InconsistentNaming
}
