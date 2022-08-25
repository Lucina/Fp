using System;
using System.Runtime.InteropServices;

namespace Fp;

public partial class Processor
{
    /// <summary>
    /// Writes array.
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    public static void SetS8Array(Span<byte> span, ReadOnlySpan<sbyte> array)
    {
        MemoryMarshal.Cast<sbyte, byte>(array).CopyTo(span);
    }

    /// <summary>
    /// Writes array.
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    public static void SetU8Array(Span<byte> span, ReadOnlySpan<byte> array)
    {
        array.CopyTo(span);
    }

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <returns>Converted array.</returns>
    public static short[] GetS16Array(ReadOnlySpan<byte> span, bool littleEndian)
    {
        short[] result = MemoryMarshal.Cast<byte, short>(span).ToArray();
        ConvertS16Array(result, littleEndian);
        return result;
    }

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void SetS16Array(Span<byte> span, ReadOnlySpan<short> array, bool littleEndian)
    {
        MemoryMarshal.Cast<short, byte>(array).CopyTo(span);
        ConvertS16Array(span, littleEndian);
    }

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <returns>Converted array.</returns>
    public static int[] GetS32Array(ReadOnlySpan<byte> span, bool littleEndian)
    {
        int[] result = MemoryMarshal.Cast<byte, int>(span).ToArray();
        ConvertS32Array(result, littleEndian);
        return result;
    }

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void SetS32Array(Span<byte> span, ReadOnlySpan<int> array, bool littleEndian)
    {
        MemoryMarshal.Cast<int, byte>(array).CopyTo(span);
        ConvertS32Array(span, littleEndian);
    }

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <returns>Converted array.</returns>
    public static long[] GetS64Array(ReadOnlySpan<byte> span, bool littleEndian)
    {
        long[] result = MemoryMarshal.Cast<byte, long>(span).ToArray();
        ConvertS64Array(result, littleEndian);
        return result;
    }

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void SetS64Array(Span<byte> span, ReadOnlySpan<long> array, bool littleEndian)
    {
        MemoryMarshal.Cast<long, byte>(array).CopyTo(span);
        ConvertS64Array(span, littleEndian);
    }

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <returns>Converted array.</returns>
    public static ushort[] GetU16Array(ReadOnlySpan<byte> span, bool littleEndian)
    {
        ushort[] result = MemoryMarshal.Cast<byte, ushort>(span).ToArray();
        ConvertU16Array(result, littleEndian);
        return result;
    }

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void SetU16Array(Span<byte> span, ReadOnlySpan<ushort> array, bool littleEndian)
    {
        MemoryMarshal.Cast<ushort, byte>(array).CopyTo(span);
        ConvertS32Array(span, littleEndian);
    }

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <returns>Converted array.</returns>
    public static uint[] GetU32Array(ReadOnlySpan<byte> span, bool littleEndian)
    {
        uint[] result = MemoryMarshal.Cast<byte, uint>(span).ToArray();
        ConvertU32Array(result, littleEndian);
        return result;
    }

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void SetU32Array(Span<byte> span, ReadOnlySpan<uint> array, bool littleEndian)
    {
        MemoryMarshal.Cast<uint, byte>(array).CopyTo(span);
        ConvertS32Array(span, littleEndian);
    }

    /// <summary>
    /// Reads converted array (with endianness switch).
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    /// <returns>Converted array.</returns>
    public static ulong[] GetU64Array(ReadOnlySpan<byte> span, bool littleEndian)
    {
        ulong[] result = MemoryMarshal.Cast<byte, ulong>(span).ToArray();
        ConvertU64Array(result, littleEndian);
        return result;
    }

    /// <summary>
    /// Writes array (with endianness switch).
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    /// <param name="littleEndian">If true, use little-endian encoding.</param>
    public static void SetU64Array(Span<byte> span, ReadOnlySpan<ulong> array, bool littleEndian)
    {
        MemoryMarshal.Cast<ulong, byte>(array).CopyTo(span);
        ConvertS32Array(span, littleEndian);
    }

    /// <summary>
    /// Gets generic array.
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Read data.</returns>
    public static Span<T> GetTArray<T>(Span<byte> span) where T : unmanaged
    {
        return MemoryMarshal.Cast<byte, T>(span);
    }

    /// <summary>
    /// Gets generic array.
    /// </summary>
    /// <param name="span">Source span.</param>
    /// <typeparam name="T">Element type.</typeparam>
    /// <returns>Read data.</returns>
    public static ReadOnlySpan<T> GetTArray<T>(ReadOnlySpan<byte> span) where T : unmanaged
    {
        return MemoryMarshal.Cast<byte, T>(span);
    }

    /// <summary>
    /// Sets generic array.
    /// </summary>
    /// <param name="span">Target span.</param>
    /// <param name="array">Source array.</param>
    /// <typeparam name="T">Element type.</typeparam>
    public static void SetTArray<T>(Span<byte> span, ReadOnlySpan<T> array) where T : unmanaged
    {
        MemoryMarshal.Cast<T, byte>(array).CopyTo(span);
    }
}
