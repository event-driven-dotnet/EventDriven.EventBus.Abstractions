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

            // Create service bus
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

            // Create service bus
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
