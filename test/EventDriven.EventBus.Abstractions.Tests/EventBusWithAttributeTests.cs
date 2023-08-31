using System;
using System.Threading.Tasks;
using Xunit;

namespace EventDriven.EventBus.Abstractions.Tests;

public class EventBusWithAttributeTests
{
    private class FakeEventBus : EventBus
    {
        public string? TopicName { get; private set; }

        public override Task PublishAsync<TIntegrationEvent>
            (TIntegrationEvent @event, string? topic = null, string? prefix = null, string? suffix = null)
        {
            TopicName = GetTopicName(@event.GetType(), topic, prefix, suffix);
            return Task.CompletedTask;
        }
    }

    private const string Topic = "my-topic";
    private const string Prefix = "my-prefix";
    private const string Suffix = "my-suffix";

    private const string TopicParam = "my-topic-param";
    private const string PrefixParam = "my-prefix-param";
    private const string SuffixParam = "my-suffix-param";

    [TopicInfo(Topic)]
    private record FakeIntegrationEventWithTopic : IntegrationEvent;

    [TopicInfo]
    private record FakeIntegrationEventNoTopic : IntegrationEvent;

    [TopicInfo(Topic, Prefix)]
    private record FakeIntegrationEventWithTopicPrefix : IntegrationEvent;

    [TopicInfo(Topic, null, Suffix)]
    private record FakeIntegrationEventWithTopicSuffix : IntegrationEvent;

    [TopicInfo(Topic, Prefix, Suffix)]
    private record FakeIntegrationEventWithTopicPrefixSuffix : IntegrationEvent;

    private class FakeHandlerWithTopic : IntegrationEventHandler<FakeIntegrationEventWithTopic>
    {
        public override Task HandleAsync(FakeIntegrationEventWithTopic @event) => throw new NotImplementedException();
    }

    private class FakeHandlerWithNoTopic : IntegrationEventHandler<FakeIntegrationEventNoTopic>
    {
        public override Task HandleAsync(FakeIntegrationEventNoTopic @event) => throw new NotImplementedException();
    }

    private class FakeHandlerWithTopicPrefix : IntegrationEventHandler<FakeIntegrationEventWithTopicPrefix>
    {
        public override Task HandleAsync(FakeIntegrationEventWithTopicPrefix @event) => throw new NotImplementedException();
    }

    private class FakeHandlerWithTopicSuffix : IntegrationEventHandler<FakeIntegrationEventWithTopicSuffix>
    {
        public override Task HandleAsync(FakeIntegrationEventWithTopicSuffix @event) => throw new NotImplementedException();
    }

    private class FakeHandlerWithTopicPrefixSuffix : IntegrationEventHandler<FakeIntegrationEventWithTopicPrefixSuffix>
    {
        public override Task HandleAsync(FakeIntegrationEventWithTopicPrefixSuffix @event) => throw new NotImplementedException();
    }

    [Theory]
    [InlineData(null, PrefixParam, null, $"{PrefixParam}.FakeIntegrationEventWithTopicPrefixSuffix")]
    [InlineData(null, null, SuffixParam, $"FakeIntegrationEventWithTopicPrefixSuffix.{SuffixParam}")]
    [InlineData(null, PrefixParam, SuffixParam, $"{PrefixParam}.FakeIntegrationEventWithTopicPrefixSuffix.{SuffixParam}")]
    [InlineData(TopicParam, null, null, TopicParam)]
    [InlineData(TopicParam, PrefixParam, null, $"{PrefixParam}.{TopicParam}")]
    [InlineData(TopicParam, null, SuffixParam, $"{TopicParam}.{SuffixParam}")]
    [InlineData(TopicParam, PrefixParam, SuffixParam, $"{PrefixParam}.{TopicParam}.{SuffixParam}")]
    public void Subscribe_WithArgs_ForHandlerWithTopic_ShouldUseArgsValues(string? topic, string? prefix, string? suffix, string expectedTopicName)
    {
        // arrange
        var busFake = new FakeEventBus();
        var handler = new FakeHandlerWithTopicPrefixSuffix();

        // act
        busFake.Subscribe(handler, topic, prefix, suffix);

        // assert
        Assert.Contains(expectedTopicName, busFake.Topics.Keys);
    }

    [Theory]
    [InlineData(IntegrationEventType.NoTopic)]
    [InlineData(IntegrationEventType.WithTopic)]
    [InlineData(IntegrationEventType.WithTopicPrefix)]
    [InlineData(IntegrationEventType.WithTopicSuffix)]
    [InlineData(IntegrationEventType.WithTopicPrefixSuffix)]
    public void Subscribe_ForHandlerWithAttribute_ShouldUseAttribute(IntegrationEventType integrationEventType)
    {
        // arrange
        var busFake = new FakeEventBus();
        var handler = GetHandler(integrationEventType);
        var topicName = GetTopicName(integrationEventType);

        // act
        busFake.Subscribe(handler!);

        // assert
        Assert.Contains(topicName, busFake.Topics.Keys);
    }

