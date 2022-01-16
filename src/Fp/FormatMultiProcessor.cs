using System;
using System.Collections.Generic;
using System.IO;

namespace Fp;

/// <summary>
/// Represents a processor that operates on a single input and generates multiple outputs.
/// </summary>
public class FormatMultiProcessor : FormatProcessor
{
    /// <summary>
    /// Attempts to process and get outputs.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    /// <param name="name">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>Outputs.</returns>
    public static IEnumerable<Data> Process<T>(Stream stream, string name, ProcessorConfiguration? configuration = null) where T : FormatMultiProcessor, new()
    {
        using T processor = new();
        return processor.Process(stream, name, configuration);
    }

    /// <summary>
    /// Attempts to process and get outputs.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    /// <param name="name">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>Outputs.</returns>
    public IEnumerable<Data> Process(Stream stream, string name, ProcessorConfiguration? configuration = null)
    {
        Prepare(stream, name, configuration);
        return Process();
    }

    /// <summary>
    /// Attempts to process and get outputs.
    /// </summary>
    /// <param name="memory">Source memory.</param>
    /// <param name="name">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>Outputs.</returns>
    public static IEnumerable<Data> Process<T>(ReadOnlyMemory<byte> memory, string name, ProcessorConfiguration? configuration = null) where T : FormatMultiProcessor, new()
    {
        using T processor = new();
        return processor.Process(memory, name, configuration);
    }

    /// <summary>
    /// Attempts to process and get outputs.
    /// </summary>
    /// <param name="memory">Source memory.</param>
    /// <param name="name">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>Outputs.</returns>
    public IEnumerable<Data> Process(ReadOnlyMemory<byte> memory, string name, ProcessorConfiguration? configuration = null)
    {
        Prepare(memory, name, configuration);
        return Process();
    }

    /// <summary>
    /// Attempts to process and get outputs.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="name">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>Outputs.</returns>
    public static IEnumerable<Data> Process<T>(BufferData<byte> data, string name, ProcessorConfiguration? configuration = null) where T : FormatMultiProcessor, new()
    {
        using T processor = new();
        return processor.Process(data, name, configuration);
    }

    /// <summary>
    /// Attempts to process and get outputs.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="name">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>Outputs.</returns>
    public IEnumerable<Data> Process(BufferData<byte> data, string name, ProcessorConfiguration? configuration = null)
    {
        Prepare(data, name, configuration);
        return Process();
    }

    /// <summary>
    /// Attempts to process and get outputs.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>Outputs.</returns>
    public static IEnumerable<Data> Process<T>(BufferData<byte> data, ProcessorConfiguration? configuration = null) where T : FormatMultiProcessor, new()
    {
        using T processor = new();
        return processor.Process(data, configuration);
    }

    /// <summary>
    /// Attempts to process and get outputs.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>Outputs.</returns>
    public IEnumerable<Data> Process(BufferData<byte> data, ProcessorConfiguration? configuration = null)
    {
        Prepare(data, configuration);
        return Process();
    }

    /// <summary>
    /// Attempts to process and get outputs.
    /// </summary>
    /// <returns>Outputs.</returns>
    public virtual IEnumerable<Data> Process() => Array.Empty<Data>();

    // TODO easy cli
}
