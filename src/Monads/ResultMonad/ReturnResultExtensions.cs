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
    public
 static Task<Result<TValue>> ReturnResultTask<TValue>(this TValue? value) where TValue : notnull =>
        Task.FromResult(value.ReturnResult());

    public static Task<Result<TValue>> ReturnResultTask<TValue>(this TValue? value, string errorMessage)
        where TValue : notnull =>
        Task.FromResult(value.ReturnResult(errorMessage));
    
    public static Task<Result<TValue>> ReturnErrorTask<TValue>(this string message)
        where TValue : notnull => Task.FromResult(Result.ReturnError<TValue>(message));
    
    public static Task<Result<TValue>> ReturnOkTask<TValue>(this TValue value) where TValue : notnull
        =>Task.FromResult(Result.ReturnOk(value));

}