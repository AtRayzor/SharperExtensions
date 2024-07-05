using Monads.Traits;

namespace Monads.ResultMonad;

public abstract record ResultType<T, TE> : IImplementsFunctor, IImplementsMonad where T : notnull where TE : notnull
{
    public record Ok(T Value) : ResultType<T, TE>;

    public record Error(TE Err) : ResultType<T, TE>;
}

public abstract record ResultType<T> : ResultType<T, string> where T : notnull;