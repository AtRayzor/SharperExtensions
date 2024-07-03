namespace Monads.ResultMonad;

public static class AsyncResultExtensions
{
    public static async Task<Result<TNewValue, TNewError>> MapAsync<TValue, TError, TNewValue, TNewError>(
        this Task<Result<TValue, TError>> resultTask,
        Func<TValue, Task<Result<TNewValue, TNewError>>> fValue,
        Func<TError, Task<Result<TNewValue, TNewError>>> fError
    )
        where TValue : notnull
        where TError : notnull
        where TNewValue : notnull
        where TNewError : notnull
        => await (
            await resultTask switch
            {
                Result<TValue, TError>.Ok okValue => fValue(okValue),
                Result<TValue, TError>.Error error => fError(error)
            }
        );


    public static async Task<Result<TNewValue, TError>> MapAsync<TValue, TError, TNewValue>(
        this Task<Result<TValue, TError>> resultTask,
        Func<TValue, Task<Result<TNewValue, TError>>> fValue
    )
        where TValue : notnull
        where TError : notnull
        where TNewValue : notnull
        => await resultTask.MapAsync(fValue,
            error => Task.FromResult(Result.ReturnError<TNewValue, TError>(error)));

    public static async Task<Result<TValue, TNewError>> MapAsync<TValue, TError, TNewError>(
        this Task<Result<TValue, TError>> resultTask,
        Func<TError, Task<Result<TValue, TNewError>>> fError
    )
        where TValue : notnull
        where TError : notnull
        where TNewError : notnull
        => await resultTask.MapAsync(
            value => Task.FromResult(Result.ReturnOk<TValue, TNewError>(value)), fError);


    public static async Task<Result<TNewValue>> MapAsync<TValue, TError, TNewValue>(
        this Task<Result<TValue, TError>> resultTask,
        Func<TValue, Task<Result<TNewValue>>> fValue,
        Func<TError, Task<Result<TNewValue>>> fError
    )
        where TValue : notnull
        where TError : notnull
        where TNewValue : notnull
        => await (
            await resultTask switch
            {
                Result<TValue, TError>.Ok okValue => fValue(okValue),
                Result<TValue, TError>.Error error => fError(error)
            }
        );

    public static async Task<Result<TNewValue>> MapAsync<TValue, TNewValue>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<Result<TNewValue>>> fValue,
        Func<string, Task<Result<TNewValue>>> fError
    )
        where TValue : notnull
        where TNewValue : notnull
        => await (
            await resultTask switch
            {
                Result<TValue>.Ok okValue => fValue(okValue),
                Result<TValue>.Error error => fError(error)
            }
        );


    public static async Task<Result<TNewValue>> MapAsync<TValue, TNewValue>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<Result<TNewValue>>> fValue
    )
        where TValue : notnull
        where TNewValue : notnull
        => await resultTask.MapAsync(fValue,
            error => Task.FromResult(Result.ReturnError<TNewValue>(error)));

    public static async Task<Result<TValue>> MapAsync<TValue>(
        this Task<Result<TValue>> resultTask,
        Func<string, Task<Result<TValue>>> fError
    )
        where TValue : notnull
        => await resultTask.MapAsync(
            value => Task.FromResult(Result.ReturnOk(value)), fError);

    public static async Task<Result<TNewValue, TNewError>> MapAsync<TValue, TNewValue, TNewError>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<Result<TNewValue, TNewError>>> fValue,
        Func<string, Task<Result<TNewValue, TNewError>>> fError) where TNewValue : notnull
        where TNewError : notnull
        where TValue : notnull => await (await resultTask switch
    {
        Result<TValue>.Ok okValue => fValue(okValue),
        Result<TValue>.Error message => fError(message)
    });

    public static Task<Result<TValue, TError>> ToResultTask<TValue, TError>(this Result<TValue, TError> result)
        where TValue : notnull where TError : notnull => Task.FromResult(result);

    public static Task<Result<TValue>> ToResultTask<TValue>(this Result<TValue> result)
        where TValue : notnull => Task.FromResult(result);
}