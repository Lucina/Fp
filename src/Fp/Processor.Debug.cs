using System;
using System.Collections.Generic;
using System.Linq;
using static Fp.FsProcessor;

namespace Fp
{
    public partial class Processor
    {
        #region Debugging utilities

        /// <summary>
        /// If true, enables debugging features.
        /// </summary>
        public bool Debug;

        /// <summary>
        /// If true, disable outputs.
        /// </summary>
        public bool Nop;

        /// <summary>
        /// Annotations for memory.
        /// </summary>
        public readonly Dictionary<ReadOnlyMemory<byte>,
                SortedList<int, (int offset, int length, string? label, ConsoleColor color)>>
            MemAnnotations = new();

        private int _memColorIdx;

        /// <summary>
        /// Clears stored memories and annotations.
        /// </summary>
        /// <remarks>No-op if <see cref="Debug"/> is false.</remarks>
        public void MemClear()
        {
            if (!Debug) return;
            MemAnnotations.Clear();
        }

        /// <summary>
        /// Labels memory with annotation.
        /// </summary>
        /// <param name="memory">Target memory.</param>
        /// <param name="offset">Data offset.</param>
        /// <param name="length">Data length.</param>
        /// <param name="label">Annotation to add.</param>
        /// <param name="color">Color, random default.</param>
        /// <remarks>No-op if <see cref="Debug"/> is false.<br/>Users should not slice memory struct between label and print, uses <see cref="MemAnnotations"/> which uses the memory as a key.</remarks>
        public void MemLabel(ReadOnlyMemory<byte> memory, int offset, int length, string? label = null,
            ConsoleColor? color = null)
        {
            if (!Debug) return;
            if (!MemAnnotations.TryGetValue(memory,
                out SortedList<int, (int offset, int length, string? label, ConsoleColor color)>? list))
                list = MemAnnotations[memory] =
                    new SortedList<int, (int offset, int length, string? label, ConsoleColor color)>();
            if (color == null)
            {
                color = ConsoleLog.Colors[_memColorIdx];
                _memColorIdx = (_memColorIdx + 1) % (ConsoleLog.Colors.Count - 1);
            }

            if (!list.ContainsKey(offset))
                list.Add(offset, (offset, length, label, color.Value));
        }

        /// <summary>
        /// Labels occurrences of sequence in memory with annotation.
        /// </summary>
        /// <param name="memory">Target memory.</param>
        /// <param name="sequence">Sequence to search for.</param>
        /// <param name="label">Annotation to add.</param>
        /// <param name="color">Color, random default.</param>
        /// <remarks>No-op if <see cref="Debug"/> is false.<br/>Users should not slice memory struct between label and print, uses <see cref="MemAnnotations"/> which uses the memory as a key.</remarks>
        /// <returns>Matches (regardless of debug enabled/not).</returns>
        public List<int> MemLabel(ReadOnlyMemory<byte> memory, ReadOnlySpan<byte> sequence, string? label = null,
            ConsoleColor? color = null)
        {
            List<int> matches = Match(memory.Span, 0, memory.Length, sequence);
            if (!Debug) return matches;
            int sl = sequence.Length;
            for (int i = 0; i + 1 < matches.Count; i++)
            {
                int count = 0;
                int offset = matches[i];
                while (i + 1 < matches.Count && offset + ++count * sl == matches[i + 1])
                    matches.RemoveAt(i + 1);
                MemLabel(memory, offset, count * sl, label, color);
            }

            return matches;
        }

        /// <summary>
        /// Prints memory with associated annotations.
        /// </summary>
        /// <param name="memory">Target memory.</param>
        /// <param name="space">Space between bytes.</param>
        /// <param name="pow2Modulus">Only display power of 2 per line.</param>
        /// <remarks>No-op if <see cref="Debug"/> is false.<br/>Users should not slice memory struct between label and print, uses <see cref="MemAnnotations"/> which uses the memory as a key.</remarks>
        public void MemPrint(ReadOnlyMemory<byte> memory, bool space = true, bool pow2Modulus = true)
        {
            if (!Debug) return;
            HexPrint.Print(memory.Span, LogReceiver,
                MemAnnotations.TryGetValue(memory,
                    out SortedList<int, (int offset, int length, string? label, ConsoleColor color)>? list)
                    ? list.Values.ToArray()
                    : Array.Empty<(int offset, int length, string? label, ConsoleColor color)>(), space,
                pow2Modulus);
        }

        #endregion
    }

    public partial class Scripting
    {
        /// <summary>
        /// If true, enable debugging features.
        /// </summary>
        public static bool _debug
        {
            get => Current.Debug;
            set => Current.Debug = value;
        }

        /// <summary>
        /// If true, disable outputs.
        /// </summary>
        public static bool _nop
        {
            get => Current.Nop;
            set => Current.Nop = value;
        }

        /// <summary>
        /// Clears stored memories and annotations.
        /// </summary>
        /// <remarks>No-op if <see cref="Processor.Debug"/> is false.</remarks>
        public static void MemClear() => Current.MemClear();

        /// <summary>
        /// Labels memory with annotation.
        /// </summary>
        /// <param name="memory">Target memory.</param>
        /// <param name="offset">Data offset.</param>
        /// <param name="length">Data length.</param>
        /// <param name="label">Annotation to add.</param>
        /// <param name="color">Color, random default.</param>
        /// <remarks>No-op if <see cref="Processor.Debug"/> is false.<br/>Users should not slice memory struct between label and print, uses <see cref="Processor.MemAnnotations"/> which uses the memory as a key.</remarks>
        public static void memLabel(ReadOnlyMemory<byte> memory, int offset, int length, string? label = null,
            ConsoleColor? color = null) => Current.MemLabel(memory, offset, length, label, color);

        /// <summary>
        /// Labels occurrences of sequence in memory with annotation.
        /// </summary>
        /// <param name="memory">Target memory.</param>
        /// <param name="sequence">Sequence to search for.</param>
        /// <param name="label">Annotation to add.</param>
        /// <param name="color">Color, random default.</param>
        /// <remarks>No-op if <see cref="Processor.Debug"/> is false.<br/>Users should not slice memory struct between label and print, uses <see cref="Processor.MemAnnotations"/> which uses the memory as a key.</remarks>
        /// <returns>Matches (regardless of debug enabled/not).</returns>
        public static List<int> memLabel(ReadOnlyMemory<byte> memory, ReadOnlySpan<byte> sequence, string? label = null,
            ConsoleColor? color = null) => Current.MemLabel(memory, sequence, label, color);

        /// <summary>
        /// Prints memory with associated annotations.
        /// </summary>
        /// <param name="memory">Target memory.</param>
        /// <param name="space">Space between bytes.</param>
        /// <param name="pow2Modulus">Only display power of 2 per line.</param>
        /// <remarks>No-op if <see cref="Processor.Debug"/> is false.<br/>Users should not slice memory struct between label and print, uses <see cref="Processor.MemAnnotations"/> which uses the memory as a key.</remarks>
        public static void memPrint(ReadOnlyMemory<byte> memory, bool space = true, bool pow2Modulus = true) =>
            Current.MemPrint(memory, space, pow2Modulus);
    }
}
