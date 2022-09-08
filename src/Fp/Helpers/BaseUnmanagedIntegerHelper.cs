namespace Fp.Helpers;

/// <summary>
/// Base unmanaged integer array data helper.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public abstract partial record BaseUnmanagedIntegerHelper<T> : BaseUnmanagedHelper<T> where T : unmanaged;
