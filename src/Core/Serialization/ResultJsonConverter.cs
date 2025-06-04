using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SharperExtensions.Serialization;

/// <summary>
/// A JSON converter for serializing and deserializing <see cref="Result{T, TError}"/> objects.
/// </summary>
/// <typeparam name="T">The type of the successful value, which must be non-nullable.</typeparam>
/// <typeparam name="TError">The type of the error value, which must be non-nullable.</typeparam>
/// <remarks>
/// Supports converting Result objects to and from JSON, handling both successful and error cases.
/// </remarks>
public class ResultJsonConverter<T, TError> : JsonConverter<Result<T, TError>>
    where T : notnull
    where TError : notnull
{
    /// <inheritdoc />
    public override Result<T, TError> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (
            JsonNode.Parse(ref reader) is not { } node
            || node.GetValueKind() is not JsonValueKind.Object
            || node.AsObject().ToArray() is not [var propertyKvp]
        )
        {
            throw new JsonException();
        }

        switch (propertyKvp)
        {
            case { Key: "value" or "Value", Value: { } valueNode }
                when valueNode.ToJsonString() is { } jsonString
                     && JsonSerializer.Deserialize<T>(jsonString) is { } value:
            {
                return Result<T, TError>.Ok(value);
            }
            case { Key: "error" or "Error", Value: { } errorNode }
                when errorNode.ToJsonString() is { } jsonString
                     && JsonSerializer.Deserialize<TError>(jsonString) is { } error:
            {
                return Result<T, TError>.Error(error);
            }
            default:
            {
                throw new JsonException();
            }
        }
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        Result<T, TError> result,
        JsonSerializerOptions options
    )
    {
        writer.WriteStartObject();
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        var (unconvertedPropertyName, propertyValue) = result switch
        {
            { IsOk: true, Value: var value } => ("Value", (object)value),
            { IsError: true, ErrorValue: var error } => ("Error", error)
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).


        var propertyName = options
                               .PropertyNamingPolicy
                               ?.ConvertName(unconvertedPropertyName)
                           ?? unconvertedPropertyName;

        writer.WritePropertyName(propertyName);
        var serializeValue = JsonSerializer.Serialize(propertyValue);
        writer.WriteRawValue(serializeValue);
        writer.WriteEndObject();
    }

    private static TOut? ConvertFromNode<TOut>(JsonNode node)
    {
        switch (node.GetValueKind())
        {
            case JsonValueKind.Null or JsonValueKind.Undefined:
            {
                throw new JsonException();
            }
            case JsonValueKind.Number
                or JsonValueKind.String
                or JsonValueKind.True
                or JsonValueKind.False:
            {
                return node.GetValue<TOut>();
            }
            case JsonValueKind.Array or JsonValueKind.Object:
            {
                var jsonString = node.ToJsonString();
                return JsonSerializer.Deserialize<TOut>(jsonString);
            }
            default:
            {
                throw new JsonException();
            }
        }
    }
}