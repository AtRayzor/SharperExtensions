namespace NetFunctional.Types.Traits;

public interface IImplementsFunctor : IKind;

public interface IFunctor<out T> : IConstructableTrait<T>
    where T : IImplementsFunctor { }

public abstract class FunctorFactory<T> : TraitFactory<T, IFunctor<T>>
    where T : IImplementsFunctor;

public static class Functor
{
    internal static IFunctor<T> Construct<T, TFunctor>(T type)
        where TFunctor : struct, IFunctor<T>
        where T : IImplementsFunctor
    {
        return FunctorFactory<T>.Construct<TFunctor>(type);
    }

    internal static IFunctor<T> ConstructDefault<T>(T type)
        where T : IImplementsFunctor
    {
        return Construct<T, Functor<T>>(type);
    }
}

internal readonly record struct Functor<T>(T Type) : IFunctor<T>
    where T : IImplementsFunctor { }
