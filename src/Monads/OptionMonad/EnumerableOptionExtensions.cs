namespace Monads.OptionMonad;

public static class EnumerableOptionExtensions
{
    public static IEnumerable<TValue> SelectSome<TValue>(this IEnumerable<Option<TValue>> options)
        where TValue : notnull
        => options.Where(o => o is Option<TValue>.Some)
            .Select(o => ((Option<TValue>.Some)o).Value);
}