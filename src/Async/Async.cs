using System.Runtime.CompilerServices;

namespace SharperExtensions.Async;

/// <summary>
/// Represents an asynchronous computation that produces a value of type
/// <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">
/// The type of the value produced by the asynchronous
/// computation.
/// </typeparam>
[AsyncMethodBuilder(typeof(AsyncMethodBuilder<>))]
public readonly struct Async<T> : IEquatable<Async<T>>
    where T : notnull
{
    internal AsyncMutableState<T> State { get; }

    /// <summary>
    /// Creates a new <see cref="Async{T}"/> instance with a <see cref="Unit"/> value.
    /// </summary>
    /// <remarks>
    /// Provides a convenient way to create an asynchronous computation that represents a completed void operation.
    /// </remarks>
    public Async<Unit> New => Async.New(Unit.Value);

    /// <summary>
    /// Gets the result of the asynchronous computation, representing either the successful value of type <typeparamref name="T"/> or an <see cref="Exception"/> if the computation failed.
    /// </summary>
    /// <returns>A <see cref="Result{T, Exception}"/> containing either the successful value or an error.</returns>
    public Result<T, Exception> Result =>
        State
            .GetResultBlocking()
            .Map(box => box.Value)
            .ValueOr(Result<T, Exception>.Error(new InvalidOperationException()));

    internal Async(
        T value,
        ExecutionContext? executionContext = null,
        CancellationToken cancellationToken = default
    )
    {
        State = new AsyncMutableState<T>(value, executionContext, cancellationToken);
        State.CaptureLocalContext();
    }

    internal Async(
        Func<T> resultCallback,
        bool shouldStart = false,
        CancellationToken cancellationToken = default
    ) : this(
        () => Result<T, Exception>.Ok(resultCallback()),
        null,
        shouldStart,
        cancellationToken
    ) { }

    internal Async(
        Func<Option<Result<T, Exception>>> resultCallback,
        ExecutionContext? executionContext = null,
        bool shouldStart = false,
        CancellationToken cancellationToken = default
    ) : this(
        new AsyncMutableState<T>(
            executionContext,
            resultCallback: resultCallback,
            token: cancellationToken
        ),
        shouldStart,
        cancellationToken
    ) { }

    internal Async(
        AsyncMutableState<T> state,
        bool shouldStart = false,
        CancellationToken cancellationToken = default
    )
    {
        State = state;
        State.CaptureLocalContext();

        if (shouldStart)
        {
            State.Start();
        }
    }

    public static Async<T> FromTask(Task<T> task, CancellationToken token = default)
    {
        var state = new AsyncMutableState<T>(
                resultCallback: (Option<Func<Option<Result<T, Exception>>>>)Callback,
                token: token
            )
            { };


        return new Async<T>(state, shouldStart: true);

        Option<Result<T, Exception>> Callback() =>
            task.Result.ToOption().Map(Result<T, Exception>.Ok);
    }

    public static Async<T> FromValueTask(
        ValueTask<T> task,
        CancellationToken token = default
    )
    {
        var state = new AsyncMutableState<T>(
            resultCallback: (Option<Func<Option<Result<T, Exception>>>>)Callback,
            token: token
        );


        return new Async<T>(state, shouldStart: true);

        Option<Result<T, Exception>> Callback() =>
            task.Result.ToOption().Map(Result<T, Exception>.Ok);
    }

    internal AsyncAwaiter<T, TResult> ConfigureAwaitInternal<TResult>(
        ResultProvider<T, TResult> resultProvider
    ) =>
        new(State, resultProvider);

    /// <summary>
    /// Configures the awaiter to return an <see cref="Option{T}"/> representing the result of the asynchronous operation.
    /// </summary>
    /// <returns>An <see cref="AsyncAwaiter{T, Option{T}}"/> that can be used to await the asynchronous operation.</returns>
    public AsyncAwaiter<T, Option<T>> ConfigureAwait() =>
        ConfigureAwaitInternal(new OptionResultProvider<T>(State));

    /// <summary>
    /// Configures the awaiter to return a <see cref="Result{T, TError}"/> with a predefined error value.
    /// </summary>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="error">The error value to use if the asynchronous operation fails.</param>
    /// <returns>An <see cref="AsyncAwaiter{T, Result{T, TError}}"/> that can be used to await the asynchronous operation.</returns>
    public AsyncAwaiter<T, Result<T, TError>> ConfigureAwait<TError>(TError error)
        where TError : notnull => ConfigureAwait(() => error);

    /// <summary>
    /// Configures the awaiter to return a <see cref="Result{T, TError}"/> with a default error factory.
    /// </summary>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="defaultErrorFactory">A function that provides the default error value if the asynchronous operation fails.</param>
    /// <returns>An <see cref="AsyncAwaiter{T, Result{T, TError}}"/> that can be used to await the asynchronous operation.</returns>
    public AsyncAwaiter<T, Result<T, TError>> ConfigureAwait<TError>(
        Func<TError> defaultErrorFactory
    ) where TError : notnull => ConfigureAwait(
        defaultErrorFactory,
        Option<Func<Exception, TError>>.None
    );

    /// <summary>
    /// Configures the awaiter to return a <see cref="Result{T, TError}"/> with a custom error handling strategy.
    /// </summary>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="defaultErrorFactory">A function that provides the default error value if the asynchronous operation fails.</param>
    /// <param name="errorHandler">An optional function to transform an exception into a specific error value.</param>
    /// <returns>An <see cref="AsyncAwaiter{T, Result{T, TError}}"/> that can be used to await the asynchronous operation.</returns>
    public AsyncAwaiter<T, Result<T, TError>> ConfigureAwait<TError>(
        Func<TError> defaultErrorFactory,
        Option<Func<Exception, TError>> errorHandler
    ) where TError : notnull => ConfigureAwaitInternal(
        new ResultTypeResultProvider<T, TError>(State, defaultErrorFactory, errorHandler)
    );

    internal AsyncAwaiter<T, T> GetAwaiter() =>
        new(State, new UnsafeResultProvider<T>(State));

    /// <inheritdoc />
    public bool Equals(Async<T> other)
    {
        return State.Equals(other.State);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Async<T> other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return State.GetHashCode();
    }

    public static bool operator ==(Async<T> left, Async<T> right) => Equals(left, right);

    public static bool operator !=(Async<T> left, Async<T> right) => !(left == right);
}

