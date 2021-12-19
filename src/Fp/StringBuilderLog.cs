using System;
using System.Text;

namespace Fp;

/// <summary>
/// Provides basic logging to <see cref="StringBuilder"/>.
/// </summary>
public sealed class StringBuilderLog : ILogReceiver
{
    /// <summary>
    /// Target builder.
    /// </summary>
    public readonly StringBuilder StringBuilder = new();

    /// <inheritdoc />
    public void LogChunk(string log, bool tail, ConsoleColor? color = null)
    {
        StringBuilder.Append(log);
        if (tail) StringBuilder.Append('\n');
    }

    /// <inheritdoc />
    public void LogInformation(string log, ConsoleColor? color = null)
    {
        StringBuilder.Append(log).Append('\n');
    }

    /// <inheritdoc />
    public void LogWarning(string log, ConsoleColor? color = null)
    {
        StringBuilder.Append(log).Append('\n');
    }

    /// <inheritdoc />
    public void LogError(string log, ConsoleColor? color = null)
    {
        StringBuilder.Append(log).Append('\n');
    }
}