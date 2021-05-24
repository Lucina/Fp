using Fp;
using System;
using System.Collections;
using System.Collections.Generic;

Processor.Run<PhantomBreakerProcessor>(args,
    "PhantomBreaker",
    "Decode / extract assets from Phantom Breaker series",
    (string?)null);

public class PhantomBreakerProcessor : DataProcessor
{
    protected override IEnumerable<Data> ProcessData()
    {
        // Header transform: hatebin.com/pssgnczaqu / pastebin.com/xhwKmqsn
        // Body transform: hatebin.com/iwelojtqqt / pastebin.com/LR1mqn36

        // Setup
        byte[] a = Load();
        var r = new Dictionary<(int i, int j), Memory<byte>>();
        var res = new HashSet<Data>();

        // Decode/load header
        int pos = i4l[a] ^= -1, numT1 = i4l[a, 4] ^= 0x12876623, posT2 = 8 + numT1 * 4;
        for (int i = 8, hSrc = 0xa9; i < pos; i++, hSrc += 6) a[i] ^= (byte)hSrc;

        // Table1: groups
        for (int i = 0; i < numT1; i++)
        {
            int ofsT1 = i4l[a, 8 + i * 4], numT2 = i2l[a, posT2 + ofsT1];
            if (ofsT1 == -1) continue;
            // Per-file existence flag
            var bits = new BitArray(a.SliceAlloc(posT2 + ofsT1 + 2, (numT2 + 7) / 8));
            // Table2: files
            for (int j = 0, p = posT2 + ofsT1 + 2 + (numT2 + 7) / 8; j < numT2; j++, p += 8)
            {
                // Skip to next flag
                while (!bits[j])
                    if (j + 1 == numT2) break;
                    else j++;
                var memory = a.AsMemory(pos + i4l[a, p], i4l[a, p + 4]);
                var sp = memory.Span;
                // Wasted a week eyeballing the transform. Technically correct, just didn't factor in flags :(
                byte s = (byte)(((sp.Length >> 3) & 0x5e) + (i & 0x7a) * 3 + (j & 0x5f) * 7);
                for (int x = 0; x < sp.Length; x++) sp[x] ^= (byte)((0xe6 & x) ^ s);

                // Result
                r[(i, j)] = memory;
                var entry = NamePathNoExt / $"{i:D4}_{j:D4}";
                res.Add(Buffer(entry, memory));
                if (HasMagic(sp, "RIFF")) res.Add(Buffer(entry.ChangeExtension(".wav"), memory));
            }
        }

        res.Add(Buffer($"{Name}_dec", a));

        var (flags, _, _) = Args.IsolateFlags();
        if (!flags.Contains("g"))
        {
            if (flags.Contains("c") ||
                // PB:BG
                _pbbgChara.Contains(Name) ||
                // PB:E
                Name.StartsWith("c_"))
                HandleMariko(NamePath, r, res);
            if (flags.Contains("t") ||
                // PB:BG
                Name == "21560002" ||
                // PB:E
                Name == "Graphic")
                HandleYunomi(NamePathNoExt, r, res);
        }

        return res;
    }

    private static readonly HashSet<string> _pbbgChara =
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
    private void HandleMariko(FpPath folder, Dictionary<(int i, int j), Memory<byte>> files, ISet<Data> set)
    {
        if (!files.TryGetValue((0, 0), out var a)) return;

        // Main graphics file
        for (int i = 0, hOffset = 0x800; i < 0x1000 / 4; i++, hOffset += 4)
        {
            int offset = i4l[a, hOffset], next = i4l[a, hOffset + 4];
            for (int p = 0; p < 8; p++)
            {
                if (!files.TryGetValue((3, p), out var palette)) continue;
                string subName = $"mn_{p:D4}_{i:D4}";
                var b = a.Slice(0x1800 + offset, next - offset);
                if (MarikoConvert(folder / "mariko" / subName, b.Span, palette.Span) is { } res)
                    set.Add(res);
                set.Add(Buffer(folder / "mariko_src" / $"{subName}.dat", b));
            }

            if (next >= a.Length - 0x1800) break;
        }

        // Sub-files
        foreach (var kvp in files)
        {
            (int i, int j) = kvp.Key;
            if (i <= 3 || i == 6) continue;
            for (int p = 0; p < 8; p++)
            {
                if (!files.TryGetValue((6, p), out var palette)) continue;
                string subName = $"ex_{p:D4}_{i:D4}_{j:D4}";
                if (MarikoConvert(folder / "mariko" / subName, kvp.Value.Span, palette.Span) is { } res)
                    set.Add(res);
            }
        }
    }

    /// Process character graphics file with palette
    private Data? MarikoConvert(FpPath path, ReadOnlySpan<byte> buffer, ReadOnlySpan<byte> paletteBuffer)
    {
        var ctx = new ReadContext<byte>(buffer);
        ushort w = u2l[ctx.ReadAdvance(2)], h = u2l[ctx.ReadAdvance(2)];
        if (w - 1 >= 2048 || h - 1 >= 2048) return null;
        try
        {
            uint[] arr = new uint[w * h];
            var wCtx = new WriteContext<uint>(arr);
            while (ctx.IsAvailable(2))
            {
                int val = u2l[ctx.ReadAdvance(2)], num = val & 0xf;
                if (num == 0)
                    wCtx.Advance(val >> 4);
                else
                {
                    int colorIdx = val >> 8, colorOffset = colorIdx * 4, alphaUpper = val & 0xf0;
                    byte alpha = (byte)(alphaUpper | (alphaUpper >> 4));
                    wCtx.WriteAdvance(paletteBuffer.Slice(colorOffset).FromRgb(alpha), num);
                }
            }

            return path.Image(w, h, arr);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// Process run-length graphics file container
    private void HandleYunomi(FpPath folder, Dictionary<(int i, int j), Memory<byte>> files, ISet<Data> set)
    {
        // Try to process all files as yunomi
        foreach (var kvp in files)
        {
            string subName = $"{kvp.Key.i:D4}_{kvp.Key.j:D4}";
            if (YunomiConvert(folder / "yunomi" / subName, kvp.Value.Span) is { } data)
                set.Add(data);
        }
    }

    /// Process run-length graphics file
    private Data? YunomiConvert(FpPath path, ReadOnlySpan<byte> buffer)
    {
        var ctx = new ReadContext<byte>(buffer);
        ushort w = u2l[ctx.ReadAdvance(2)], h = u2l[ctx.ReadAdvance(2)];
        if (w - 1 >= 2048 || h - 1 >= 2048) return null;
        try
        {
            uint[] arr = new uint[w * h];
            uint prevColor = 0;
            var wCtx = new WriteContext<uint>(arr);
            while (ctx.IsAvailable(4))
            {
                uint color = ctx.ReadAdvance(4).FromBgra();
                if (color == prevColor) wCtx.WriteAdvance(color, ctx.ReadAdvance() + 1);
                else wCtx.WriteAdvance(color);

                prevColor = color;
            }

            return path.Image(w, h, arr);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
