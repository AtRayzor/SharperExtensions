namespace SharperExtensions;

public interface IOption<out T>
    where T : notnull;

public static class OptionInterfaceExtensions
{
    public static Option<T> ToOption<T>(this IOption<T> option)
        where T : notnull
    {
        return option as Option<T> ?? Option<T>.None;
    }
}
