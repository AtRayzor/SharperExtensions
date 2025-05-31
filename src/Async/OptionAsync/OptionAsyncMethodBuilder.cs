using System.Runtime.CompilerServices;

namespace SharperExtensions.Async;

public struct OptionAsyncMethodBuilder<T>()
    where T : notnull
{
    public OptionAsync<T> Task { get; } =
        new(new Async<Option<T>>(new AsyncMutableState<Option<T>>()));

    public static OptionAsyncMethodBuilder<T> Create() => new();

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

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(
        ref TAwaiter awaiter,
        ref TStateMachine stateMachine
    )
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine =>
        awaiter.UnsafeOnCompleted(stateMachine.MoveNext);

    public void SetResult(T? result) => 
        Task.WrappedOption.State.SetResult(Option.Return(result));

    public void SetException(Exception _) =>
        Task.WrappedOption.State.SetResult(Option<T>.None);
}
