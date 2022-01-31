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
        [InlineData(TopicType.Implicit, null)]
        [InlineData(TopicType.Implicit, "v1")]
        [InlineData(TopicType.Explicit, null)]
        [InlineData(TopicType.Explicit, "v1")]
        public async Task EventBus_Should_Invoke_Event_Handlers(TopicType topicType, string prefix)
        {
            // Topic name
            var topicName = topicType == TopicType.Explicit ? "my-topic" : null;
            
            // Create handlers
            var state = new FakeState { Data = "A" };
            var fakeHandler1 = new FakeEventHandler1(state);
            var fakeHandler2 = new FakeEventHandler2(state);

            // Create message broker
            var messageBroker = new FakeMessageBroker();
            messageBroker.Subscribe(fakeHandler1, topicName, prefix);
            messageBroker.Subscribe(fakeHandler2, topicName, prefix);

            // Create event bus
            var eventBus = new FakeEventBus(messageBroker);
            eventBus.Subscribe(fakeHandler1, topicName, prefix);
            eventBus.Subscribe(fakeHandler2, topicName, prefix);

            // Publish to service bus
            var @event = new FakeIntegrationEvent("B");
            await eventBus.PublishAsync(@event, topicName, prefix);

            // Assert
            Assert.Equal(@event.CreationDate, state.Date);
            Assert.Equal("B", state.Data);
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
            var options = new EventBusOptions
            {
                EnableEventCache = enableCache,
                EventCacheTimeout = expire ? TimeSpan.FromMilliseconds(200) : TimeSpan.FromSeconds(60)
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
        [InlineData(TopicType.Implicit, null)]
        [InlineData(TopicType.Implicit, "v1")]
        [InlineData(TopicType.Explicit, null)]
        [InlineData(TopicType.Explicit, "v1")]
        public void EventBus_Should_Remove_Event_Handlers(TopicType topicType, string prefix)
        {
            // Topic name
            var topicName = topicType == TopicType.Explicit ? "my-topic" : null;
            
            // Create handlers
            var state = new FakeState { Data = "A" };
            var fakeHandler1 = new FakeEventHandler1(state);
            var fakeHandler2 = new FakeEventHandler2(state);

            // Create message broker
            var messageBroker = new FakeMessageBroker();
            messageBroker.Subscribe(fakeHandler1, topicName, prefix);
            messageBroker.Subscribe(fakeHandler2, topicName, prefix);

            // Create event bus
            var eventBus = new FakeEventBus(messageBroker);
            eventBus.Subscribe(fakeHandler1, topicName, prefix);
            eventBus.Subscribe(fakeHandler2, topicName, prefix);

            // Remove handler
            eventBus.UnSubscribe(fakeHandler1, topicName, prefix);

            // Assert
            Assert.Single(eventBus.Topics);
            Assert.Single(eventBus.Topics.First().Value);
            
            // Remove handler
            eventBus.UnSubscribe(fakeHandler2, topicName, prefix);

            // Assert
            Assert.Empty(eventBus.Topics);
        }
    }

    public enum TopicType
    {
        Implicit,
        Explicit
    }
}