/// <summary>
/// Provides static methods for creating and working with
/// <see cref="Async{T}" /> instances.
/// </summary>
public static class Async
{
    /// <summary>
    /// Creates a new <see cref="Async{T}" /> that has already completed
    /// successfully with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to wrap in an asynchronous computation.</param>
    /// <param name="cancellationToken">
    /// The cancellation token to associate with
    /// this async operation.
    /// </param>
    /// <returns>
    /// An <see cref="Async{T}" /> representing a completed asynchronous
    /// computation with the specified value.
    /// </returns>
    public static Async<T> New<T>(T value, CancellationToken cancellationToken = default)
        where T : notnull => new(value, cancellationToken: cancellationToken);

    /// <summary>
    /// Creates a new <see cref="Async{T}" /> from an existing
    /// <see cref="Task{TResult}" /> and cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The task to wrap.</param>
    /// <param name="cancellationToken">
    /// The cancellation token to associate with
    /// this async operation.
    /// </param>
    /// <returns>An <see cref="Async{T}" /> wrapping the specified task.</returns>
    public static Async<T> FromTask<T>(Task<T> task, CancellationToken cancellationToken)
        where T : notnull => Async<T>.FromTask(task, cancellationToken);

    /// <summary>
    /// Creates a new <see cref="Async{T}" /> from an existing
    /// <see cref="Task{TResult}" />.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The task to wrap.</param>
    /// <returns>An <see cref="Async{T}" /> wrapping the specified task.</returns>
    public static Async<T> FromTask<T>(Task<T> task)
        where T : notnull => Async<T>.FromTask(task);

