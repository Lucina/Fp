using Fp;
using Fp.Fs;
using Fp.Platforms.Nitro;

FsFormatMultiProcessor.Run<RnRProcessor>(args,
    "RnR",
    "Ryuusei no Rockman 1/2/3 .bin containers",
    ".bin");

public class RnRProcessor : FsFormatMultiProcessor
{
    public override IEnumerable<Data> Process()
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
        for (int i = 0, h = 0, offset, next = i4l[0]; (offset = next) < InputLength; h += 8)
        {
            next = i4l[h + 8];
            Memory<byte> file = buf[offset, next - offset];
            int fake = i4l[h + 4];
            if (fake < 0 && next != offset) file = file.DeNitro(fake & int.MaxValue);
            yield return Buffer(NamePathNoExt / ($"{i++:D4}" + file.GetNitroExtension()), file);
        }
    }
}
