using Monads.Traits;

namespace Monads.ResultMonad;

public abstract record ResultType<T, TE> : IImplementsFunctor, IImplementsMonad where T : notnull where TE : notnull
{
    public record Ok(T Value) : ResultType<T, TE>;

    public record Error(TE Err) : ResultType<T, TE>;
}

public abstract record ResultType<T> : IImplementsFunctor, IImplementsMonad where T : notnull
{
    internal abstract ResultType<T, string> BaseResultType { get; }

    public record Ok(T Value) : ResultType<T>
    {
        internal override ResultType<T, string> BaseResultType => new ResultType<T, string>.Ok(Value);
    }

    public record Error(string Message) : ResultType<T>
    {
        internal override ResultType<T, string> BaseResultType => new ResultType<T, string>.Error(Message);
    }
}