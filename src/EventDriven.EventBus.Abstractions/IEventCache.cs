namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event cache to enable idempotency.
/// </summary>
public interface IEventCache
{
    /// <summary>
    /// Event bus options.
    /// </summary>
    EventCacheOptions EventCacheOptions { get; set; }

    /// <summary>
    /// Attempts to add the integration event to the event cache.
    /// </summary>
    /// <param name="event">The integration event</param>
    /// <returns>
    /// True if the event was added to the event cache.
    /// False if the event is in the cache and not expired or it cannot be removed. 
    /// </returns>
    bool TryAdd(IntegrationEvent @event);
}