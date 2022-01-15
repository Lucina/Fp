using System.Diagnostics.CodeAnalysis;

namespace Fp;

/// <summary>
/// Block cipher padding mode.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum CipherPaddingMode
{
    /// <summary>
    /// End of message is padded with null bytes.
    /// </summary>
    Zero,

    /// <summary>
    /// ANSI X9.23 padding.
    /// </summary>
    AnsiX9_23,

    /// <summary>
    /// ISO 10126 padding.
    /// </summary>
    Iso10126,

    /// <summary>
    /// PKCS#7 padding.
    /// </summary>
    Pkcs7,

    /// <summary>
    /// PKCS#5 padding.
    /// </summary>
    Pkcs5,

    /// <summary>
    /// ISO/IEC 7816-4:2005 padding.
    /// </summary>
    Iso_Iec_7816_4
}
