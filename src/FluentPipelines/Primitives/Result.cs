namespace FluentPipelines.Primitives;

public abstract record Result<TValue>
{
    public record Ok(TValue Value) : Result<TValue>;

    public record Error<TError>(TError ErrorOutput) : Result<TValue>;
}

public static class Results
{
    public static Result<TValue> Ok<TValue>(TValue value) => new Result<TValue>.Ok(value);
    public static Result<TValue> Error<TValue, TError>(TError error) => new Result<TValue>.Error<TError>(error);
}