    /// <summary>
    /// Creates a new <see cref="Async{T}" /> from an existing
    /// <see cref="ValueTask{TResult}" /> and cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The value task to wrap.</param>
    /// <param name="cancellationToken">
    /// The cancellation token to associate with
    /// this async operation.
    /// </param>
    /// <returns>An <see cref="Async{T}" /> wrapping the specified value task.</returns>
    public static Async<T> FromValueTask<T>(
        ValueTask<T> task,
        CancellationToken cancellationToken
    )
        where T : notnull => Async<T>.FromValueTask(task, cancellationToken);

    /// <summary>
    /// Creates a new <see cref="Async{T}" /> from an existing
    /// <see cref="ValueTask{TResult}" />.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The value task to wrap.</param>
    /// <returns>An <see cref="Async{T}" /> wrapping the specified value task.</returns>
    public static Async<T> FromValueTask<T>(ValueTask<T> task)
        where T : notnull => Async<T>.FromValueTask(task, CancellationToken.None);

    /// <summary>
    /// Creates a new <see cref="Async{Unit}" /> from a <see cref="Task" /> and
    /// cancellation token.
    /// </summary>
    /// <param name="task">The task to wrap.</param>
    /// <param name="cancellationToken">
    /// The cancellation token to associate with
    /// this async operation.
    /// </param>
    /// <returns>
    /// An <see cref="Async{Unit}" /> representing the completion of the
    /// task.
    /// </returns>
    public static Async<Unit> FromTask(Task task, CancellationToken cancellationToken)
    {
        return ToUnitTaskAsync(task);

        static async Async<Unit> ToUnitTaskAsync(Task task)
        {
            await task.ConfigureAwait(ConfigureAwaitOptions.None);

            return Unit.Value;
        }
    }

    /// <summary>
    /// Creates a new <see cref="Async{Unit}" /> from a
    /// <see cref="Task" />.
    /// </summary>
    /// <param name="task">The task to wrap.</param>
    /// <returns>
    /// An <see cref="Async{Unit}" /> representing the completion of the
    /// task.
    /// </returns>
    public static Async<Unit> FromTask(Task task) =>
        FromTask(task, CancellationToken.None);

    /// <summary>
    /// Creates a new <see cref="Async{Unit}" /> from a <see cref="ValueTask" />
    /// and cancellation token.
    /// </summary>
    /// <param name="task">The value task to wrap.</param>
    /// <param name="cancellationToken">
    /// The cancellation token to associate with
    /// this async operation.
    /// </param>
    /// <returns>
    /// An <see cref="Async{Unit}" /> representing the completion of the
    /// value task.
    /// </returns>
    public static Async<Unit> FromValueTask(
        ValueTask task,
        CancellationToken cancellationToken
    )
    {
        return ToUnitTask(task);

        static async Async<Unit> ToUnitTask(ValueTask task)
        {
            await task.ConfigureAwait(false);

            return Unit.Value;
        }
    }

    public static Task<T?> AsTask<T>(Async<T> async)
        where T : notnull
    {
        var tcs = async.State.Tcs;

        Task.Factory.StartNew(() =>
            {
                async
                    .Result
                    .Do(tcs.SetResult, tcs.SetException);
            }
        );

        return tcs.Task;
    }

    public static ValueTask<T?> AsValueTask<T>(Async<T> async)
        where T : notnull
    {
        return async.State switch
        {
            { IsCompleted: true } when async.State.Result.ValueOrDefault is
                    { Value: var result } =>
                result.Match(ValueTask.FromResult<T?>, ValueTask.FromException<T?>),
            { IsCompleted: true } => ValueTask.FromResult<T?>(default),
            _ => new ValueTask<T?>(AsTask(async)),
        };
    }

