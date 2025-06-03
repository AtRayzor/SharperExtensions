using System.Diagnostics.CodeAnalysis;

namespace SharperExtensions.Async;

internal sealed class Box<T> where T : notnull
{
    public T Value { get; }

    private Box(T value)
    {
        Value = value;
    }

    public static Box<T> Create(T value) => new(value);

    public static Box<T>? BoxOrNull(T? value) =>
        value is not null ? new Box<T>(value) : null;

    public static Option<Box<T>> BoxOrNone(T? value) => BoxOrNull(value).ToOption();

    public static implicit operator Box<T>?(T? value) => BoxOrNull(value);

    public static implicit operator T?(Box<T>? box) =>
        box is { Value: { } value } ? value : default;
}

internal sealed class AsyncMutableState<T>(
    ExecutionContext? executionContext = null,
    Option<Func<Option<Result<T, Exception>>>> resultCallback = default,
    CancellationToken token = default
) : IEquatable<AsyncMutableState<T>>
    where T : notnull
{
    [field: AllowNull, MaybeNull]
    private Lock Lock => field ??= new Lock();

    public Option<Box<Result<T, Exception>>> Result { get; private set; }

    public bool IsCompleted => Status is AsyncStatus.Completed or AsyncStatus.Failed;

    public AsyncStatus Status { get; set; } =
        resultCallback.Match(_ => AsyncStatus.Ready, () => AsyncStatus.Created);

    public ExecutionContext? ExecutionContext { get; private set; } =
        executionContext ?? ExecutionContext.Capture();

    public void CaptureLocalContext() => ExecutionContext = ExecutionContext.Capture();

    public Action? Continuation { get; set; }
    private event Action? OnCompleted;

    public Option<Func<Option<Result<T, Exception>>>> ResultCallback { get; } =
        resultCallback;

    public CancellationToken Token
    {
        get => field;
        set
        {
            field = value;
            field.Register(() => Status = AsyncStatus.Canceled);
        }
        
    } = token;

    public Task<T>? SourceTask { get; set; }

    [field: AllowNull, MaybeNull]
    public TaskCompletionSource<T?> Tcs =>
        field ?? new TaskCompletionSource<T?>();

    public AsyncMutableState(
        T value,
        ExecutionContext? executionContext = null,
        CancellationToken token = default
    )
        : this(executionContext, token: token)
    {
        Result = Box<Result<T, Exception>>.BoxOrNone(Result<T, Exception>.Ok(value));
        Status = AsyncStatus.Completed;
    }

    public void Start()
    {
        lock (Lock)
        {
            ResultCallback.IfSome(callback =>
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                        {
                            Result =
                                callback()
                                    .Bind(result =>
                                        Box<Result<T, Exception>>.BoxOrNone(result)
                                    );
                            NotifyCompleted();
                        }
                    );
                }
            );

            Status = AsyncStatus.RunningAsync;
        }
    }

    public AsyncMutableState(
        Exception exception,
        ExecutionContext? executionContext = null,
        CancellationToken token = default
    )
        : this(executionContext, token: token)
    {
        Result = Box<Result<T, Exception>>.BoxOrNone(
            Result<T, Exception>.Error(exception)
        );
    }

    public void OnComplete(Action continuation)
    {
        Continuation = continuation;

        if (Status is AsyncStatus.Ready)
        {
            Start();
        }
    }

    public void SetResult(Option<Box<Result<T, Exception>>> option)
    {
        Result = option;
        NotifyCompleted();
    }

    public void SetResult(T result) =>
        SetResult(Box<Result<T, Exception>>.BoxOrNone(Result<T, Exception>.Ok(result)));

    public void SetException(Exception exception)
    {
        Result = Box<Result<T, Exception>>.BoxOrNone(
            Result<T, Exception>.Error(exception)
        );
        NotifyCompleted(AsyncStatus.Failed);
    }

    public void NotifyCompleted(AsyncStatus status = AsyncStatus.Completed)
    {
        Status = status;
        OnCompleted?.Invoke();
        Continuation?.Invoke();
    }

    public override bool Equals(object? obj)
    {
        return obj is AsyncMutableState<T> other && Equals(other);
    }

    public Option<Box<Result<T, Exception>>> GetResultBlocking()
    {
        var mres = new ManualResetEventSlim();

        while (true)
        {
            switch (this)
            {
                case { Status: AsyncStatus.Created, ResultCallback.IsNone: true }:
                {
                    continue;
                }
                case { Token.IsCancellationRequested: true }:
                case { Status: AsyncStatus.Completed or AsyncStatus.Failed }:
                {
                    return Result;
                }
                case { Status: AsyncStatus.RunningAsync }:
                {
                    OnCompleted += Set;
                    mres.Wait(Token);
                    OnCompleted -= Set;
                    break;
                }
                case { Status: AsyncStatus.Ready }
                    when ResultCallback.TryGetValue(out var resultCallback):
                {
                    ThreadPool.QueueUserWorkItem(StatefulWorkItem, this);
                    Status = AsyncStatus.Running;

                    mres.Wait(Token);

                    break;

                    void StatefulWorkItem(object? obj)
                    {
                        if (ExecutionContext is not null)
                        {
                            ExecutionContext.Restore(ExecutionContext);
                        }

                        try
                        {
                            Result =
                                resultCallback
                                    .Invoke()
                                    .Bind(result =>
                                        Box<Result<T, Exception>>.BoxOrNone(result)
                                    );
                            Status = AsyncStatus.Completed;
                        }
                        catch (Exception exc)
                        {
                            Result =
                                Box<Result<T, Exception>>.BoxOrNone(
                                    Result<T, Exception>.Error(exc)
                                );
                            Status = AsyncStatus.Failed;
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
                default:
                {
                    return Result;
                }
            }
        }

        void Set() => mres.Set();
    }

    public bool Equals(AsyncMutableState<T>? other)
    {
        return other is not null
               && EqualityComparer<Option<Box<Result<T, Exception>>>>
                   .Default
                   .Equals(Result, other.Result)
               && Status == other.Status
               && Equals(Continuation, other.Continuation)
               && Token.Equals(other.Token)
               && Equals(SourceTask, other.SourceTask);
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public static bool operator ==(
        AsyncMutableState<T> left,
        AsyncMutableState<T> right
    ) =>
        Equals(left, right);

    public static bool operator !=(
        AsyncMutableState<T> left,
        AsyncMutableState<T> right
    ) =>
        !(left == right);

}