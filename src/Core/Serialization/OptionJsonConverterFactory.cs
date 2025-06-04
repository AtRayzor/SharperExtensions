using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharperExtensions.Serialization;

/// <summary>
/// Represents a JSON converter factory for Option types.
/// </summary>
public class OptionJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert is
               {
                   IsGenericType: true,
                   GenericTypeArguments: [var arg]
               }
               && typeToConvert == typeof(Option<>).MakeGenericType(arg);
    }

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        var valueConverter = GetValueConverter(valueType);

        var converterType = typeof(OptionJsonConverter<>).MakeGenericType(valueType);

        return (JsonConverter?)Activator.CreateInstance(converterType, valueConverter);
    }

    private static JsonConverter? GetValueConverter(Type valueType)
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