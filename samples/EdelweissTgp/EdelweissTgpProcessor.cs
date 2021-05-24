using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using Esper.Zstandard;
using Fp;

Processor.Run<EdelweissTgpProcessor>(args,
    "EdelweissTgp",
    "Edelweiss TGP container",
    ".tgp");

public class EdelweissTgpProcessor : DataProcessor
{
    protected override IEnumerable<Data> ProcessData()
    {
        if (!HasMagic("TGP0")) return Nothing;
        ushort ver = u2l[0x4];
        return ver switch
        {
            0x0003 => Process_0x0003(),
            _ => UnsupportedVersion(ver)
        };
    }

    private IEnumerable<Data> Process_0x0003()
    {
        ushort flags = u2l[0x6];
        int count = i4l[0x8];
        int mainOff = 0x10 + count * 0x70;
        bool compress = (flags & 0x2) != 0;
        List<int>? positions = null;
        if (compress)
        {
            positions = new List<int>(Match(mainOff, "\x28\xb5\x2f\xfd").Select(v => (int)v));
            if (positions.Count != count)
            {
                LogWarn("Incorrect match count for zstd magic");
                yield break;
            }
        }

        for (int i = 0; i < count; i++)
        {
            byte[] entry = buf[0x10 + i * 0x70, 0x70];
            string name = utf8[entry, 0x60].String;
            int offset = positions?[i] ?? mainOff + i4l[entry, 0x60];
            int uncompressedLength = i4l[entry, 0x64];
            int end = i + 1 == count ? (int)InputLength : positions?[i + 1] ?? offset + uncompressedLength;
            if (Debug) LogInfo($"{offset:X8}/{uncompressedLength:X8} {name}");
            var stream = new ZstandardStream(buf[offset, end - offset].Stream(), CompressionMode.Decompress);
            yield return Buffer(name, stream.Dump());
        }
    }
}
