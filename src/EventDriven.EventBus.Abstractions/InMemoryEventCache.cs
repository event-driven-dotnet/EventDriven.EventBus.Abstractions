using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.Utilities;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event cache to enable idempotency.
/// </summary>
public class InMemoryEventCache : IEventCache
{
    private readonly object _syncRoot = new object();
    
    /// <summary>
    /// Thread-safe event cache.
    /// </summary>
    protected ConcurrentDictionary<string, EventHandling> Cache { get; } = new();

    /// <summary>
    /// Cleanup timer.
    /// </summary>
    protected Timer CleanupTimer { get; }

    /// <summary>
    /// Lock timeout.
    /// </summary>
    protected TimeSpan LockTimeout { get; set; } = TimeSpan.FromSeconds(60);
    
    /// <inheritdoc />
    public EventBusOptions EventBusOptions { get; set; }

    /// <summary>
    /// Cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="eventBusOptions">Event bus options.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public InMemoryEventCache(
        EventBusOptions eventBusOptions,
        CancellationToken cancellationToken = default)
    {
        EventBusOptions = eventBusOptions;
        CancellationToken = cancellationToken;
        async void TimerCallback(object state) => await CleanupEventCacheAsync();
        if (EventBusOptions.EnableEventCacheCleanup)
            CleanupTimer = new Timer(TimerCallback, null, TimeSpan.Zero, EventBusOptions.EventCacheCleanupInterval);
    }

    /// <summary>
    /// Cleans up event cache.
    /// </summary>
    /// <returns>Task that will complete when the operation has completed.</returns>
    protected virtual async Task CleanupEventCacheAsync()
    {
        using (new TimedLock(_syncRoot).Lock(LockTimeout))
        {
            // End timer and exit if cache cleanup disabled or cancellation pending
            if (!EventBusOptions.EnableEventCacheCleanup || CancellationToken.IsCancellationRequested)
            {
                await CleanupTimer.DisposeAsync();
                return;
            }

            // Remove expired events
            var expired = Cache
                .Where(kvp =>
                    kvp.Value.EventHandledTimeout < DateTime.UtcNow - kvp.Value.EventHandledTime);
            foreach (var keyValuePair in expired)
                Cache.TryRemove(keyValuePair);
        }
    }

    /// <inheritdoc />
    public virtual bool TryAdd(IIntegrationEvent @event)
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