using System;

namespace Fp;

/// <summary>
/// Provides extensions for <see cref="ReadContext{T}"/> and <see cref="WriteContext{T}"/> of byte.
/// </summary>
public static class ReadWriteContextByteExtensions
{
    #region 8

    /// <summary>
    /// Checks if space for 8-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead8(this ref ReadContext<byte> context) => context.IsAvailable(1);

    /// <summary>
    /// Checks if space for 8-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead8(this ref WriteContext<byte> context) => context.IsAvailable(1);

    /// <summary>
    /// Checks if space for 8-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanWrite8(this ref WriteContext<byte> context) => context.IsAvailable(1);

    /// <summary>
    /// Reads 8-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read8(this ref ReadContext<byte> context) => context.ReadAdvance(1);

    /// <summary>
    /// Reads 8-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read8(this ref WriteContext<byte> context) => context.ReadAdvance(1);

    /// <summary>
    /// Reads 8-bit unsigned value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static byte ReadU8(this ref ReadContext<byte> context) => context.ReadAdvance();

    /// <summary>
    /// Reads 8-bit unsigned value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static byte ReadU8(this ref WriteContext<byte> context) => context.ReadAdvance();

    /// <summary>
    /// Writes 8-bit unsigned value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteU8(this ref WriteContext<byte> context, byte value) => context.WriteAdvance(value);

    /// <summary>
    /// Reads 8-bit signed value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static sbyte ReadS8(this ref ReadContext<byte> context) => (sbyte)context.ReadAdvance();

    /// <summary>
    /// Reads 8-bit signed value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static sbyte ReadS8(this ref WriteContext<byte> context) => (sbyte)context.ReadAdvance();

    /// <summary>
    /// Writes 8-bit signed value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteS8(this ref WriteContext<byte> context, sbyte value) => context.WriteAdvance((byte)value);

    #endregion

    #region 16

    /// <summary>
    /// Checks if space for 16-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead16(this ref ReadContext<byte> context) => context.IsAvailable(2);

    /// <summary>
    /// Checks if space for 16-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead16(this ref WriteContext<byte> context) => context.IsAvailable(2);

    /// <summary>
    /// Checks if space for 16-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanWrite16(this ref WriteContext<byte> context) => context.IsAvailable(2);

    /// <summary>
    /// Reads 16-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read16(this ref ReadContext<byte> context) => context.ReadAdvance(2);

    /// <summary>
    /// Reads 16-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read16(this ref WriteContext<byte> context) => context.ReadAdvance(2);

    /// <summary>
    /// Reads 16-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ushort ReadU16L(this ref ReadContext<byte> context) => Processor.GetU16(context.ReadAdvance(2), true);

    /// <summary>
    /// Reads 16-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ushort ReadU16L(this ref WriteContext<byte> context) => Processor.GetU16(context.ReadAdvance(2), true);

    /// <summary>
    /// Writes 16-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteU16L(this ref WriteContext<byte> context, ushort value)
    {
        Span<byte> buf = stackalloc byte[2];
        Processor.SetU16(buf, value, true);
        buf.CopyTo(context.GetAdvance(2));
    }

    /// <summary>
    /// Reads 16-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static short ReadS16L(this ref ReadContext<byte> context) => Processor.GetS16(context.ReadAdvance(2), true);

    /// <summary>
    /// Reads 16-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static short ReadS16L(this ref WriteContext<byte> context) => Processor.GetS16(context.ReadAdvance(2), true);

    /// <summary>
    /// Writes 16-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteS16L(this ref WriteContext<byte> context, short value)
    {
        Span<byte> buf = stackalloc byte[2];
        Processor.SetS16(buf, value, true);
        buf.CopyTo(context.GetAdvance(2));
    }

    /// <summary>
    /// Reads 16-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ushort ReadU16B(this ref ReadContext<byte> context) => Processor.GetU16(context.ReadAdvance(2), false);

    /// <summary>
    /// Reads 16-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ushort ReadU16B(this ref WriteContext<byte> context) => Processor.GetU16(context.ReadAdvance(2), false);

    /// <summary>
    /// Writes 16-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteU16B(this ref WriteContext<byte> context, ushort value)
    {
        Span<byte> buf = stackalloc byte[2];
        Processor.SetU16(buf, value, false);
        buf.CopyTo(context.GetAdvance(2));
    }

    /// <summary>
    /// Reads 16-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static short ReadS16B(this ref ReadContext<byte> context) => Processor.GetS16(context.ReadAdvance(2), false);

    /// <summary>
    /// Reads 16-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static short ReadS16B(this ref WriteContext<byte> context) => Processor.GetS16(context.ReadAdvance(2), false);

    /// <summary>
    /// Writes 16-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteS16B(this ref WriteContext<byte> context, short value)
    {
        Span<byte> buf = stackalloc byte[2];
        Processor.SetS16(buf, value, false);
        buf.CopyTo(context.GetAdvance(2));
    }

    #endregion

    #region 32

    /// <summary>
    /// Checks if space for 32-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead32(this ref ReadContext<byte> context) => context.IsAvailable(4);

    /// <summary>
    /// Checks if space for 32-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead32(this ref WriteContext<byte> context) => context.IsAvailable(4);

    /// <summary>
    /// Checks if space for 32-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanWrite32(this ref WriteContext<byte> context) => context.IsAvailable(4);

    /// <summary>
    /// Reads 32-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read32(this ref ReadContext<byte> context) => context.ReadAdvance(4);

    /// <summary>
    /// Reads 32-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read32(this ref WriteContext<byte> context) => context.ReadAdvance(4);

