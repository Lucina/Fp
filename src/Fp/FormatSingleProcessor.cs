using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Fp;

/// <summary>
/// Represents a processor that operates on a single input and generates a single output.
/// </summary>
public class FormatSingleProcessor : FormatProcessor
{
    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    /// <param name="name">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>True if successful and output was created.</returns>
    public static bool TryProcess<T>(Stream stream, string name, [NotNullWhen(true)] out Data? result, ProcessorConfiguration? configuration = null) where T : FormatSingleProcessor, new()
    {
        using T processor = new();
        return processor.TryProcess(stream, name, out result, configuration);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    /// <param name="name">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>True if successful and output was created.</returns>
    public bool TryProcess(Stream stream, string name, [NotNullWhen(true)] out Data? result, ProcessorConfiguration? configuration = null)
    {
        Prepare(stream, name, configuration);
        return TryProcess(out result);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="memory">Source memory.</param>
    /// <param name="name">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>True if successful and output was created.</returns>
    public static bool TryProcess<T>(ReadOnlyMemory<byte> memory, string name, [NotNullWhen(true)] out Data? result, ProcessorConfiguration? configuration = null) where T : FormatSingleProcessor, new()
    {
        using T processor = new();
        return processor.TryProcess(memory, name, out result, configuration);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="memory">Source memory.</param>
    /// <param name="name">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>True if successful and output was created.</returns>
    public bool TryProcess(ReadOnlyMemory<byte> memory, string name, [NotNullWhen(true)] out Data? result, ProcessorConfiguration? configuration = null)
    {
        Prepare(memory, name, configuration);
        return TryProcess(out result);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="name">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>True if successful and output was created.</returns>
    public static bool TryProcess<T>(BufferData<byte> data, string name, [NotNullWhen(true)] out Data? result, ProcessorConfiguration? configuration = null) where T : FormatSingleProcessor, new()
    {
        using T processor = new();
        return processor.TryProcess(data, name, out result, configuration);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="name">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>True if successful and output was created.</returns>
    public bool TryProcess(BufferData<byte> data, string name, [NotNullWhen(true)] out Data? result, ProcessorConfiguration? configuration = null)
    {
        Prepare(data, name, configuration);
        return TryProcess(out result);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>True if successful and output was created.</returns>
    public static bool TryProcess<T>(BufferData<byte> data, [NotNullWhen(true)] out Data? result, ProcessorConfiguration? configuration = null) where T : FormatSingleProcessor, new()
    {
        using T processor = new();
        return processor.TryProcess(data, out result, configuration);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>True if successful and output was created.</returns>
    public bool TryProcess(BufferData<byte> data, [NotNullWhen(true)] out Data? result, ProcessorConfiguration? configuration = null)
    {
        Prepare(data, configuration);
        return TryProcess(out result);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="result">Data output.</param>
    /// <returns>True if successful and output was created.</returns>
    public virtual bool TryProcess([NotNullWhen(true)] out Data? result)
    {
        result = null;
        return false;
    }
}

/// <summary>
/// Represents a processor that operates on a single input and generates multiple outputs.
/// </summary>
public class FormatSingleProcessor<T> : FormatSingleProcessor where T : Data
{
    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    /// <param name="inputFile">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="TProcessor">Processor type.</typeparam>
    /// <returns>True if successful and output was created.</returns>
    public static bool TryProcess<TProcessor>(Stream stream, string inputFile, [NotNullWhen(true)] out T? result, ProcessorConfiguration? configuration = null) where TProcessor : FormatSingleProcessor<T>, new()
    {
        using TProcessor processor = new();
        return processor.TryProcess(stream, inputFile, out result, configuration);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="stream">Source stream.</param>
    /// <param name="inputFile">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>True if successful and output was created.</returns>
    public bool TryProcess(Stream stream, string inputFile, [NotNullWhen(true)] out T? result, ProcessorConfiguration? configuration = null)
    {
        Prepare(stream, inputFile, configuration);
        return TryProcess(out result);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="memory">Source memory.</param>
    /// <param name="inputFile">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="TProcessor">Processor type.</typeparam>
    /// <returns>True if successful and output was created.</returns>
    public static bool TryProcess<TProcessor>(ReadOnlyMemory<byte> memory, string inputFile, [NotNullWhen(true)] out T? result, ProcessorConfiguration? configuration = null) where TProcessor : FormatSingleProcessor<T>, new()
    {
        using TProcessor processor = new();
        return processor.TryProcess(memory, inputFile, out result, configuration);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="memory">Source memory.</param>
    /// <param name="inputFile">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>True if successful and output was created.</returns>
    public bool TryProcess(ReadOnlyMemory<byte> memory, string inputFile, [NotNullWhen(true)] out T? result, ProcessorConfiguration? configuration = null)
    {
        Prepare(memory, inputFile, configuration);
        return TryProcess(out result);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="inputFile">Input name.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="TProcessor">Processor type.</typeparam>
    /// <returns>True if successful and output was created.</returns>
    public static bool TryProcess<TProcessor>(BufferData<byte> data, string inputFile, [NotNullWhen(true)] out T? result, ProcessorConfiguration? configuration = null) where TProcessor : FormatSingleProcessor<T>, new()
    {
        using TProcessor processor = new();
        return processor.TryProcess(data, inputFile, out result, configuration);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="inputFile">Input name.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <param name="result">Data output.</param>
    /// <returns>True if successful and output was created.</returns>
    public bool TryProcess(BufferData<byte> data, string inputFile, [NotNullWhen(true)] out T? result, ProcessorConfiguration? configuration = null)
    {
        Prepare(data, inputFile, configuration);
        return TryProcess(out result);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <typeparam name="TProcessor">Processor type.</typeparam>
    /// <returns>True if successful and output was created.</returns>
    public static bool TryProcess<TProcessor>(BufferData<byte> data, [NotNullWhen(true)] out T? result, ProcessorConfiguration? configuration = null) where TProcessor : FormatSingleProcessor<T>, new()
    {
        using TProcessor processor = new();
        return processor.TryProcess(data, out result, configuration);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="data">Source data.</param>
    /// <param name="result">Data output.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <returns>True if successful and output was created.</returns>
    public bool TryProcess(BufferData<byte> data, [NotNullWhen(true)] out T? result, ProcessorConfiguration? configuration = null)
    {
        Prepare(data, configuration);
        return TryProcess(out result);
    }

    /// <summary>
    /// Attempts to process and get an output.
    /// </summary>
    /// <param name="result">Data output.</param>
    /// <returns>True if successful and output was created.</returns>
    public virtual bool TryProcess([NotNullWhen(true)] out T? result)
    {
        result = null;
        return false;
    }

    /// <inheritdoc />
    public sealed override bool TryProcess([NotNullWhen(true)] out Data? result)
    {
        if (TryProcess(out T? d))
        {
            result = d;
            return true;
        }
        result = null;
        return false;
    }
}