    public static Async<T> SetToken<T>(Async<T> async, CancellationToken token)
        where T : notnull
    {
        async.State.Token = token;

        return async;
    }

    /// <summary>
    /// Creates a new <see cref="Async{Unit}" /> from a
    /// <see cref="ValueTask" />.
    /// </summary>
    /// <param name="task">The value task to wrap.</param>
    /// <returns>
    /// An <see cref="Async{Unit}" /> representing the completion of the
    /// value task.
    /// </returns>
    public static Async<Unit> FromValueTask(ValueTask task) =>
        FromValueTask(task, CancellationToken.None);

    /// <summary>
    /// Transforms the result of an asynchronous computation using the specified
    /// mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to mapper.</param>
    /// <param name="mapper">
    /// A function to transform the input value and
    /// cancellation token into the output value.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to associate with
    /// this async operation.
    /// </param>
    /// <returns>
    /// A new <see cref="Async{TNew}" /> representing the mapped
    /// asynchronous computation.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Map<T, TNew>(
        Async<T> async,
        Func<T, CancellationToken, TNew> mapper,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull
    {
        return new Async<TNew>(ResultCallback, async.State.ExecutionContext);

        Option<Result<TNew, Exception>> ResultCallback() =>
            async.Result.Map(value => mapper(value, cancellationToken));
    }

