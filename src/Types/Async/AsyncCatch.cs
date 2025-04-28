using System.Runtime.CompilerServices;
using DotNetCoreFunctional.Result;

namespace DotNetCoreFunctional.Async;

[AsyncMethodBuilder(typeof(AsyncCatchMethodBuilder<>))]
public readonly struct AsyncCatch<T>
    where T : notnull
{
    private readonly Result<T, Exception> _result;

    internal AsyncCatch(T value)
    {
        _result = Result<T, Exception>.Ok(value);
    }

    internal AsyncCatch(Exception exception)
    {
        _result = Result<T, Exception>.Error(exception);
    }

    public AsyncResult<T, Exception> AsAsyncResult() => AsyncResult.Create(_result);

    public Result<T, Exception> AsResult() => _result;

    public Result<T, TError> CreateResult<TError>(Func<Exception, TError> errorMapper) where TError : notnull => _result.MapError(errorMapper);

    public AsyncResult<T, TError> CreateAsyncResult<TError>(Func<Exception, TError> errorMapper)
        where TError : notnull =>
        AsAsyncResult().MapError(errorMapper);

   public AsyncCatchAwaiter<T> GetAwaiter() => new(_result);
}

public sealed class AsyncCatchAwaiter<T> : ICriticalNotifyCompletion where T : notnull
{
    public bool IsCompleted { get; private set; }

    private Result<T, Exception> _result;
    internal AsyncCatchAwaiter(Result<T, Exception> result)
    {
        _result = result;
    }
    
    public void OnCompleted(Action continuation)
    {
        continuation.Invoke();
        IsCompleted = true;
    }

    public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

    Result<T, Exception> GetResult() => _result;

    public AsyncCatchAwaiter<T> GetAwaiter() => this;
}

public struct AsyncCatchMethodBuilder<TResult>() where TResult : notnull
{
    public AsyncCatch<TResult> Task { get; private set; } = new();

    public static AsyncCatchMethodBuilder<TResult> Create() => default;

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine => stateMachine.MoveNext();

    public void SetStateMachine(IAsyncStateMachine _) { }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine
    )
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine => awaiter.OnCompleted(stateMachine.MoveNext);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine
    )
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine =>
        awaiter.UnsafeOnCompleted(stateMachine.MoveNext);

    public void SetResult(TResult result) => Task = new AsyncCatch<TResult>(result);

    public void SetException(Exception exception) =>
        Task = new AsyncCatch<TResult>(exception);
}