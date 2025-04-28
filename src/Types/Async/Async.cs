using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DotNetCoreFunctional.Result;

namespace DotNetCoreFunctional.Async;

internal sealed record Box<T>(T Value);

internal sealed class AsyncMutableState<T>(CancellationToken token = default)
    where T : notnull
{
    public AsyncMutableState(Func<T> executionDelegate, CancellationToken cancellationToken)
        : this(cancellationToken)
    {
        _executionDelegate = executionDelegate;
    }

    public AsyncMutableState(T value, CancellationToken token = default)
        : this(token)
    {
        _executionDelegate = () => value;
    }

    public AsyncMutableState(Exception exception, CancellationToken token = default)
        : this(token)
    {
        _exception = exception;
    }

    public AsyncMutableState(Task<T> sourceTask, CancellationToken token = default)
        : this(token)
    {
        _sourceTask = sourceTask;
    }

    public bool IsCompleted { get; set; }

    private Func<T>? _executionDelegate;

    private Exception? _exception;

    private Task<T>? _sourceTask;

    [field: AllowNull, MaybeNull]
    public TaskCompletionSource<T> CompletionSource => field ?? new TaskCompletionSource<T>();
    public Action? Continuation { get; set; }

    public CancellationToken Token { get; set; } = token;

    public T GetResult()
    {
        Continuation?.Invoke();
        return this switch
        {
            { _executionDelegate: { } execution, _sourceTask: null } => execution(),
            { _sourceTask: { } sourceTask } => sourceTask.Result,
            _ => throw new InvalidOperationException("No value was set."),
        };
    }

    public Task<T> BuildTask()
    {
        switch (this)
        {
            case { _sourceTask: not null }:
            {
                return _sourceTask;
            }
            case { IsCompleted: false, _executionDelegate: not null }:
            {
                var task = new Task<T>(_executionDelegate, Token);
                task.Start();

                return task;
            }
            case { IsCompleted: true, _executionDelegate: not null, _exception: null }:
            {
                if (CompletionSource.Task.Status is TaskStatus.Created)
                {
                    CompletionSource.Task.Start();
                }

                CompletionSource.SetResult(_executionDelegate());
                break;
            }
            case { _exception: not null }:
            {
                if (CompletionSource.Task.Status is TaskStatus.Created)
                {
                    CompletionSource.Task.Start();
                }

                CompletionSource.SetException(_exception);
                break;
            }
            default:
            {
                if (CompletionSource.Task.Status is TaskStatus.Created)
                {
                    CompletionSource.Task.Start();
                }

                CompletionSource.SetException(new InvalidOperationException("No value was set."));
                break;
            }
        }

        return CompletionSource.Task;
    }
}

