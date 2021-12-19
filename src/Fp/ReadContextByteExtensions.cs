using System;

namespace Fp;

/// <summary>
/// Provides extensions for <see cref="ReadContext{T}"/> of byte.
/// </summary>
public static class ReadContextByteExtensions
{
    #region 8

    /// <summary>
    /// Checks if 8-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead8(this ref ReadContext<byte> context) => context.IsAvailable(1);

    /// <summary>
    /// Reads 8-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read8(this ref ReadContext<byte> context) => context.ReadAdvance(1);

    /// <summary>
    /// Reads 8-bit unsigned value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static byte ReadU8(this ref ReadContext<byte> context) => context.ReadAdvance();

    /// <summary>
    /// Reads 8-bit signed value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static sbyte ReadS8(this ref ReadContext<byte> context) => (sbyte)context.ReadAdvance();

    #endregion

    #region 16

    /// <summary>
    /// Checks if 16-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead16(this ref ReadContext<byte> context) => context.IsAvailable(2);

    /// <summary>
    /// Reads 16-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read16(this ref ReadContext<byte> context) => context.ReadAdvance(2);

    /// <summary>
    /// Reads 16-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ushort ReadU16L(this ref ReadContext<byte> context) =>
        Processor.GetU16(context.ReadAdvance(2), true);

    /// <summary>
    /// Reads 16-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static short ReadS16L(this ref ReadContext<byte> context) =>
        Processor.GetS16(context.ReadAdvance(2), true);

    /// <summary>
    /// Reads 16-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ushort ReadU16B(this ref ReadContext<byte> context) =>
        Processor.GetU16(context.ReadAdvance(2), false);

    /// <summary>
    /// Reads 16-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static short ReadS16B(this ref ReadContext<byte> context) =>
        Processor.GetS16(context.ReadAdvance(2), false);

    #endregion

    #region 32

    /// <summary>
    /// Checks if 32-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead32(this ref ReadContext<byte> context) => context.IsAvailable(4);

    /// <summary>
    /// Reads 32-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read32(this ref ReadContext<byte> context) => context.ReadAdvance(4);

    /// <summary>
    /// Reads 32-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static uint ReadU32L(this ref ReadContext<byte> context) =>
        Processor.GetU32(context.ReadAdvance(4), true);

    /// <summary>
    /// Reads 32-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static int ReadS32L(this ref ReadContext<byte> context) =>
        Processor.GetS32(context.ReadAdvance(4), true);

    /// <summary>
    /// Reads 32-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static uint ReadU32B(this ref ReadContext<byte> context) =>
        Processor.GetU32(context.ReadAdvance(4), false);

    /// <summary>
    /// Reads 32-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static int ReadS32B(this ref ReadContext<byte> context) =>
        Processor.GetS32(context.ReadAdvance(4), false);

    #endregion

    #region 64

    /// <summary>
    /// Checks if 64-bit value is available.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>True if available.</returns>
    public static bool CanRead64(this ref ReadContext<byte> context) => context.IsAvailable(4);

    /// <summary>
    /// Reads 64-bit value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ReadOnlySpan<byte> Read64(this ref ReadContext<byte> context) => context.ReadAdvance(8);

    /// <summary>
    /// Reads 64-bit unsigned little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ulong ReadU64L(this ref ReadContext<byte> context) =>
        Processor.GetU64(context.ReadAdvance(8), true);

    /// <summary>
    /// Reads 64-bit signed little-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static long ReadS64L(this ref ReadContext<byte> context) =>
        Processor.GetS64(context.ReadAdvance(8), true);

    /// <summary>
    /// Reads 64-bit unsigned big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static ulong ReadU64B(this ref ReadContext<byte> context) =>
        Processor.GetU64(context.ReadAdvance(8), false);

    /// <summary>
    /// Reads 64-bit signed big-endian value.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>Read value.</returns>
    public static long ReadS64B(this ref ReadContext<byte> context) =>
        Processor.GetS64(context.ReadAdvance(8), false);

    #endregion
}