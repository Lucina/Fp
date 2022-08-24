using System;
using System.Text;

namespace Fp;

/// <summary>
/// Provides basic logging to <see cref="StringBuilder"/>.
/// </summary>
public sealed class StringBuilderLog : ILogReceiver
{
    /// <summary>
    /// Newline delimiter for terminating log lines.
    /// </summary>
    /// <remarks>
    /// This property does not apply within log bodies, it only affects the terminating sequence for messages.
    /// By default, this coresponds to <see cref="Environment.NewLine"/>.
    /// </remarks>
    public string Delimiter { get; set; }

    /// <summary>
    /// Creates an instance of <see cref="StringBuilderLog"/>.
    /// </summary>
    public StringBuilderLog()
    {
        Delimiter = Environment.NewLine;
    }

    /// <summary>
    /// Creates an instance of <see cref="StringBuilderLog"/> with the specified line delimiter.
    /// </summary>
    /// <param name="delimiter">Newline delimiter for terminating log lines.</param>
    public StringBuilderLog(string delimiter)
    {
        Delimiter = delimiter;
    }

    /// <summary>
    /// Target builder.
    /// </summary>
    public readonly StringBuilder StringBuilder = new();

    /// <inheritdoc />
    public void LogChunk(string log, bool tail, ConsoleColor? color = null)
    {
        StringBuilder.Append(log);
        if (tail) StringBuilder.Append(Delimiter);
    }

    /// <inheritdoc />
    public void LogInformation(string log, ConsoleColor? color = null)
    {
        StringBuilder.Append(log).Append(Delimiter);
    }

    /// <inheritdoc />
    public void LogWarning(string log, ConsoleColor? color = null)
    {
        StringBuilder.Append(log).Append(Delimiter);
    }

    /// <inheritdoc />
    public void LogError(string log, ConsoleColor? color = null)
    {
        StringBuilder.Append(log).Append(Delimiter);
    }

    /// <summary>
    /// Gets the log content.
    /// </summary>
    /// <returns>Log content.</returns>
    public string GetContent() => StringBuilder.ToString();

    /// <summary>
    /// Clears log content.
    /// </summary>
    public void Clear() => StringBuilder.Clear();
}
