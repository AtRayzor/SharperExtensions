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
            where TNew : notnull =>
            result switch
            {
                Ok<T, TError> ok => binder(ok),
                Error<T, TError> error => new Error<TNew, TError>(error)
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T, TError> Flatten<T, TError>(
            Result<Result<T, TError>, TError> wrappedResult
        )
            where T : notnull
            where TError : notnull =>
            wrappedResult switch
            {
                Ok<Result<T, TError>, TError> result => result.Value,
                Error<Result<T, TError>, TError> error => new Error<T, TError>(error)
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<TNew, TError>> BindAsync<T, TError, TNew>(
            Task<Result<T, TError>> resultTask,
            Func<T, CancellationToken, Task<Result<TNew, TError>>> binder,
            CancellationToken cancellationToken
        )
            where T : notnull
            where TError : notnull
            where TNew : notnull =>
            await resultTask switch
            {
                Ok<T, TError> ok => await binder(ok, cancellationToken),
                Error<T, TError> error => new Error<TNew, TError>(error)
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<TNew, TError>> BindAsync<T, TError, TNew>(
            Task<Result<T, TError>> resultTask,
            Func<T, Task<Result<TNew, TError>>> binder
        )
            where T : notnull
            where TError : notnull
            where TNew : notnull => BindAsync(resultTask, (value, _) => binder(value), default);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TError>> FlattenAsync<T, TError>(
            Task<Result<Task<Result<T, TError>>, TError>> wrappedResultTask
        )
            where T : notnull
            where TError : notnull =>
            await wrappedResultTask switch
            {
                Ok<Task<Result<T, TError>>, TError> ok => await ok.Value,
                Error<Task<Result<T, TError>>, TError> error => new Error<T, TError>(error)
            };
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
        where TNew : notnull => Result.Monad.Bind(result, binder);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> Flatten<T, TError>(
        this Result<Result<T, TError>, TError> wrappedResult
    )
        where T : notnull
        where TError : notnull => Result.Monad.Flatten(wrappedResult);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TNew, TError>> BindAsync<T, TError, TNew>(
        this Task<Result<T, TError>> resultTask,
        Func<T, Task<Result<TNew, TError>>> binder
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull => Result.Monad.BindAsync(resultTask, binder);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TNew, TError>> BindAsync<T, TError, TNew>(
        this Task<Result<T, TError>> resultTask,
        Func<T, CancellationToken, Task<Result<TNew, TError>>> binder,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull => Result.Monad.BindAsync(resultTask, binder, cancellationToken);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TError>> FlattenAsync<T, TError>(
        this Task<Result<Task<Result<T, TError>>, TError>> wrappedResultTask
    )
        where T : notnull
        where TError : notnull => Result.Monad.FlattenAsync(wrappedResultTask);
}
