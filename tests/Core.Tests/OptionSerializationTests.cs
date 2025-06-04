using System.Text.Json;

namespace SharperExtensions.Core.Tests;

public class OptionSerializationTests
{
    private const string SerializedValueObject =
        "{\"Name\":\"Jack Black\",\"Email\":\"jack.black@example.com\"}";

    private const string SerializedSomeValue =
        $$"""{"Value":{{SerializedValueObject}}}""";

    private static readonly TestWrapper Wrapper = new() { Field = OptionTestData.Value };

    [Fact]
    public void SerializeSomeOption_IntValue()
    {
        var option = Option<int>.Some(7);
        var serialized = JsonSerializer.Serialize(option);

        serialized.Should().Be("7");
    }

    [Fact]
    public void DeserializeSomeOption_IntValue()
    {
        const string serialized = "7";

        var option = JsonSerializer.Deserialize<Option<int>>(serialized);

        option.Should().Be(Option<int>.Some(7));
    }

    [Fact]
    public void SerializeNoneOption()
    {
        var option = OptionTestData.NoneValue;
        var serialized = JsonSerializer.Serialize(option);

        serialized.Should().BeEmpty();
    }

    [Fact]
    public void DeserializeNoneOption()
    {
        const string serialized = "null";

        var option = JsonSerializer.Deserialize<Option<DummyValue>>(serialized);

        option.Should().Be(Option<DummyValue>.None);
    }

    [Fact]
    public void SerializeSomeOption_ObjectValue()
    {
        var option = OptionTestData.SomeValue;

        var serialized = JsonSerializer.Serialize(option);

        serialized.Should().Be(SerializedValueObject);
    }

    [Fact]
    public void DeserializeSomeOption_ObjectValue()
    {
        var option =
            JsonSerializer.Deserialize<Option<DummyValue>>(SerializedValueObject);

        option
            .Value
            .Should()
            .BeEquivalentTo(OptionTestData.Value);
    }

    [Fact]
    public void SerializeNestedSomeOption()
    {
        const string expected = $$"""{"Field":{{SerializedValueObject}}}""";
        var serialized = JsonSerializer.Serialize(Wrapper);

        serialized.Should().Be(expected);
    }

    [Fact]
    public void DeserializedNestedSomeOption()
    {
        var serialized = $$"""{"Field":{{SerializedValueObject}}}""";

        var nested = JsonSerializer.Deserialize<TestWrapper>(serialized);

        nested?.Field.Value.Should().BeEquivalentTo(Wrapper.Field.Value);
    }

    [Fact]
    public void SerializeTypeNullValue()
    {
        var wrapper = new MissingValueTestWrapper("hello!", Option<DummyValue>.None);
        const string expected = """{"Str":"hello!","Dummy":null}""";

        var serialized = JsonSerializer.Serialize(wrapper);

        serialized.Should().Be(expected);
    }

    [Fact]
    public void DeserializeTypeWithNullValue_ShouldReturnNone()
    {
        const string unserialized = """{"Str":"hello!","Dummy":null}""";
        var expected = new MissingValueTestWrapper("hello!", Option<DummyValue>.None);

        var serialized =
            JsonSerializer.Deserialize<MissingValueTestWrapper>(unserialized);

        serialized.Should().Be(expected);
    }

    [Fact]
    public void DeserializeTypeWithMissingValue()
    {
        const string unserialized = """{"Str":"hello!"}""";
        var expected = new MissingValueTestWrapper("hello!", Option<DummyValue>.None);

        var serialized =
            JsonSerializer.Deserialize<MissingValueTestWrapper>(unserialized);

        serialized.Should().Be(expected);
    }

    sealed class TestWrapper
    {
        public Option<DummyValue> Field { get; init; } = Option<DummyValue>.None;
    }

    sealed record MissingValueTestWrapper(string Str, Option<DummyValue> Dummy);
}