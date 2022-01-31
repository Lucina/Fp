using System;
using System.IO;

namespace Fp;

/// <summary>
/// Represents a processor that operates on a single named input stream.
/// </summary>
public class FormatProcessor : FileProcessor
{
    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    /// <param name="inputFile">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    public void Prepare(Stream stream, string inputFile, ProcessorConfiguration? configuration = null)
    {
        Prepare(inputFile, configuration);
        UseStream(stream);
    }

    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="memory">Source memory.</param>
    /// <param name="inputFile">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    public void Prepare(ReadOnlyMemory<byte> memory, string inputFile, ProcessorConfiguration? configuration = null) => Prepare(new MStream(memory), inputFile, configuration);

    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="inputFile">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    public void Prepare(BufferData<byte> data, string inputFile, ProcessorConfiguration? configuration = null) => Prepare(new MStream(data.Buffer), inputFile, configuration);

    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="configuration">Additional configuration object.</param>
    public void Prepare(BufferData<byte> data, ProcessorConfiguration? configuration = null) => Prepare(data, Path.GetFileName(data.BasePath), configuration);
}
