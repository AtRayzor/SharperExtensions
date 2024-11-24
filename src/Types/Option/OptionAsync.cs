using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Option;

public static partial class Option
{
    public static class Async
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Option<T>> AwaitIfSome<T>(Option<Task<T>> optionTask)
            where T : notnull =>
            Unsafe.TryGetValue(optionTask, out var task)
                ? await task.ConfigureAwait(ConfigureAwaitOptions.None)
                : Option<T>.None;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<Option<T>> AwaitIfSome<T>(
            Option<ValueTask<T>> optionValueTask
        )
            where T : notnull =>
            Unsafe.TryGetValue(optionValueTask, out var task)
                ? await task.ConfigureAwait(false)
                : Option<T>.None;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<T> ValueOrAsync<T>(
            ValueTask<Option<T>> optionValueTask,
            T fallback
        )
            where T : notnull =>
            (await optionValueTask.ConfigureAwait(false)).TryGetValue(out var value)
                ? value
                : fallback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> ValueOrAsync<T>(Task<Option<T>> optionTask, T fallback)
            where T : notnull =>
            (await optionTask.ConfigureAwait(ConfigureAwaitOptions.None)).TryGetValue(out var value)
                ? value
                : fallback;
    }
}

public static class OptionAsyncExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Option<T>> AwaitIfSome<T>(this Option<ValueTask<T>> optionTask)
        where T : notnull => Option.Async.AwaitIfSome(optionTask);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Option<T>> AwaitIfSome<T>(this Option<Task<T>> optionTask)
        where T : notnull => Option.Async.AwaitIfSome(optionTask);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<T> ValueOrAsync<T>(
        this ValueTask<Option<T>> optionValueTask,
        T fallback
    )
        where T : notnull => Option.Async.ValueOrAsync(optionValueTask, fallback);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<T> ValueOrAsync<T>(this Task<Option<T>> optionTask, T fallback)
        where T : notnull => Option.Async.ValueOrAsync(optionTask, fallback);
}
