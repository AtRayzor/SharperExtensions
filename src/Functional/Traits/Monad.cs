using Monads.OptionKind;

namespace Monads.Traits;

public interface IImplementsMonad : IKind;

public interface IMonad<T> : IConstructableTrait<T> where T : IImplementsMonad
{
    static IConstructableTrait<T> IConstructableTrait<T>.Construct(T type) => Create(type);
    new static IMonad<T> Create(T type) => new Monad<T>(type);
}

public static class Monad
{
    public static class Option
    {
        public static IMonad<OptionType<T>> Create<T>(T? value) where T : notnull =>
            OptionTraitExtension.Create<T, IMonad<OptionType<T>>>(value);

        public static IMonad<OptionType<T>> Some<T>(T value) where T : notnull =>
            OptionTraitExtension.Some<T, IMonad<OptionType<T>>>(value);

        public static IMonad<OptionType<T>> None<T>() where T : notnull =>
            OptionTraitExtension.None<T, IMonad<OptionType<T>>>();
    }
}

internal readonly record struct Monad<T>(T Type) : IMonad<T> where T : IImplementsMonad;