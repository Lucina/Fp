using System;
using System.Collections.Generic;

namespace Fp;

/// <summary>
/// Configuration for batch-processing fs with processors.
/// </summary>
public record ProcessorConfiguration
{
    /// <summary>
    /// Default configuration.
    /// </summary>
    public static readonly ProcessorConfiguration Default = new(Array.Empty<string>());

    /// <summary>
    /// Creates configuration.
    /// </summary>
    /// <param name="args">Arguments.</param>
    /// <param name="preload">Whether to read all streams to memory.</param>
    /// <param name="debug">Whether to enable <see cref="Processor.Debug"/></param>
    /// <param name="nop">Whether to disable outputs.</param>
    /// <param name="logWriter">Log writer.</param>
    public ProcessorConfiguration(IReadOnlyList<string> args, bool preload = false, bool debug = false, bool nop = false, ILogWriter? logWriter = default)
    {
        Preload = preload;
        Debug = debug;
        Nop = nop;
        LogWriter = logWriter;
        Args = args;
    }

    /// <summary>
    /// Whether to read all streams to memory.
    /// </summary>
    public bool Preload { get; init; }

    /// <summary>
    /// Whether to enable <see cref="Processor.Debug"/>.
    /// </summary>
    public bool Debug { get; init; }

    /// <summary>
    /// Whether to disable outputs.
    /// </summary>
    public bool Nop { get; init; }

    /// <summary>
    /// Log writer.
    /// </summary>
    public ILogWriter? LogWriter { get; init; }

    /// <summary>
    /// Arguments.
    /// </summary>
    public IReadOnlyList<string> Args { get; init; }
}
