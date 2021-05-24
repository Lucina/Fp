using System.Collections.Generic;
using Fp;

Processor.Run<VibRibbonPakProcessor>(args,
    "VibRibbonPak",
    "yourExtendedDescription",
    ".pak");

public class VibRibbonPakProcessor : DataProcessor
{
    protected override IEnumerable<Data> ProcessData()
    {
        foreach (int ofs in i4la[4, i4l[0]])
        {
            (string name, int nameLen) = utf8[ofs];
            int fpos = ofs + (nameLen + 3) / 4 * 4;
            yield return Buffer(NamePathNoExt / name, buf[fpos + 4, i4l[fpos]]);
        }
    }
}
