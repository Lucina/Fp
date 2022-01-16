using System;
using System.IO;

namespace Fp;

/// <summary>
/// Represents a processor that operates on a single input.
/// </summary>
public class FormatProcessor : Processor
{
    /// <summary>
    /// Current file name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    /// <param name="name">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    public void Prepare(Stream stream, string name, ProcessorConfiguration? configuration = null)
    {
        Prepare(configuration);
        UseStream(stream);
        Name = name;
    }

    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="memory">Source memory.</param>
    /// <param name="name">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    public void Prepare(ReadOnlyMemory<byte> memory, string name, ProcessorConfiguration? configuration = null) => Prepare(new MStream(memory), name, configuration);

    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="name">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    public void Prepare(BufferData<byte> data, string name, ProcessorConfiguration? configuration = null) => Prepare(new MStream(data.Buffer), name, configuration);

    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="configuration">Additional configuration object.</param>
    public void Prepare(BufferData<byte> data, ProcessorConfiguration? configuration = null) => Prepare(data, Path.GetFileName(data.BasePath), configuration);
}
