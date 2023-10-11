namespace EventDriven.EventBus.Abstractions.Tests.Fakes;

public class FakeInMemoryEventCache : InMemoryEventCache
{
    public FakeInMemoryEventCache(EventCacheOptions eventBusOptions) : base(eventBusOptions)
    {
    }

    public int GetCacheCount() => Cache.Count;
}