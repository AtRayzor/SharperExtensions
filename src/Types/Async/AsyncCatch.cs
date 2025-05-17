using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DotNetCoreFunctional.Result;

namespace DotNetCoreFunctional.Async;

internal sealed class AsyncCatchState<T>
    where T : notnull
{
    public Action? Continuation { get; set; }

    public Action? Action { get; set; }

    public bool IsCompleted { get; set; }

    public AsyncCatchState()
    {
        return;
    }

    public AsyncCatchState(Result<T, Exception> result)
        : this()
    {
        Result = result;
    }

    [field: AllowNull, MaybeNull]
    public Result<T, Exception> Result
    {
        get =>
            field
            ?? Result<T, Exception>.Error(
                new InvalidOperationException("The value was never set.")
            );
        set
        {
            field = value;
            return;
        }
    }

    public Func<Result<T, Exception>> CreateExecutionDelegate()
    {
        return Execution;

        Result<T, Exception> Execution()
        {
            Action?.Invoke();

            return Result;
        }
    }
}

[AsyncMethodBuilder(typeof(AsyncCatchMethodBuilder<>))]
public struct AsyncCatch<T>
    where T : notnull
{
    [field: AllowNull, MaybeNull]
    internal AsyncCatchState<T> State { get; private init; }

    public AsyncCatch()
    {
        State = new AsyncCatchState<T>();
    }

    internal AsyncCatch(T value)
        : this(new AsyncCatchState<T>(Result<T, Exception>.Ok(value))) { }

    internal AsyncCatch(Exception exception)
        : this(new AsyncCatchState<T>(Result<T, Exception>.Error(exception))) { }

    internal AsyncCatch(AsyncCatchState<T> state)
        : this()
    {
        State = state;
    }

    internal void SetResult(T result)
    {
        State.Result = Result<T, Exception>.Ok(result);
        State.IsCompleted = true;
        State.Continuation?.Invoke();
    }

    internal void SetException(Exception exception)
    {
        State.Result = Result<T, Exception>.Error(exception);
        State.IsCompleted = true;
        State.Continuation?.Invoke();
    }

    public AsyncResult<T, Exception> AsAsyncResult() =>
        new(new Async<Result<T, Exception>>(State.CreateExecutionDelegate(), null));

    public Result<T, Exception> Result() => AsAsyncResult().WrappedResult.Result;

    public Result<T, TError> CreateResult<TError>(Func<Exception, TError> errorMapper)
        where TError : notnull => AsAsyncResult().MapError(errorMapper).WrappedResult.Result;

    public AsyncResult<T, TError> CreateAsyncResult<TError>(Func<Exception, TError> errorMapper)
        where TError : notnull => AsAsyncResult().MapError(errorMapper);

    public AsyncCatchAwaiter<T> GetAwaiter()
    {
        return new AsyncCatchAwaiter<T>(this);
    }
}

public struct AsyncCatchAwaiter<T> : ICriticalNotifyCompletion
    where T : notnull
{
    public bool IsCompleted => _catch.State.IsCompleted;

    private AsyncCatch<T> _catch;

    public AsyncCatchAwaiter()
    {
        return;
    }

    internal AsyncCatchAwaiter(AsyncCatch<T> @catch)
    {
        _catch = @catch;
    }

    public void OnCompleted(Action continuation)
    {
        _catch.State.Continuation = continuation;
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        OnCompleted(continuation);
    }

    public Result<T, Exception> GetResult()
    {
        return _catch.State.Result;
    }

    public AsyncCatchAwaiter<T> GetAwaiter() => this;
}

public struct AsyncCatchMethodBuilder<TResult>
    where TResult : notnull
{
    public AsyncCatch<TResult> Task { get; private set; }
    private readonly Lock _lock;

    internal AsyncCatchMethodBuilder(AsyncCatch<TResult> task, Lock @lock)
    {
        Task = task;
        _lock = @lock;
    }

    public static AsyncCatchMethodBuilder<TResult> Create()
    {
        var @lock = new Lock();
        return new AsyncCatchMethodBuilder<TResult>(
            new AsyncCatch<TResult>(new AsyncCatchState<TResult>()),
            @lock
        );
    }

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine
    {
        stateMachine.MoveNext();
    }

    public void SetStateMachine(IAsyncStateMachine _) { }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine
    )
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        Task.State.Action = stateMachine.MoveNext;
        awaiter.OnCompleted(stateMachine.MoveNext);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine
    )
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        Task.State.Action = stateMachine.MoveNext;
        awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
    }

    public void SetResult(TResult result)
    {
        Task.SetResult(result);
        return;
    }

    public void SetException(Exception exception)
    {
        Task.SetException(exception);
    }

    private void CreateWaitForResultAction(Action taskAction)
    {
        taskAction.Invoke();
    }
}
