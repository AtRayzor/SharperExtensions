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
        public static async ValueTask<Option<T>> AwaitIfSome<T>(Option<ValueTask<T>> optionTask)
            where T : notnull =>
            Unsafe.TryGetValue(optionTask, out var task)
                ? await task.ConfigureAwait(false)
                : Option<T>.None;
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
}