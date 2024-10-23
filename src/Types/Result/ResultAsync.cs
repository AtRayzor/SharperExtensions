using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Result;

public static partial class Result
{
    public static class Async
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Result<T, TError>> AwaitIfOk<T, TError>(
            Result<ValueTask<T>, TError> valueTaskResult
        )
            where T : notnull where TError : notnull
            => valueTaskResult switch
            {
                Ok<ValueTask<T>, TError> okValueTask
                    => Result<T, TError>.Ok(
                        await okValueTask
                            .Value
                            .ConfigureAwait(false)
                    ),
                Error<ValueTask<T>, TError> error
                    => Result<T, TError>.Error(error)
            };


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> AwaitIfOk<T, TError>(
            Result<Task<T>, TError> valueTaskResult
        )
            where T : notnull where TError : notnull
            => valueTaskResult switch
            {
                Ok<Task<T>, TError> okValueTask
                    => Result<T, TError>.Ok(
                        await okValueTask
                            .Value
                            .ConfigureAwait(ConfigureAwaitOptions.None)
                    ),
                Error<Task<T>, TError> error
                    => Result<T, TError>.Error(error)
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Result<T, TError>> AwaitIfError<T, TError>(
            Result<T, ValueTask<TError>> valueTaskResult
        )
            where T : notnull where TError : notnull
            => valueTaskResult switch
            {
                Ok<T, ValueTask<TError>> okValue
                    => Result<T, TError>.Ok(okValue.Value),
                Error<T, ValueTask<TError>> errorValueTask
                    => Result<T, TError>.Error(
                        await errorValueTask.Err.ConfigureAwait(false)
                    )
            };


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> AwaitIfError<T, TError>(
            Result<T, Task<TError>> valueTaskResult
        )
            where T : notnull where TError : notnull
            => valueTaskResult switch
            {
                Ok<T, Task<TError>> okValue
                    => Result<T, TError>.Ok(okValue.Value),
                Error<T, Task<TError>> errorValueTask
                    => Result<T, TError>.Error(
                        await errorValueTask.Err.ConfigureAwait(ConfigureAwaitOptions.None)
                    )
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Result<T, TError>> Await<T, TError>(
            Result<ValueTask<T>, ValueTask<TError>> valueTaskResult
        )
            where T : notnull where TError : notnull
            => await AwaitIfError(await AwaitIfOk(valueTaskResult))
                .ConfigureAwait(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> Await<T, TError>(
            Result<Task<T>, Task<TError>> valueTaskResult
        )
            where T : notnull where TError : notnull
            => await AwaitIfError(await AwaitIfOk(valueTaskResult))
                .ConfigureAwait(ConfigureAwaitOptions.None);
    }
}

public static class ResultAsyncExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> AwaitIfOk<T, TError>(
        this Result<ValueTask<T>, TError> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfOk(valueTaskResult);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> AwaitIfOk<T, TError>(
        this Result<Task<T>, TError> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfOk(valueTaskResult);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> AwaitIfError<T, TError>(
        this Result<T, ValueTask<TError>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfError(valueTaskResult);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> AwaitIfError<T, TError>(
        this Result<T, Task<TError>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfError(valueTaskResult);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> Await<T, TError>(
        this Result<ValueTask<T>, ValueTask<TError>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.Await(valueTaskResult);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> Await<T, TError>(
        this Result<Task<T>, Task<TError>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.Await(valueTaskResult);
}