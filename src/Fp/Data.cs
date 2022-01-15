using System;
using System.Collections.Generic;
using System.IO;

namespace Fp;

/// <summary>
/// Intermediate-format data container.
/// </summary>
public abstract class Data : IDisposable, ICloneable
{
    /// <summary>
    /// Generic format.
    /// </summary>
    public static readonly Guid Generic = Guid.Parse("2FE8E773-EE1F-42BB-95B2-F8E4B92B46C2");

    /// <summary>
    /// Export unsupported.
    /// </summary>
    public static readonly Guid ExportUnsupported = Guid.Parse("DF6E479A-F417-4D32-BD38-E85815F3D07A");

    /// <summary>
    /// UTF-8 encoded text format.
    /// </summary>
    public static readonly Guid UTF8 = Guid.Parse("760F762B-C882-4D82-8F0D-43502641C1F8");

    /// <summary>
    /// Base path of resource.
    /// </summary>
    public string BasePath { get; protected init; }

    /// <summary>
    /// If true, object does not contain complete data, e.g. for <see cref="WriteConvertedData"/>.
    /// </summary>
    public bool Dry { get; protected init; }

    /// <summary>
    /// Default format for container.
    /// </summary>
    public virtual Guid DefaultFormat => Generic;

    /// <summary>
    /// Supported formats for container.
    /// </summary>
    public virtual IReadOnlyCollection<Guid> SupportedFormats { get; } = new[] { Generic };

    /// <summary>
    /// Creates instance of <see cref="Data"/>.
    /// </summary>
    /// <param name="basePath">Base path of resource.</param>
    protected Data(string basePath)
    {
        BasePath = basePath;
    }

    /// <summary>
    /// Checks if a specific file format and configuration is supported for export.
    /// </summary>
    /// <param name="format">Requested file format.</param>
    /// <param name="formatOptions">Format-specific options.</param>
    /// <returns>False if requested format is not supported.</returns>
    public abstract bool SupportsFormat(Guid format, Dictionary<object, object>? formatOptions = null);

    /// <summary>
    /// Writes stream of data converted to common file format.
    /// </summary>
    /// <param name="outputStream">Target stream.</param>
    /// <param name="format">Requested file format.</param>
    /// <param name="formatOptions">Format-specific options.</param>
    /// <returns>False if requested format is not supported.</returns>
    public abstract bool WriteConvertedData(Stream outputStream, Guid format, Dictionary<object, object>? formatOptions = null);

    /// <summary>
    /// Gets file extension for format.
    /// </summary>
    /// <param name="format">Format to get extension of.</param>
    /// <returns>File extension or null if unrecognized.</returns>
    public virtual string? GetExtension(Guid? format = null)
    {
        format ??= DefaultFormat;
        return format switch
        {
            _ when format == Generic => "", // If appending extension, generic -> no change in extension
            _ when format == ExportUnsupported => "",
            _ when format == UTF8 => ".txt",
            _ => null
        };
    }

    /// <inheritdoc />
    public abstract void Dispose();

    /// <inheritdoc />
    public abstract object Clone();
}
