using System;
using System.Collections.Generic;

namespace Fp.Fs;

/// <summary>
/// <see cref="FsProcessor"/> wrapper for <see cref="FormatSingleProcessor"/>.
/// </summary>
public class FormatSingleProcessorFsWrapper : FsProcessor
{
    private readonly FormatSingleProcessor _baseProcessor;

    /// <summary>
    /// Creates a new instance of <see cref="FormatSingleProcessorFsWrapper"/> with the specified base processor.
    /// </summary>
    /// <param name="baseProcessor">Base processor.</param>
    public FormatSingleProcessorFsWrapper(FormatSingleProcessor baseProcessor)
    {
        _baseProcessor = baseProcessor;
    }

    /// <inheritdoc />
    protected override IEnumerable<Data> ProcessSegmentedImpl()
    {
        var input = OpenMainFile();
        _baseProcessor.Prepare(input, Name, Configuration);
        return _baseProcessor.TryProcess(out Data? data) ? new[] { data! } : Array.Empty<Data>();
    }
}

/// <summary>
/// <see cref="FsProcessor"/> wrapper for <see cref="FormatSingleProcessor"/>.
/// </summary>
public class FormatSingleProcessorFsWrapper<T> : FormatSingleProcessorFsWrapper where T : FormatSingleProcessor, new()
{
    /// <summary>
    /// Creates a new instance of <see cref="FormatSingleProcessorFsWrapper{T}"/>.
    /// </summary>
    public FormatSingleProcessorFsWrapper() : base(new T())
    {
    }
}
