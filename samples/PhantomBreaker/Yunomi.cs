using Fp;
using System;

public class Yunomi : ProcessorChild<(int i, int j), Memory<byte>>
{
    public override string Flag => "t";
    public override bool Filter => Name is "21560002" or "Graphic";

    public override void Run()
    {
        // Try to process all files as yunomi
        foreach (var kvp in Lookup)
            if (YunomiConvert(NamePathNoExt / "yunomi" / $"{kvp.Key.i:D4}_{kvp.Key.j:D4}", kvp.Value.Span) is
                { } data)
                Content.Add(data);
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
