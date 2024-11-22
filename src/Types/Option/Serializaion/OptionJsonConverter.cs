using System.Buffers.Text;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetCoreFunctional.Option.Serializaion;

public class OptionJsonConverter<T> : JsonConverter<Option<T>> where T : notnull
{
    public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonTokenType previousTokenType = default;
        List<char>? overflow = default;
        Span<char> buffer = stackalloc char[8192];
        var bufferIndex = 0;


        while (reader.Read())
        {
            if (reader is { TokenType: JsonTokenType.EndArray, CurrentDepth: 0 })
            {
                break;
            }

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

            previousTokenType = reader.TokenType;
        }

        if (buffer.IsEmpty && overflow is null or [])
        {
            return new None<T>();
        }

        ReadOnlySpan<char> valueBytes = overflow is null ? buffer[.. bufferIndex] : [.. overflow];

        if (JsonSerializer.Deserialize<T>(valueBytes, options) is not { } value)
        {
            throw new JsonException("The value could not be deserialized");
        }

        return Option<T>.Some(value);
    }

    public override void Write(Utf8JsonWriter writer, Option<T> option, JsonSerializerOptions serializerOptions)
    {
        switch (option)
        {
            case Some<T> { Value: var value }:
                try
                {
                    var serializedValue = JsonSerializer.Serialize(value, serializerOptions);

                    writer.WriteStartArray();
                    writer.WriteRawValue(serializedValue);
                    writer.WriteEndArray();
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                break;

            case None<T>:
                writer.WriteStartArray();
                writer.WriteEndArray();
                break;
        }
    }
}