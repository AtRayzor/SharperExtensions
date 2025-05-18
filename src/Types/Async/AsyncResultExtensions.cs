using System.Data;
using DotNetCoreFunctional.Result;

namespace DotNetCoreFunctional.Async;

/// <summary>
/// Provides extension methods for <see cref="AsyncResult{T, TError}"/> to enable fluent and convenient usage.
/// </summary>
public static class AsyncResultExtensions
{
    extension<T>(Task<T?> task)
        where T : notnull
    {
        public AsyncResult<T, TError> ToAsyncResult<TError>(TError nullError)
            where TError : notnull => AsyncResult.LiftFromTask(
            task,
            nullError,
            CancellationToken.None
        );

        public AsyncResult<T, TError> ToAsyncResult<TError>(
            TError nullError,
            CancellationToken token
        )
            where TError : notnull => AsyncResult.LiftFromTask(task, nullError, token);
    }

    extension<T>(ValueTask<T?> valueTask)
        where T : notnull
    {
        public AsyncResult<T, TError> ToAsyncResult<TError>(TError nullError)
            where TError : notnull =>
            AsyncResult.LiftFromValueTask(valueTask, nullError, CancellationToken.None);

        public AsyncResult<T, TError> ToAsyncResult<TError>(
            TError nullError,
            CancellationToken token
        )
            where TError : notnull =>
            AsyncResult.LiftFromValueTask(valueTask, nullError, token);
    }

    extension<T, TError>(Task<Result<T, TError>> resultTask)
        where T : notnull
        where TError : notnull
    {
        public AsyncResult<T, TError> ToAsyncResult() => AsyncResult.Create(resultTask);

        public AsyncResult<T, TError> ToAsyncResult(CancellationToken token) =>
            AsyncResult.Create(resultTask, token);
    }

    extension<T, TError>(ValueTask<Result<T, TError>> resultValueTask)
        where T : notnull
        where TError : notnull
    {
        public AsyncResult<T, TError> ToAsyncResult() =>
            AsyncResult.Create(resultValueTask);

        public AsyncResult<T, TError> ToAsyncResult(CancellationToken token) =>
            AsyncResult.Create(resultValueTask, token);
    }

    extension<T, TError>(Async<Result<T, TError>> wrappedResult)
        where T : notnull
        where TError : notnull
    {
        public AsyncResult<T, TError> AsAsyncResult() => new(wrappedResult);
    }

    extension<T, TError>(Result<T, TError> result)
        where T : notnull
        where TError : notnull
    {
        public AsyncResult<T, TError> ToAsyncResult() => AsyncResult.Create(result);
    }

    extension<T, TError>(AsyncResult<T, TError> asyncResult)
        where T : notnull
        where TError : notnull
    {
        public Async<Result<T, TError>> AsAsync() =>
            asyncResult.WrappedResult;

        public AsyncResult<TNew, TError> Map<TNew>(Func<T, TNew> mapper)
            where TNew : notnull => AsyncResult.Map(asyncResult, mapper);


        public AsyncResult<TNew, TError> Map<TNew>(
            Func<T, CancellationToken, TNew> mapper
        )
            where TNew : notnull => AsyncResult.Map(asyncResult, mapper);


        public AsyncResult<T, TNewError> MapError<TNewError>(
            Func<TError, TNewError> mapper
        )
            where TNewError : notnull => AsyncResult.MapError(asyncResult, mapper);


        public AsyncResult<T, TNewError> MapError<TNewError>(
            Func<TError, CancellationToken, TNewError> mapper
        )
            where TNewError : notnull => AsyncResult.MapError(asyncResult, mapper);

        public AsyncResult<TNew, TError> Bind<TNew>(
            Func<T, AsyncResult<TNew, TError>> mapper
        )
            where TNew : notnull => AsyncResult.Bind(asyncResult, mapper);

        public AsyncResult<TNew, TError> Bind<TNew>(
            Func<T, CancellationToken, AsyncResult<TNew, TError>> mapper
        )
            where TNew : notnull => AsyncResult.Bind(asyncResult, mapper);


        public AsyncResult<TNew, TError> Apply<TNew>(
            AsyncResult<Func<T, CancellationToken, TNew>, TError> wrappedMapper
        )
            where TNew : notnull => AsyncResult.Apply(asyncResult, wrappedMapper);

        public AsyncResult<(T, T2), TError> Combine<T2>(
            AsyncResult<T2, TError> asyncResult2,
            Func<TError, TError, TError> errorCollisionHandler
        ) where T2 : notnull =>
            AsyncResult.Combine(asyncResult, asyncResult2, errorCollisionHandler);

        public Task<TResult> MatchAsync<TResult>(
            Func<T, TResult> matchOk,
            Func<TError, TResult> matchError
        )
            where TResult : notnull =>
            AsyncResult.MatchAsync(asyncResult, matchOk, matchError);

        public Task<TResult> MatchAsync<TResult>(
            Func<T, CancellationToken, TResult> matchOk,
            Func<TError, CancellationToken, TResult> matchError
        )
            where TResult : notnull =>
            AsyncResult.MatchAsync(asyncResult, matchOk, matchError);

        public Task DoIfOkAsync(Action<T> action) =>
            AsyncResult.Unsafe.DoIfOkAsync(asyncResult, action);

        public Task DoIfErrorAsync(Action<TError> errorAction) =>
            AsyncResult.Unsafe.DoIfErrorAsync(asyncResult, errorAction);

        public Task DoAsync(Action<T> okAction, Action<TError> errorAction) =>
            AsyncResult.Unsafe.DoAsync(asyncResult, okAction, errorAction);

        public Task<T?> GetValueOrDefaultAsync() =>
            AsyncResult.Unsafe.GetValueOrDefaultAsync(asyncResult);


        public Task<T> GetValueOrDefaultAsync(T defaultValue) =>
            AsyncResult.Unsafe.GetValueOrDefaultAsync(asyncResult, defaultValue);

        public Task<TError?> GetErrorOrDefaultAsync() =>
            AsyncResult.Unsafe.GetErrorOrDefaultAsync(asyncResult);


        public Task<TError> GetErrorOrDefaultAsync(TError defaultError) =>
            AsyncResult.Unsafe.GetErrorOrDefaultAsync(asyncResult, defaultError);
    }

    extension<T1, T2, TError>(AsyncResult<(T1, T2), TError> asyncResult)
        where T1 : notnull
        where T2 : notnull
        where TError : notnull
    {
        public AsyncResult<TNew, TError> Map<TNew>(Func<T1, T2, TNew> mapper)
            where TNew : notnull => AsyncResult.Map(asyncResult, mapper);

        public AsyncResult<TNew, TError> Bind<TNew>(
            Func<T1, T2, AsyncResult<TNew, TError>> binder
        )
            where TNew : notnull => AsyncResult.Bind(asyncResult, binder);
    }
}

public static class AsyncValueResultExtensions
{
    extension<T>(Async<T> async)
        where T : notnull
    {
        public AsyncResult<T, TError> ToResult<TError>()
            where TError : notnull => AsyncResult.LiftToOk<T, TError>(async);
    }
}

public static class AsyncErrorResultExtensions
{
    extension<TError>(Async<TError> async)
        where TError : notnull
    {
        public AsyncResult<T, TError> ToResult<T>()
            where T : notnull => AsyncResult.LiftToError<T, TError>(async);
    }
}