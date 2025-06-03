namespace SharperExtensions.Async;

internal abstract class ResultProvider<T, TResult>(AsyncMutableState<T> state)
    where T : notnull
{
    protected AsyncMutableState<T> State { get; } = state;

    public abstract TResult GetResult();
}

internal sealed class UnsafeResultProvider<T>(AsyncMutableState<T> state)
    : ResultProvider<T, T>(state) where T : notnull
{
    public override T GetResult()
    {
        return State.Result
            .Map(box => box.Value)
            .GetValueOrThrow<Result<T, Exception>, InvalidOperationException>()
            .ValueOrThrow;
    }
}