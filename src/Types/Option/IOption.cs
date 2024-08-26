namespace NetFunctional.Types;

public interface IOption<out T> : IEnumerable<T> where T : notnull;

public static class OptionInterfaceExtensions
{
    public static Option<T> ToOption<T>(this IOption<T> option) where T : notnull
    => option as Option<T> ?? Option<T>.None;
}