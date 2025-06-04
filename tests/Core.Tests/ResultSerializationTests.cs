using System.Text.Json;
using SharperExtensions.Serialization;

namespace SharperExtensions.Core.Tests;

public class ResultJsonConverterTests
{
    private readonly JsonSerializerOptions _options;

    public ResultJsonConverterTests()
    {
        _options = new JsonSerializerOptions
        {
            Converters =
            {
                new ResultJsonConverter<object, object>()
            } // Use object/object as base, converter handles generics
        };
    }

    // Helper method to perform serialization and deserialization round trip
    private Result<T, TError> SerializeAndDeserialize<T, TError>(Result<T, TError> result)
        where T : notnull
        where TError : notnull
    {
        // Need options specific to the T, TError types for the converter
        var options = new JsonSerializerOptions
        {
            Converters = { new ResultJsonConverter<T, TError>() }
        };

        var json = JsonSerializer.Serialize(result, options);
        return JsonSerializer.Deserialize<Result<T, TError>>(json, options);
    }

    // Helper method to deserialize from a JSON string
    private Result<T, TError> Deserialize<T, TError>(string json)
        where T : notnull
        where TError : notnull
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new ResultJsonConverter<T, TError>() }
        };
        return JsonSerializer.Deserialize<Result<T, TError>>(json, options);
    }

    // Helper method to serialize to a JSON string
    private string Serialize<T, TError>(Result<T, TError> result)
        where T : notnull
        where TError : notnull
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new ResultJsonConverter<T, TError>() }
        };
        return JsonSerializer.Serialize(result, options);
    }

    public static IEnumerable<object[]> PrimitiveResultData =>
        new List<object[]>
        {
            // Ok results with primitives
            new object[] { Result.Ok<int, string>(123) },
            new object[] { Result.Ok<string, int>("hello") },
            new object[] { Result.Ok<double, bool>(45.67) },
            new object[] { Result.Ok<bool, double>(true) },
            new object[] { Result.Ok<char, string>('a') },
            new object[] { Result.Ok<long, int>(9876543210L) },

            // Error results with primitives
            new object[] { Result.Error<int, string>("error message") },
            new object[] { Result.Error<string, int>(999) },
            new object[] { Result.Error<double, bool>(false) },
            new object[] { Result.Error<bool, double>(1.23) },
            new object[] { Result.Error<char, string>("another error") },
            new object[] { Result.Error<long, int>(111) },
        };

    public static IEnumerable<object[]> CustomTypeResultData =>
        new List<object[]>
        {
            // Ok results with custom types
            new object[] { Result.Ok<DummyValue, DummyError>(ResultTestData.Value) },
            new object[]
                { Result.Ok<DummyNewValue, DummyNewError>(ResultTestData.NewValue) },

            // Error results with custom types
            new object[] { Result.Error<DummyValue, DummyError>(ResultTestData.Error) },
            new object[]
                { Result.Error<DummyNewValue, DummyNewError>(ResultTestData.NewError) },

            // Mixed types
            new object[] { Result.Ok<DummyValue, string>(ResultTestData.Value) },
            new object[] { Result.Error<string, DummyError>(ResultTestData.Error) },
        };

    [Theory]
    [MemberData(nameof(PrimitiveResultData))]
    public void SerializeAndDeserialize_ShouldPreservePrimitiveResult(object resultObj)
    {
        // Use dynamic to handle the generic Result type from MemberData
        dynamic result = resultObj;

        // Call the generic helper using the actual types
        object deserializedResult = SerializeAndDeserialize(result);

        deserializedResult.Should().BeEquivalentTo<object>(result);
    }

    [Theory]
    [MemberData(nameof(CustomTypeResultData))]
    public void SerializeAndDeserialize_ShouldPreserveCustomTypeResult(object resultObj)
    {
        // Use dynamic to handle the generic Result type from MemberData
        dynamic result = resultObj;

        // Call the generic helper using the actual types
        var deserializedResult = SerializeAndDeserialize(result);
        object value = deserializedResult.Value;
        object errorValue = deserializedResult.ErrorValue;

        value.Should().BeEquivalentTo<object>(result.Value);
        errorValue.Should().BeEquivalentTo<object>(result.ErrorValue);
    }

    [Fact]
    public void Serialize_OkResult_ShouldContainValueProperty()
    {
        var result = Result.Ok<string, int>("success");
        var json = Serialize(result);

        json.Should().Contain("\"Value\":\"success\"");
        json.Should().NotContain("Error"); // Should not contain the error property
    }

    [Fact]
    public void Serialize_ErrorResult_ShouldContainErrorProperty()
    {
        var result = Result.Error<string, int>(999);
        var json = Serialize(result);

        json.Should().Contain("\"Error\":999");
        json.Should().NotContain("Value"); // Should not contain the value property
    }

    [Fact]
    public void Serialize_OkResult_WithCustomType_ShouldContainValueProperty()
    {
        var result = Result.Ok<DummyValue, DummyError>(ResultTestData.Value);
        var json = Serialize(result);

        json.Should().Contain("\"Value\":");
        json.Should().Contain("\"Name\":\"Jack Black\"");
        json.Should().Contain("\"Email\":\"jack.black@example.com\"");
        json.Should().NotContain("Error");
    }

    [Fact]
    public void Serialize_ErrorResult_WithCustomType_ShouldContainErrorProperty()
    {
        var result = Result.Error<DummyValue, DummyError>(ResultTestData.Error);
        var json = Serialize(result);

        json.Should().Contain("\"Error\":");
        json.Should().Contain("\"Message\":\"error message\"");
        json.Should().NotContain("Value");
    }

    [Fact]
    public void Deserialize_OkResult_FromPrimitiveJson()
    {
        var json = "{\"Value\":123}";
        var result = Deserialize<int, string>(json);

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(123);
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void Deserialize_ErrorResult_FromPrimitiveJson()
    {
        var json = "{\"Error\":\"an error\"}";
        var result = Deserialize<int, string>(json);

        result.IsError.Should().BeTrue();
        result.ErrorValue.Should().Be("an error");
        result.IsOk.Should().BeFalse();
    }

    [Fact]
    public void Deserialize_OkResult_FromCustomTypeJson()
    {
        var json =
            "{\"Value\":{\"Name\":\"Jack Black\",\"Email\":\"jack.black@example.com\"}}";
        var result = Deserialize<DummyValue, DummyError>(json);

        result.IsOk.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(ResultTestData.Value);
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void Deserialize_ErrorResult_FromCustomTypeJson()
    {
        var json = "{\"Error\":{\"Message\":\"error message\"}}";
        var result = Deserialize<DummyValue, DummyError>(json);

        result.IsError.Should().BeTrue();
        result.ErrorValue.Should().BeEquivalentTo(ResultTestData.Error);
        result.IsOk.Should().BeFalse();
    }

    [Fact]
    public void Deserialize_InvalidJson_NotAnObject_ShouldThrowJsonException()
    {
        var json = "\"not an object\"";
        Action action = () => Deserialize<int, string>(json);

        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_InvalidJson_MissingProperty_ShouldThrowJsonException()
    {
        var json = "{}"; // Missing "Value" or "Error"
        Action action = () => Deserialize<int, string>(json);

        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_InvalidJson_UnknownProperty_ShouldThrowJsonException()
    {
        var json = "{\"Unknown\":\"value\"}";
        Action action = () => Deserialize<int, string>(json);

        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_InvalidJson_BothPropertiesPresent_ShouldThrowJsonException()
    {
        var json = "{\"Value\":123, \"Error\":\"error\"}"; // Should only have one
        Action action = () => Deserialize<int, string>(json);

        // The stared comments were written by Gemini Flash 2.5.
        // I am leaving them in as reminder not to trust AI without question.

        //* The current implementation reads the first property ("Value") and returns.
        //* It doesn't explicitly check for both. Let's verify it behaves as it does.
        //* If the requirement was to throw, the converter would need modification.
        //* Based on the existing code, it will successfully deserialize the "Value" case.
        //* Let's write a test that reflects the current behavior, or update the converter
        //* if throwing is the desired behavior for this case.
        //* Assuming current behavior is acceptable:
        
        
        _ = // lang=csharp
            """
                var result = Deserialize<int, string>(json);
                result.IsOk.Should().BeTrue();
                result.Value.Should().Be(123);
            """;

        //* If throwing was required, the test would be:
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_InvalidJson_ValueIsNull_ShouldThrowJsonException()
    {
        var json = "{\"Value\":null}";
        Action action = () => Deserialize<int, string>(json);

        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_InvalidJson_ErrorIsNull_ShouldThrowJsonException()
    {
        var json = "{\"Error\":null}";
        Action action = () => Deserialize<int, string>(json);

        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_InvalidJson_ValueWrongType_ShouldThrowJsonException()
    {
        var json = "{\"Value\":\"not an int\"}";
        Action action = () => Deserialize<int, string>(json);

        // This will likely throw a JsonException or a related serialization exception
        // depending on how JsonSerializer.Deserialize handles type mismatches.
        action.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_InvalidJson_ErrorWrongType_ShouldThrowJsonException()
    {
        var json = "{\"Error\":123}";
        Action action = () => Deserialize<int, string>(json);

        // This will likely throw a JsonException or a related serialization exception
        // depending on how JsonSerializer.Deserialize handles type mismatches.
        action.Should().Throw<JsonException>();
    }
}