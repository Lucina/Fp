using Fp.Helpers;

namespace Fp;

public partial class Processor
{
    /// <summary>
    /// Helper for unsigned 8-bit integer arrays.
    /// </summary>
    public U8ArrayHelper buf = null!;

    /// <summary>
    /// Helper for signed 8-bit integers.
    /// </summary>
    public S8Helper i1 = null!;

    /// <summary>
    /// Helper for signed 8-bit integer arrays.
    /// </summary>
    public S8ArrayHelper i1a = null!;

    /// <summary>
    /// Helper for unsigned 8-bit integers.
    /// </summary>
    public U8Helper u1 = null!;

    /// <summary>
    /// Helper for unsigned 8-bit integer arrays.
    /// </summary>
    public U8ArrayHelper u1a = null!;

    /// <summary>
    /// Helper for signed 16-bit integers.
    /// </summary>
    public S16Helper i2l = null!;

    /// <summary>
    /// Helper for signed 16-bit integer arrays.
    /// </summary>
    public S16ArrayHelper i2la = null!;

    /// <summary>
    /// Helper for signed 32-bit integers.
    /// </summary>
    public S32Helper i4l = null!;

    /// <summary>
    /// Helper for signed 32-bit integer arrays.
    /// </summary>
    public S32ArrayHelper i4la = null!;

    /// <summary>
    /// Helper for signed 64-bit integers.
    /// </summary>
    public S64Helper i8l = null!;

    /// <summary>
    /// Helper for signed 64-bit integer arrays.
    /// </summary>
    public S64ArrayHelper i8la = null!;

    /// <summary>
    /// Helper for unsigned 16-bit integers.
    /// </summary>
    public U16Helper u2l = null!;

    /// <summary>
    /// Helper for unsigned 16-bit integer arrays.
    /// </summary>
    public U16ArrayHelper u2la = null!;

    /// <summary>
    /// Helper for unsigned 32-bit integers.
    /// </summary>
    public U32Helper u4l = null!;

    /// <summary>
    /// Helper for unsigned 32-bit integer arrays.
    /// </summary>
    public U32ArrayHelper u4la = null!;

    /// <summary>
    /// Helper for unsigned 64-bit integers.
    /// </summary>
    public U64Helper u8l = null!;

    /// <summary>
    /// Helper for unsigned 64-bit integer arrays.
    /// </summary>
    public U64ArrayHelper u8la = null!;

    /// <summary>
    /// Helper for signed 16-bit integers.
    /// </summary>
    public S16Helper i2b = null!;

    /// <summary>
    /// Helper for signed 16-bit integer arrays.
    /// </summary>
    public S16ArrayHelper i2ba = null!;

    /// <summary>
    /// Helper for signed 32-bit integers.
    /// </summary>
    public S32Helper i4b = null!;

    /// <summary>
    /// Helper for signed 32-bit integer arrays.
    /// </summary>
    public S32ArrayHelper i4ba = null!;

    /// <summary>
    /// Helper for signed 64-bit integers.
    /// </summary>
    public S64Helper i8b = null!;

    /// <summary>
    /// Helper for signed 64-bit integer arrays.
    /// </summary>
    public S64ArrayHelper i8ba = null!;

    /// <summary>
    /// Helper for unsigned 16-bit integers.
    /// </summary>
    public U16Helper u2b = null!;

    /// <summary>
    /// Helper for unsigned 16-bit integer arrays.
    /// </summary>
    public U16ArrayHelper u2ba = null!;

    /// <summary>
    /// Helper for unsigned 32-bit integers.
    /// </summary>
    public U32Helper u4b = null!;

    /// <summary>
    /// Helper for unsigned 32-bit integer arrays.
    /// </summary>
    public U32ArrayHelper u4ba = null!;

    /// <summary>
    /// Helper for unsigned 64-bit integers.
    /// </summary>
    public U64Helper u8b = null!;

    /// <summary>
    /// Helper for unsigned 64-bit integer arrays.
    /// </summary>
    public U64ArrayHelper u8ba = null!;

    /// <summary>
    /// Helper for 16-bit floating point numbers.
    /// </summary>
    public F16Helper f2 = null!;

    /// <summary>
    /// Helper for 16-bit floating point number arrays.
    /// </summary>
    public F16ArrayHelper f2a = null!;

    /// <summary>
    /// Helper for 32-bit floating point numbers.
    /// </summary>
    public THelper<float> f4 = null!;

    /// <summary>
    /// Helper for 32-bit floating point number arrays.
    /// </summary>
    public TArrayHelper<float> f4a = null!;

    /// <summary>
    /// Helper for 64-bit floating point numbers.
    /// </summary>
    public THelper<double> f8 = null!;

    /// <summary>
    /// Helper for 64-bit floating point number arrays.
    /// </summary>
    public TArrayHelper<double> f8a = null!;

    /// <summary>
    /// Helper for ASCII strings.
    /// </summary>
    public AsciiStringHelper ascii = null!;

    /// <summary>
    /// Helper for UTF-8 strings.
    /// </summary>
    public Utf8StringHelper utf8 = null!;

    /// <summary>
    /// Helper for UTF-8 strings.
    /// </summary>
    public Utf16StringHelper utf16 = null!;

    private void InitEncodingDecodingHelpers()
    {
        i1 = new S8Helper(this);
        i1a = new S8ArrayHelper(this);
        u1 = new U8Helper(this);
        u1a = new U8ArrayHelper(this);
        buf = u1a;
        i2l = new S16Helper(this, true);
        i2la = new S16ArrayHelper(this, true);
        i4l = new S32Helper(this, true);
        i4la = new S32ArrayHelper(this, true);
        i8l = new S64Helper(this, true);
        i8la = new S64ArrayHelper(this, true);
        u2l = new U16Helper(this, true);
        u2la = new U16ArrayHelper(this, true);
        u4l = new U32Helper(this, true);
        u4la = new U32ArrayHelper(this, true);
        u8l = new U64Helper(this, true);
        u8la = new U64ArrayHelper(this, true);
        i2b = new S16Helper(this, false);
        i2ba = new S16ArrayHelper(this, false);
        i4b = new S32Helper(this, false);
        i4ba = new S32ArrayHelper(this, false);
        i8b = new S64Helper(this, false);
        i8ba = new S64ArrayHelper(this, false);
        u2b = new U16Helper(this, false);
        u2ba = new U16ArrayHelper(this, false);
        u4b = new U32Helper(this, false);
        u4ba = new U32ArrayHelper(this, false);
        u8b = new U64Helper(this, false);
        u8ba = new U64ArrayHelper(this, false);
        f2 = new F16Helper(this);
        f2a = new F16ArrayHelper(this);
        f4 = new THelper<float>(this);
        f4a = new TArrayHelper<float>(this);
        f8 = new THelper<double>(this);
        f8a = new TArrayHelper<double>(this);
        ascii = new AsciiStringHelper(this);
        utf8 = new Utf8StringHelper(this);
        utf16 = new Utf16StringHelper(this);
    }
}
