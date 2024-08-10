using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event cache to enable idempotency.
/// </summary>
public interface IEventCache
{
    /// <summary>
    /// Check if integration event has been handled.
    /// </summary>
    /// <param name="event">The integration event</param>
    /// <param name="handlerTypeName">Handler type name</param>
    /// <returns>
    /// Task that will complete when the operation has completed.
    /// Task contains true if the event is in the event cache, not expired and not in an error state;
    /// false if the event is not in the cache, is expired or is in an error state.
    /// </returns>
    Task<bool> HasBeenHandledAsync(IntegrationEvent @event, string handlerTypeName);

    /// <summary>
    /// Add integration event to the event cache, or update event with error state.
    /// </summary>
    /// <param name="event">The integration event</param>
    /// <param name="handlerTypeName">Handler type name</param>
    /// <param name="errorMessage">Error message</param>
    /// <returns>
    /// Task that will complete when the operation has completed.
    /// </returns>
    Task AddEventAsync(IntegrationEvent @event, string? handlerTypeName = null, string? errorMessage = null);

    /// <summary>
    /// Update integration event in the event cache with or without error state.
    /// </summary>
    /// <param name="event">The integration event</param>
    /// <param name="handlerTypeName">Handler type name</param>
    /// <param name="errorMessage">Error message</param>
    /// <returns>
    /// Task that will complete when the operation has completed.
    /// </returns>
    Task UpdateEventAsync(IntegrationEvent @event, string? handlerTypeName = null, string? errorMessage = null);

    /// <summary>
    /// Check if integration event has been handled; if not, persist integration event to the event cache.
    /// </summary>
    /// <param name="event">The integration event</param>
    /// <param name="handlerTypeName">Handler type name</param>
    /// <returns>
    /// Task that will complete when the operation has completed.
    /// Task contains true if the event is in the event cache, not expired and not in an error state;
    /// false if the event is not in the cache, is expired or is in an error state.
    /// </returns>
    Task<bool> HasBeenHandledPersistEventAsync(IntegrationEvent @event, string? handlerTypeName = null);
}