using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event cache to enable idempotency.
/// </summary>
public interface IEventCache
{
    /// <summary>
    /// Attempts to add the integration event to the event cache.
    /// </summary>
    /// <param name="event">The integration event</param>
    /// <returns>
    /// True if the event was added to the event cache.
    /// False if the event is in the cache and not expired or it cannot be removed. 
    /// </returns>
    bool TryAdd(IntegrationEvent @event);
    
    /// <summary>
    /// Attempts to add the integration event to the event cache.
    /// </summary>
    /// <param name="event">The integration event</param>
    /// <returns>
    /// Task that will complete when the operation has completed.
    /// Task contains true if the event was added to the event cache,
    /// false if the event is in the cache and not expired or it cannot be removed.
    /// </returns>
    Task<bool> TryAddAsync(IntegrationEvent @event);
}