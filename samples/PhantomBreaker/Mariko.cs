using Fp;
using System;
using System.Collections.Generic;

public static class Mariko
{
    public static bool Filter(string name) =>
            // PB:BG
            PbbgChara.Contains(name) ||
            // PB:E
            name.StartsWith("c_");

    public static readonly HashSet<string> PbbgChara =
        new(new[]
        {
            "00418468", "01020304", "01193155", "01226238", "01349975", "01435347", "01742329", "01811477",
            "01866963", "02305020", "02847117", "03003471", "03469024", "03926848", "04118858", "04191176",
            "04375162", "04432230", "04956233", "06408852", "08495654", "09044127", "09099665", "09815310",
            "10172253", "10765374", "10905857", "11686873", "13486364", "13965846", "14637860", "15038632",
            "15055436", "15293542", "16048821", "16055641", "16474100", "18742350", "19605937", "20871645",
            "21930918", "21980123", "22116428", "22688356", "22797361", "23851726", "24081731", "25648659",
            "29251622", "31807380", "32009802", "32304172", "34475525", "35007635", "35060143", "35381167",
            "35557166", "39106579", "44778901", "46936134", "47909151", "48665449", "54810019", "56431189",
            "59989544", "64678139", "73075369", "77046140", "77826178", "83521470"
        });

    /// Process character graphics set
    public static void HandleMariko(FpPath folder, Dictionary<(int i, int j), Memory<byte>> files, ISet<Data> set)
    {
        if (!files.TryGetValue((0, 0), out var a)) return;

        // Main graphics file
        for (int i = 0, hOffset = 0x800; i < 0x1000 / 4; i++, hOffset += 4)
        {
            var s = a.Span;
            int offset = Processor.GetS32(s, true, hOffset), next = Processor.GetS32(s, true, hOffset + 4);
            for (int p = 0; p < 8; p++)
            {
                if (!files.TryGetValue((3, p), out var palette)) continue;
                string subName = $"mn_{p:D4}_{i:D4}";
                var b = a.Slice(0x1800 + offset, next - offset);
                if (MarikoConvert(folder / "mariko" / subName, b.Span, palette.Span) is { } res)
                    set.Add(res);
                set.Add(Processor.Buffer(folder / "mariko_src" / $"{subName}.dat", b));
            }

            if (next >= a.Length - 0x1800) break;
        }

        // Sub-files
        foreach (var kvp in files)
        {
            (int i, int j) = kvp.Key;
            if (i <= 3 || i == 6) continue;
            for (int p = 0; p < 8; p++)
                if (files.TryGetValue((6, p), out var palette) &&
                    MarikoConvert(folder / "mariko" / $"ex_{p:D4}_{i:D4}_{j:D4}", kvp.Value.Span, palette.Span) is { } res)
                    set.Add(res);
        }
    }

    /// Process character graphics file with palette
    public static Data? MarikoConvert(FpPath path, ReadOnlySpan<byte> buffer, ReadOnlySpan<byte> paletteBuffer)
    {
        try
        {
            var ctx = new ReadContext<byte>(buffer);
            var wCtx = path.CreateImage(ctx.ReadU16L().ImageRes(2048), ctx.ReadU16L().ImageRes(2048), out var res);
            while (ctx.CanRead16())
            {
                int val = ctx.ReadU16L(), num = val & 0xf;
                if (num == 0)
                    wCtx.Advance(val >> 4);
                else
                {
                    int colorIdx = val >> 8, alphaUpper = val & 0xf0;
                    byte alpha = (byte)(alphaUpper | (alphaUpper >> 4));
                    wCtx.WriteAdvance(paletteBuffer.Slice(colorIdx * 4).FromRgb(alpha), num);
                }
            }

            return res;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
