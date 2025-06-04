namespace SharperExtensions.Async;

internal class OptionResultProvider<T>(AsyncMutableState<T> state)
    : ResultProvider<T, Option<T>>(state) where T : notnull
{
    public override Option<T> GetResult()
    {
        return
            State
                .Result
                .Bind(box =>
                    box
                        .Value
                        .Match(value => Option<T>.Some(value), _ => Option<T>.None)
                );
    }
}

internal sealed class OptionAsyncResultProvider<T>(AsyncMutableState<Option<T>> state)
    : ResultProvider<Option<T>, Option<T>>(state)
    where T : notnull
{
    public override Option<T> GetResult()
    {
        return State.Result.Bind(box => box.Value.Match(
                option => option,
                _ => Option<T>.None
            )
        );
    }
}