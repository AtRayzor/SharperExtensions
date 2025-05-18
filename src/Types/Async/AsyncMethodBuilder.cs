using System.Runtime.CompilerServices;
using DotNetCoreFunctional.Async;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

public readonly struct AsyncMethodBuilder<TResult>()
    where TResult : notnull
{
    public Async<TResult> Task { get; } = new();

    private AsyncMethodBuilder(AsyncMutableState<TResult> state)
        : this() => Task = new Async<TResult>(state);

    public static AsyncMethodBuilder<TResult> Create() =>
        new(new AsyncMutableState<TResult>());

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine => stateMachine.MoveNext();

    public void SetStateMachine(IAsyncStateMachine _) { }

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine
    )
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine =>
        awaiter.OnCompleted(stateMachine.MoveNext);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine
    )
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine =>
        awaiter.UnsafeOnCompleted(stateMachine.MoveNext);

    public void SetResult(TResult result) => Task.State.SetResult(result);

    public void SetException(Exception exception) => Task.State.SetException(exception);
}