/// <summary>
/// Represents an asynchronous computation that produces a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the value produced by the asynchronous computation.</typeparam>
[AsyncMethodBuilder(typeof(AsyncMethodBuilder<>))]
public readonly struct Async<T>
    where T : notnull
{
    private readonly AsyncMutableState<T> _state;

    /// <summary>
    /// Gets the underlying <see cref="Task{TResult}"/> representing the asynchronous operation.
    /// </summary>
    public Task<T> Task => _state.BuildTask();

    /// <summary>
    /// Gets the <see cref="CancellationToken"/> associated with this asynchronous computation.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    private Async<Unit> New => Async.New(Unit.Value);

    /// <summary>
    /// Initializes a new instance of the <see cref="Async{T}"/> struct from a <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    internal Async(CancellationToken cancellationToken = default)
    {
        _state = new AsyncMutableState<T>(cancellationToken);
    }

    internal Async(T value, CancellationToken cancellationToken = default)
    {
        _state = new AsyncMutableState<T>(value, cancellationToken);
    }

    internal Async(Exception exception, CancellationToken cancellationToken = default)
    {
        _state = new AsyncMutableState<T>(exception, cancellationToken);
    }

    internal Async(AsyncMutableState<T> state)
    {
        _state = state;
    }

    internal Async(Task<T> task, CancellationToken cancellationToken = default)
    {
        _state = new AsyncMutableState<T>(task, cancellationToken);
    }

    /// <summary>
    /// Gets an awaiter used to await this asynchronous operation.
    /// </summary>
    /// <returns>A task awaiter.</returns>
    public AsyncAwaiter<T> GetAwaiter() => new(_state);

    internal void SetContinuationAction(Action continuation) => _state.Continuation = continuation;
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
        where T : notnull => new(value, cancellationToken);

    /// <summary>
    /// Creates a new <see cref="Async{T}"/> from an existing <see cref="Task{TResult}"/> and cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The task to wrap.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>An <see cref="Async{T}"/> wrapping the specified task.</returns>
    public static Async<T> FromTask<T>(Task<T> task, CancellationToken cancellationToken)
        where T : notnull => new(task, cancellationToken);

    /// <summary>
    /// Creates a new <see cref="Async{T}"/> from an existing <see cref="Task{TResult}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The task to wrap.</param>
    /// <returns>An <see cref="Async{T}"/> wrapping the specified task.</returns>
    public static Async<T> FromTask<T>(Task<T> task)
        where T : notnull => new(task, CancellationToken.None);

    /// <summary>
    /// Creates a new <see cref="Async{T}"/> from an existing <see cref="ValueTask{TResult}"/> and cancellation token.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The value task to wrap.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>An <see cref="Async{T}"/> wrapping the specified value task.</returns>
    public static Async<T> FromValueTask<T>(ValueTask<T> task, CancellationToken cancellationToken)
        where T : notnull => new(task.AsTask(), cancellationToken);

    /// <summary>
    /// Creates a new <see cref="Async{T}"/> from an existing <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="task">The value task to wrap.</param>
    /// <returns>An <see cref="Async{T}"/> wrapping the specified value task.</returns>
    public static Async<T> FromValueTask<T>(ValueTask<T> task)
        where T : notnull => new(task.AsTask(), CancellationToken.None);

    /// <summary>
    /// Creates a new <see cref="Async{Unit}"/> from a <see cref="Task"/> and cancellation token.
    /// </summary>
    /// <param name="task">The task to wrap.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>An <see cref="Async{Unit}"/> representing the completion of the task.</returns>
    public static Async<Unit> FromTask(Task task, CancellationToken cancellationToken)
    {
        return new Async<Unit>(ToUnitTask(task), cancellationToken);

        static async Task<Unit> ToUnitTask(Task task)
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
        return new Async<Unit>(ToUnitTask(task).AsTask(), cancellationToken);

        static async ValueTask<Unit> ToUnitTask(ValueTask task)
        {
            await task.ConfigureAwait(false);

            return Unit.Value;
        }
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
    /// <param name="async">The asynchronous computation to map.</param>
    /// <param name="map">A function to transform the input value and cancellation token into the output value.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the mapped asynchronous computation.</returns>
    public static Async<TNew> Map<T, TNew>(
        Async<T> async,
        Func<T, CancellationToken, TNew> map,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull
    {
        return new Async<TNew>(MapAsync(async, map, cancellationToken), cancellationToken);

        static async Task<TNew> MapAsync(
            Async<T> async,
            Func<T, CancellationToken, TNew> map,
            CancellationToken cancellationToken
        ) => map(await async, cancellationToken);
    }

    /// <summary>
    /// Transforms the result of an asynchronous computation using the specified mapping function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to map.</param>
    /// <param name="map">A function to transform the input value and cancellation token into the output value.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the mapped asynchronous computation.</returns>
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
    public static Async<TNew> Map<T, TNew>(Async<T> async, Func<T, TNew> map)
        where T : notnull
        where TNew : notnull => Map(async, (value, _) => map(value), async.CancellationToken);

    /// <summary>
    /// Binds the result of an asynchronous computation to another asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to bind.</param>
    /// <param name="binder">A function that takes the input value and cancellation token and returns a new asynchronous computation.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the bound asynchronous computation.</returns>
    public static Async<TNew> Bind<T, TNew>(
        Async<T> async,
        Func<T, CancellationToken, Async<TNew>> binder,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull
    {
        return new Async<TNew>(BindAsync(async, binder, cancellationToken), cancellationToken);

        static async Task<TNew> BindAsync(
            Async<T> async,
            Func<T, CancellationToken, Async<TNew>> binder,
            CancellationToken cancellationToken
        ) => await binder(await async, cancellationToken);
    }

    /// <summary>
    /// Binds the result of an asynchronous computation to another asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to bind.</param>
    /// <param name="binder">A function that takes the input value and cancellation token and returns a new asynchronous computation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the bound asynchronous computation.</returns>
    public static Async<TNew> Bind<T, TNew>(
        Async<T> async,
        Func<T, CancellationToken, Async<TNew>> binder
    )
        where T : notnull
        where TNew : notnull => Bind(async, binder, async.CancellationToken);

    /// <summary>
    /// Binds the result of an asynchronous computation to another asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to bind.</param>
    /// <param name="binder">A function that takes the input value and returns a new asynchronous computation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the bound asynchronous computation.</returns>
    public static Async<TNew> Bind<T, TNew>(Async<T> async, Func<T, Async<TNew>> binder)
        where T : notnull
        where TNew : notnull => Bind(async, (value, _) => binder(value), async.CancellationToken);

    /// <summary>
    /// Flattens a nested asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the inner asynchronous computation's value.</typeparam>
    /// <param name="nestedAsync">The nested asynchronous computation to flatten.</param>
    /// <returns>The flattened asynchronous computation.</returns>
    public static Async<T> Flatten<T>(Async<Async<T>> nestedAsync)
        where T : notnull => FlattenAsync(nestedAsync).Result;

    private static async Task<Async<T>> FlattenAsync<T>(Async<Async<T>> nestedAsync)
        where T : notnull => await nestedAsync;

    /// <summary>
    /// Applies a wrapped mapping function to an asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to apply the mapping to.</param>
    /// <param name="wrappedMap">The wrapped mapping function as an asynchronous computation.</param>
    /// <param name="cancellationToken">The cancellation token to associate with this async operation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the result of applying the mapping function.</returns>
    public static Async<TNew> Apply<T, TNew>(
        Async<T> async,
        Async<Func<T, CancellationToken, TNew>> wrappedMap,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull =>
        Bind(wrappedMap, (map, ct) => Map(async, map, ct), cancellationToken);

    /// <summary>
    /// Applies a wrapped mapping function to an asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to apply the mapping to.</param>
    /// <param name="wrappedMap">The wrapped mapping function as an asynchronous computation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the result of applying the mapping function.</returns>
    public static Async<TNew> Apply<T, TNew>(
        Async<T> async,
        Async<Func<T, CancellationToken, TNew>> wrappedMap
    )
        where T : notnull
        where TNew : notnull => Apply(async, wrappedMap, async.CancellationToken);

    /// <summary>
    /// Applies a wrapped mapping function to an asynchronous computation.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="async">The asynchronous computation to apply the mapping to.</param>
    /// <param name="wrappedMap">The wrapped mapping function as an asynchronous computation.</param>
    /// <returns>A new <see cref="Async{TNew}"/> representing the result of applying the mapping function.</returns>
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
