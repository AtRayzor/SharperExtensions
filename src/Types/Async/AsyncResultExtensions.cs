using DotNetCoreFunctional.Result;

namespace DotNetCoreFunctional.Async;

/// <summary>
/// Provides extension methods for <see cref="AsyncResult{T, TError}"/> to enable fluent and convenient usage.
/// </summary>
public static class AsyncResultExtensions
{
    /// <summary>
    /// Creates an <see cref="AsyncResult{T, TError}"/> from a synchronous <see cref="Result{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="result">The synchronous result to convert.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> wrapping the given result.</returns>
    public static AsyncResult<T, TError> Create<T, TError>(this Result<T, TError> result)
        where T : notnull
        where TError : notnull => AsyncResult.Create(result);

    /// <summary>
    /// Lifts an asynchronous operation producing a success value into an <see cref="AsyncResult{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="async">The asynchronous operation producing the success value.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted operation.</returns>
    public static AsyncResult<T, TError> LiftToOk<T, TError>(this Async<T> async)
        where T : notnull
        where TError : notnull => AsyncResult.LiftToOk<T, TError>(async);

    /// <summary>
    /// Lifts an asynchronous operation producing an error value into an <see cref="AsyncResult{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="async">The asynchronous operation producing the error value.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted error operation.</returns>
    public static AsyncResult<T, TError> LiftToError<T, TError>(this Async<TError> async)
        where T : notnull
        where TError : notnull => AsyncResult.LiftToError<T, TError>(async);

    /// <summary>
    /// Maps the success value of an <see cref="AsyncResult{T, TError}"/> to a new value.
    /// </summary>
    /// <typeparam name="T">The type of the original success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the new success value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to map.</param>
    /// <param name="mapper">A mapping function that takes the success value and a cancellation token.</param>
    /// <returns>A new <see cref="AsyncResult{TNew, TError}"/> with the mapped success value.</returns>
    public static AsyncResult<TNew, TError> Map<T, TError, TNew>(
        this AsyncResult<T, TError> asyncResult,
        Func<T, CancellationToken, TNew> mapper
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull => AsyncResult.Map(asyncResult, mapper);

    /// <summary>
    /// Binds the success value of an <see cref="AsyncResult{T, TError}"/> to a new asynchronous result.
    /// </summary>
    /// <typeparam name="T">The type of the original success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the new success value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to bind.</param>
    /// <param name="binder">A binder function that takes the success value and a cancellation token and returns a new <see cref="AsyncResult{TNew, TError}"/>.</param>
    /// <returns>A new <see cref="AsyncResult{TNew, TError}"/> resulting from the bind operation.</returns>
    public static AsyncResult<TNew, TError> Bind<T, TError, TNew>(
        this AsyncResult<T, TError> asyncResult,
        Func<T, CancellationToken, AsyncResult<TNew, TError>> binder
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull => AsyncResult.Bind(asyncResult, binder);

    /// <summary>
    /// Applies a wrapped mapping function to the success value of an <see cref="AsyncResult{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the original success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the new success value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to apply the mapper to.</param>
    /// <param name="wrappedMapper">An asynchronous result wrapping a mapping function.</param>
    /// <returns>A new <see cref="AsyncResult{TNew, TError}"/> with the mapped success value.</returns>
    public static AsyncResult<TNew, TError> Apply<T, TError, TNew>(
        this AsyncResult<T, TError> asyncResult,
        AsyncResult<Func<T, CancellationToken, TNew>, TError> wrappedMapper
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull => AsyncResult.Apply(asyncResult, wrappedMapper);

    /// <summary>
    /// Matches on the result of an <see cref="AsyncResult{T, TError}"/>, invoking the appropriate function for success or error.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TOut">The return type of the match functions.</typeparam>
    /// <param name="asyncResult">The asynchronous result to match on.</param>
    /// <param name="okArm">Function to invoke if the result is success.</param>
    /// <param name="errorArm">Function to invoke if the result is error.</param>
    /// <returns>A task producing the result of the invoked function.</returns>
    public static Task<TOut> MatchAsync<T, TError, TOut>(
        this AsyncResult<T, TError> asyncResult,
        Func<T, CancellationToken, TOut> okArm,
        Func<TError, CancellationToken, TOut> errorArm
    )
        where T : notnull
        where TError : notnull => AsyncResult.MatchAsync(asyncResult, okArm, errorArm);

    /// <summary>
    /// Matches on the result of an <see cref="AsyncResult{T, TError}"/>, invoking the appropriate function for success or error.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TOut">The return type of the match functions.</typeparam>
    /// <param name="asyncResult">The asynchronous result to match on.</param>
    /// <param name="okArm">Function to invoke if the result is success.</param>
    /// <param name="errorArm">Function to invoke if the result is error.</param>
    /// <returns>A task producing the result of the invoked function.</returns>
    public static Task<TOut> MatchAsync<T, TError, TOut>(
        this AsyncResult<T, TError> asyncResult,
        Func<T, TOut> okArm,
        Func<TError, TOut> errorArm
    )
        where T : notnull
        where TError : notnull => AsyncResult.MatchAsync(asyncResult, okArm, errorArm);

    /// <summary>
    /// Executes an action if the <see cref="AsyncResult{T, TError}"/> is successful.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="asyncResult">The asynchronous result.</param>
    /// <param name="action">The action to execute on the success value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task DoIfOkAsync<T, TError>(
        this AsyncResult<T, TError> asyncResult,
        Action<T> action
    )
        where T : notnull
        where TError : notnull => AsyncResult.Unsafe.DoIfOkAsync(asyncResult, action);

    /// <summary>
    /// Executes an action if the <see cref="AsyncResult{T, TError}"/> is an error.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="asyncResult">The asynchronous result.</param>
    /// <param name="action">The action to execute on the error value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task DoIfErrorAsync<T, TError>(
        this AsyncResult<T, TError> asyncResult,
        Action<TError> action
    )
        where T : notnull
        where TError : notnull => AsyncResult.Unsafe.DoIfErrorAsync(asyncResult, action);

    /// <summary>
    /// Executes actions depending on whether the <see cref="AsyncResult{T, TError}"/> is success or error.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="asyncResult">The asynchronous result.</param>
    /// <param name="okAction">Action to execute if success.</param>
    /// <param name="errorAction">Action to execute if error.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task DoAsync<T, TError>(
        this AsyncResult<T, TError> asyncResult,
        Action<T> okAction,
        Action<TError> errorAction
    )
        where T : notnull
        where TError : notnull =>
        await AsyncResult
            .Unsafe.DoAsync(asyncResult, okAction, errorAction)
            .ConfigureAwait(ConfigureAwaitOptions.None);

