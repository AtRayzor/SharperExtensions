namespace DotNetCoreFunctional.Option;

public static class OptionTypeExtensions
{
    public static Option<T> ToOption<T>(this T? value)
        where T : notnull
    {
        return Option.Return(value);
    }

    public static Option<T> ToSome<T>(this T value)
        where T : notnull
    {
        return Option.Some(value);
    }

    public static IEnumerable<Option<T>> ToOptionEnumerable<T>(this IEnumerable<T?> enumerable)
        where T : notnull
    {
        return enumerable.Select(val => val.ToOption());
    }

    public static IEnumerable<Option<T>> ToSomeEnumerable<T>(this IEnumerable<T> enumerable)
        where T : notnull
    {
        return enumerable.Select(val => val.ToSome());
    }

    public static Option<TCast> CastToOption<T, TCast>(this T? value)
        where T : notnull
        where TCast : notnull
    {
        try
        {
            return ((TCast?)Convert.ChangeType(value, typeof(TCast))).ToOption();
        }
        catch (InvalidCastException)
        {
            return new None<TCast>();
        }
    }
}
