using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DotNetCoreFunctional.Result;

namespace DotNetCoreFunctional.Async;

/// <summary>
/// Represents an asynchronous computation that produces a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the value produced by the asynchronous computation.</typeparam>
[AsyncMethodBuilder(typeof(AsyncMethodBuilder<>))]
public struct Async<T> : IEquatable<Async<T>>
    where T : notnull
{
    internal AsyncMutableState<T> State { get; }

    private Async<Unit> New => Async.New(Unit.Value);

    public T Result => State.GetResultBlocking();

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
        ExecutionContext? executionContext,
        CancellationToken cancellationToken = default
    )
    {
        State = new AsyncMutableState<T>(executionContext, cancellationToken)
        {
            ResultCallback = resultCallback,
        };
    }

    internal Async(AsyncMutableState<T> state, CancellationToken cancellationToken = default)
    {
        State = state;
        State.CaptureLocalContext();
        State.Status = AsyncStatus.RunningAsync;
    }

    public static Async<T> FromTask(Task<T> task, CancellationToken token = default)
    {
        return ConvertAsync(task);

        static async Async<T> ConvertAsync(Task<T> task) =>
            await task.ConfigureAwait(ConfigureAwaitOptions.None);
    }

    public static Async<T> FromValueTask(ValueTask<T> task, CancellationToken token = default)
    {
        return ConvertAsync(task);

        static async Async<T> ConvertAsync(ValueTask<T> valueTask) =>
            await valueTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Gets an awaiter used to await this asynchronous operation.
    /// </summary>
    /// <returns>A task awaiter.</returns>
    public AsyncAwaiter<T> GetAwaiter() => new(State);

    public bool Equals(Async<T> other)
    {
        return State.Equals(other.State);
    }

    public override bool Equals(object? obj)
    {
        return obj is Async<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return State.GetHashCode();
    }

    public static bool operator ==(Async<T> left, Async<T> right) => Equals(left, right);

    public static bool operator !=(Async<T> left, Async<T> right) => !(left == right);
}

/// <summary>
/// Provides static methods for creating and working with <see cref="Async{T}"/> instances.
/// </summary>
public static class Async
{
    /// <summary>
    /// Creates a new <see cref="Async{T}"/> that has already completed successfully with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to wrap in an asynchronous computation.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>An <see cref="Async{T}"/> representing a completed asynchronous computation with the specified value.</returns>
    public static Async<T> New<T>(T value, CancellationToken cancellationToken = default)
        where T : notnull => new(value, cancellationToken: cancellationToken);

    /// <summary>
    /// Creates a new <see cref="Async{T}"/> from an existing <see cref="Task{TResult}"/> and cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The task to wrap.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>An <see cref="Async{T}"/> wrapping the specified task.</returns>
    public static Async<T> FromTask<T>(Task<T> task, CancellationToken cancellationToken)
        where T : notnull => Async<T>.FromTask(task, cancellationToken);

    /// <summary>
    /// Creates a new <see cref="Async{T}"/> from an existing <see cref="Task{TResult}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The task to wrap.</param>
    /// <returns>An <see cref="Async{T}"/> wrapping the specified task.</returns>
    public static Async<T> FromTask<T>(Task<T> task)
        where T : notnull => Async<T>.FromTask(task);

    /// <summary>
    /// Creates a new <see cref="Async{T}"/> from an existing <see cref="ValueTask{TResult}"/> and cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The value task to wrap.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>An <see cref="Async{T}"/> wrapping the specified value task.</returns>
    public static Async<T> FromValueTask<T>(ValueTask<T> task, CancellationToken cancellationToken)
        where T : notnull => Async<T>.FromValueTask(task, cancellationToken);

    /// <summary>
    /// Creates a new <see cref="Async{T}"/> from an existing <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The value task to wrap.</param>
    /// <returns>An <see cref="Async{T}"/> wrapping the specified value task.</returns>
    public static Async<T> FromValueTask<T>(ValueTask<T> task)
        where T : notnull => Async<T>.FromValueTask(task, CancellationToken.None);

    /// <summary>
    /// Creates a new <see cref="Async{Unit}"/> from a <see cref="Task"/> and cancellation token.
    /// </summary>
    /// <param name="task">The task to wrap.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>An <see cref="Async{Unit}"/> representing the completion of the task.</returns>
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
    /// Creates a new <see cref="Async{Unit}"/> from a <see cref="Task"/>.
    /// </summary>
    /// <param name="task">The task to wrap.</param>
    /// <returns>An <see cref="Async{Unit}"/> representing the completion of the task.</returns>
    public static Async<Unit> FromTask(Task task) => FromTask(task, CancellationToken.None);

    /// <summary>
    /// Creates a new <see cref="Async{Unit}"/> from a <see cref="ValueTask"/> and cancellation token.
    /// </summary>
    /// <param name="task">The value task to wrap.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>An <see cref="Async{Unit}"/> representing the completion of the value task.</returns>
    public static Async<Unit> FromValueTask(ValueTask task, CancellationToken cancellationToken)
    {
        return ToUnitTask(task);

        static async Async<Unit> ToUnitTask(ValueTask task)
        {
            await task.ConfigureAwait(false);

            return Unit.Value;
        }
    }

    public static Task<T> AsTask<T>(Async<T> async)
        where T : notnull
    {
        return ToTaskAsync(async);

        static async Task<T> ToTaskAsync(Async<T> async)
        {
            return await async;
        }
    }

