using Fp;

FsProcessor.Run<LinkDataAbcProcessor>(args,
    "LinkDataAbcProcessor",
    "Dynasty Warriors Gundam linkdata files",
    ".ans", ".bns", ".cns");

public class LinkDataAbcProcessor : FsProcessor
{
    protected override IEnumerable<Data> ProcessSegmentedImpl()
    {
        OpenMainFile();
        int numEntries = i4b[0x4];
        for (int i = 0; i < numEntries; i++)
        {
            int tableOffset = 0x10 + i * 8;
            int offset = i4b[tableOffset] * 0x800;
            int length = i4b[tableOffset + 4];
            byte[] buffer = buf[offset, length];
            string ext = buffer._WAV().___(".bin");
            LogInfo($"{NameNoExt}: {i}/{numEntries} {(float)(offset + length) / InputLength * 100.0f:F2}% ({ext})");
            yield return Buffer(NamePathNoExt / $"{i:D4}{ext}", buffer);
        }
    }
}
