using System.Collections.Generic;

namespace Fp.Fs;

/// <inheritdoc />
public class FsFormatSingleProcessor : FormatSingleProcessor
{
    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FormatSingleProcessor, new() =>
        FsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<T>>(new FileProcessorInfo(name, description, description, extensions)));
}

/// <inheritdoc />
public class FsFormatSingleProcessor<T> : FormatSingleProcessor<T> where T : Data
{
    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="TProcessor">Processor type.</typeparam>
    public static void Run<TProcessor>(IList<string>? args, string name, string description, params string?[] extensions) where TProcessor : FormatSingleProcessor, new() =>
        FsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatSingleProcessorFsWrapper<TProcessor>>(new FileProcessorInfo(name, description, description, extensions)));
}