    public static ValueTask<T> AsValueTask<T>(Async<T> async)
        where T : notnull
    {
        return ToValueTaskAsync(async);

        static async ValueTask<T> ToValueTaskAsync(Async<T> async)
        {
            return await async;
        }
    }

    public static Async<T> SetToken<T>(Async<T> async, CancellationToken token)
        where T : notnull
    {
        async.State.Token = token;

        return async;
    }

    /// <summary>
    /// Creates a new <see cref="Async{Unit}"/> from a <see cref="ValueTask"/>.
    /// </summary>
    /// <param name="task">The value task to wrap.</param>
    /// <returns>An <see cref="Async{Unit}"/> representing the completion of the value task.</returns>
    public static Async<Unit> FromValueTask(ValueTask task) =>
        FromValueTask(task, CancellationToken.None);

    /// <summary>
    /// Transforms the result of an asynchronous computation using the specified mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to mapper.</param>
    /// <param name="mapper">A function to transform the input value and cancellation token into the output value.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the mapped asynchronous computation.</returns>
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

        TNew ResultCallback() => mapper(async.Result, cancellationToken);
    }

    /// <summary>
    /// Transforms the result of an asynchronous computation using the specified mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to map.</param>
    /// <param name="map">A function to transform the input value and cancellation token into the output value.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the mapped asynchronous computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Map<T, TNew>(Async<T> async, Func<T, CancellationToken, TNew> map)
        where T : notnull
        where TNew : notnull => Map(async, map, CancellationToken.None);

    /// <summary>
    /// Transforms the result of an asynchronous computation using the specified mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to map.</param>
    /// <param name="map">A function to transform the input value into the output value.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the mapped asynchronous computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Map<T, TNew>(Async<T> async, Func<T, TNew> map)
        where T : notnull
        where TNew : notnull => Map(async, (value, _) => map(value), async.State.Token);

    /// <summary>
    /// Binds the result of an asynchronous computation to another asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to bind.</param>
    /// <param name="binder">A function that takes the input value and cancellation token and returns a new asynchronous computation.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the bound asynchronous computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Bind<T, TNew>(
        Async<T> async,
        Func<T, CancellationToken, Async<TNew>> binder,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull
    {
        return new Async<TNew>(ResultCallback, async.State.ExecutionContext, cancellationToken);

        TNew ResultCallback() => binder(async.Result, cancellationToken).Result;
    }

    /// <summary>
    /// Binds the result of an asynchronous computation to another asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to bind.</param>
    /// <param name="binder">A function that takes the input value and cancellation token and returns a new asynchronous computation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the bound asynchronous computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Bind<T, TNew>(
        Async<T> async,
        Func<T, CancellationToken, Async<TNew>> binder
    )
        where T : notnull
        where TNew : notnull => Bind(async, binder, async.State.Token);

    /// <summary>
    /// Binds the result of an asynchronous computation to another asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to bind.</param>
    /// <param name="binder">A function that takes the input value and returns a new asynchronous computation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the bound asynchronous computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Bind<T, TNew>(Async<T> async, Func<T, Async<TNew>> binder)
        where T : notnull
        where TNew : notnull => Bind(async, (value, _) => binder(value), async.State.Token);

    /// <summary>
    /// Flattens a nested asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the inner asynchronous computation's value.</typeparam>
    /// <param name="nestedAsync">The nested asynchronous computation to flatten.</param>
    /// <returns>The flattened asynchronous computation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<T> Flatten<T>(Async<Async<T>> nestedAsync)
        where T : notnull
    {
        return new Async<T>(
            ResultCallback,
            nestedAsync.State.ExecutionContext,
            nestedAsync.State.Token
        );

        T ResultCallback() => nestedAsync.Result.Result;
    }

    /// <summary>
    /// Applies a wrapped mapping function to an asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to apply the mapping to.</param>
    /// <param name="wrappedMap">The wrapped mapping function as an asynchronous computation.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the result of applying the mapping function.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Apply<T, TNew>(
        Async<T> async,
        Async<Func<T, CancellationToken, TNew>> wrappedMap,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull
    {
        return new Async<TNew>(ResultCallback, async.State.ExecutionContext, cancellationToken);

        TNew ResultCallback() => wrappedMap.Result.Invoke(async.Result, cancellationToken);
    }

    /// <summary>
    /// Applies a wrapped mapping function to an asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to apply the mapping to.</param>
    /// <param name="wrappedMap">The wrapped mapping function as an asynchronous computation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the result of applying the mapping function.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Apply<T, TNew>(
        Async<T> async,
        Async<Func<T, CancellationToken, TNew>> wrappedMap
    )
        where T : notnull
        where TNew : notnull => Apply(async, wrappedMap, async.State.Token);

    /// <summary>
    /// Applies a wrapped mapping function to an asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to apply the mapping to.</param>
    /// <param name="wrappedMap">The wrapped mapping function as an asynchronous computation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the result of applying the mapping function.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Async<TNew> Apply<T, TNew>(Async<T> async, Async<Func<T, TNew>> wrappedMap)
        where T : notnull
        where TNew : notnull => Bind(wrappedMap, map => Map(async, map));

    /// <summary>
    /// Executes an asynchronous action on the result of an asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="async">The asynchronous computation whose result the action will be executed on.</param>
    /// <param name="actionAsync">The asynchronous action to execute.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task DoAsync<T>(
        Async<T> async,
        Func<T, CancellationToken, Task> actionAsync,
        CancellationToken cancellationToken
    )
        where T : notnull
    {
        await actionAsync(await async, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);
    }
}
