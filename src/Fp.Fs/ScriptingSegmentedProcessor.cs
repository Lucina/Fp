using System;
using System.Collections.Generic;

namespace Fp;

/// <summary>
/// Represents a processor designed for scripting.
/// </summary>
internal sealed class ScriptingSegmentedProcessor : FsProcessor
{
    private readonly Func<IEnumerable<Data>> _func;

    /// <summary>
    /// Creates a new instance of <see cref="ScriptingSegmentedProcessor"/>.
    /// </summary>
    /// <param name="func">Func creating enumerable.</param>
    public ScriptingSegmentedProcessor(Func<IEnumerable<Data>> func)
    {
        _func = func;
    }

    /// <inheritdoc />
    protected override IEnumerable<Data> ProcessSegmentedImpl()
    {
        OpenMainFile();
        return _func();
    }
}