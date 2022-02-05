using System;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Handling of an event.
/// </summary>
public class EventHandling
{
    /// <summary>
    /// Event identifier.
    /// </summary>
    public string EventId { get; set; } = null!;

    /// <summary>
    /// The event.
    /// </summary>
    public IntegrationEvent IntegrationEvent { get; set; } = null!;

    /// <summary>
    /// Time at which the event was handled.
    /// </summary>
    public DateTime EventHandledTime { get; set; }

    /// <summary>
    /// Timespan during which event is considered to be handled.
    /// </summary>
    public TimeSpan EventHandledTimeout { get; set; }
}

/// <summary>
/// Generic handling of an event.
/// </summary>
/// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
public class EventHandling<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent

{
    /// <summary>
    /// Event identifier.
    /// </summary>
    public string EventId { get; set; } = null!;

    /// <summary>
    /// The event.
    /// </summary>
    public TIntegrationEvent IntegrationEvent { get; set; } = default!;

    /// <summary>
    /// Time at which the event was handled.
    /// </summary>
    public DateTime EventHandledTime { get; set; }

    /// <summary>
    /// Timespan during which event is considered to be handled.
    /// </summary>
    public TimeSpan EventHandledTimeout { get; set; }
}