    /// <summary>
    /// Transforms the result of an asynchronous computation using the specified
    /// mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to map.</param>
    /// <param name="map">
    /// A function to transform the input value and cancellation
    /// token into the output value.
    /// </param>
    /// <returns>
    /// A new <see cref="Async{TNew}" /> representing the mapped
    /// asynchronous computation.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Map<T, TNew>(
        Async<T> async,
        Func<T, CancellationToken, TNew> map
    )
        where T : notnull
        where TNew : notnull => Map(async, map, CancellationToken.None);

    /// <summary>
    /// Transforms the result of an asynchronous computation using the specified
    /// mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to map.</param>
    /// <param name="map">
    /// A function to transform the input value into the output
    /// value.
    /// </param>
    /// <returns>
    /// A new <see cref="Async{TNew}" /> representing the mapped
    /// asynchronous computation.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Map<T, TNew>(Async<T> async, Func<T, TNew> map)
        where T : notnull
        where TNew : notnull => Map(async, (value, _) => map(value), async.State.Token);

    /// <summary>
    /// Binds the result of an asynchronous computation to another
    /// asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to bind.</param>
    /// <param name="binder">
    /// A function that takes the input value and cancellation token and returns a
    /// new asynchronous computation.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to associate with
    /// this async operation.
    /// </param>
    /// <returns>
    /// A new <see cref="Async{TNew}" /> representing the bound
    /// asynchronous computation.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Bind<T, TNew>(
        Async<T> async,
        Func<T, CancellationToken, Async<TNew>> binder,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull
    {
        return new Async<TNew>(
            ResultCallback,
            executionContext: async.State.ExecutionContext,
            cancellationToken: cancellationToken
        );

        Option<Result<TNew, Exception>> ResultCallback() =>
            async.Result.Bind(value => binder(value, cancellationToken).Result);
    }

    /// <summary>
    /// Binds the result of an asynchronous computation to another
    /// asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to bind.</param>
    /// <param name="binder">
    /// A function that takes the input value and cancellation token and returns a
    /// new asynchronous computation.
    /// </param>
    /// <returns>
    /// A new <see cref="Async{TNew}" /> representing the bound
    /// asynchronous computation.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Bind<T, TNew>(
        Async<T> async,
        Func<T, CancellationToken, Async<TNew>> binder
    )
        where T : notnull
        where TNew : notnull => Bind(async, binder, async.State.Token);

    /// <summary>
    /// Binds the result of an asynchronous computation to another
    /// asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to bind.</param>
    /// <param name="binder">
    /// A function that takes the input value and returns a
    /// new asynchronous computation.
    /// </param>
    /// <returns>
    /// A new <see cref="Async{TNew}" /> representing the bound
    /// asynchronous computation.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Bind<T, TNew>(Async<T> async, Func<T, Async<TNew>> binder)
        where T : notnull
        where TNew : notnull => Bind(
        async,
        (value, _) => binder(value),
        async.State.Token
    );

    /// <summary>Flattens a nested asynchronous computation.</summary>
    /// <typeparam name="T">
    /// The type of the inner asynchronous computation's
    /// value.
    /// </typeparam>
    /// <param name="nestedAsync">The nested asynchronous computation to flatten.</param>
    /// <returns>The flattened asynchronous computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<T> Flatten<T>(Async<Async<T>> nestedAsync)
        where T : notnull
    {
        return new Async<T>(
            ResultCallback,
            executionContext: nestedAsync.State.ExecutionContext,
            cancellationToken: nestedAsync.State.Token
        );

        Option<Result<T, Exception>> ResultCallback() =>
            nestedAsync.Result.Bind(async => async.Result);
    }

    /// <summary>
    /// Applies a wrapped mapping function to an asynchronous
    /// computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to apply the mapping to.</param>
    /// <param name="wrappedMap">
    /// The wrapped mapping function as an asynchronous
    /// computation.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to associate with
    /// this async operation.
    /// </param>
    /// <returns>
    /// A new <see cref="Async{TNew}" /> representing the result of applying the
    /// mapping function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Apply<T, TNew>(
        Async<T> async,
        Async<Func<T, CancellationToken, TNew>> wrappedMap,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull
    {
        return new Async<TNew>(
            ResultCallback,
            executionContext: async.State.ExecutionContext,
            cancellationToken: cancellationToken
        );

        Option<Result<TNew, Exception>> ResultCallback() =>
            wrappedMap.Result.Bind(mapper =>
                async.Result.Map((value) => mapper(value, cancellationToken))
            );
    }

    /// <summary>
    /// Applies a wrapped mapping function to an asynchronous
    /// computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to apply the mapping to.</param>
    /// <param name="wrappedMap">
    /// The wrapped mapping function as an asynchronous
    /// computation.
    /// </param>
    /// <returns>
    /// A new <see cref="Async{TNew}" /> representing the result of applying the
    /// mapping function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Apply<T, TNew>(
        Async<T> async,
        Async<Func<T, CancellationToken, TNew>> wrappedMap
    )
        where T : notnull
        where TNew : notnull => Apply(async, wrappedMap, async.State.Token);

    /// <summary>
    /// Applies a wrapped mapping function to an asynchronous
    /// computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to apply the mapping to.</param>
    /// <param name="wrappedMap">
    /// The wrapped mapping function as an asynchronous
    /// computation.
    /// </param>
    /// <returns>
    /// A new <see cref="Async{TNew}" /> representing the result of applying the
    /// mapping function.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Apply<T, TNew>(
        Async<T> async,
        Async<Func<T, TNew>> wrappedMap
    )
        where T : notnull
        where TNew : notnull => Bind(wrappedMap, map => Map(async, map));

    /// <summary>
    /// Executes an asynchronous action on the result of an asynchronous
    /// computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="async">
    /// The asynchronous computation whose result the action
    /// will be executed on.
    /// </param>
    /// <param name="actionAsync">The asynchronous action to execute.</param>
    /// <param name="cancellationToken">
    /// The cancellation token to associate with
    /// this async operation.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task DoAsync<T>(
        Async<T> async,
        Func<T, CancellationToken, Task> actionAsync,
        CancellationToken cancellationToken
    )
        where T : notnull
    {
        var tcs = async.State.Tcs;
        
        Task.Factory.StartNew(
            () =>
                async.Result.Do(Execute, tcs.SetException),
            cancellationToken
        );

        return tcs.Task;

        void Execute(T result)
        {
            actionAsync(result, cancellationToken).Wait(cancellationToken);
            tcs.SetResult(result);
        }
    }
}