    [Theory]
    [InlineData(null, PrefixParam, null, $"{PrefixParam}.FakeIntegrationEventWithTopicPrefixSuffix")]
    [InlineData(null, null, SuffixParam, $"FakeIntegrationEventWithTopicPrefixSuffix.{SuffixParam}")]
    [InlineData(null, PrefixParam, SuffixParam, $"{PrefixParam}.FakeIntegrationEventWithTopicPrefixSuffix.{SuffixParam}")]
    [InlineData(TopicParam, null, null, TopicParam)]
    [InlineData(TopicParam, PrefixParam, null, $"{PrefixParam}.{TopicParam}")]
    [InlineData(TopicParam, null, SuffixParam, $"{TopicParam}.{SuffixParam}")]
    [InlineData(TopicParam, PrefixParam, SuffixParam, $"{PrefixParam}.{TopicParam}.{SuffixParam}")]
    public async Task PublishAsync_WithArgs_ForEventWithAttribute_ShouldUseArgsValues(string? topic, string? prefix, string? suffix, string expectedTopicName)
    {
        // arrange
        var busFake = new FakeEventBus();
        var @event = new FakeIntegrationEventWithTopicPrefixSuffix();
        // act
        await busFake.PublishAsync(@event, topic, prefix, suffix);

        // assert
        Assert.Equal(expectedTopicName, busFake.TopicName);
    }

    [Theory]
    [InlineData(IntegrationEventType.NoTopic)]
    [InlineData(IntegrationEventType.WithTopic)]
    [InlineData(IntegrationEventType.WithTopicPrefix)]
    [InlineData(IntegrationEventType.WithTopicSuffix)]
    [InlineData(IntegrationEventType.WithTopicPrefixSuffix)]
    public async Task PublishAsync_WithoutArgs_ForEventWithAttribute_ShouldUseAttribute(IntegrationEventType integrationEventType)
    {
        var busFake = new FakeEventBus();
        var @event = GetEvent(integrationEventType);
        var topicName = GetTopicName(integrationEventType);

        await busFake.PublishAsync(@event);

        Assert.Equal(topicName, busFake.TopicName);
    }

    private string? GetTopicName(IntegrationEventType integrationEventType)
    {
        return integrationEventType switch
        {
            IntegrationEventType.NoTopic => nameof(FakeIntegrationEventNoTopic),
            IntegrationEventType.WithTopic => Topic,
            IntegrationEventType.WithTopicPrefix => $"{Prefix}.{Topic}",
            IntegrationEventType.WithTopicSuffix => $"{Topic}.{Suffix}",
            IntegrationEventType.WithTopicPrefixSuffix => $"{Prefix}.{Topic}.{Suffix}",
            _ => null
        };
    }

    private IIntegrationEventHandler? GetHandler(IntegrationEventType integrationEventType)
    {
        return integrationEventType switch
        {
            IntegrationEventType.NoTopic => new FakeHandlerWithNoTopic(),
            IntegrationEventType.WithTopic => new FakeHandlerWithTopic(),
            IntegrationEventType.WithTopicPrefix => new FakeHandlerWithTopicPrefix(),
            IntegrationEventType.WithTopicSuffix => new FakeHandlerWithTopicSuffix(),
            IntegrationEventType.WithTopicPrefixSuffix => new FakeHandlerWithTopicPrefixSuffix(),
            _ => null
        };
    }

    private IntegrationEvent GetEvent(IntegrationEventType integrationEventType)
    {
        return integrationEventType switch
        {
            IntegrationEventType.NoTopic => new FakeIntegrationEventNoTopic(),
            IntegrationEventType.WithTopic => new FakeIntegrationEventWithTopic(),
            IntegrationEventType.WithTopicPrefix => new FakeIntegrationEventWithTopicPrefix(),
            IntegrationEventType.WithTopicSuffix => new FakeIntegrationEventWithTopicSuffix(),
            IntegrationEventType.WithTopicPrefixSuffix => new FakeIntegrationEventWithTopicPrefixSuffix(),
            _ => new FakeIntegrationEventNoTopic()
        };
    }
    
    public enum IntegrationEventType
    {
        NoTopic,
        WithTopic,
        WithTopicPrefix,
        WithTopicSuffix,
        WithTopicPrefixSuffix
    }
}