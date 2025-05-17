using System.Diagnostics.CodeAnalysis;

namespace DotNetCoreFunctional.Async;

internal sealed class AsyncMutableState<T>(
    ExecutionContext? executionContext = null,
    CancellationToken token = default
) : IEquatable<AsyncMutableState<T>>
    where T : notnull
{
    public T? Result { get; private set; }

    public Exception? Exception { get; set; }
    public bool IsCompleted => Status is AsyncStatus.Completed or AsyncStatus.Failed;
    public AsyncStatus Status { get; set; } = AsyncStatus.Running;

    public ExecutionContext? ExecutionContext { get; private set; } =
        executionContext ?? ExecutionContext.Capture();

    public void CaptureLocalContext() => ExecutionContext = ExecutionContext.Capture();

    public Action? Continuation { get; set; }
    private event Action? OnCompleted;
    public Func<T>? ResultCallback { get; set; }

    public CancellationToken Token { get; set; } = token;

    public Task<T>? SourceTask { get; set; }

    public AsyncMutableState(
        T value,
        ExecutionContext? executionContext = null,
        CancellationToken token = default
    )
        : this(executionContext, token)
    {
        Result = value;
        Status = AsyncStatus.Completed;
    }

    public AsyncMutableState(
        Exception exception,
        ExecutionContext? executionContext = null,
        CancellationToken token = default
    )
        : this(executionContext, token)
    {
        Exception = exception;
    }

    public void OnComplete(Action continuation)
    {
        Continuation = continuation;

        if (ResultCallback is not null)
        {
            ThreadPool.QueueUserWorkItem(SetResultCallback);
        }

        void SetResultCallback(object? _) => SetResult(GetResultBlocking());
    }

    public void SetResult(T result)
    {
        Result = result;
        Update(AsyncStatus.Completed);
    }

    public void SetException(Exception exception)
    {
        Exception = exception;
        Update(AsyncStatus.Failed);
    }

    public void Update(AsyncStatus status)
    {
        Status = status;
        OnCompleted?.Invoke();
        Continuation?.Invoke();
    }

    public override bool Equals(object? obj)
    {
        return obj is AsyncMutableState<T> other && Equals(other);
    }

    public T GetResultBlocking()
    {
        Action? innerAction = this switch
        {
            { IsCompleted: true } or { Status: AsyncStatus.RunningAsync } => null,
            { ResultCallback: { } callback } => () => Result = callback(),
            _ => () => Exception = new InvalidOperationException(),
        };
        var mres = new ManualResetEventSlim();

        if (Status is AsyncStatus.RunningAsync)
        {
            OnCompleted += Set;
        }

        ThreadPool.QueueUserWorkItem(StatefulWorkItem, this);

        mres.Wait(Token);

        return Result
            ?? throw new InvalidOperationException(
                "The result of the async operation could not be returned."
            );

        void Set() => mres.Set();

        void StatefulWorkItem(object? obj)
        {
            if (ExecutionContext is not null)
            {
                ExecutionContext.Restore(ExecutionContext);
            }

            try
            {
                innerAction?.Invoke();
            }
            finally
            {
                if (Status is not AsyncStatus.RunningAsync)
                {
                    Set();
                }
            }
        }
    }

    private Action ResolveAction() =>
        this switch
        {
            { IsCompleted: true, Result: not null } => () => { },
            { IsCompleted: true, Exception: { } exception } => () => SetException(exception),
            { Continuation: { } act, ResultCallback: null } => act,
            { Continuation: null, ResultCallback: { } callback } => () => SetResult(callback()),
            _ => () => SetException(new InvalidOperationException()),
        };

    public bool Equals(AsyncMutableState<T>? other)
    {
        return other is not null
            && EqualityComparer<T?>.Default.Equals(Result, other.Result)
            && Equals(Exception, other.Exception)
            && Status == other.Status
            && Equals(Continuation, other.Continuation)
            && Token.Equals(other.Token)
            && Equals(SourceTask, other.SourceTask);
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public static bool operator ==(AsyncMutableState<T> left, AsyncMutableState<T> right) =>
        Equals(left, right);

    public static bool operator !=(AsyncMutableState<T> left, AsyncMutableState<T> right) =>
        !(left == right);
}
