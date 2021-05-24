using System;
using System.Collections.Generic;
using System.IO;
using DSDecmp.Formats.Nitro;
using Fp;

Processor.Run<RnRProcessor>(args,
    "RnR",
    "Ryuusei no Rockman 1/2/3 .bin containers",
    ".bin");

public class RnRProcessor : DataProcessor
{
    private static readonly NitroCFormat _lz10 = new LZ10();
    private static readonly NitroCFormat _lz11 = new LZ11();

    protected override IEnumerable<Data> ProcessData()
    {
        /*
         * Format
         * (little)
         * {
         *   u32 offset
         *   u32 fake (top bit: compressed, rest: decomp len)
         * } * files
         * u32 filesize
         * u32 0xffff (RnR1/2) or 0 (RnR3)
         */
        byte[] b = Load();
        for (int i = 0, offset; (offset = i4l[b, i * 8]) != b.Length; i++)
        {
            int fake = i4l[b, i * 8 + 4]; // Second u32
            int len = i4l[b, i * 8 + 8] - offset;
            Memory<byte> tar = b.AsMemory(offset, len);
            if (fake < 0 && len != 0) // highest bit set -> compressed
            {
                MemoryStream ms = new(fake & int.MaxValue); // Mask away top bit

                // Use magic to get compression format
                (tar.Span[0] switch
                {
                    0x10 => _lz10,
                    0x11 => _lz11,
                    _ => throw new ApplicationException($"Unknown compression magic {tar.Span[0]}")
                }).Decompress(tar.Stream(), len, ms);
                tar = ms.Dump();
            }

            // https://wiki.vg-resource.com/Nitro_Files
            string ext = utf8[tar, 4].String switch
            {
                "BMD0" => ".nsbmd",
                "BTX0" => ".nsbtx",
                "BCA0" => ".nsbca",
                "BTP0" => ".nsbtp",
                "BTA0" => ".nsbta",
                "BMA0" => ".nsbma",
                "BVA0" => ".nsbva",
                _ => ".dat"
            };

            yield return Buffer(NamePathNoExt / $"{i:D4}{ext}", tar);
        }
    }
}
