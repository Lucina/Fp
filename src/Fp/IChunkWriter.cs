using System;

namespace Fp;

/// <summary>
/// Receives log chunks.
/// </summary>
public interface IChunkWriter
{
    /// <summary>
    /// Logs raw chunk.
    /// </summary>
    /// <param name="log">Log.</param>
    /// <param name="tail">If true, final chunk.</param>
    /// <param name="color">Text color.</param>
    void WriteChunk(string log, bool tail, ConsoleColor? color = null);
}
