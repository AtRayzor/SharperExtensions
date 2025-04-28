using System.Runtime.CompilerServices;
using DotNetCoreFunctional.Async;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

public struct AsyncMethodBuilder<TResult>()
    where TResult : notnull
{
    public Async<TResult> Task { get; private set; } = new();

    public static AsyncMethodBuilder<TResult> Create() => default;

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

    public void SetResult(TResult result) => Task = Async.New(result);

    public void SetException(Exception exception) =>
        Task = new Async<TResult>(exception);
}
