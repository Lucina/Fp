using System.Collections.Generic;

namespace Fp;

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
        var input = OpenMainFile();
        _baseProcessor.Prepare(input, Name, Configuration);
        return _baseProcessor.Process();
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
