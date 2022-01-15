using System;
using System.Collections.Concurrent;

namespace Fp;

/// <summary>
/// Manage singleton instances of new() types.
/// </summary>
public static class One
{
    /// <summary>
    /// Clears all existing singletons.
    /// </summary>
    public static void ClearAll()
    {
        while (s_clearActions.TryTake(out Action? action))
            action();
    }

    private static readonly ConcurrentBag<Action> s_clearActions = new();

    internal static void AddClearAction(Action clearAction) => s_clearActions.Add(clearAction);
}

/// <summary>
/// Manage singleton instances of new() types.
/// </summary>
/// <typeparam name="T">Type.</typeparam>
public static class One<T> where T : new()
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static T Value => s_v ??= CreateAndRegister();

    private static T? s_v;

    private static T CreateAndRegister()
    {
        T t = new();
        One.AddClearAction(Clear);
        return t;
    }

    /// <summary>
    /// Clears existing singleton instance.
    /// </summary>
    public static void Clear() => s_v = default;
}
