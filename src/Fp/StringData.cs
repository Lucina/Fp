using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fp;

/// <summary>
/// Represents general text data.
/// </summary>
public class StringData : Data
{
    /// <summary>
    /// String value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new instance of <see cref="StringData"/>.
    /// </summary>
    /// <param name="basePath">Base path of resource.</param>
    /// <param name="stringBuilder">String builder to get content string from.</param>
    public StringData(string basePath, StringBuilder stringBuilder) : this(basePath, stringBuilder.ToString())
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="StringData"/>.
    /// </summary>
    /// <param name="basePath">Base path of resource.</param>
    /// <param name="value">String value.</param>
    public StringData(string basePath, string value) : base(basePath)
    {
        Value = value;
    }

    /// <inheritdoc />
    public override Guid DefaultFormat => UTF8;

    /// <inheritdoc />
    public override IReadOnlyCollection<Guid> SupportedFormats { get; } = new[] { Generic, UTF8 };

    /// <inheritdoc />
    public override bool SupportsFormat(Guid format, Dictionary<object, object>? formatOptions = null)
    {
        return format == Generic || format == UTF8;
    }

    /// <inheritdoc />
    public override bool WriteConvertedData(Stream outputStream, Guid format, Dictionary<object, object>? formatOptions = null)
    {
        if (format == Generic || format == UTF8)
        {
            using var tw = new StreamWriter(outputStream, Encoding.UTF8, 4096, true);
            tw.Write(Value);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
    }

    /// <inheritdoc />
    public override object Clone() => new StringData(BasePath, Value);

    /// <inheritdoc />
    // ReSharper disable once ReturnTypeCanBeNotNullable
    public override string? ToString() => Value;
}

public partial class Processor
{
    /// <summary>
    /// Creates string data object.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="value">Value.</param>
    /// <returns>Data object.</returns>
    public static StringData Text(FpPath path, string value) => new(path.AsCombined(), value);

    /// <summary>
    /// Creates string data object.
    /// </summary>
    /// <param name="name">Path.</param>
    /// <param name="value">Value.</param>
    /// <returns>Data object.</returns>
    public static StringData Text(string name, string value) => new(name, value);
}

public partial class Scripting
{
    /// <summary>
    /// Creates string data object.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="value">Value.</param>
    /// <returns>Data object.</returns>
    public static Data text(this FpPath path, string value) => Processor.Text(path, value);

    /// <summary>
    /// Creates string data object.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="value">Value.</param>
    /// <returns>Data object.</returns>
    public static Data text(this string path, string value) => Processor.Text(path, value);
}
