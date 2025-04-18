using System.Runtime.CompilerServices;
using DotNetCoreFunctional.Operations;
using DotNetCoreFunctional.Result;

namespace DotNetCoreFunctional.Async;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

/// <summary>
/// Represents an asynchronous operation that produces a <see cref="Result{T, TError}"/>.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error value.</typeparam>
public readonly struct AsyncResult<T, TError>
    where T : notnull
    where TError : notnull
{
    /// <summary>
    /// Gets the underlying asynchronous wrapped result.
    /// </summary>
    internal Async<Result<T, TError>> WrappedResult { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncResult{T, TError}"/> struct.
    /// </summary>
    /// <param name="wrappedResult">The wrapped asynchronous result.</param>
    internal AsyncResult(Async<Result<T, TError>> wrappedResult)
    {
        WrappedResult = wrappedResult;
    }

    /// <summary>
    /// Creates a successful <see cref="AsyncResult{T, TError}"/> wrapping the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="value">The success value to wrap.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> representing a successful result.</returns>
    public static AsyncResult<T, TError> Ok(T value) => AsyncResult.CreateOk<T, TError>(value);

    /// <summary>
    /// Creates a failed <see cref="AsyncResult{T, TError}"/> wrapping the specified error.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="error">The error value to wrap.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> representing a failed result.</returns>
    public static AsyncResult<T, TError> Error(TError error) =>
        AsyncResult.CreateError<T, TError>(error);

    /// <summary>
    /// Configures how awaits on the underlying task are performed.
    /// </summary>
    /// <param name="options">The configure await options.</param>
    /// <returns>A configured task awaitable.</returns>
    public ConfiguredTaskAwaitable<Result<T, TError>> ConfigureAwait(
        ConfigureAwaitOptions options = ConfigureAwaitOptions.None
    ) => WrappedResult.ConfigureAwait(options);

    /// <summary>
    /// Gets an awaiter used to await this <see cref="AsyncResult{T, TError}"/>.
    /// </summary>
    /// <returns>A configured task awaiter.</returns>
    public ConfiguredTaskAwaitable<Result<T, TError>>.ConfiguredTaskAwaiter GetAwaiter() =>
        ConfigureAwait().GetAwaiter();

    /// <summary>
    /// Returns the underlying task representing the asynchronous operation.
    /// </summary>
    /// <returns>The task of the result.</returns>
    public Task<Result<T, TError>> AsTask() => WrappedResult.Task;
}

/// <summary>
/// Provides static methods for creating and working with <see cref="AsyncResult{T, TError}"/> instances.
/// </summary>
public static class AsyncResult
{
    /// <summary>
    /// Creates a successful <see cref="AsyncResult{T, TError}"/> wrapping the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="value">The success value.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> representing success.</returns>
    public static AsyncResult<T, TError> CreateOk<T, TError>(T value)
        where T : notnull
        where TError : notnull => new(Async.New(Result<T, TError>.Ok(value)));

    /// <summary>
    /// Creates a failed <see cref="AsyncResult{T, TError}"/> wrapping the specified error.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="error">The error value.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> representing failure.</returns>
    public static AsyncResult<T, TError> CreateError<T, TError>(TError error)
        where T : notnull
        where TError : notnull => new(Async.New(Result<T, TError>.Error(error)));

    /// <summary>
    /// Creates an <see cref="AsyncResult{T, TError}"/> from a nullable value, returning an error if the value is null.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="value">The nullable value to wrap.</param>
    /// <param name="nullError">The error to use if the value is null.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> representing either success or failure.</returns>
    public static AsyncResult<T, TError> Create<T, TError>(T? value, TError nullError)
        where T : notnull
        where TError : notnull =>
        value is not null ? CreateOk<T, TError>(value) : CreateError<T, TError>(nullError);

    /// <summary>
    /// Creates an <see cref="AsyncResult{T, TError}"/> from an existing <see cref="Result{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> wrapping the given result.</returns>
    public static AsyncResult<T, TError> Create<T, TError>(Result<T, TError> result)
        where TError : notnull
        where T : notnull => new(Async.New(result));

    /// <summary>
    /// Creates an <see cref="AsyncResult{T, TError}"/> from a task producing a <see cref="Result{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="result">The task producing the result.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> wrapping the given task result with no cancellation token.</returns>
    public static AsyncResult<T, TError> Create<T, TError>(Task<Result<T, TError>> result)
        where T : notnull
        where TError : notnull => Create(result, CancellationToken.None);

    /// <summary>
    /// Creates an <see cref="AsyncResult{T, TError}"/> from a task producing a <see cref="Result{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="result">The task producing the result.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> wrapping the given task result.</returns>
    public static AsyncResult<T, TError> Create<T, TError>(
        Task<Result<T, TError>> result,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TError : notnull => new(new Async<Result<T, TError>>(result, cancellationToken));

    /// <summary>
    /// Creates an <see cref="AsyncResult{T, TError}"/> from a value task producing a <see cref="Result{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="result">The value task producing the result.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> wrapping the given value task result.</returns>
    public static AsyncResult<T, TError> Create<T, TError>(ValueTask<Result<T, TError>> result)
        where T : notnull
        where TError : notnull => Create(result, CancellationToken.None);

    /// <summary>
    /// Creates an <see cref="AsyncResult{T, TError}"/> from a value task producing a <see cref="Result{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="result">The value task producing the result.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> wrapping the given value task result.</returns>
    public static AsyncResult<T, TError> Create<T, TError>(
        ValueTask<Result<T, TError>> result,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TError : notnull => new(new Async<Result<T, TError>>(result, cancellationToken));

    /// <summary>
    /// Creates an <see cref="AsyncResult{T, TError}"/> from a function that produces a task of <see cref="Result{T, TError}"/> given a cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="func">A function that takes a cancellation token and returns a task producing a result.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> wrapping the result of the given function.</returns>
    public static AsyncResult<T, TError> Create<T, TError>(
        Func<CancellationToken, Task<Result<T, TError>>> func,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TError : notnull => Create(func(cancellationToken), cancellationToken);

    /// <summary>
    /// Creates an <see cref="AsyncResult{T, TError}"/> from a function that produces a task of <see cref="Result{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="func">A function that returns a task producing a result.</param>
    /// <returns>A new <see cref="AsyncResult{T, TError}"/> wrapping the result of the given function.</returns>
    public static AsyncResult<T, TError> Create<T, TError>(Func<Task<Result<T, TError>>> func)
        where T : notnull
        where TError : notnull => Create(_ => func(), CancellationToken.None);

    /// <summary>
    /// Lifts an asynchronous operation producing a success value into an <see cref="AsyncResult{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="async">The asynchronous operation producing the success value.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted operation.</returns>
    public static AsyncResult<T, TError> LiftToOk<T, TError>(Async<T> async)
        where T : notnull
        where TError : notnull =>
        new(async.Bind((value, ct) => Async.New(Result<T, TError>.Ok(value), ct)));

    /// <summary>
    /// Lifts an asynchronous task producing a success value into an <see cref="AsyncResult{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="task">The task producing the success value.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted task.</returns>
    public static AsyncResult<T, TError> LiftToOk<T, TError>(Task<T> task)
        where T : notnull
        where TError : notnull => LiftToOk<T, TError>(new Async<T>(task));

    /// <summary>
    /// Lifts an asynchronous task producing a success value into an <see cref="AsyncResult{T, TError}"/> with a specified cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="task">The task producing the success value.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted task.</returns>
    public static AsyncResult<T, TError> LiftToOk<T, TError>(
        Task<T> task,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TError : notnull => LiftToOk<T, TError>(new Async<T>(task, cancellationToken));

    /// <summary>
    /// Lifts an asynchronous value task producing a success value into an <see cref="AsyncResult{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="task">The value task producing the success value.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted value task.</returns>
    public static AsyncResult<T, TError> LiftToOk<T, TError>(ValueTask<T> task)
        where T : notnull
        where TError : notnull => LiftToOk<T, TError>(new Async<T>(task));

    /// <summary>
    /// Lifts an asynchronous value task producing a success value into an <see cref="AsyncResult{T, TError}"/> with a specified cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="task">The value task producing the success value.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted value task.</returns>
    public static AsyncResult<T, TError> LiftToOk<T, TError>(
        ValueTask<T> task,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TError : notnull => LiftToOk<T, TError>(new Async<T>(task, cancellationToken));

    /// <summary>
    /// Lifts an asynchronous operation producing an error value into an <see cref="AsyncResult{T, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="async">The asynchronous operation producing the error value.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted error operation.</returns>
    public static AsyncResult<T, TError> LiftToError<T, TError>(Async<TError> async)
        where T : notnull
        where TError : notnull =>
        new(async.Bind((error, ct) => Async.New(Result<T, TError>.Error(error), ct)));

    /// <summary>
    /// Lifts an asynchronous task producing an error value into an <see cref="AsyncResult{T, TError}"/> without a specific cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="task">The task producing the error value.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted task with no cancellation token.</returns>
    public static AsyncResult<T, TError> LiftToError<T, TError>(Task<TError> task)
        where T : notnull
        where TError : notnull => LiftToError<T, TError>(task, CancellationToken.None);

    /// <summary>
    /// Lifts an asynchronous task producing an error value into an <see cref="AsyncResult{T, TError}"/> with a specified cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="task">The task producing the error value.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted task.</returns>
    public static AsyncResult<T, TError> LiftToError<T, TError>(
        Task<TError> task,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TError : notnull =>
        LiftToError<T, TError>(new Async<TError>(task, cancellationToken));

    /// <summary>
    /// Lifts an asynchronous value task producing an error value into an <see cref="AsyncResult{T, TError}"/> without a specific cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="task">The value task producing the error value.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted value task with no cancellation token.</returns>
    public static AsyncResult<T, TError> LiftToError<T, TError>(ValueTask<TError> task)
        where T : notnull
        where TError : notnull => LiftToError<T, TError>(task, CancellationToken.None);

    /// <summary>
    /// Lifts an asynchronous value task producing an error value into an <see cref="AsyncResult{T, TError}"/> with a specified cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="task">The value task producing the error value.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted value task.</returns>
    public static AsyncResult<T, TError> LiftToError<T, TError>(
        ValueTask<TError> task,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TError : notnull =>
        LiftToError<T, TError>(new Async<TError>(task, cancellationToken));

    /// <summary>
    /// Lifts an asynchronous task producing a nullable value into an <see cref="AsyncResult{T, TError}"/> with a specified cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="task">The task producing the nullable value.</param>
    /// <param name="nullError">The error to return if the value is null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted task.</returns>
    public static AsyncResult<T, TError> LiftFromTask<T, TError>(
        Task<T?> task,
        TError nullError,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TError : notnull
    {
        return Create(CreateResultAsync(task, nullError, cancellationToken));

        static async Task<Result<T, TError>> CreateResultAsync(
            Task<T?> task,
            TError nullError,
            CancellationToken cancellationToken
        ) =>
            await task.ConfigureAwait(ConfigureAwaitOptions.None) is { } value
                ? Result<T, TError>.Ok(value)
                : Result<T, TError>.Error(nullError);
    }

    /// <summary>
    /// Lifts an asynchronous value task producing a nullable value into an <see cref="AsyncResult{T, TError}"/> with a specified cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="task">The value task producing the nullable value.</param>
    /// <param name="nullError">The error to return if the value is null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="AsyncResult{T, TError}"/> representing the lifted value task.</returns>
    public static AsyncResult<T, TError> LiftFromValueTask<T, TError>(
        ValueTask<T?> task,
        TError nullError,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TError : notnull
    {
        return Create(CreateResultAsync(task, nullError, cancellationToken));

        static async Task<Result<T, TError>> CreateResultAsync(
            ValueTask<T?> task,
            TError nullError,
            CancellationToken cancellationToken
        ) =>
            await task.ConfigureAwait(false) is { } value
                ? Result<T, TError>.Ok(value)
                : Result<T, TError>.Error(nullError);
    }

    /// <summary>
    /// Maps the success value of an <see cref="AsyncResult{T, TError}"/> to a new value without using the cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the original success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the new success value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to map.</param>
    /// <param name="mapper">A mapping function that takes the success value.</param>
    /// <returns>A new <see cref="AsyncResult{TNew, TError}"/> with the mapped success value.</returns>
    public static AsyncResult<TNew, TError> Map<T, TError, TNew>(
        AsyncResult<T, TError> asyncResult,
        Func<T, TNew> mapper
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull => Map(asyncResult, (value, _) => mapper(value));

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
        AsyncResult<T, TError> asyncResult,
        Func<T, CancellationToken, TNew> mapper
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull =>
        new(
            asyncResult.WrappedResult.Bind(
                (result, ct) => Async.New(result.Map(Lambda.Partial(mapper, ct)), ct)
            )
        );

    /// <summary>
    /// Binds the success value of an <see cref="AsyncResult{T, TError}"/> to a new asynchronous result without using the cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the original success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the new success value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to bind.</param>
    /// <param name="binder">A binder function that takes the success value and returns a new <see cref="AsyncResult{TNew, TError}"/>.</param>
    /// <returns>A new <see cref="AsyncResult{TNew, TError}"/> resulting from the bind operation.</returns>
    public static AsyncResult<TNew, TError> Bind<T, TError, TNew>(
        AsyncResult<T, TError> asyncResult,
        Func<T, AsyncResult<TNew, TError>> binder
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull => Bind(asyncResult, (value, _) => binder(value));

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
        AsyncResult<T, TError> asyncResult,
        Func<T, CancellationToken, AsyncResult<TNew, TError>> binder
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull
    {
        var partialBinder = Lambda.Partial(binder, asyncResult.WrappedResult.CancellationToken);
        var asyncBinder = Lambda.Partial<
            Result<T, TError>,
            Func<T, AsyncResult<TNew, TError>>,
            Async<Result<TNew, TError>>
        >(ResultBinder, partialBinder);

        return new AsyncResult<TNew, TError>(asyncResult.WrappedResult.Bind(asyncBinder));

        static Async<Result<TNew, TError>> ResultBinder(
            Result<T, TError> result,
            Func<T, AsyncResult<TNew, TError>> binder
        )
        {
            return result switch
            {
                Ok<T, TError> { Value: var value } => binder(value).WrappedResult,
                Error<T, TError> { Err: var error } => Async.New(Result<TNew, TError>.Error(error)),
            };
        }
    }

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
        AsyncResult<T, TError> asyncResult,
        AsyncResult<Func<T, CancellationToken, TNew>, TError> wrappedMapper
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull
    {
        return Bind(wrappedMapper, (mapper, ct) => Bind(asyncResult, CreateBinder(mapper, ct)));

        static Func<T, CancellationToken, AsyncResult<TNew, TError>> CreateBinder(
            Func<T, CancellationToken, TNew> mapper,
            CancellationToken cancellation
        )
        {
            return (value, ct) => CreateOk<TNew, TError>(mapper(value, ct));
        }
    }

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
    public static async Task<TOut> MatchAsync<T, TError, TOut>(
        AsyncResult<T, TError> asyncResult,
        Func<T, CancellationToken, TOut> okArm,
        Func<TError, CancellationToken, TOut> errorArm
    )
        where T : notnull
        where TError : notnull
    {
        var token = asyncResult.WrappedResult.CancellationToken;
        return await asyncResult.WrappedResult.ConfigureAwait() switch
        {
            Ok<T, TError> { Value: var value } => okArm(value, token),
            Error<T, TError> { Err: var error } => errorArm(error, token),
        };
    }

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
    public static async Task<TOut> MatchAsync<T, TError, TOut>(
        AsyncResult<T, TError> asyncResult,
        Func<T, TOut> okArm,
        Func<TError, TOut> errorArm
    )
        where T : notnull
        where TError : notnull =>
        await MatchAsync(asyncResult, (value, _) => okArm(value), (err, _) => errorArm(err))
            .ConfigureAwait(ConfigureAwaitOptions.None);

    /// <summary>
    /// Provides unsafe operations on <see cref="AsyncResult{T, TError}"/>.
    /// </summary>
    public static class Unsafe
    {
        /// <summary>
        /// Executes an action if the <see cref="AsyncResult{T, TError}"/> is successful.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <param name="action">The action to execute on the success value.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task DoIfOkAsync<T, TError>(
            AsyncResult<T, TError> asyncResult,
            Action<T> action
        )
            where T : notnull
            where TError : notnull
        {
            await asyncResult
                .WrappedResult.DoAsync(result =>
                {
                    result.DoIfOk(action);

                    return Task.CompletedTask;
                })
                .ConfigureAwait(ConfigureAwaitOptions.None);
        }

        /// <summary>
        /// Executes an action if the <see cref="AsyncResult{T, TError}"/> is an error.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <param name="action">The action to execute on the error value.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task DoIfErrorAsync<T, TError>(
            AsyncResult<T, TError> asyncResult,
            Action<TError> action
        )
            where T : notnull
            where TError : notnull
        {
            await asyncResult
                .WrappedResult.DoAsync(result =>
                {
                    result.DoIfError(action);

                    return Task.CompletedTask;
                })
                .ConfigureAwait(ConfigureAwaitOptions.None);
        }

        /// <summary>
        /// Executes actions depending on whether the <see cref="AsyncResult{T, TError}"/> is success or error.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <param name="okAction">Action to execute if success.</param>
        /// <param name="errorEAction">Action to execute if error.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task DoAsync<T, TError>(
            AsyncResult<T, TError> asyncResult,
            Action<T> okAction,
            Action<TError> errorEAction
        )
            where T : notnull
            where TError : notnull =>
            await asyncResult
                .WrappedResult.DoAsync(result =>
                {
                    result.Do(okAction, errorEAction);
                    return Task.CompletedTask;
                })
                .ConfigureAwait(ConfigureAwaitOptions.None);

        /// <summary>
        /// Gets the success value or default asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <returns>The success value or default if error.</returns>
        public static async Task<T?> GetValueOrDefaultAsync<T, TError>(
            AsyncResult<T, TError> asyncResult
        )
            where T : notnull
            where TError : notnull => (await asyncResult.ConfigureAwait()).GetValueOrDefault();

        /// <summary>
        /// Gets the success value or a specified default asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <param name="defaultValue">The default value to return if error.</param>
        /// <returns>The success value or the specified default.</returns>
        public static async Task<T> GetValueOrDefaultAsync<T, TError>(
            AsyncResult<T, TError> asyncResult,
            T defaultValue
        )
            where T : notnull
            where TError : notnull =>
            (await asyncResult.ConfigureAwait()).GetValueOrDefault(defaultValue);

        /// <summary>
        /// Gets the error value or default asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        ///
        public static async Task<TError?> GetErrorOrDefaultAsync<T, TError>(
            AsyncResult<T, TError> asyncResult
        )
            where T : notnull
            where TError : notnull => (await asyncResult.ConfigureAwait()).GetErrorOrDefault();

        /// <summary>
        /// Gets the error value or a specified default asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        /// <param name="asyncResult">The asynchronous result.</param>
        /// <param name="error">The default error value to return if success.</param>
        /// <returns>The error value or the specified default.</returns>
        public static async Task<TError> GetErrorOrDefaultAsync<T, TError>(
            AsyncResult<T, TError> asyncResult,
            TError error
        )
            where T : notnull
            where TError : notnull => (await asyncResult.ConfigureAwait()).GetErrorOrDefault(error);
    }
}
