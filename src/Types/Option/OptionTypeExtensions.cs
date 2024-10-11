namespace DotNetCoreFunctional.Option;

public static class 
    OptionTypeExtensions
{
    public static Option<T> ToOption<T>(this T? value)
        where T : notnull
    {
        return Option.Return(value);
    }

    public static Option<T> ToSome<T>(this T value)
        where T : notnull => Option.Some(value);

    public static IEnumerable<Option<T>> ToOptionEnumerable<T>(this IEnumerable<T?> enumerable)
        where T : notnull => enumerable.Select(val => val.ToOption());

    public static IEnumerable<Option<T>> ToSomeEnumerable<T>(this IEnumerable<T> enumerable)
        where T : notnull => enumerable.Select(val => val.ToSome());

    public static Option<TCast> CastToOption<T, TCast>(this T? value) where TCast : notnull
    {
        if (value is null)
        {
            return Option<TCast>.None;
        }
        object obj = value;

        try
        {
            return ((TCast)obj).ToOption();
        }
        catch (InvalidCastException)
        {
            return Option<TCast>.None;
        }
    }

}
