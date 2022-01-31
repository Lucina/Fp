using System;
using System.IO;
using System.Linq;

namespace Fp;

/// <summary>
/// Represents a processor that operates on a single named input.
/// </summary>
public class FileProcessor : Processor
{
    /// <summary>
    /// Current file name.
    /// </summary>
    public string InputFile { get; set; } = "";

    /// <summary>
    /// Current file name.
    /// </summary>
    public string Name => Path.GetFileName(InputFile);

    /// <summary>
    /// Current file name without extension.
    /// </summary>
    public string NameNoExt => Path.GetFileNameWithoutExtension(InputFile);

    /// <summary>
    /// Current file name.
    /// </summary>
    public FpPath NamePath => FpPath.GetFromString(Name) ?? throw new InvalidOperationException();

    /// <summary>
    /// Current file name without extension.
    /// </summary>
    public FpPath NamePathNoExt => FpPath.GetFromString(NameNoExt) ?? throw new InvalidOperationException();

    /// <summary>
    /// Selected file extension from input file, if available.
    /// </summary>
    public string? SelectedExtension;

    /// <summary>
    /// Info for this processor, if available.
    /// </summary>
    public FileProcessorInfo? Info;

    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="inputFile">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    public void Prepare(string inputFile, ProcessorConfiguration? configuration = null)
    {
        Prepare(configuration);
        InputFile = inputFile;
        ValidateExtension(InputFile, out SelectedExtension);
    }

    /// <summary>
    /// Checks if this processor will accept the specified path.
    /// </summary>
    /// <param name="path">Path to check.</param>
    /// <returns>True if accepted.</returns>
    /// <remarks>
    /// The default implementation uses <see cref="ValidateExtension"/>.
    /// </remarks>
    public virtual bool AcceptFile(string path) => ValidateExtension(path, out _);

    /// <summary>
    /// Validates extension based on <see cref="Info"/>.<see cref="FileProcessorInfo.Extensions"/>.
    /// </summary>
    /// <param name="path">Path to check.</param>
    /// <param name="extension">Null or selected extension (may also be null if successful).</param>
    /// <returns>True if succeeded.</returns>
    public bool ValidateExtension(string path, out string? extension)
    {
        extension = null;
        if (Info?.Extensions is not { Length: > 0 } exts) return true;
        foreach (string? ext in exts.OrderByDescending(e => e?.Length ?? int.MaxValue))
            if (ext == null)
            {
                if (!path.Contains('.')) return true;
            }
            else if (path.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase))
            {
                extension = ext;
                return true;
            }
        return false;
    }
}
