using System;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeState
    {
        public DateTime Date { get; set; }
        public string Data { get; set; } = null!;
        public int Value { get; set; }
    }
}
