using System;
using System.Collections.Generic;
using System.Linq;

namespace Fp.Fs;

// ReSharper disable InconsistentNaming
public partial class FsProcessor
{
    /// <summary>
    /// Keyword arg for stopping cli execution.
    /// </summary>
    public const string NO_EXECUTE_CLI = "--no-execute-cli";

    /// <summary>
    /// Registered scripting processors.
    /// </summary>
    public static readonly FsProcessorSource Registered = new();

    /// <summary>
    /// Processes using processor factory.
    /// </summary>
    /// <param name="fileSystemSource">Filesystem.</param>
    /// <param name="args">Arguments. If null, only register processors.</param>
    /// <param name="factories">Processor factories.</param>
    public static void Run(FileSystemSource? fileSystemSource, IList<string>? args, params FsProcessorFactory[] factories)
    {
        Registered.Factories.UnionWith(factories);
        if (args == null || args.Count == 1 && args[0] == NO_EXECUTE_CLI) return;
        Coordinator.CliRunFilesystem(args.ToArray(), default, default, fileSystemSource, factories);
    }

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    public static void Run(Action func, IList<string>? args, FsProcessorInfo? info = null) =>
        Run(null, args, new DelegateFsProcessorFactory(info, () => new ScriptingDirectProcessor(func)));

    /// <summary>
    /// Processes using direct function.
    /// </summary>
    /// <param name="func">Function or delegate run per file.</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    public static void Run(Action func, IList<string>? args, string name, string description, params string?[] extensions) =>
        Run(func, args, new FsProcessorInfo(name, description, description, extensions));

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="info">Processor info.</param>
    public static void Run(Func<IEnumerable<Data>> func, IList<string>? args, FsProcessorInfo? info = null) =>
        Run(null, args, new DelegateFsProcessorFactory(info, () => new ScriptingSegmentedProcessor(func)));

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="func">Function that returns enumerable (segmented processing enumerator).</param>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    public static void Run(Func<IEnumerable<Data>> func, IList<string>? args, string name, string description, params string?[] extensions) =>
        Run(func, args, new FsProcessorInfo(name, description, description, extensions));

    /// <summary>
    /// Processes using segmented function.
    /// </summary>
    /// <param name="args">Arguments. If null, only register processor.</param>
    /// <param name="name">Processor name.</param>
    /// <param name="description">Processor description.</param>
    /// <param name="extensions">Processor extensions.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    public static void Run<T>(IList<string>? args, string name, string description, params string?[] extensions) where T : FsProcessor, new() =>
        Run(null, args, new GenericNewFsProcessorFactory<T>(new FsProcessorInfo(name, description, description, extensions)));
}
