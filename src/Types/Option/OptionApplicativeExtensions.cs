using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NetFunctional.Types;

public static partial class Option
{
    public static class Applicative
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Apply<T, TNew>(Option<T> option, Option<Func<T, TNew>> wrappedMapping)
            where T : notnull where TNew : notnull => wrappedMapping switch
        {
            Some<Func<T, TNew>> someWrapped => option.Map(someWrapped.Value),
            _ => new None<TNew>()
        };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Option<TNew>> ApplyAsync<T, TNew>(Task<Option<T>> optionTask,
            Task<Option<Func<T, TNew>>> wrappedMappingTask)
            where T : notnull where TNew : notnull => await wrappedMappingTask switch
        {
            Some<Func<T, TNew>> someWrapped => (await optionTask).Map(someWrapped.Value),
            _ => new None<TNew>()
        };
    }
}

public static class OptionApplicativeExtensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TNew> Apply<T, TNew>(this Option<T> option, Option<Func<T, TNew>> wrappedMapping)
        where T : notnull where TNew : notnull => Option.Applicative.Apply(option, wrappedMapping);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Option<TNew>> ApplyAsync<T, TNew>(this Task<Option<T>> optionTask,
        Task<Option<Func<T, TNew>>> wrappedMappingTask)
        where T : notnull where TNew : notnull => Option.Applicative.ApplyAsync(optionTask, wrappedMappingTask);
}