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
            Result<Task<T>, TError> taskResult
        )
            where T : notnull where TError : notnull
            => taskResult switch
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
        public static async Task<Result<T, TError>> AwaitIfOk<T, TError>(
            Task<Result<Task<T>, TError>> task
        )
            where T : notnull
            where TError : notnull
            => await (await task.ConfigureAwait(ConfigureAwaitOptions.None))
                .AwaitIfOk()
                .ConfigureAwait(ConfigureAwaitOptions.None);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> AwaitIfOk<T, TError>(
            Task<Result<ValueTask<T>, TError>> task
        )
            where T : notnull
            where TError : notnull
            => await (await task.ConfigureAwait(false))
                .AwaitIfOk()
                .ConfigureAwait(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Result<T, TError>> AwaitIfOk<T, TError>(
            ValueTask<Result<ValueTask<T>, TError>> task
        )
            where T : notnull
            where TError : notnull
            => await (await task.ConfigureAwait(false))
                .AwaitIfOk()
                .ConfigureAwait(false);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Result<T, TError>> AwaitIfOk<T, TError>(
            ValueTask<Result<Task<T>, TError>> task
        )
            where T : notnull
            where TError : notnull
            => await (await task.ConfigureAwait(false))
                .AwaitIfOk()
                .ConfigureAwait(ConfigureAwaitOptions.None);

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
            Result<T, Task<TError>> taskResult
        )
            where T : notnull where TError : notnull
            => taskResult switch
            {
                Ok<T, Task<TError>> okValue
                    => Result<T, TError>.Ok(okValue.Value),
                Error<T, Task<TError>> errorValueTask
                    => Result<T, TError>.Error(
                        await errorValueTask.Err.ConfigureAwait(ConfigureAwaitOptions.None)
                    )
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> AwaitIfError<T, TError>(
            Task<Result<T, Task<TError>>> task
        ) where T : notnull where TError : notnull
            => await (await task.ConfigureAwait(ConfigureAwaitOptions.None))
                .AwaitIfError()
                .ConfigureAwait(ConfigureAwaitOptions.None);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> AwaitIfError<T, TError>(
            Task<Result<T, ValueTask<TError>>> task
        ) where T : notnull where TError : notnull
            => await (await task.ConfigureAwait(ConfigureAwaitOptions.None))
                .AwaitIfError()
                .ConfigureAwait(false);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Result<T, TError>> AwaitIfError<T, TError>(
            ValueTask<Result<T, Task<TError>>> task
        ) where T : notnull where TError : notnull
            => await (await task.ConfigureAwait(false))
                .AwaitIfError()
                .ConfigureAwait(ConfigureAwaitOptions.None);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Result<T, TError>> AwaitIfError<T, TError>(
            ValueTask<Result<T, ValueTask<TError>>> task
        ) where T : notnull where TError : notnull
            => await (await task.ConfigureAwait(false))
                .AwaitIfError()
                .ConfigureAwait(false);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Result<T, TError>> Await<T, TError>(
            Result<ValueTask<T>, ValueTask<TError>> valueTaskResult
        )
            where T : notnull where TError : notnull
            => await AwaitIfError(await AwaitIfOk(valueTaskResult))
                .ConfigureAwait(false);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> Await<T, TError>(
            Result<Task<T>, Task<TError>> taskResult
        )
            where T : notnull where TError : notnull
            => await AwaitIfError(await AwaitIfOk(taskResult))
                .ConfigureAwait(ConfigureAwaitOptions.None);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> Await<T, TError>(
            Task<Result<Task<T>, Task<TError>>> task
        )
            where T : notnull where TError : notnull
            => await Await(await task.ConfigureAwait(ConfigureAwaitOptions.None))
                .ConfigureAwait(ConfigureAwaitOptions.None);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> Await<T, TError>(
            Task<Result<ValueTask<T>, ValueTask<TError>>> task
        )
            where T : notnull where TError : notnull
            => await Await(await task.ConfigureAwait(ConfigureAwaitOptions.None))
                .ConfigureAwait(false);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Result<T, TError>> Await<T, TError>(
            ValueTask<Result<Task<T>, Task<TError>>> task
        )
            where T : notnull where TError : notnull
            => await Await(await task.ConfigureAwait(false))
                .ConfigureAwait(ConfigureAwaitOptions.None);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Result<T, TError>> Await<T, TError>(
            ValueTask<Result<ValueTask<T>, ValueTask<TError>>> task
        )
            where T : notnull where TError : notnull
            => await Await(await task.ConfigureAwait(false))
                .ConfigureAwait(false);
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
        this Task<Result<ValueTask<T>, TError>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfOk(valueTaskResult);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> AwaitIfOk<T, TError>(
        this Task<Result<Task<T>, TError>> task
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfOk(task);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> AwaitIfOk<T, TError>(
        this ValueTask<Result<Task<T>, TError>> task
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfOk(task);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> AwaitIfOk<T, TError>(
        this ValueTask<Result<ValueTask<T>, TError>> task
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfOk(task);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> AwaitIfOk<T, TError>(
        this Result<Task<T>, TError> taskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfOk(taskResult);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> AwaitIfError<T, TError>(
        this Result<T, ValueTask<TError>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfError(valueTaskResult);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> AwaitIfError<T, TError>(
        this Result<T, Task<TError>> taskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfError(taskResult);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> AwaitIfError<T, TError>(
        this Task<Result<T, ValueTask<TError>>> task
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfError(task);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> AwaitIfError<T, TError>(
        this ValueTask<Result<T, Task<TError>>> task
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfError(task);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> AwaitIfError<T, TError>(
        this ValueTask<Result<T, ValueTask<TError>>> task
    )
        where T : notnull where TError : notnull
        => Result.Async.AwaitIfError(task);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> Await<T, TError>(
        this Result<ValueTask<T>, ValueTask<TError>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.Await(valueTaskResult);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> Await<T, TError>(
        this Result<Task<T>, Task<TError>> taskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.Await(taskResult);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> Await<T, TError>(
        this Task<Result<ValueTask<T>, ValueTask<TError>>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.Await(valueTaskResult);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> Await<T, TError>(
        this Task<Result<Task<T>, Task<TError>>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.Await(valueTaskResult);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> Await<T, TError>(
        this ValueTask<Result<ValueTask<T>, ValueTask<TError>>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.Await(valueTaskResult);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Result<T, TError>> Await<T, TError>(
        this ValueTask<Result<Task<T>, Task<TError>>> valueTaskResult
    )
        where T : notnull where TError : notnull
        => Result.Async.Await(valueTaskResult);
}