namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public record FakeIntegrationEvent(string Data) : IntegrationEvent;
}
