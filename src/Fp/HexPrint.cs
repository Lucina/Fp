using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Fp
{
    /// <summary>
    /// Prints hex text with color codes for labelled sections
    /// </summary>
    public static class HexPrint
    {
        /// <summary>
        /// Maximum width of offset label
        /// </summary>
        public const int PosWidth = 10;

        /// <summary>
        /// Maximum width of annotation label
        /// </summary>
        public const int TextWidth = 16;

        /// <summary>
        /// Print hex text
        /// </summary>
        /// <param name="data">Data to print</param>
        /// <param name="annotations">Data annotations</param>
        /// <param name="target">Log target</param>
        /// <param name="space">Space between bytes</param>
        /// <param name="pow2Modulus">Only display power of 2 per line</param>
        /// <param name="displayWidth">Available display width</param>
        /// <exception cref="ApplicationException"></exception>
        public static void Print(ReadOnlySpan<byte> data, ILogReceiver target,
            IEnumerable<(int offset, int length, string? label, ConsoleColor color)>? annotations = null,
            bool space = true, bool pow2Modulus = false, int? displayWidth = null)
        {
            var annotationsList =
                new List<(int offset, int length, string? label, ConsoleColor color)>(
                    annotations?.OrderBy(a => a.offset) ??
                    Enumerable.Empty<(int offset, int length, string? label, ConsoleColor color)>());
            int width = displayWidth ?? Console.WindowWidth;
            int availableSpace = width - TextWidth - PosWidth - 2 - 1;
            int charWidth = space ? 3 : 2;
            //if (availableSpace < charWidth) throw new ApplicationException("Console width too small for output");
            availableSpace = Math.Max(availableSpace, charWidth * 4);
            int w = availableSpace / charWidth;
            if (pow2Modulus)
            {
                int mod = 1;
                while (mod <= w) mod <<= 1;
                w = mod >> 1;
            }

            int left = data.Length;
            int cur = 0;
            int annotationOffset = 0;
            Queue<(int offset, int length, ConsoleColor color)> annotationQueue = new();
            Queue<(int offset, int length, string label, ConsoleColor color)> annotationPrintQueue = new();

            while (left > 0)
            {
                int curLine = 0;
                foreach ((int offset, int length, string? label, ConsoleColor color) x in
                    annotationsList.Skip(annotationOffset)
                )
                {
                    (int xOf, int xLe, string? xLa, ConsoleColor xCo) = x;
                    if (xOf >= cur + w) break;
                    annotationQueue.Enqueue((xOf, xLe, xCo));
                    if (xLa != null)
                        annotationPrintQueue.Enqueue((xOf, xLe, xLa, xCo));
                    annotationOffset++;
                }

                target.LogChunk(string.Format(CultureInfo.InvariantCulture, $"0x{{0:X{PosWidth}}} ", cur), false,
                    ConsoleColor.White);
                for (; curLine < w && cur < data.Length; curLine++)
                {
                    bool consumed = false;
                    foreach ((int offset, int length, ConsoleColor color) x in annotationQueue)
                        if (x.offset <= cur && x.offset + x.length > cur)
                        {
                            consumed = true;
                            target.LogChunk($"{data[cur]:X2}", false, x.color);
                            break;
                        }

                    if (!consumed) target.LogChunk($"{data[cur]:X2}", false, ConsoleColor.White);
                    if (space && curLine + 1 != w) target.LogChunk(" ", false);
                    cur++;
                }

                while (annotationQueue.Count > 0)
                {
                    (int offset, int length, _) = annotationQueue.Peek();
                    if (offset + length <= cur) annotationQueue.Dequeue();
                    else break;
                }

                if (curLine != w)
                    target.LogChunk(
                        string.Format(CultureInfo.InvariantCulture, $"{{0,{(w - curLine) * (space ? 3 : 2) - 1}}}",
                            ' '), false);

                if (annotationPrintQueue.Count > 0)
                {
                    (_, _, string label, ConsoleColor color) = annotationPrintQueue.Dequeue();
                    target.LogChunk(" ", false);
                    target.LogChunk(label.Length > TextWidth
                        ? label.Substring(0, TextWidth)
                        : label, false, color);
                }

                target.LogChunk("\n", false);

                left -= curLine;
            }

            while (annotationPrintQueue.Count > 0)
            {
                (_, _, string label, ConsoleColor color) = annotationPrintQueue.Dequeue();
                target.LogChunk(string.Format(CultureInfo.InvariantCulture,
                    $"{{0,{2 + PosWidth + 1 + w * (space ? 3 : 2) + (space ? 0 : 1)}}}",
                    ' '), false, color);
                target.LogChunk(label.Length > TextWidth
                    ? label.Substring(0, TextWidth)
                    : label, false);
                target.LogChunk("\n", false);
            }

            target.LogChunk("", true);
        }
    }
}
