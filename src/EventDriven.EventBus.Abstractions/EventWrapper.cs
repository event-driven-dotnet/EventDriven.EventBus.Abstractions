namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event wrapper.
/// </summary>
/// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
public class EventWrapper<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Event wrapper identifier.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Event wrapper etag.
    /// </summary>
    public string Etag { get; set; } = null!;

    /// <summary>
    /// Event wrapper value.
    /// </summary>
    public EventHandling<TIntegrationEvent>? Value { get; set; }
}