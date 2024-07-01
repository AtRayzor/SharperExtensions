namespace Monads.ResultMonad;

public static class ReturnResultExtensions
{
    public static Result<TValue> ReturnResult<TValue>(this TValue? value) where TValue : notnull
        => Result.Return(value);
    
    public static Result<TValue> ReturnResult<TValue>(this TValue? value, string errorMessage) where TValue : notnull
        => Result.Return(value, errorMessage);

    public static Result<TValue> ReturnOk<TValue>(this TValue value) where TValue : notnull
        => Result.ReturnOk(value);

    public static Result<TValue> ReturnError<TValue>(this string message)
        where TValue : notnull => Result.ReturnError<TValue>(message);
}