using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DotNetCoreFunctional.Option;

public static class OptionTypeExtensions
{

    public static IEnumerable<Option<T>> ToOptionEnumerable<T>(this IEnumerable<T?> enumerable)
        where T : notnull => enumerable.Select(val => val.ToOption());

    public static IEnumerable<Option<T>> ToSomeEnumerable<T>(this IEnumerable<T> enumerable)
        where T : notnull => enumerable.Select(val => val.ToOption());

    public static Option<TCast> CastToOption<T, TCast>(this T? value)
        where TCast : notnull
    {
        switch (value)
        {
            case null:
                return Option<TCast>.None;
            case TCast castTypeValue:
                return castTypeValue;
            default:

                if (TryGetMethodInfo(value.GetType(), typeof(TCast), out var methodInfo))
                {
                    return ((TCast?)methodInfo.Invoke(value, [value])).ToOption();
                }

                try
                {
                    return ((TCast?)(object?)value).ToOption();
                }
                catch (InvalidCastException)
                {
                    return Option<TCast>.None;
                }
        }
    }

    private static bool TryGetMethodInfo(
        Type valueType,
        Type castType,
        [NotNullWhen(true)] out MethodInfo? methodInfo
    )
    {
        methodInfo = valueType
            .GetMethods()
            .FirstOrDefault(m =>
                m is { Name: "op_Implicit" or "op_Explicit", ReturnType: var returnType }
                && returnType == castType
                && m.GetParameters() is [{ ParameterType: var parameterType }]
                && parameterType == valueType
            );

        return methodInfo is not null;
    }

    private static Option<TCast> CastToReferenceTypeOption<T, TCast>(this T? value)
        where TCast : class => (value as TCast).ToOption();

    public static Option<TCast> CastValueToOption<T, TCast>(this T? value)
        where TCast : notnull, T => ((TCast?)value).ToOption();

    private static Option<TCast> CastInnerValue<T, TCast>(this Option<T> option)
        where TCast : notnull
        where T : notnull =>
        option switch
        {
            Some<T> some => some.Value.CastToOption<T, TCast>(),
            _ => Option<TCast>.None,
        };
}
