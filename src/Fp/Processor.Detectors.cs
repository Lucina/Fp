using System;
using System.Collections.Generic;
using System.Linq;
using static Fp.Detector;
using static Fp.Processor;

namespace Fp;

#region General util

// ReSharper disable InconsistentNaming
public partial class Processor
{
    /// <summary>
    /// Empty enumerable of data.
    /// </summary>
    public static readonly IEnumerable<Data> Nothing = Enumerable.Empty<Data>();

    /// <summary>
    /// Warns version value as unsupported.
    /// </summary>
    /// <param name="value">Version value.</param>
    /// <returns>Empty enumerable.</returns>
    public IEnumerable<Data> UnsupportedVersion(long value)
    {
        LogWarn($"Version {value:X2} unsupported");
        return Nothing;
    }

    /// <summary>
    /// Warns version value as unsupported.
    /// </summary>
    /// <param name="value">Version value.</param>
    /// <returns>Empty enumerable.</returns>
    public IEnumerable<Data> UnsupportedVersion(ulong value)
    {
        LogWarn($"Version {value:X2} unsupported");
        return Nothing;
    }

    /// <summary>
    /// Warns extension value as unsupported.
    /// </summary>
    /// <param name="extension">Extension value.</param>
    /// <returns>Empty enumerable.</returns>
    public IEnumerable<Data> UnsupportedExtension(string? extension)
    {
        LogWarn(extension != null ? $"Extension {extension} unsupported" : "Empty extension unsupported");
        return Nothing;
    }
}

#endregion


public partial class FpUtil
{
    #region Extension detectors

    #region Magic

    /// <summary>
    /// Detects based on magic value.
    /// </summary>
    /// <param name="magicValue">Magic value.</param>
    /// <param name="value">Target value.</param>
    /// <param name="offset">Base offset of magic.</param>
    /// <returns>Detector.</returns>
    public static Detector __(string magicValue, string value, int offset = 0) =>
        __(null, magicValue, value, offset);

    /// <summary>
    /// Detects based on magic value.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <param name="magicValue">Magic value.</param>
    /// <param name="value">Target value.</param>
    /// <param name="offset">Base offset of magic.</param>
    /// <returns>Detector.</returns>
    public static Detector __(this object source, string magicValue, string value, int offset = 0) =>
        __(null, magicValue, value, offset, source);

    /// <summary>
    /// Detects based on magic value.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="magicValue">Magic value.</param>
    /// <param name="value">Target value.</param>
    /// <param name="offset">Base offset of magic.</param>
    /// <param name="source">Data source.</param>
    /// <returns>Detector.</returns>
    public static Detector __(this Detector? detector, string magicValue, string value, int offset = 0,
        object? source = null) =>
        new(detector, ResolveSource(detector, source), o => o switch
        {
            Processor p when p.HasMagic(magicValue, offset) => value,
            ReadOnlyMemory<byte> m when HasMagic(m.Span, magicValue, offset) => value,
            _ => null
        });

    #endregion

    #region Fallback

    /// <summary>
    /// Fallback to value.
    /// </summary>
    /// <param name="detector">Existing detector.</param>
    /// <param name="value">Fallback value.</param>
    /// <returns>Detector.</returns>
    public static Fallback ___(this Detector detector, string value) => new(detector, value);

    #endregion

    #endregion
}


// ReSharper restore InconsistentNaming

/// <summary>
/// Represents a detector.
/// </summary>
public record Detector(Detector? Prev, object Source, Func<object?, string?> DetectionFunction)
{
    /// <summary>
    /// Detects from this detector or any that preceded it.
    /// </summary>
    /// <returns>Detected value.</returns>
    public static implicit operator string?(Detector detector)
    {
        string? v = detector.DetectionFunction(CoerceToROM(detector.Source));
        if (v != null || detector.Prev == null) return v;
        return detector.Prev;
    }

    /// <summary>
    /// Detects from this detector or any that preceded it using late-bound source.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detected value.</returns>
    public string? Detect(object? source = null)
    {
        string? v = DetectionFunction(CoerceToROM(ResolveSource(this, source, Source)));
        if (v != null || Prev == null) return v;
        return Prev.Detect(source);
    }

    /// <summary>
    /// Resolves source.
    /// </summary>
    /// <param name="detector">Previous detector if available.</param>
    /// <param name="source">Source if available.</param>
    /// <returns>Resolved source.</returns>
    public static object ResolveSource(Detector? detector, object? source) =>
        source ?? detector?.Source ?? Nothing;

    /// <summary>
    /// Resolves source.
    /// </summary>
    /// <param name="detector">Previous detector if available.</param>
    /// <param name="source">Source if available.</param>
    /// <param name="originalSource">Original source.</param>
    /// <returns>Resolved source.</returns>
    public static object ResolveSource(Detector? detector, object? source, object? originalSource) =>
        source ?? originalSource ?? detector?.Source ?? Nothing;

    private static object? CoerceToROM(object? value) => value switch
    {
        byte[] b => (ReadOnlyMemory<byte>)b,
        ArraySegment<byte> b => (ReadOnlyMemory<byte>)b,
        Memory<byte> b => (ReadOnlyMemory<byte>)b,
        ReadOnlyMemory<byte> b => b,
        _ => value
    };
}

/// <summary>
/// Represents a fallback detector.
/// </summary>
public record Fallback(Detector Prev, string Value)
{
    /// <summary>
    /// Detects from previous detectors or fallback to value.
    /// </summary>
    /// <returns>Detected value.</returns>
    public static implicit operator string(Fallback detector)
    {
        string? v = detector.Prev;
        return v ?? detector.Value;
    }

    /// <summary>
    /// Detects from this detector or any that preceded it using late-bound source.
    /// </summary>
    /// <param name="source">Data source.</param>
    /// <returns>Detected value.</returns>
    public string Detect(object? source = null)
    {
        string? v = Prev.Detect(source);
        return v ?? Value;
    }
}
