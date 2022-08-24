using System;

namespace Fp;

/// <summary>
/// Receives logs.
/// </summary>
public interface ILogWriter : IChunkWriter
{
    /// <summary>
    /// Logs information.
    /// </summary>
    /// <param name="log">Log.</param>
    /// <param name="color">Text color.</param>
    void WriteInformation(string log, ConsoleColor? color = null);

    /// <summary>
    /// Logs warning.
    /// </summary>
    /// <param name="log">Log.</param>
    /// <param name="color">Text color.</param>
    void WriteWarning(string log, ConsoleColor? color = null);

    /// <summary>
    /// Logs error.
    /// </summary>
    /// <param name="log">Log.</param>
    /// <param name="color">Text color.</param>
    void WriteError(string log, ConsoleColor? color = null);
}
