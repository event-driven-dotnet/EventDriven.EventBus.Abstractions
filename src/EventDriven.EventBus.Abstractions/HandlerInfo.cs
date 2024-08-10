namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Handler information.
/// </summary>
public class HandlerInfo
{
    /// <summary>
    /// Handler state.
    /// </summary>
    public HandlerState HanderState { get; set; }
    
    /// <summary>
    /// True if handler is in an error state.
    /// </summary>
    public bool HasError { get; set; }

    /// <summary>
    /// Error message (if any)
    /// </summary>
    public string? ErrorMessage { get; set; }
}