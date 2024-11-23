using System.Buffers.Text;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetCoreFunctional.Option.Serializaion;

public class OptionJsonConverter<T> : JsonConverter<Option<T>> where T : notnull
{
    public override Option<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        T? value = default;
        var valueFound = false;

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Option types are required to be serialized as an object");
        }

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            if (valueFound)
            {
                continue;
            }

            switch (reader)
            {
                case { TokenType: JsonTokenType.Comment }:
                case { TokenType: JsonTokenType.None }:
                    continue;
                case { TokenType: JsonTokenType.PropertyName }:
                    if (reader.GetString() is not ("Value" or "value"))
                    {
                        throw new JsonException("Invalid property");
                    }


                    reader.Read();

                    value = ReadValue(ref reader, typeof(T), options);
                    valueFound = true;
                    continue;
            }
        }

        return valueFound ? Option<T>.Some(value!) : Option<T>.None;
    }

    public T ReadValue(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        while (true)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.None:
                case JsonTokenType.Comment:
                    if (!reader.Read())
                    {
                        throw new JsonException("The value is empty.");
                    }

                    continue;

                case JsonTokenType.EndArray:
                case JsonTokenType.EndObject:
                    throw new JsonException("Invalid end of object or array");
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                    return ReadObjectOrArrayValue(ref reader, typeToConvert, options);
                case JsonTokenType.PropertyName:
                    throw new JsonException("A property name cannot occur before an object start");
                case JsonTokenType.Null:
                    throw new JsonException("An option value cannot be null.");
                case JsonTokenType.String:
                case JsonTokenType.Number:
                case JsonTokenType.True:
                case JsonTokenType.False:
                    return JsonSerializer.Deserialize<T>(reader.ValueSpan)
                           ?? throw new JsonException("The could not be deserialized");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public T ReadObjectOrArrayValue(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var terminationToken = reader.TokenType switch
        {
            JsonTokenType.StartArray => JsonTokenType.EndArray,
            JsonTokenType.StartObject => JsonTokenType.EndObject,
            _ => throw new JsonException("Invalid start token.")
        };

        JsonTokenType previousTokenType = default;
        List<char>? overflow = default;
        Span<char> buffer = stackalloc char[8192];

        buffer[0] = '{';
        var bufferIndex = 1;
        var baselineDepth = reader.CurrentDepth;

        while (reader.Read())
        {

            // Remove trailing commas.
            if (reader.TokenType is JsonTokenType.EndArray or JsonTokenType.EndObject)
            {
                if (overflow is [_, .., ','])
                {
                    overflow.Slice(0, overflow.Count - 1);
                }

                if (buffer[..bufferIndex] is [_, .., ','])
                {
                    buffer[--bufferIndex] = '\0';
                }
            }

            var addSeparator = previousTokenType is JsonTokenType.PropertyName
                               && reader.TokenType
                                   is JsonTokenType.String
                                   or JsonTokenType.Number
                                   or JsonTokenType.True
                                   or JsonTokenType.False
                                   or JsonTokenType.EndArray
                                   or JsonTokenType.EndObject;

            ReadOnlySpan<char> valueRead = reader.TokenType switch
            {
                JsonTokenType.PropertyName => $"\"{reader.GetString()}\":",
                JsonTokenType.String => $"\"{reader.GetString()}\"",
                _ => Encoding.UTF8.GetChars([.. reader.ValueSpan])
            };

            char[] separator = addSeparator ? [','] : [];
            var totalBufferUsed = bufferIndex + valueRead.Length + separator.Length;

            if (totalBufferUsed > buffer.Length)
            {
                overflow ??= [.. buffer];
                overflow = [.. overflow, .. valueRead, .. separator];
                continue;
            }

            valueRead.CopyTo(buffer.Slice(bufferIndex, valueRead.Length));
            bufferIndex += valueRead.Length;
            separator.CopyTo(buffer.Slice(bufferIndex, 1));
            bufferIndex += separator.Length;
            
            
            if (reader.TokenType == terminationToken && reader.CurrentDepth == baselineDepth)
            {
                break;
            }

            previousTokenType = reader.TokenType;
        }

        if (bufferIndex == 0 && overflow is null or [])
        {
        }

        ReadOnlySpan<char> valueBytes = overflow is null ? buffer[.. bufferIndex] : [.. overflow];

        if (JsonSerializer.Deserialize<T>(valueBytes, options) is not { } value)
        {
            throw new JsonException("The value could not be deserialized");
        }

        return value;
    }


    public override void Write(Utf8JsonWriter writer, Option<T> option, JsonSerializerOptions serializerOptions)
    {
        switch (option)
        {
            case Some<T> { Value: var value }:
                var serializedValue = JsonSerializer.Serialize(value, serializerOptions);
                const string propertyName = "Value";
                var serializedPropertyName = serializerOptions
                                                 .PropertyNamingPolicy
                                                 ?.ConvertName(propertyName)
                                             ?? propertyName;

                writer.WriteStartObject();
                writer.WritePropertyName(serializedPropertyName);
                writer.WriteRawValue(serializedValue);
                writer.WriteEndObject();
                break;
            case None<T>:
                writer.WriteStartObject();
                writer.WriteEndObject();
                break;
        }
    }
}