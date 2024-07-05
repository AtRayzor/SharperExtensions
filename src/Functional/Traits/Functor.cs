using Monads.OptionKind;

namespace Monads.Traits;

public interface IImplementsFunctor : IKind;

public interface IFunctor<T> : IConstructableTrait<T> where T : IImplementsFunctor
{
    static IConstructableTrait<T> IConstructableTrait<T>.Construct(T type) => Construct(type);
    new static IFunctor<T> Construct(T type) => new Functor<T>(type);
}

public static class Functor
{
    public static class Option
    {
        public static IFunctor<OptionType<T>> Create<T>(T? value) where T : notnull
            => OptionTraitExtension.Create<T, IFunctor<OptionType<T>>>(value);

        public static IFunctor<OptionType<T>> Some<T>(T value) where T : notnull
            => OptionTraitExtension.Some<T, IFunctor<OptionType<T>>>(value);

        public static IFunctor<OptionType<T>> None<T>() where T : notnull
            => OptionTraitExtension.None<T, IFunctor<OptionType<T>>>();
    }
}

internal readonly record struct Functor<T>(T Type) : IFunctor<T> where T : IImplementsFunctor
{
    public static implicit operator Functor<T>(T kind) => new(kind);
    
}