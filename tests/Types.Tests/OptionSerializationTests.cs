using System.Text.Json;
using DotNetCoreFunctional.Option;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using Xunit;

namespace NetFunction.Types.Tests;

public class OptionSerializationTests
{
    private const string SerializedValueObject = "{\"Name\":\"Jack Black\",\"Email\":\"jack.black@example.com\"}";
    private const string SerializedSomeValue = $$"""{"Value":{{SerializedValueObject}}}""";


    [Fact]
    public async Task SerializeSomeOption_IntValue()
    {
        var option = Option<int>.Some(7);

       var serialized = JsonSerializer.Serialize(option);

       await Verify(serialized);
    }

    [Fact]
    public async Task DeserializeSomeOption_IntValue()
    {
        const string serialized = "{\"Value\":7}";

        var option = JsonSerializer.Deserialize<Option<int>>(serialized);

        await Verify(option);
    }

    [Fact]
    public async Task SerializeNoneOption()
    {
        var option = OptionTestData.NoneValue;
        const string expected = "{}";

        var serialized = JsonSerializer.Serialize(option);
        
        await Verify(serialized);
    }

    [Fact]
    public async Task DeserializeNoneOption()
    {
        const string serialized = "{}";

        var option = JsonSerializer.Deserialize<Option<DummyValue>>(serialized);

        await Verify(option);

    }


    [Fact]
    public async Task SerializeSomeOption_ObjectValue()
    {
        var option = OptionTestData.SomeValue;

        var serialized = JsonSerializer.Serialize(option);

        await Verify(serialized);
    }

    [Fact]
    public async Task DeserializeSomeOption_ObjectValue()
    {

        var option = JsonSerializer.Deserialize<Option<DummyValue>>(SerializedSomeValue);

        await Verify(option);
    }

    [Fact]
    public async Task SerializeNestedSomeOption()
    {
        var wrapper = new TestWrapper { Value = OptionTestData.SomeValue };

       var serialized = JsonSerializer.Serialize(wrapper);

       await Verify(serialized);
    }

    [Fact]
    public async Task DeserializedNestedSomeOption()
    {
        var serialized = $$"""{"Value":{{SerializedSomeValue}}}""";

        var nested = JsonSerializer.Deserialize<TestWrapper>(serialized);

        await Verify(nested);
    }

    sealed class TestWrapper
    {
        public Option<DummyValue> Value { get; set; }
    }
}