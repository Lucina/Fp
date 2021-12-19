using System;

namespace Fp;

/// <summary>
/// Marks unknown mapping with <see cref="NotSupportedException"/>.
/// </summary>
/// <typeparam name="T">Type.</typeparam>
public static class Unknown<T>
{
    /// <summary>
    /// Marks unknown mapping with <see cref="NotSupportedException"/>.
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public static T Value => throw new NotSupportedException($"Unsupported value for {typeof(T)}");

    /// <summary>
    /// Marks unknown mapping with <see cref="NotSupportedException"/>.
    /// </summary>
    /// <param name="source">Source object.</param>
    /// <returns>Nothing (exception always thrown).</returns>
    /// <exception cref="NotSupportedException"></exception>
    public static T FromKey(object source) =>
        throw new NotSupportedException($"Unsupported value {source} for {typeof(T)}");
}

/// <summary>
/// Marks unknown mapping with <see cref="NotSupportedException"/>.
/// </summary>
/// <typeparam name="T">Type.</typeparam>
/// <typeparam name="TOut">Result type.</typeparam>
public static class Unknown<T, TOut>
{
    /// <summary>
    /// Marks unknown mapping with <see cref="NotSupportedException"/>.
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public static T Value => throw new NotSupportedException($"Unsupported value for {typeof(T)}");

    /// <summary>
    ///Marks unknown mapping with <see cref="NotSupportedException"/>.
    /// </summary>
    /// <param name="source">Source object.</param>
    /// <returns>Nothing (exception always thrown).</returns>
    /// <exception cref="NotSupportedException"></exception>
    public static TOut FromKey(object source) =>
        throw new NotSupportedException($"Unsupported value {source} for {typeof(T)}");
}
