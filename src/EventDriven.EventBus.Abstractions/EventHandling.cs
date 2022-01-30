using System;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Handling of an event
/// </summary>
public class EventHandling
{
    /// <summary>
    /// Event identifier.
    /// </summary>
    public string EventId { get; set; }

    /// <summary>
    /// The event.
    /// </summary>
    public IIntegrationEvent IntegrationEvent { get; set; }

    /// <summary>
    /// Time at which the event was handled.
    /// </summary>
    public DateTime EventHandledTime { get; set; }

    /// <summary>
    /// Timespan during which event is considered to be handled.
    /// </summary>
    public TimeSpan EventHandledTimeout { get; set; }
}