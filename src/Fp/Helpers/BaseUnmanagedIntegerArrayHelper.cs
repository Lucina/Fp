namespace Fp.Helpers;

/// <summary>
/// Base unmanaged integer array data helper.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public abstract partial record BaseUnmanagedIntegerArrayHelper<T> : BaseUnmanagedArrayHelper<T>
    where T : unmanaged
#if NET7_0_OR_GREATER
    , System.Numerics.INumber<T>
#else
    , System.IComparable<T>
#endif
;
