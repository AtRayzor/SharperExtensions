using System.Runtime.CompilerServices;

namespace SharperExtensions.Async;

public struct AsyncAwaiter<T, TResult> : ICriticalNotifyCompletion
    where T : notnull
{
    private AsyncMutableState<T> _state;
    private ResultProvider<T, TResult> _provider;
    public bool IsCompleted => _state.IsCompleted;

    internal AsyncAwaiter(
        AsyncMutableState<T> state,
        ResultProvider<T, TResult> resultProvider
    )
    {
        _state = state;
        _provider = resultProvider;
    }

    public void OnCompleted(Action continuation) => _state.OnComplete(continuation);

    public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

    public TResult GetResult()
    {
        return _provider.GetResult();
    }

    public AsyncAwaiter<T, TResult> GetAwaiter()
    {
        return this;
    }
}