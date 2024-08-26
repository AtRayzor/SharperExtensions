namespace NetFunctional.Types.Traits;

public interface IImplementsMonad : IKind;

public interface IMonad<out T> : IConstructableTrait<T>
    where T : IImplementsMonad;

public abstract class MonadFactory<T> : TraitFactory<T, IMonad<T>>
    where T : IImplementsMonad;

public static class Monad
{
    internal static IMonad<T> Construct<T, TMonad>(T type)
        where TMonad : struct, IMonad<T>
        where T : IImplementsMonad
    {
        return MonadFactory<T>.Construct<TMonad>(type);
    }

    internal static IMonad<T> ConstructDefault<T>(T type)
        where T : IImplementsMonad
    {
        return Construct<T, Monad<T>>(type);
    }
}

internal readonly record struct Monad<T>(T Type) : IMonad<T>
    where T : IImplementsMonad;
