namespace Monads.Traits;

public interface IImplementsApplicative : IKind;

public interface IApplicative<out T> : ITrait<T> where T : IImplementsApplicative
{
}

public abstract class ApplicativeFactory<T> : TraitFactory<T, IApplicative<T>> where T : IImplementsApplicative;

public static partial class Applicative
{
    internal static IApplicative<T> Construct<T, TApplicative>(T type)
        where TApplicative : struct, IApplicative<T>
        where T : IImplementsApplicative
        => ApplicativeFactory<T>.Construct<TApplicative>(type);

    internal static IApplicative<T> ConstructDefault<T>(T type) where T : IImplementsFunctor, IImplementsApplicative =>
        Construct<T, Applicative<T>>(type);
}

internal readonly record struct Applicative<T>(T Type) : IApplicative<T> where T : IImplementsApplicative;