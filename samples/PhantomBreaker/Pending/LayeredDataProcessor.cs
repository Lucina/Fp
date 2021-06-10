using System;
using System.Collections.Generic;
using System.Linq;
using Fp;

public class LayeredDataProcessor : DataProcessor
{
    protected virtual string[] OptKeys => Array.Empty<string>();
    protected virtual HashSet<ProcessorChild> Children { get; set; } = new();
    public HashSet<string> Flags { get; set; } = null!;
    public Dictionary<string, string> Opts { get; set; } = null!;
    public List<string> PosArgs { get; set; } = null!;
    public HashSet<Data> Content { get; set; } = new();

    // TODO DataProcessor should seal ProcessImpl as well
    protected override IEnumerable<Data> ProcessData()
    {
        Content.Clear();
        (Flags, Opts, PosArgs) = Args.IsolateFlags(OptKeys);
        ProcessLayered();
        foreach (var c in Children)
            if (c.Filter || c.Flag == null || Flags.Contains(c.Flag))
                c.Run();
        return new List<Data>(Content);
    }

    protected virtual void ProcessLayered()
    {
    }

    public void UseChild<TProcessorChild>() where TProcessorChild : ProcessorChild, new()
    {
        if (Children.All(c => c is not TProcessorChild))
            Register(new TProcessorChild());
    }

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

public class LayeredDataProcessor<TKey> : DataProcessor where TKey : notnull
{
    protected virtual string[] OptKeys => Array.Empty<string>();
    protected virtual HashSet<ProcessorChild<TKey>> Children { get; set; } = new();
    public HashSet<string> Flags { get; set; } = null!;
    public Dictionary<string, string> Opts { get; set; } = null!;
    public List<string> PosArgs { get; set; } = null!;
    public HashSet<Data> Content { get; set; } = new();
    public Dictionary<TKey, Memory<byte>> Lookup { get; set; } = new();

    protected override IEnumerable<Data> ProcessData()
    {
        Content.Clear();
        Lookup.Clear();
        (Flags, Opts, PosArgs) = Args.IsolateFlags(OptKeys);
        ProcessLayered();
        foreach (var c in Children)
            if (c.Filter || c.Flag == null || Flags.Contains(c.Flag))
                c.Run();
        return new List<Data>(Content);
    }

    protected virtual void ProcessLayered()
    {
    }

    public void UseChild<TProcessorChild>() where TProcessorChild : ProcessorChild<TKey>, new()
    {
        if (Children.All(c => c is not TProcessorChild))
            Register(new TProcessorChild());
    }

    public void UseChild(params Type[] types)
    {
        foreach (var type in types)
            if (Children.All(c => !type.IsInstanceOfType(c)))
                Register(Activator.CreateInstance(type) as ProcessorChild<TKey> ?? throw new ArgumentException());
    }

    private void Register(ProcessorChild<TKey> child)
    {
        child.Parent = this;
        Children.Add(child);
    }
}
