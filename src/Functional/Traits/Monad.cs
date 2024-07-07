using Monads.OptionKind;

namespace Monads.Traits;

public interface IImplementsMonad : IKind;

public interface IMonad<out T> : ITrait<T> where T : IImplementsMonad;

public abstract class MonadFactory<T> : TraitFactory<T, IMonad<T>> where T : IImplementsMonad;

public static partial class Monad
{
    internal static IMonad<T> Construct<T, TMonad>(T type) where TMonad : struct, IMonad<T> where T : IImplementsMonad
        => MonadFactory<T>.Construct<TMonad>(type);

    internal static IMonad<T> ConstructDefault<T>(T type) where T : IImplementsMonad
        => Construct<T, Monad<T>>(type);
}

internal readonly record struct Monad<T>(T Type) : IMonad<T> where T : IImplementsMonad;