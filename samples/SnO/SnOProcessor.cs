using System.Runtime.InteropServices;
using Fp;

FsProcessor.Run<RnRProcessor>(args,
    "SnO",
    "Sousei no Onmyouji containers");

public class RnRProcessor : DataProcessor
{
    protected override IEnumerable<Data> ProcessData()
    {
        switch (ReadUtf8String(out _, out _, 4))
        {
            case "bg  ":
                {
                    int w = i4l[4], h = i4l[8];
                    WriteContext<uint> wc = NamePathNoExt.CreateImage(w, h, out Data image);
                    Read(28, MemoryMarshal.Cast<uint, byte>(wc.Source), false);
                    yield return image;
                    break;
                }
            default:
                yield break;
        }
    }
}
