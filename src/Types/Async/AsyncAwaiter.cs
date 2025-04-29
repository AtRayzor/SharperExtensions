using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Async;

public struct AsyncAwaiter<T> : ICriticalNotifyCompletion
    where T : notnull
{
    private AsyncMutableState<T> _state;
    public bool IsCompleted => _state.IsCompleted;

    internal AsyncAwaiter(AsyncMutableState<T> state)
    {
        _state = state;
    }

    public void OnCompleted(Action continuation) => _state.Continuation = continuation;

    public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

    public T GetResult()
    {
        return _state switch
        {
            { Status: AsyncStatus.Completed, Result: { } result } => result,
            { Status: AsyncStatus.Failed, Exception: { } exception } => throw exception,
            { Status: AsyncStatus.Canceled } => throw new TaskCanceledException(),
            _ => throw new InvalidOperationException("The result was invalid.")
        };
    }

    public AsyncAwaiter<T> GetAwaiter() => this;
}
