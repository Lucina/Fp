using System;

namespace Fp;

/// <summary>
/// Provides dummy log receiver.
/// </summary>
public sealed class NullLog : ILogWriter
{
    /// <summary>
    /// Static instance.
    /// </summary>
    public static readonly NullLog Instance = new();

    /// <inheritdoc />
    public void WriteChunk(string log, bool tail, ConsoleColor? color = null)
    {
    }

    /// <inheritdoc />
    public void WriteInformation(string log, ConsoleColor? color = null)
    {
    }

    /// <inheritdoc />
    public void WriteWarning(string log, ConsoleColor? color = null)
    {
    }

    /// <inheritdoc />
    public void WriteError(string log, ConsoleColor? color = null)
    {
    }
}
