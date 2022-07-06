using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSmart.AsyncLock;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event cache to enable idempotency.
/// </summary>
public class InMemoryEventCache : IEventCache
{
    private readonly AsyncLock _syncRoot = new();
    
    /// <summary>
    /// Event cache options.
    /// </summary>
    protected readonly EventCacheOptions EventCacheOptions;

    /// <summary>
    /// Thread-safe event cache.
    /// </summary>
    protected ConcurrentDictionary<string, EventHandling> Cache { get; } = new();

    /// <summary>
    /// Cleanup timer.
    /// </summary>
    protected Timer? CleanupTimer { get; }

    /// <summary>
    /// Cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="eventCacheOptions">Event cache options.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public InMemoryEventCache(
        EventCacheOptions eventCacheOptions,
        CancellationToken cancellationToken = default)
    {
        EventCacheOptions = eventCacheOptions;
        CancellationToken = cancellationToken;
        async void TimerCallback(object? state) => await CleanupEventCacheAsync();
        if (EventCacheOptions.EnableEventCacheCleanup)
            CleanupTimer = new Timer(TimerCallback, null, TimeSpan.Zero, EventCacheOptions.EventCacheCleanupInterval);
    }

    /// <summary>
    /// Cleans up event cache.
    /// </summary>
    /// <returns>Task that will complete when the operation has completed.</returns>
    protected virtual async Task CleanupEventCacheAsync()
    {
        using (await _syncRoot.LockAsync(CancellationToken))
        {
            // End timer and exit if cache cleanup disabled or cancellation pending
            if (!EventCacheOptions.EnableEventCacheCleanup || CancellationToken.IsCancellationRequested)
            {
                if (CleanupTimer != null) await CleanupTimer.DisposeAsync();
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
    public virtual bool TryAdd(IntegrationEvent @event)
    {
        // Return true if not enabled
        if (!EventCacheOptions.EnableEventCache) return true;
        
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
            EventHandledTimeout = EventCacheOptions.EventCacheTimeout
        };
        return Cache.TryAdd(@event.Id, handling);
    }

    /// <inheritdoc />
    public Task<bool> TryAddAsync(IntegrationEvent? @event) =>
        Task.FromResult(TryAdd(@event!));
}