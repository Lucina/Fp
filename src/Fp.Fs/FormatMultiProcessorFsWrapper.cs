using System.Collections.Generic;

namespace Fp.Fs;

/// <summary>
/// <see cref="FsProcessor"/> wrapper for <see cref="FormatMultiProcessor"/>.
/// </summary>
public class FormatMultiProcessorFsWrapper : FsProcessor
{
    private readonly FormatMultiProcessor _baseProcessor;

    /// <summary>
    /// Creates a new instance of <see cref="FormatMultiProcessorFsWrapper"/> with the specified base processor.
    /// </summary>
    /// <param name="baseProcessor">Base processor.</param>
    protected FormatMultiProcessorFsWrapper(FormatMultiProcessor baseProcessor)
    {
        _baseProcessor = baseProcessor;
    }

    /// <inheritdoc />
    protected override IEnumerable<Data> ProcessSegmentedImpl()
    {
        _baseProcessor.Info = Info;
        var input = OpenMainFile();
        _baseProcessor.Prepare(input, Name, Configuration);
        return _baseProcessor.Process();
    }

    /// <inheritdoc />
    public override void Cleanup(bool warn = false)
    {
        base.Cleanup(warn);
        _baseProcessor.Cleanup(warn);
    }
}

/// <summary>
/// <see cref="FsProcessor"/> wrapper for <see cref="FormatMultiProcessor"/>.
/// </summary>
public class FormatMultiProcessorFsWrapper<T> : FormatMultiProcessorFsWrapper where T : FormatMultiProcessor, new()
{
    /// <summary>
    /// Creates a new instance of <see cref="FormatMultiProcessorFsWrapper{T}"/>.
    /// </summary>
    public FormatMultiProcessorFsWrapper() : base(new T())
    {
    }
}
