using Xunit;

namespace EventDriven.EventBus.Abstractions.Tests;

public class TopicInfoAttributeTests
{
    [Theory]
    [InlineData("", "", "")]
    [InlineData(null, null, null)]
    [InlineData(" ", " ", " ")]
    [InlineData("some-topic", "some-prefix", "some-suffix")]
    [InlineData("some-topic", "", "some-suffix")]
    [InlineData("some-topic", "some-prefix", null)]
    public void CreateInstance_TopicInfoAttribute_ShouldBeCreate(string? name, string? prefix, string? suffix)
    {
        // act
        var attribute = new TopicInfoAttribute(name, prefix, suffix);

        // assert
        Assert.IsType<TopicInfoAttribute>(attribute);
        Assert.Equal(name, attribute.Topic);
        Assert.Equal(prefix, attribute.Prefix);
        Assert.Equal(suffix, attribute.Suffix);
    }
}