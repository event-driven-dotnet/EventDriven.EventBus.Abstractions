using System;
using System.Collections.Concurrent;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event cache to enable idempotency.
/// </summary>
public class EventCache
{
    /// <summary>
    /// Thread-safe event cache.
    /// </summary>
    protected ConcurrentDictionary<string, EventHandling> Cache { get; set; } = new();

    /// <summary>
    /// Event bus options.
    /// </summary>
    protected EventBusOptions EventBusOptions { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="eventBusOptions">Event bus options.</param>
    public EventCache(EventBusOptions eventBusOptions)
    {
        EventBusOptions = eventBusOptions;
    }

    /// <summary>
    /// Attempts to add the integration event to the event cache.
    /// </summary>
    /// <param name="event">The integration event</param>
    /// <returns>
    /// True if the event was added to the event cache.
    /// False if the event is in the cache and not expired or it cannot be removed. 
    /// </returns>
    public bool TryAdd(IIntegrationEvent @event)
    {
        // Return false if event exists and is not expired
        bool expired = false;
        if (Cache.TryGetValue(@event.Id, out var existing))
            expired = existing.EventHandledTimeout < DateTime.UtcNow - existing.EventHandledTime;
        if (existing != null && !expired) return false;
        
        // Remove existing; return false if unable to remove
        if (existing != null
            && !Cache.TryRemove(@event.Id, out existing))
            return false;
            
        // Add event handling
        var handling = new EventHandling
        {
            EventId = @event.Id,
            IntegrationEvent = @event,
            EventHandledTime = DateTime.UtcNow,
            EventHandledTimeout = EventBusOptions.EventCacheTimeout
        };
        return Cache.TryAdd(@event.Id, handling);
    }
}