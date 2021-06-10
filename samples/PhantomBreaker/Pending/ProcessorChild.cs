using System;
using System.Collections.Generic;
using Fp;

public class ProcessorChild
{
    public LayeredDataProcessor Parent { get; set; } = null!;
    public virtual string? Flag => null;
    public virtual bool Filter => true;
    public string Name => Parent.Name;
    public FpPath NamePath => Parent.NamePath;
    public string NameNoExt => Parent.NameNoExt;
    public FpPath NamePathNoExt => Parent.NamePathNoExt;
    public HashSet<string> Flags => Parent.Flags;
    public Dictionary<string, string> Opts => Parent.Opts;
    public List<string> PosArgs => Parent.PosArgs;
    public HashSet<Data> Content => Parent.Content;

    public virtual void Run()
    {
    }
}

public class ProcessorChild<TKey> where TKey : notnull
{
    public LayeredDataProcessor<TKey> Parent { get; set; } = null!;
    public virtual string? Flag => null;
    public virtual bool Filter => true;
    public string Name => Parent.Name;
    public FpPath NamePath => Parent.NamePath;
    public string NameNoExt => Parent.NameNoExt;
    public FpPath NamePathNoExt => Parent.NamePathNoExt;
    public HashSet<string> Flags => Parent.Flags;
    public Dictionary<string, string> Opts => Parent.Opts;
    public List<string> PosArgs => Parent.PosArgs;
    public HashSet<Data> Content => Parent.Content;
    public Dictionary<TKey, Memory<byte>> Lookup => Parent.Lookup;

    public virtual void Run()
    {
    }
}