    /// <summary>
    /// Gets the success value or default asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="asyncResult">The asynchronous result.</param>
    /// <returns>The success value or default if error.</returns>
    public static Task<T?> GetValueOrDefaultAsync<T, TError>(
        this AsyncResult<T, TError> asyncResult
    )
        where T : notnull
        where TError : notnull => AsyncResult.Unsafe.GetValueOrDefaultAsync(asyncResult);

    /// <summary>
    /// Gets the success value or a specified default asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="asyncResult">The asynchronous result.</param>
    /// <param name="defaultValue">The default value to return if error.</param>
    /// <returns>The success value or the specified default.</returns>
    public static Task<T> GetValueOrDefaultAsync<T, TError>(
        this AsyncResult<T, TError> asyncResult,
        T defaultValue
    )
        where T : notnull
        where TError : notnull =>
        AsyncResult.Unsafe.GetValueOrDefaultAsync(asyncResult, defaultValue);

    /// <summary>
    /// Gets the error value or default asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="asyncResult">The asynchronous result.</param>
    /// <returns>The error value or default if success.</returns>
    public static Task<TError?> GetErrorOrDefaultAsync<T, TError>(
        this AsyncResult<T, TError> asyncResult
    )
        where T : notnull
        where TError : notnull => AsyncResult.Unsafe.GetErrorOrDefaultAsync(asyncResult);

    /// <summary>
    /// Gets the error value or a specified default asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="asyncResult">The asynchronous result.</param>
    /// <param name="error">The default error value to return if success.</param>
    /// <returns>The error value or the specified default.</returns>
    public static Task<TError> GetErrorOrDefaultAsync<T, TError>(
        this AsyncResult<T, TError> asyncResult,
        TError error
    )
        where T : notnull
        where TError : notnull => AsyncResult.Unsafe.GetErrorOrDefaultAsync(asyncResult, error);
}
