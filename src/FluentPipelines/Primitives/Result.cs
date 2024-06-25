namespace FluentPipelines.Primitives;

public abstract record Result<T>
{
    public record Ok(T Value) : Result<T>;

    public record Error(string Message) : Result<T>;
}

public static class Results
{
    public static Result<TValue> Ok<TValue>(TValue value) => new Result<TValue>.Ok(value);
    public static Result<TValue> Error<TValue>(string message) => new Result<TValue>.Error(message);
}