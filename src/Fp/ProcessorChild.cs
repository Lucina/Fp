using System.Collections.Generic;

namespace Fp
{
    /// <summary>
    /// Represents a child for <see cref="LayeredDataProcessor"/>.
    /// </summary>
    public class ProcessorChild
    {
        /// <summary>
        /// Parent processor.
        /// </summary>
        public LayeredDataProcessor Parent { get; set; } = null!;

        /// <summary>
        /// Required flag for using child.
        /// </summary>
        public virtual string? Flag => null;

        /// <summary>
        /// Filter condition for using child.
        /// </summary>
        public virtual bool Filter => true;

        /// <summary>
        /// Current file name.
        /// </summary>
        public string Name => Parent.Name;

        /// <summary>
        /// Current file name.
        /// </summary>
        public FpPath NamePath => Parent.NamePath;

        /// <summary>
        /// Current file name without extension.
        /// </summary>
        public string NameNoExt => Parent.NameNoExt;

        /// <summary>
        /// Current file name without extension.
        /// </summary>
        public FpPath NamePathNoExt => Parent.NamePathNoExt;

        /// <summary>
        /// Input flags.
        /// </summary>
        public HashSet<string> Flags => Parent.Flags;

        /// <summary>
        /// Input options.
        /// </summary>
        public Dictionary<string, string> Opts => Parent.Opts;

        /// <summary>
        /// Input positional arguments.
        /// </summary>
        public List<string> PosArgs => Parent.PosArgs;

        /// <summary>
        /// Current output content for processor.
        /// </summary>
        public HashSet<Data> Content => Parent.Content;

        /// <summary>
        /// Executes.
        /// </summary>
        public virtual void Run()
        {
        }
    }

    /// <summary>
    /// Represents a child for <see cref="LayeredDataProcessor{TKey,TValue}"/>.
    /// </summary>
    public class ProcessorChild<TKey, TValue> where TKey : notnull
    {
        /// <summary>
        /// Parent processor.
        /// </summary>
        public LayeredDataProcessor<TKey, TValue> Parent { get; set; } = null!;


        /// <summary>
        /// Required flag for using child.
        /// </summary>
        public virtual string? Flag => null;

        /// <summary>
        /// Filter condition for using child.
        /// </summary>
        public virtual bool Filter => true;

        /// <summary>
        /// Current file name.
        /// </summary>
        public string Name => Parent.Name;

        /// <summary>
        /// Current file name.
        /// </summary>
        public FpPath NamePath => Parent.NamePath;

        /// <summary>
        /// Current file name without extension.
        /// </summary>
        public string NameNoExt => Parent.NameNoExt;

        /// <summary>
        /// Current file name without extension.
        /// </summary>
        public FpPath NamePathNoExt => Parent.NamePathNoExt;

        /// <summary>
        /// Input flags.
        /// </summary>
        public HashSet<string> Flags => Parent.Flags;

        /// <summary>
        /// Input options.
        /// </summary>
        public Dictionary<string, string> Opts => Parent.Opts;

        /// <summary>
        /// Input positional arguments.
        /// </summary>
        public List<string> PosArgs => Parent.PosArgs;

        /// <summary>
        /// Current output content for processor.
        /// </summary>
        public HashSet<Data> Content => Parent.Content;

        /// <summary>
        /// Current output content for processor.
        /// </summary>
        public Dictionary<TKey, TValue> Lookup => Parent.Lookup;

        /// <summary>
        /// Executes.
        /// </summary>
        public virtual void Run()
        {
        }
    }
}
