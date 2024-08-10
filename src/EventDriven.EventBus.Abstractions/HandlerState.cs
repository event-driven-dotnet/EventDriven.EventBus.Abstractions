namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Handler state.
/// </summary>
public enum HandlerState
{
    /// <summary>
    /// Not started.
    /// </summary>
    NotStarted,

    /// <summary>
    /// Started.
    /// </summary>
    Started,
    
    /// <summary>
    /// Completed
    /// </summary>
    Completed
}