using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetCoreFunctional.Option.Serializaion;

public class OptionJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert is { IsGenericType: true, GenericTypeArguments: [var typeArg] }
            && typeof(Option<>).MakeGenericType(typeArg).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter? CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(OptionJsonConverter<>).MakeGenericType(valueType);

        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}
