using System;
using System.Linq;
using System.Threading.Tasks;
using EventDriven.EventBus.Abstractions.Tests.Fakes;
using Xunit;

namespace EventDriven.EventBus.Abstractions.Tests
{
    public class EventBusTests
    {
        [Theory]
        [InlineData(TopicType.Implicit, null, null)]
        [InlineData(TopicType.Implicit, "v1", null)]
        [InlineData(TopicType.Explicit, null, null)]
        [InlineData(TopicType.Explicit, "v1", null)]
        [InlineData(TopicType.Explicit, null, "suffix")]
        [InlineData(TopicType.Explicit, "v1", "suffix")]
        public async Task EventBus_Should_Invoke_Event_Handlers(TopicType topicType, string prefix, string suffix)
        {
            // Topic name
            var topicName = topicType == TopicType.Explicit ? "my-topic" : null;
            
            // Create handlers
            var state = new FakeState { Data = "A" };
            var fakeHandler1 = new FakeEventHandler1(state);
            var fakeHandler2 = new FakeEventHandler2(state);

            // Create message broker
            var messageBroker = new FakeMessageBroker();
            messageBroker.Subscribe(fakeHandler1, topicName, prefix, suffix);
            messageBroker.Subscribe(fakeHandler2, topicName, prefix, suffix);

            // Create event bus
            var eventBus = new FakeEventBus(messageBroker);
            eventBus.Subscribe(fakeHandler1, topicName, prefix, suffix);
            eventBus.Subscribe(fakeHandler2, topicName, prefix, suffix);

            // Publish to service bus
            var @event = new FakeIntegrationEvent("B");
            await eventBus.PublishAsync(@event, topicName, prefix, suffix);

            // Assert
            Assert.Equal(@event.CreationDate, state.Date);
            Assert.Equal("B", state.Data);
        }
        
        [Theory]
        [InlineData(TopicType.Implicit, null, null)]
        [InlineData(TopicType.Implicit, "v1", null)]
        [InlineData(TopicType.Explicit, null, null)]
        [InlineData(TopicType.Explicit, "v1", null)]
        [InlineData(TopicType.Explicit, null, "suffix")]
        [InlineData(TopicType.Explicit, "v1", "suffix")]
        public void EventBus_Should_Remove_Event_Handlers(TopicType topicType, string prefix, string suffix)
        {
            // Topic name
            var topicName = topicType == TopicType.Explicit ? "my-topic" : null;
            
            // Create handlers
            var state = new FakeState { Data = "A" };
            var fakeHandler1 = new FakeEventHandler1(state);
            var fakeHandler2 = new FakeEventHandler2(state);

            // Create message broker
            var messageBroker = new FakeMessageBroker();
            messageBroker.Subscribe(fakeHandler1, topicName, prefix, suffix);
            messageBroker.Subscribe(fakeHandler2, topicName, prefix, suffix);

            // Create event bus
            var eventBus = new FakeEventBus(messageBroker);
            eventBus.Subscribe(fakeHandler1, topicName, prefix, suffix);
            eventBus.Subscribe(fakeHandler2, topicName, prefix, suffix);

            // Remove handler
            eventBus.UnSubscribe(fakeHandler1, topicName, prefix, suffix);

            // Assert
            Assert.Single(eventBus.Topics);
            Assert.Single(eventBus.Topics.First().Value);
            
            // Remove handler
            eventBus.UnSubscribe(fakeHandler2, topicName, prefix, suffix);

            // Assert
            Assert.Empty(eventBus.Topics);
        }
        
        [Theory]
        [InlineData(false, false, 2)]
        [InlineData(true, false, 2)]
        [InlineData(true, true, 2)]
        public async Task EventBus_With_Cache_Should_Invoke_Event_Handlers(bool enableCache, bool expire, int iterations)
        {
            // Create handler
            var initial = 1;
            var state = new FakeState { Value = initial };
            var fakeHandler3 = new FakeEventHandler3(state);

            // Create message broker
            const string prefix = "v1";
            var options = new EventCacheOptions
            {
                EnableEventCache = enableCache,
                EventCacheTimeout = expire ? TimeSpan.FromMilliseconds(200) : TimeSpan.FromSeconds(60),
                EnableEventCacheCleanup = false
            };
            var eventCache = new InMemoryEventCache(options);
            var messageBroker = new FakeCachingMessageBroker(eventCache);
            messageBroker.Subscribe(fakeHandler3, null, prefix);

            // Create event bus
            var eventBus = new FakeRepeatingEventBus(messageBroker, options, iterations, expire);
            messageBroker.EventBus = eventBus;
            eventBus.Subscribe(fakeHandler3, null, prefix);

            // Publish to service bus
            var @event = new FakeIntegrationEvent(string.Empty);
            await eventBus.PublishAsync(@event, null, prefix);

            // Assert
            var expected = enableCache ? initial + 1 : initial + iterations;
            if (enableCache && expire) expected = initial + iterations;
            Assert.Equal(expected, state.Value);
        }
        
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task EventBus_With_Cache_Should_Clean_Up_Event_Handlers(bool enableCacheCleanup)
        {
            // Create handler
            var initial = 1;
            var state = new FakeState { Value = initial };
            var fakeHandler1 = new FakeEventHandler1(state);

            // Create message broker
            const string prefix = "v1";
            var options = new EventCacheOptions
            {
                EnableEventCache = true,
                EventCacheTimeout = TimeSpan.FromMilliseconds(20),
                EnableEventCacheCleanup = enableCacheCleanup,
                EventCacheCleanupInterval = TimeSpan.FromMilliseconds(80)
            };
            var eventCache = new FakeInMemoryEventCache(options);
            var messageBroker = new FakeCachingMessageBroker(eventCache);
            messageBroker.Subscribe(fakeHandler1, null, prefix);

            // Create event bus
            var eventBus = new FakeEventBus(messageBroker);
            messageBroker.EventBus = eventBus;
            eventBus.Subscribe(fakeHandler1, null, prefix);

            // Publish to service bus
            var event1 = new FakeIntegrationEvent(string.Empty);
            var event2 = new FakeIntegrationEvent(string.Empty);
            await eventBus.PublishAsync(event1, null, prefix);
            await eventBus.PublishAsync(event2, null, prefix);
            await Task.Delay(TimeSpan.FromMilliseconds(200));

            // Assert
            var expected = enableCacheCleanup ? 0 : 2;
            var actual = eventCache.GetCacheCount();
            Assert.Equal(expected, actual);
        }
    }

    public enum TopicType
    {
        Implicit,
        Explicit
    }
}
