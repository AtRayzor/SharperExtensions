namespace Monads.OptionMonad;

public static class OptionAsyncExtensions
{
    public static async Task<Option<TNewValue>> MapAsync<TValue, TNewValue>(
        this Task<Option<TValue>> optionTask,
        Func<TValue, Task<Option<TNewValue>>> fSome, Func<Task<Option<TNewValue>>> fNone)
        where TValue : notnull where TNewValue : notnull
        => await optionTask switch
        {
            Option<TValue>.Some some => await fSome(some),
            Option<TValue>.None => await fNone()
        };

    public static  Task<Option<TNewValue>> MapAsync<TValue, TNewValue>(
        this Task<Option<TValue>> optionTask,
        Func<TValue, Task<Option<TNewValue>>> fSome)
        where TValue : notnull where TNewValue : notnull
        => optionTask.MapAsync(fSome, () => Task.FromResult(Option.None<TNewValue>()));
}