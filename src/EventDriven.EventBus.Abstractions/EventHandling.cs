using System;
using System.Collections.Generic;

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

    /// <summary>
    /// Handlers for the event.
    /// </summary>
    public Dictionary<string, HandlerInfo> Handlers { get; set; } = new();
}

/// <summary>
/// Generic handling of an event.
/// </summary>
/// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
public class EventHandling<TIntegrationEvent> : EventHandling
    where TIntegrationEvent : IntegrationEvent

{
}