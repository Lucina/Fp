using System;

namespace Fp
{
    /// <summary>
    /// Receives logs
    /// </summary>
    public interface ILogReceiver
    {
        /// <summary>
        /// Logs raw chunk
        /// </summary>
        /// <param name="log">Log</param>
        /// <param name="tail">If true, final chunk</param>
        /// <param name="color">Text color</param>
        void LogChunk(string log, bool tail, ConsoleColor? color = null);

        /// <summary>
        /// Logs information
        /// </summary>
        /// <param name="log">Log</param>
        /// <param name="color">Text color</param>
        void LogInformation(string log, ConsoleColor? color = null);

        /// <summary>
        /// Logs warning
        /// </summary>
        /// <param name="log">Log</param>
        /// <param name="color">Text color</param>
        void LogWarning(string log, ConsoleColor? color = null);

        /// <summary>
        /// Logs error
        /// </summary>
        /// <param name="log">Log</param>
        /// <param name="color">Text color</param>
        void LogError(string log, ConsoleColor? color = null);

        /// <summary>
        /// Log level
        /// </summary>
        [Flags]
        public enum LogLevel
        {
            /// <summary>
            /// Information
            /// </summary>
            Information = 1,

            /// <summary>
            /// Warning
            /// </summary>
            Warning = 1 << 1,

            /// <summary>
            /// Error
            /// </summary>
            Error = 1 << 2
        }
    }
}
