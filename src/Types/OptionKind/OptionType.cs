using NetFunctional.Types.Traits;

namespace NetFunctional.Types.OptionKind;

public abstract record OptionType<T>
    : IImplementsApplicative,
        IImplementsFunctor,
        IImplementsMonad,
        IImplementsSideEffects
    where T : notnull
{
    public static implicit operator OptionType<T>(T? value)
    {
        return value is not null ? new Some(value) : new None();
    }

    public record Some(T Value) : OptionType<T>
    {
        public static implicit operator T(Some some)
        {
            return some.Value;
        }
    }

    public record None : OptionType<T>;
}