    /// <summary>
    /// Reads 32-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static uint ReadU32L(this ref ReadContext<byte> context) => Processor.GetU32(context.ReadAdvance(4), true);

    /// <summary>
    /// Reads 32-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static uint ReadU32L(this ref WriteContext<byte> context) => Processor.GetU32(context.ReadAdvance(4), true);

    /// <summary>
    /// Writes 32-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteU32L(this ref WriteContext<byte> context, uint value)
    {
        Span<byte> buf = stackalloc byte[4];
        Processor.SetU32(buf, value, true);
        buf.CopyTo(context.GetAdvance(4));
    }

    /// <summary>
    /// Reads 32-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static int ReadS32L(this ref ReadContext<byte> context) => Processor.GetS32(context.ReadAdvance(4), true);

    /// <summary>
    /// Reads 32-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static int ReadS32L(this ref WriteContext<byte> context) => Processor.GetS32(context.ReadAdvance(4), true);

    /// <summary>
    /// Writes 32-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteU32L(this ref WriteContext<byte> context, int value)
    {
        Span<byte> buf = stackalloc byte[4];
        Processor.SetS32(buf, value, true);
        buf.CopyTo(context.GetAdvance(4));
    }

    /// <summary>
    /// Reads 32-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static uint ReadU32B(this ref ReadContext<byte> context) => Processor.GetU32(context.ReadAdvance(4), false);

    /// <summary>
    /// Reads 32-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static uint ReadU32B(this ref WriteContext<byte> context) => Processor.GetU32(context.ReadAdvance(4), false);

    /// <summary>
    /// Writes 32-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteU32B(this ref WriteContext<byte> context, uint value)
    {
        Span<byte> buf = stackalloc byte[4];
        Processor.SetU32(buf, value, false);
        buf.CopyTo(context.GetAdvance(4));
    }

    /// <summary>
    /// Reads 32-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static int ReadS32B(this ref ReadContext<byte> context) => Processor.GetS32(context.ReadAdvance(4), false);

    /// <summary>
    /// Reads 32-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static int ReadS32B(this ref WriteContext<byte> context) => Processor.GetS32(context.ReadAdvance(4), false);

    /// <summary>
    /// Writes 32-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteS32B(this ref WriteContext<byte> context, int value)
    {
        Span<byte> buf = stackalloc byte[4];
        Processor.SetS32(buf, value, false);
        buf.CopyTo(context.GetAdvance(4));
    }

    #endregion

    #region 64

    /// <summary>
    /// Checks if space for 64-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead64(this ref ReadContext<byte> context) => context.IsAvailable(8);

    /// <summary>
    /// Checks if space for 64-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead64(this ref WriteContext<byte> context) => context.IsAvailable(8);

    /// <summary>
    /// Checks if space for 64-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanWrite64(this ref WriteContext<byte> context) => context.IsAvailable(8);

    /// <summary>
    /// Reads 64-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read64(this ref ReadContext<byte> context) => context.ReadAdvance(8);

    /// <summary>
    /// Reads 64-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Write64(this ref ReadContext<byte> context) => context.ReadAdvance(8);

    /// <summary>
    /// Reads 64-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ulong ReadU64L(this ref ReadContext<byte> context) => Processor.GetU64(context.ReadAdvance(8), true);

    /// <summary>
    /// Reads 64-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ulong ReadU64L(this ref WriteContext<byte> context) => Processor.GetU64(context.ReadAdvance(8), true);

    /// <summary>
    /// Writes 64-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteU64L(this ref WriteContext<byte> context, ulong value)
    {
        Span<byte> buf = stackalloc byte[8];
        Processor.SetU64(buf, value, true);
        buf.CopyTo(context.GetAdvance(8));
    }

    /// <summary>
    /// Reads 64-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static long ReadS64L(this ref ReadContext<byte> context) => Processor.GetS64(context.ReadAdvance(8), true);

    /// <summary>
    /// Reads 64-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static long ReadS64L(this ref WriteContext<byte> context) => Processor.GetS64(context.ReadAdvance(8), true);

    /// <summary>
    /// Writes 64-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteS64L(this ref WriteContext<byte> context, long value)
    {
        Span<byte> buf = stackalloc byte[8];
        Processor.SetS64(buf, value, true);
        buf.CopyTo(context.GetAdvance(8));
    }

    /// <summary>
    /// Reads 64-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ulong ReadU64B(this ref ReadContext<byte> context) => Processor.GetU64(context.ReadAdvance(8), false);

    /// <summary>
    /// Reads 64-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ulong ReadU64B(this ref WriteContext<byte> context) => Processor.GetU64(context.ReadAdvance(8), false);

    /// <summary>
    /// Writes 64-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteU64B(this ref WriteContext<byte> context, ulong value)
    {
        Span<byte> buf = stackalloc byte[8];
        Processor.SetU64(buf, value, false);
        buf.CopyTo(context.GetAdvance(8));
    }

    /// <summary>
    /// Reads 64-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static long ReadS64B(this ref ReadContext<byte> context) => Processor.GetS64(context.ReadAdvance(8), false);

    /// <summary>
    /// Reads 64-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static long ReadS64B(this ref WriteContext<byte> context) => Processor.GetS64(context.ReadAdvance(8), false);

    /// <summary>
    /// Writes 64-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="value">Value to write.</param>
    public static void WriteS64B(this ref WriteContext<byte> context, long value)
    {
        Span<byte> buf = stackalloc byte[8];
        Processor.SetS64(buf, value, false);
        buf.CopyTo(context.GetAdvance(8));
    }

    #endregion
}
