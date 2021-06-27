using System;
using System.Collections.Generic;
using System.Linq;

namespace Fp
{
    /// <summary>
    /// Provides processor with additional sub-processing children.
    /// </summary>
    public class LayeredDataProcessor : DataProcessor
    {
        /// <summary>
        /// Children to use.
        /// </summary>
        protected virtual HashSet<ProcessorChild> Children { get; set; } = new();

        /// <summary>
        /// Current output.
        /// </summary>
        public HashSet<Data> Content { get; set; } = new();

        /// <inheritdoc />
        protected override IEnumerable<Data> ProcessData()
        {
            Content.Clear();
            ProcessLayered();
            foreach (var c in Children)
                if (c.Filter || c.Flag == null || Flags.Contains(c.Flag))
                    c.Run();
            return new List<Data>(Content);
        }

        /// <summary>
        /// Process using children.
        /// </summary>
        protected virtual void ProcessLayered()
        {
        }

        /// <summary>
        /// Register child of specified type.
        /// </summary>
        /// <typeparam name="TProcessorChild">Target type.</typeparam>
        public void UseChild<TProcessorChild>() where TProcessorChild : ProcessorChild, new()
        {
            if (Children.All(c => c is not TProcessorChild))
                Register(new TProcessorChild());
        }

        /// <summary>
        /// Register children of specified types.
        /// </summary>
        /// <param name="types">Target types.</param>
        /// <exception cref="ArgumentException">Thrown if failed to use <see cref="Activator.CreateInstance(Type)"/>.</exception>
        public void UseChild(params Type[] types)
        {
            foreach (var type in types)
                if (Children.All(c => !type.IsInstanceOfType(c)))
                    Register(Activator.CreateInstance(type) as ProcessorChild ?? throw new ArgumentException());
        }

        private void Register(ProcessorChild child)
        {
            child.Parent = this;
            Children.Add(child);
        }
    }

    /// <summary>
    /// Provides processor with additional sub-processing children using keyed lookup.
    /// </summary>
    public class LayeredDataProcessor<TKey, TValue> : DataProcessor where TKey : notnull
    {
        /// <summary>
        /// Children to use.
        /// </summary>
        protected virtual HashSet<ProcessorChild<TKey, TValue>> Children { get; set; } = new();

        /// <summary>
        /// Current output.
        /// </summary>
        public HashSet<Data> Content { get; set; } = new();

        /// <summary>
        /// Buffer lookup.
        /// </summary>
        public Dictionary<TKey, TValue> Lookup { get; set; } = new();

        /// <inheritdoc />
        protected override IEnumerable<Data> ProcessData()
        {
            Content.Clear();
            Lookup.Clear();
            ProcessLayered();
            foreach (var c in Children)
                if (c.Filter || c.Flag == null || Flags.Contains(c.Flag))
                    c.Run();
            return new List<Data>(Content);
        }

        /// <summary>
        /// Process using children.
        /// </summary>
        protected virtual void ProcessLayered()
        {
        }

        /// <summary>
        /// Register child of specified type.
        /// </summary>
        /// <typeparam name="TProcessorChild">Target type.</typeparam>
        public void UseChild<TProcessorChild>() where TProcessorChild : ProcessorChild<TKey, TValue>, new()
        {
            if (Children.All(c => c is not TProcessorChild))
                Register(new TProcessorChild());
        }

        /// <summary>
        /// Register children of specified types.
        /// </summary>
        /// <param name="types">Target types.</param>
        /// <exception cref="ArgumentException">Thrown if failed to use <see cref="Activator.CreateInstance(Type)"/>.</exception>
        public void UseChild(params Type[] types)
        {
            foreach (var type in types)
                if (Children.All(c => !type.IsInstanceOfType(c)))
                    Register(Activator.CreateInstance(type) as ProcessorChild<TKey, TValue> ??
                             throw new ArgumentException());
        }

        private void Register(ProcessorChild<TKey, TValue> child)
        {
            child.Parent = this;
            Children.Add(child);
        }
    }
}
