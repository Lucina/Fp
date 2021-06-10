using Fp;
using System;
using System.Collections;

public static class PendingUtil
{
    public static int AlignDown(this int value, int align) => align == 0 ? value : value / align * align;
    public static int AlignUp(this int value, int align) => align == 0 ? value : (value + align - 1) / align * align;
    public static int Up1To8(this int value) => (value + 7) / 8;

    public static sbyte ImageRes(this sbyte x, sbyte max)
    {
        if ((byte)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    public static byte ImageRes(this byte x, byte max)
    {
        if ((byte)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    public static short ImageRes(this short x, short max)
    {
        if ((ushort)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    public static ushort ImageRes(this ushort x, ushort max)
    {
        if ((ushort)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    public static int ImageRes(this int x, int max)
    {
        if ((uint)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    public static uint ImageRes(this uint x, uint max)
    {
        if ((uint)(x - 1) >= max) throw new ArgumentException();
        return x;
    }

    public static byte ReadU8(this ReadContext<byte> context) => context.ReadAdvance();
    public static ushort ReadU16L(this ReadContext<byte> context) => Processor.GetU16(context.ReadAdvance(2), true);
    public static bool CanRead16(this ReadContext<byte> context) => context.IsAvailable(2);
    public static ReadOnlySpan<byte> Read32(this ReadContext<byte> context) => context.ReadAdvance(4);
    public static bool CanRead32(this ReadContext<byte> context) => context.IsAvailable(4);

    public static WriteContext<uint> CreateImage(this FpPath path, int width, int height, out Data image)
    {
        uint[] a = new uint[width * height];
        image = path.Image(width, height, a);
        return new WriteContext<uint>(a);
    }

    public static void ConstrainedSkipBits(this BitArray array, ref int i, bool skipValue, int maxExc)
    {
        while (i + 1 < maxExc && array[i] == skipValue) i++;
    }
}