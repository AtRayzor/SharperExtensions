using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Result;

public static partial class Result
{
    public static class Monad
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TNew, TError> Bind<T, TError, TNew>(
            Result<T, TError> result,
            Func<T, Result<TNew, TError>> binder
        )
            where T : notnull
            where TError : notnull
            where TNew : notnull
        {
            return result switch
            {
                Ok<T, TError> ok => binder(ok),
                Error<T, TError> error => new Error<TNew, TError>(error)
            };
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T, TError> Flatten<T, TError>(
            Result<Result<T, TError>, TError> wrappedResult
        )
            where T : notnull
            where TError : notnull
        {
            
            return wrappedResult switch
            {
                Ok<Result<T, TError>, TError> result => result.Value,
                Error<Result<T, TError>, TError> error => new Error<T, TError>(error)
            };
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<TNew, TError>> BindAsync<T, TError, TNew>(
            Task<Result<T, TError>> resultTask,
            Func<T, Task<Result<TNew, TError>>> binder
        )
            where T : notnull
            where TError : notnull
            where TNew : notnull
        {
            return await resultTask switch
            {
                Ok<T, TError> ok => await binder(ok),
                Error<T, TError> error => new Error<TNew, TError>(error)
            };
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> FlattenAsync<T, TError>(
            Task<Result<Task<Result<T, TError>>, TError>> wrappedResultTask
        )
            where T : notnull
            where TError : notnull
        {
            return await wrappedResultTask switch
            {
                Ok<Task<Result<T, TError>>, TError> ok => await ok.Value,
                Error<Task<Result<T, TError>>, TError> error => new Error<T, TError>(error)
            };
        }
    }
}

public static class ResultMonadExtensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TNew, TError> Bind<T, TError, TNew>(
        this Result<T, TError> result,
        Func<T, Result<TNew, TError>> binder
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull
    {
        return Result.Monad.Bind(result, binder);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> Flatten<T, TError>(
        this Result<Result<T, TError>, TError> wrappedResult
    )
        where T : notnull
        where TError : notnull
    {
        return Result.Monad.Flatten(wrappedResult);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TNew, TError>> BindAsync<T, TError, TNew>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task<Result<TNew, TError>>> binder
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull
    {
        return Result.Monad.BindAsync(resultTask, binder);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> FlattenAsync<T, TError>(
        this Task<Result<Task<Result<T, TError>>, TError>> wrappedResultTask
    )
        where T : notnull
        where TError : notnull
    {
        return Result.Monad.FlattenAsync(wrappedResultTask);
    }
}
