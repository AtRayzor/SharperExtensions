using SharperExtensions.Async;

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

/// <summary>
/// Represents a method builder for creating and managing asynchronous methods with a specific result type.
/// </summary>
/// <typeparam name="TResult">The type of the result produced by the asynchronous method, which must be a non-null type.</typeparam>
/// <remarks>
/// This struct is used by the compiler to build and manage the state of async methods returning <see cref="Async{TResult}"/>.
/// </remarks>
public readonly struct AsyncMethodBuilder<TResult>()
    where TResult : notnull
{
    /// <inheritdoc cref="Async{TResult}"/>
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