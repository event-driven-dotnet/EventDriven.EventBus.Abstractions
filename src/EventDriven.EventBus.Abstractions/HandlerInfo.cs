namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Handler information.
/// </summary>
public class HandlerInfo
{
    /// <summary>
    /// True if handler is in an error state.
    /// </summary>
    public bool HasError { get; set; }

    /// <summary>
    /// Error message (if anyy)
    /// </summary>
    public string? ErrorMessage { get; set; }
}