namespace Fp;

/// <summary>
/// Manage singleton instances of new() types.
/// </summary>
/// <typeparam name="T">Type.</typeparam>
public static class One<T> where T : new()
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static T Value => s_v ??= new T();

    private static T? s_v;
}