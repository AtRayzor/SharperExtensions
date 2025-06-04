using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharperExtensions.Serialization;

/// <summary>
/// A JSON converter factory for creating converters for Result types with generic value and error types.
/// </summary>
/// <remarks>
/// This factory enables custom JSON serialization and deserialization for Result types 
/// by dynamically creating appropriate JSON converters based on the generic type arguments.
/// </remarks>
public class ResultJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc /> 
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert is
               {
                   IsGenericType: true,
                   GenericTypeArguments: [var valueArg, var errorArg]
               }
               && typeToConvert == typeof(Result<,>).MakeGenericType(valueArg, errorArg);
    }

    /// <inheritdoc /> 
    public override JsonConverter? CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var args = typeToConvert.GenericTypeArguments[..1];
        var converterType = typeof(ResultJsonConverter<,>).MakeGenericType(args);

        return (JsonConverter?)Activator.CreateInstance(converterType);
    }

    private static JsonConverter? GetJsonConverter(Type valueType)
    {
        if (
            valueType
                .GetCustomAttributes(typeof(JsonConverterAttribute), false)
                .OfType<JsonConverterAttribute>()
                .SingleOrDefault() is not { } valueConverterAttribute
            || valueConverterAttribute.CreateConverter(valueType)
                is not { } valueConverter
        )
        {
            return null;
        }

        var converterType = valueConverter.GetType();

        if (
            !converterType.IsGenericType
            || converterType.GenericTypeArguments is not [var arg]
            || converterType.IsAssignableTo(typeof(JsonConverter).MakeGenericType(arg))
        )
        {
            throw new JsonException("The converter type does not match the given value.");
        }

        return valueConverter;
    }
}