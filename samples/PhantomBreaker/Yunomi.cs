using Fp;
using System;
using System.Collections.Generic;

public static class Yunomi
{
    public static bool Filter(string name) =>
            // PB:BG
            name == "21560002" ||
            // PB:E
            name == "Graphic";

    /// Process run-length graphics file container
    public static void HandleYunomi(FpPath folder, Dictionary<(int i, int j), Memory<byte>> files, ISet<Data> set)
    {
        // Try to process all files as yunomi
        foreach (var kvp in files)
            if (YunomiConvert(folder / "yunomi" / $"{kvp.Key.i:D4}_{kvp.Key.j:D4}", kvp.Value.Span) is { } data)
                set.Add(data);
    }

    /// Process run-length graphics file
    public static Data? YunomiConvert(FpPath path, ReadOnlySpan<byte> buffer)
    {
        try
        {
            var ctx = new ReadContext<byte>(buffer);
            var wCtx = path.CreateImage(ctx.ReadU16L().ImageRes(2048), ctx.ReadU16L().ImageRes(2048), out var res);
            uint prevColor = 0;
            while (ctx.CanRead32())
            {
                uint color = ctx.Read32().FromBgra();
                if (color == prevColor) wCtx.WriteAdvance(color, ctx.ReadU8() + 1);
                else wCtx.WriteAdvance(color);

                prevColor = color;
            }

            return res;
        }
        catch (Exception)
        {
            return null;
        }
    }
}