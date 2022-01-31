using Fp;

public partial class PhantomBreakerProcessor
{
    public IEnumerable<Data> RunYunomi(IReadOnlyDictionary<(int i, int j), BufferData<byte>> dict)
    {
        if (!Flags.Contains("t") && Name is not ("21560002" or "Graphic")) yield break;
        // Try to process all files as yunomi
        foreach (var kvp in dict)
            if (YunomiConvert(NamePathNoExt / "yunomi" / $"{kvp.Key.i:D4}_{kvp.Key.j:D4}", kvp.Value.Buffer.Span) is { } data)
                yield return (data);
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
