namespace Fp;

/// <summary>
/// Result of processing.
/// </summary>
public struct ProcessResult
{
    /// <summary>
    /// Successful operation.
    /// </summary>
    public bool Success;

    /// <summary>
    /// Request no more runs.
    /// </summary>
    public bool Locked;

    /// <summary>
    /// Creates a new instance of <see cref="ProcessResult"/>.
    /// </summary>
    /// <param name="success">Successful operation.</param>
    /// <param name="locked">Request no more runs.</param>
    public ProcessResult(bool success, bool locked)
    {
        Success = success;
        Locked = locked;
    }
}