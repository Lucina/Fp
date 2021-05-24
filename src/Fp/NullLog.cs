using System;

namespace Fp
{
    /// <summary>
    /// Provides dummy log receiver.
    /// </summary>
    public sealed class NullLog : ILogReceiver
    {
        /// <summary>
        /// Static instance
        /// </summary>
        public static readonly NullLog Instance = new();

        /// <inheritdoc />
        public void LogChunk(string log, bool tail, ConsoleColor? color = null)
        {
        }

        /// <inheritdoc />
        public void LogInformation(string log, ConsoleColor? color = null)
        {
        }

        /// <inheritdoc />
        public void LogWarning(string log, ConsoleColor? color = null)
        {
        }

        /// <inheritdoc />
        public void LogError(string log, ConsoleColor? color = null)
        {
        }
    }
}
