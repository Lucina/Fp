using Fp;
using System;
using System.Collections;

Processor.Run<PhantomBreakerProcessor>(args,
    "PhantomBreaker",
    "Decode / extract assets from Phantom Breaker series",
    (string?)null);

public class PhantomBreakerProcessor : LayeredDataProcessor<(int i, int j), Memory<byte>>
{
    protected override void ProcessLayered()
    {
        UseChild(typeof(Mariko), typeof(Yunomi));

        byte[] a = Load();

        int pos = i4l[a] ^= -1, numT1 = i4l[a, 4] ^= 0x12876623, posT2 = 8 + numT1 * 4;
        for (int i = 8, hSrc = 0xa9; i < pos; i++, hSrc += 6) a[i] ^= (byte)hSrc;

        // Table1: groups
        for (int i = 0; i < numT1; i++)
        {
            int ofsT1 = i4l[a, 8 + i * 4];
            if (ofsT1 == -1) continue;
            int numT2 = i2l[a, posT2 + ofsT1], t2Bytes = numT2.GetBytesForBits();
            var bits = new BitArray(a.SliceAlloc(posT2 + ofsT1 + 2, t2Bytes));
            // Table2: files
            for (int j = 0, p = posT2 + ofsT1 + 2 + t2Bytes; j < numT2; j++, p += 8)
            {
                bits.ConstrainedSkipBits(ref j, false, numT2);
                var memory = a.AsMemory(pos + i4l[a, p], i4l[a, p + 4]);
                var sp = memory.Span;
                byte s = (byte)(((sp.Length >> 3) & 0x5e) + (i & 0x7a) * 3 + (j & 0x5f) * 7);
                for (int x = 0; x < sp.Length; x++) sp[x] ^= (byte)((0xe6 & x) ^ s);

                Lookup[(i, j)] = memory;
                Content.Add(Buffer(NamePathNoExt / $"{i:D4}_{j:D4}" + memory._WAV().___(""), memory));
            }
        }
    }
}
