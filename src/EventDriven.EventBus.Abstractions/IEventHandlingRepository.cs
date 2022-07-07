using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event handing repository interface.
/// </summary>
/// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
public interface IEventHandlingRepository<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Delete the value associated with the provided <paramref name="appName" /> and <paramref name="eventId" /> in the event state store.
    /// </summary>
    /// <param name="appName">App name.</param>
    /// <param name="eventId">Event id.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> that can be used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Task" /> that will complete when the operation has completed.
    /// Task contains an EventWrapper of TIntegrationEvent.
    /// </returns>
    Task<EventWrapper<TIntegrationEvent>?> GetEventAsync(string appName, string eventId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add an event handling to the event state store.
    /// </summary>
    /// <param name="appName">App name.</param>
    /// <param name="eventId">Event id.</param>
    /// <param name="eventHandling">Event handling.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> that can be used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Task" /> that will complete when the operation has completed.
    /// </returns>
    Task AddEventAsync(string appName, string eventId, EventHandling eventHandling,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete the value associated with the provided <paramref name="appName" /> and <paramref name="eventId" /> in the event state store.
    /// </summary>
    /// <param name="appName">App name.</param>
    /// <param name="eventId">Event id.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Task" /> that will complete when the operation has completed.</returns>
    Task DeleteEventAsync(string appName, string eventId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get expired integration events.
    /// </summary>
    /// <param name="appName">Optional app name.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> that can be used to cancel the operation.</param>
    /// <returns>
    /// A <see cref="Task" /> that will complete when the operation has completed.
    /// Task contains an IEnumerable of EventWrapper of TIntegrationEvent.
    /// </returns>
    Task<IEnumerable<EventWrapper<TIntegrationEvent>>> GetExpiredEventsAsync(
        string? appName = null,
        CancellationToken cancellationToken = default);
}