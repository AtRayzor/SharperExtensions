namespace Monads.OptionMonad;

public static class OptionTypeExtensions
{
    public static Option<TValue> ToOption<TValue>(this TValue? value) where TValue : notnull
        => value;
}