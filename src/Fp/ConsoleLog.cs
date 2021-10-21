using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static Fp.ILogReceiver;

namespace Fp
{
    /// <summary>
    /// Provides basic logging to console output.
    /// </summary>
    public class ConsoleLog : ILogReceiver
    {
        private static class WindowsAnsi
        {
            private const int STD_OUTPUT_HANDLE = -11;
            private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
            private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

            [DllImport("kernel32")]
            private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

            [DllImport("kernel32")]
            private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

            [DllImport("kernel32")]
            private static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32")]
            private static extern uint GetLastError();

            internal static void TryInit()
            {
                try
                {
                    // https://gist.github.com/tomzorz/6142d69852f831fb5393654c90a1f22e
                    var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
                    if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
                    {
                        Console.WriteLine("failed to get output console mode");
                        return;
                    }

                    outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
                    if (!SetConsoleMode(iStdOut, outConsoleMode))
                    {
                        Console.WriteLine($"failed to set output console mode, error code: {GetLastError()}");
                    }
                }
                catch
                {
                    // Ignored
                }
            }
        }

        static ConsoleLog()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) WindowsAnsi.TryInit();
        }


        /// <summary>
        /// Supported color sequences
        /// </summary>
        public static readonly Dictionary<ConsoleColor, string> Sequences = new()
        {
            /*{Color.Black, "\u001b[30;1m"},*/
            { ConsoleColor.Red, "\u001b[31;1m" },
            { ConsoleColor.Green, "\u001b[32;1m" },
            { ConsoleColor.Yellow, "\u001b[33;1m" },
            { ConsoleColor.Blue, "\u001b[34;1m" },
            { ConsoleColor.Magenta, "\u001b[35;1m" },
            { ConsoleColor.Cyan, "\u001b[36;1m" },
            { ConsoleColor.White, "\u001b[37;1m" },
        };

        /// <summary>
        /// Supported colors
        /// </summary>
        public static readonly IReadOnlyList<ConsoleColor> Colors = new List<ConsoleColor>
        {
            /*Color.Black,*/
            ConsoleColor.Red,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Blue,
            ConsoleColor.Magenta,
            ConsoleColor.Cyan,
            /*Color.White*/
        };

        /// <summary>
        /// Default instance with all logs enabled
        /// </summary>
        public static readonly ConsoleLog Default = new(new Config());

        /// <summary>
        /// Stores configuration properties for <see cref="ConsoleLog"/>.
        /// </summary>
        public class Config
        {
            /// <summary>
            /// Enabled log levels.
            /// </summary>
            public LogLevel EnabledLevels { get; set; }

            /// <summary>
            /// Creates a new instance of <see cref="Config"/>.
            /// </summary>
            public Config()
            {
                EnabledLevels = LogLevel.Information | LogLevel.Warning | LogLevel.Error;
            }

            /// <summary>
            /// Creates a new instance of <see cref="Config"/> with the specified log levels.
            /// </summary>
            /// <param name="enabledLevels">Enabled log levels.</param>
            public Config(LogLevel enabledLevels)
            {
                EnabledLevels = enabledLevels;
            }
        }


        private readonly Config _config;

        /// <summary>
        /// Creates an instance of <see cref="ConsoleLog"/> with the specified configuration.
        /// </summary>
        /// <param name="config">Log configuration.</param>
        public ConsoleLog(Config config)
        {
            _config = config;
        }

        private void Log(LogLevel logLevel, string log, bool reset, ConsoleColor? color)
        {
            if (!IsEnabled(logLevel)) return;
            if (reset) Console.Write("\u001b[0m");
            if (color != null)
            {
                if (!Sequences.TryGetValue(color.Value, out var c)) c = Sequences[ConsoleColor.White];
                Console.Write(c);
            }

            Console.Write(log);
        }

        private bool IsEnabled(LogLevel logLevel) => (logLevel & _config.EnabledLevels) != 0;

        /// <inheritdoc />
        public void LogChunk(string log, bool tail, ConsoleColor? color = null) =>
            Log(LogLevel.Information, log, tail, color);

        /// <inheritdoc />
        public void LogInformation(string log, ConsoleColor? color = null)
        {
            Log(LogLevel.Information, log, false, color);
            Log(LogLevel.Information, "\n", false, color);
        }

        /// <inheritdoc />
        public void LogWarning(string log, ConsoleColor? color = null)
        {
            Log(LogLevel.Warning, log, false, color);
            Log(LogLevel.Warning, "\n", false, color);
        }

        /// <inheritdoc />
        public void LogError(string log, ConsoleColor? color = null)
        {
            Log(LogLevel.Error, log, false, color);
            Log(LogLevel.Error, "\n", false, color);
        }
    }
}
