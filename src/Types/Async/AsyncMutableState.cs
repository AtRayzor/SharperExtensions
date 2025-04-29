using System.Diagnostics.CodeAnalysis;

namespace DotNetCoreFunctional.Async;

internal sealed class AsyncMutableState<T>(CancellationToken token = default) : IEquatable<AsyncMutableState<T>>
    where T : notnull
{
    [field: AllowNull, MaybeNull]
    private Lock Lock => field ??= new Lock();
    
    public T? Result { get; private set; }
    
    public Exception? Exception { get; set; }
    public bool IsCompleted => Status is AsyncStatus.Completed or AsyncStatus.Failed;
    public AsyncStatus Status { get; private set; } = AsyncStatus.Running;
    

    public Action? Continuation { get; set; }

    public CancellationToken Token { get; set; } = token;

    
    public Task<T>? SourceTask { get; set; }


    public AsyncMutableState(T value, CancellationToken token = default)
        : this(token)
    {
        Result = value;
        Status = AsyncStatus.Completed;
    }
    
    public AsyncMutableState(Exception exception, CancellationToken token = default)
        : this(token)
    {
        Exception = exception;
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
        Continuation?.Invoke();
    }


    public override bool Equals(object? obj)
    {
        return obj is AsyncMutableState<T> other && Equals(other);
    }

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

    public static bool operator ==(AsyncMutableState<T> left, AsyncMutableState<T> right) => Equals(left, right);

    public static bool operator !=(AsyncMutableState<T> left, AsyncMutableState<T> right) => !(left == right);
}