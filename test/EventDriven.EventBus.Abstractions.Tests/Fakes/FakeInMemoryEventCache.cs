using System;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes;

public class FakeInMemoryEventCache : InMemoryEventCache
{
    public FakeInMemoryEventCache(EventCacheOptions eventBusOptions) : base(eventBusOptions)
    {
    }

    public int GetCacheCount() => Cache.Count;

    public override bool TryAdd(IntegrationEvent @event)
    {
        // Return true if not enabled
        if (!EventCacheOptions.EnableEventCache) return true;
        
        // Return false if event exists
        if (Cache.TryGetValue(@event.Id, out _))
            return false;
        
        // Add event handling
        var handling = new EventHandling
        {
            EventId = @event.Id,
            IntegrationEvent = @event,
            EventHandledTime = DateTime.UtcNow,
            EventHandledTimeout = EventCacheOptions.EventCacheTimeout
        };
        return Cache.TryAdd(@event.Id, handling);
    }
}