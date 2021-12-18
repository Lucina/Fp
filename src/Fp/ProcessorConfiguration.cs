using System.Collections.Generic;

namespace Fp
{
    /// <summary>
    /// Configuration for batch-processing fs with processors.
    /// </summary>
    public record ProcessorConfiguration
    {
        /// <summary>
        /// Creates configuration.
        /// </summary>
        /// <param name="preload">Whether to read all streams to memory.</param>
        /// <param name="debug">Whether to enable <see cref="Processor.Debug"/></param>
        /// <param name="nop">Whether to disable outputs.</param>
        /// <param name="logReceiver">Log writer.</param>
        /// <param name="args">Arguments.</param>
        public ProcessorConfiguration(bool preload, bool debug, bool nop, ILogReceiver logReceiver, IReadOnlyList<string> args)
        {
            Preload = preload;
            Debug = debug;
            Nop = nop;
            LogReceiver = logReceiver;
            Args = args;
        }

        /// <summary>
        /// Whether to read all streams to memory.
        /// </summary>
        public bool Preload { get; init; }

        /// <summary>
        /// Whether to enable <see cref="Processor.Debug"/>.
        /// </summary>
        public bool Debug { get; init; }

        /// <summary>
        /// Whether to disable outputs.
        /// </summary>
        public bool Nop { get; init; }

        /// <summary>
        /// Log writer.
        /// </summary>
        public ILogReceiver LogReceiver { get; init; }

        /// <summary>
        /// Arguments.
        /// </summary>
        public IReadOnlyList<string> Args { get; init; }
    }
}
