using System.Collections;

namespace Fp
{
    /// <summary>
    /// Bitwise operations.
    /// </summary>
    public static class BitUtil
    {
        /// <summary>
        /// Aligns value down.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="align">Alignment.</param>
        /// <returns>Aligned value.</returns>
        public static int AlignDown(this int value, int align) => align == 0 ? value : value / align * align;

        /// <summary>
        /// Aligns value up.
        /// </summary>
        /// <param name="value">Input value.</param>
        /// <param name="align">Alignment.</param>
        /// <returns>Aligned value.</returns>
        public static int AlignUp(this int value, int align) =>
            align == 0 ? value : (value + align - 1) / align * align;

        /// <summary>
        /// Gets bytes for input bits.
        /// </summary>
        /// <param name="bits">Input bits.</param>
        /// <returns>Bytes required to store bits.</returns>
        public static int GetBytesForBits(this int bits) => (bits + 7) / 8;

        /// <summary>
        /// Skips over bits matching specified value.
        /// </summary>
        /// <param name="array">Bit array to use.</param>
        /// <param name="i">Index to modify.</param>
        /// <param name="skipValue">Value to skip over.</param>
        public static void SkipBits(this BitArray array, ref int i, bool skipValue)
        {
            while (i < array.Length && array[i] == skipValue) i++;
        }

        /// <summary>
        /// Skip over bits matching specified value.
        /// </summary>
        /// <param name="array">Bit array to use.</param>
        /// <param name="i">Index to modify.</param>
        /// <param name="skipValue">Value to skip over.</param>
        /// <param name="maxExc">Maximum index (exclusive).</param>
        public static void ConstrainedSkipBits(this BitArray array, ref int i, bool skipValue, int maxExc)
        {
            while (i + 1 < maxExc && array[i] == skipValue) i++;
        }
    }
}
