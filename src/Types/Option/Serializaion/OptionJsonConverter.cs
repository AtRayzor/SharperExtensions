using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetCoreFunctional.Option.Serializaion;

public class OptionJsonConverter<T> : JsonConverter<Option<T>> where T : notnull
{
    public override Option<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        List<byte>? overflow = default;
        Span<byte> buffer = stackalloc byte[16384];
        var bufferIndex = 0; 
        
        
        while (reader.Read())
        {
            if (reader is { TokenType: JsonTokenType.EndArray, CurrentDepth: 0 })
            {
                break;
            }
            
            
            var valueRead = reader.ValueSpan;
            var totalBytesConsumed = bufferIndex + valueRead.Length;

            if (totalBytesConsumed > buffer.Length)
            {
                overflow ??= [.. buffer];
                overflow = [.. overflow, .. valueRead];
                continue;
            }

            valueRead.CopyTo(buffer.Slice(bufferIndex, valueRead.Length));
            bufferIndex += valueRead.Length;
        }

        if (buffer.IsEmpty)
        {
            return new None<T>();
        }

        ReadOnlySpan<byte> valueBytes = overflow is null ? buffer[.. bufferIndex] : [.. overflow];

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