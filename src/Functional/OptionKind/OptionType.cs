using Monads.Traits;

namespace Monads.OptionKind;

public abstract record OptionType<T> : IImplementsApplicative, IImplementsFunctor, IImplementsMonad,
    IImplementsSideEffects where T : notnull
{
    public record Some(T Value) : OptionType<T>
    {
        public static implicit operator T(Some some) => some.Value;
    }

    public static implicit operator OptionType<T>(T? value) =>
        value is not null ? new Some(value) : new None();

    public record None : OptionType<T>;
}