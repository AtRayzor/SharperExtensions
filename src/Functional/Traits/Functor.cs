using Monads.OptionKind;

namespace Monads.Traits;

public interface IImplementsFunctor : IKind;


public interface IFunctor<out T> : IConstructableTrait<T> where T : IImplementsFunctor
{
}

public abstract class FunctorFactory<T> : TraitFactory<T, IFunctor<T>> where T : IImplementsFunctor;

public static partial class Functor
{
    internal static IFunctor<T> Construct<T, TFunctor>(T type)
        where TFunctor : struct, IFunctor<T>
        where T : IImplementsFunctor
        => FunctorFactory<T>.Construct<TFunctor>(type);
    internal static IFunctor<T> ConstructDefault<T>(T type) where T : IImplementsFunctor
        => Construct<T, Functor<T>>(type);
}

internal readonly record struct Functor<T>(T Type)
    : IFunctor<T> where T : IImplementsFunctor
{
}