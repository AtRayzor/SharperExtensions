using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Async;

public struct AsyncAwaiter<T> : ICriticalNotifyCompletion
    where T : notnull
{
    private AsyncMutableState<T> _state;
    public bool IsCompleted => _state.IsCompleted;

    internal AsyncAwaiter(AsyncMutableState<T> state) => _state = state;

    public void OnCompleted(Action continuation)
    {
        continuation.Invoke();
        _state.IsCompleted = true;
    }

    public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

    public T GetResult()
    {
        return _state.GetResult();
    }

    public AsyncAwaiter<T> GetAwaiter() => this;
}

public static class Ext { }
