using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SharperExtensions.Serialization;

    /// <summary>
    /// Represents a JSON converter for handling Option types with optional custom value conversion.
    /// </summary>
    /// <typeparam name="T">The underlying type of the Option being serialized or deserialized.</typeparam>
    /// <remarks>
    /// Allows custom JSON conversion for Option types with an optional underlying value converter.
    /// </remarks>
    public class OptionJsonConverter<T>(JsonConverter<T>? valueConverter)
    : JsonConverter<Option<T>>
    where T : notnull
{
    
    /// <inheritdoc />
    public override Option<T> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (valueConverter is not null)
        {
            return valueConverter.Read(ref reader, typeof(T), options);
        }

        try
        {
            if (
                JsonNode.Parse(ref reader) is not { } node
            )

            {
                return Option<T>.None;
            }

            switch (node.GetValueKind())
            {
                case JsonValueKind.Null or JsonValueKind.Undefined:
                {
                    return Option<T>.None;
                }
                case JsonValueKind.Number
                    or JsonValueKind.String
                    or JsonValueKind.True
                    or JsonValueKind.False:
                {
                    return node.GetValue<T>();
                }
                case JsonValueKind.Object or JsonValueKind.Array:
                {
                    var jsonString = node.ToJsonString();
                    return JsonSerializer.Deserialize<T>(jsonString);
                }
                default:
                {
                    return Option<T>.None;
                }
            }
        }
        catch (Exception)
        {
            return Option<T>.None;
        }
    }
    
    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        Option<T> option,
        JsonSerializerOptions serializerOptions
    )
    {
        if (option is not { IsSome: true, Value: var value })
        {
            writer.WriteNullValue();
            return;
        }

        if (valueConverter is not null)
        {
            valueConverter.Write(writer, value, serializerOptions);
        }

        var serialized = JsonSerializer.Serialize(value);
        writer.WriteRawValue(serialized);
    }
}