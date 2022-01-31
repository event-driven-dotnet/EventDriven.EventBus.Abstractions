using System;
using System.Collections.Concurrent;

namespace EventDriven.EventBus.Abstractions;

/// <inheritdoc />
public class InMemoryEventCache : IEventCache
{
    /// <summary>
    /// Thread-safe event cache.
    /// </summary>
    protected ConcurrentDictionary<string, EventHandling> Cache { get; } = new();

    /// <inheritdoc />
    public EventBusOptions EventBusOptions { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="eventBusOptions">Event bus options.</param>
    public InMemoryEventCache(EventBusOptions eventBusOptions)
    {
        EventBusOptions = eventBusOptions;
    }

    /// <inheritdoc />
    public bool TryAdd(IIntegrationEvent @event)
    {
        // Return true if not enabled
        if (!EventBusOptions.EnableEventCache) return true;
        
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