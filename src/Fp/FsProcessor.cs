using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using static Fp.FsProcessor;

namespace Fp;

/// <summary>
/// Base type for file processors.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
[SuppressMessage("ReSharper", "NotAccessedField.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class FsProcessor : Processor
{
    #region Properties and fields

    /// <summary>
    /// Currently running <see cref="FsProcessor"/> on this thread.
    /// </summary>
    public static FsProcessor Current => s_current ?? throw new InvalidOperationException();

    /// <summary>
    /// Currently running <see cref="FsProcessor"/> on this thread, or null.
    /// </summary>
    public static FsProcessor? NullableCurrent => s_current;

    [ThreadStatic] internal static FsProcessor? s_current;

    /// <summary>
    /// Per-thread instance.
    /// </summary>
    public static FsProcessor FsInstance => s_fsInstance ??= new FsProcessor { LogReceiver = ConsoleLog.Default };

    [ThreadStatic] private static FsProcessor? s_fsInstance;

    /// <summary>
    /// Hint for segmented processor to perform dry-run.
    /// </summary>
    public bool Dry;

    /// <summary>
    /// ID of worker thread processor is using.
    /// </summary>
    public int WorkerId;

    /// <summary>
    /// Root input directory.
    /// </summary>
    public string InputRootDirectory = null!;

    /// <summary>
    /// Current input directory.
    /// </summary>
    public string InputDirectory = null!;

    /// <summary>
    /// Current input file.
    /// </summary>
    public string InputFile = null!;

    /// <summary>
    /// Selected file extension from input file, if available.
    /// </summary>
    public string? SelectedExtension;

    /// <summary>
    /// Root output directory.
    /// </summary>
    public string OutputRootDirectory = null!;

    /// <summary>
    /// Current output directory.
    /// </summary>
    public string OutputDirectory = null!;

    /// <summary>
    /// Current output file index.
    /// </summary>
    public int OutputCounter;

    /// <summary>
    /// Filesystem provider for this processor.
    /// </summary>
    public FileSystemSource FileSystem = null!;

    /// <summary>
    /// Set by subclasses to indicate if no more processors
    /// should be run on the current input file.
    /// </summary>
    /// <remarks>
    /// Not applicable in a multithreaded environment.
    /// </remarks>
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public bool Lock;

    /// <summary>
    /// Current file path.
    /// </summary>
    public FpPath FilePath => FpPath.GetFromString(Current.InputFile) ?? throw new InvalidOperationException();

    /// <summary>
    /// Current file path without extension.
    /// </summary>
    public FpPath FilePathNoExt => new(Path.GetFileNameWithoutExtension(Current.InputFile), Current.InputDirectory);

    /// <summary>
    /// Current file name.
    /// </summary>
    public string Name => Path.GetFileName(Current.InputFile);

    /// <summary>
    /// Current file name without extension.
    /// </summary>
    public string NameNoExt => Path.GetFileNameWithoutExtension(Current.InputFile);

    /// <summary>
    /// Current file name.
    /// </summary>
    public FpPath NamePath => FpPath.GetFromString(Name) ?? throw new InvalidOperationException();

    /// <summary>
    /// Current file name without extension.
    /// </summary>
    public FpPath NamePathNoExt => FpPath.GetFromString(NameNoExt) ?? throw new InvalidOperationException();

    /// <summary>
    /// Origin factory for this processor, if available.
    /// </summary>
    public FsProcessorFactory? Source;

    private bool _overrideProcess = true;
    private bool _overrideProcessSegmented = true;

    #endregion

    #region Main operation functions

    /// <summary>
    /// Prepares critical state for operation.
    /// </summary>
    /// <param name="fileSystem">Filesystem source.</param>
    /// <param name="inputRoot">Input root directory.</param>
    /// <param name="outputRoot">Output root directory.</param>
    /// <param name="file">Input file.</param>
    /// <param name="configuration">Additional configuration object.</param>
    /// <param name="workerId">Worker ID.</param>
    public void Prepare(FileSystemSource fileSystem, string inputRoot, string outputRoot, string file,
        ProcessorConfiguration? configuration = null, int workerId = 0)
    {
        Prepare(configuration);
        InputRootDirectory = fileSystem.NormalizePath(inputRoot);
        InputFile = fileSystem.NormalizePath(Path.Combine(InputRootDirectory, file));
        InputDirectory = Path.GetDirectoryName(InputFile) ?? throw new ArgumentException("File is root");
        OutputRootDirectory = fileSystem.NormalizePath(outputRoot);
        OutputDirectory = fileSystem.NormalizePath(Join(false,
            OutputRootDirectory, InputDirectory.Substring(InputRootDirectory.Length)));
        ValidateExtension(InputFile, out SelectedExtension);
        OutputCounter = 0;
        FileSystem = fileSystem;
        WorkerId = workerId;
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
    /// Validates extension based on <see cref="Source"/>.<see cref="FsProcessorFactory.Info"/>.<see cref="FsProcessorInfo.Extensions"/>.
    /// </summary>
    /// <param name="path">Path to check.</param>
    /// <param name="extension">Null or selected extension (may also be null if successful).</param>
    /// <returns>True if succeeded.</returns>
    public bool ValidateExtension(string path, out string? extension)
    {
        extension = null;
        if (Source?.Info.Extensions is not { } exts) return true;
        if (exts.Length == 0) return true;
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

    /// <summary>
    /// Initializes a new processor.
    /// </summary>
    /// <param name="args">Arguments.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>Created processor.</returns>
    public T Initialize<T>(string[]? args = null)
        where T : Processor, new()
    {
        T child = new();
        child.Prepare(new ProcessorConfiguration(Preload, Debug, Nop, LogReceiver, args ?? Array.Empty<string>()));
        return child;
    }

    /// <summary>
    /// Initializes a new file processor with an isolated filesystem containing the specified binary files.
    /// </summary>
    /// <param name="main">Main file.</param>
    /// <param name="args">Arguments.</param>
    /// <param name="additionalFiles">Additional files to pass to processor.</param>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>Created processor.</returns>
    public T Initialize<T>(BufferData<byte> main, string[]? args = null,
        IEnumerable<BufferData<byte>>? additionalFiles = null)
        where T : FsProcessor, new()
    {
        T child = new();
        IEnumerable<BufferData<byte>> seq = new[] { main };
        if (additionalFiles != null) seq = seq.Concat(additionalFiles);
        var layer1 = new SegmentedFileSystemSource(FileSystem, true, seq);
        child.Prepare(layer1, InputRootDirectory, OutputRootDirectory, main.BasePath, new ProcessorConfiguration(Preload, Debug, Nop, LogReceiver, args ?? Array.Empty<string>()));
        return child;
    }

    /// <summary>
    /// Processes current file.
    /// </summary>
    protected virtual void ProcessImpl() => _overrideProcess = false;

    /// <summary>
    /// Processes current file in parts.
    /// </summary>
    /// <returns>Generated outputs.</returns>
    protected virtual IEnumerable<Data> ProcessSegmentedImpl()
    {
        _overrideProcessSegmented = false;
        yield break;
    }

    /// <summary>
    /// Processes current file.
    /// </summary>
    public void Process()
    {
        SetupArgs();
        ShieldProcess();
        if (_overrideProcess) return;
        foreach (Data d in ShieldProcessSegmented())
        {
            if (Nop || d is MetaData && !Debug) continue;
            using Data data = d;
            using Stream stream =
                (FileSystem ?? throw new InvalidOperationException()).OpenWrite(
                    GenPath(data.GetExtension(), data.BasePath));
            data.WriteConvertedData(stream, data.DefaultFormat);
        }
    }

    /// <summary>
    /// Processes current file in parts.
    /// </summary>
    /// <returns>Generated outputs.</returns>
    public IEnumerable<Data> ProcessSegmented()
    {
        SetupArgs();
        foreach (Data entry in ShieldProcessSegmented()) yield return entry;
        if (_overrideProcessSegmented) yield break;
        FileSystemSource prevFs = FileSystem ?? throw new InvalidOperationException();
        SegmentedFileSystemSource fs = new(prevFs, false);
        FileSystem = fs;
        try
        {
            ShieldProcess();
            foreach ((string path, Stream stream) in fs)
                yield return new BufferData<byte>(path, GetMemory(stream));
        }
        finally
        {
            FileSystem = prevFs;
        }
    }

    /// <summary>
    /// Sets <see cref="Current"/>.
    /// </summary>
    public void ShieldUp() => s_current = this;

    /// <summary>
    /// Unsets <see cref="Current"/>.
    /// </summary>
    public static void ShieldDown() => s_current = null;

    /// <summary>
    /// Processes current file.
    /// </summary>
    protected void ShieldProcess()
    {
        ShieldUp();
        try
        {
            ProcessImpl();
        }
        finally
        {
            ShieldDown();
        }
    }

    /// <summary>
    /// Processes current file in parts.
    /// </summary>
    /// <returns>Generated outputs.</returns>
    protected IEnumerable<Data> ShieldProcessSegmented()
    {
        try
        {
            ShieldUp();
            using var enumerator = ProcessSegmentedImpl().GetEnumerator();
            ShieldDown();
            bool has;
            do
            {
                ShieldUp();
                try
                {
                    has = enumerator.MoveNext();
                }
                finally
                {
                    ShieldDown();
                }

                if (has)
                    yield return
#if NET6_0_OR_GREATER
                            enumerator.Current;
#else
                        enumerator.Current!;
#endif
            } while (has);
        }
        finally
        {
            ShieldDown();
        }
    }

    #endregion

    #region Factory utilities

    /// <summary>
    /// Creates a factory for the specified type, obtaining information from applied <see cref="FsProcessorInfoAttribute"/> if possible.
    /// </summary>
    /// <typeparam name="T">Processor type.</typeparam>
    /// <returns>Processor factory.</returns>
    public static FsProcessorFactory GetFsFactory<T>() where T : FsProcessor, new()
    {
        FsProcessorInfo? processorInfo = null;
        try
        {
            object[] attrs = typeof(T).GetCustomAttributes(typeof(FsProcessorInfoAttribute), true);
            if (attrs.FirstOrDefault() is FsProcessorInfoAttribute attr) processorInfo = attr.Info;
        }
        catch
        {
            // When reflection doesn't work, just fallback
        }

        return new GenericNewFsProcessorFactory<T>(processorInfo);
    }

    #endregion
}

// ReSharper disable InconsistentNaming
public partial class Scripting
{
    #region Properties

    /// <summary>
    /// Current file path.
    /// </summary>
    public static FpPath _file => FpPath.GetFromString(Current.InputFile) ?? throw new InvalidOperationException();

    /// <summary>
    /// Current file path without extension.
    /// </summary>
    public static FpPath _fileNoExt =>
        new(Path.GetFileNameWithoutExtension(Current.InputFile), Current.InputDirectory);

    /// <summary>
    /// Current file name.
    /// </summary>
    public static string _name => Path.GetFileName(Current.InputFile);

    /// <summary>
    /// Current file length.
    /// </summary>
    public static long _length => Current.InputLength;

    /// <summary>
    /// Current file name without extension.
    /// </summary>
    public static string _nameNoExt => Path.GetFileNameWithoutExtension(Current.InputFile);

    /// <summary>
    /// Current file name.
    /// </summary>
    public static FpPath _namePath => FpPath.GetFromString(_name) ?? throw new InvalidOperationException();

    /// <summary>
    /// Current file name without extension.
    /// </summary>
    public static FpPath _namePathNoExt =>
        FpPath.GetFromString(_nameNoExt) ?? throw new InvalidOperationException();

    #endregion

    #region Shield

    /// <summary>
    /// Sets <see cref="FsProcessor.Current"/>.
    /// </summary>
    /// <param name="preserve">If true, preserve existing value.</param>
    public static void shield(bool preserve = true)
    {
        if (NullableCurrent == null || !preserve)
            (NullableCurrent ?? FsInstance).ShieldUp();
    }

    /// <summary>
    /// Unsets <see cref="FsProcessor.Current"/>.
    /// </summary>
    public static void unshield() => ShieldDown();

    #endregion

    #region Logging

    /// <summary>
    /// Invokes logger with formatted string containing specified information log.
    /// </summary>
    /// <param name="log">Message.</param>
    public static void info(string log) => Current.LogInfo(log);

    /// <summary>
    /// Invokes logger with formatted string containing specified warning log.
    /// </summary>
    /// <param name="log">Message.</param>
    public static void warn(string log) => Current.LogWarn(log);

    /// <summary>
    /// Invokes logger with formatted string containing specified error log.
    /// </summary>
    /// <param name="log">Message.</param>
    public static void fail(string log) => Current.LogFail(log);

    #endregion
}
// ReSharper restore InconsistentNaming