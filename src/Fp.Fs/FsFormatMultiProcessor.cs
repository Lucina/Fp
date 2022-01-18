using System.Collections.Generic;

namespace Fp;

/// <inheritdoc />
public class FsFormatMultiProcessor : FormatMultiProcessor
{
    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FormatMultiProcessor, new() =>
        FsProcessor.Run(null, args, new GenericNewFsProcessorFactory<FormatMultiProcessorFsWrapper<T>>(new FsProcessorInfo(name, description, description, extensions)));
}
