using System;

namespace Fp;

/// <summary>
/// Log level.
/// </summary>
[Flags]
public enum LogLevel
{
    /// <summary>
    /// Information.
    /// </summary>
    Information = 1,

    /// <summary>
    /// Warning.
    /// </summary>
    Warning = 1 << 1,

    /// <summary>
    /// Error.
    /// </summary>
    Error = 1 << 2
}
