using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SharperExtensions;

public static partial class Option
{
    public static class Applicative
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Apply<T, TNew>(
            Option<T> option,
            Option<Func<T, TNew>> wrappedMapping
        )
            where T : notnull
            where TNew : notnull =>
            wrappedMapping switch
            {
                Some<Func<T, TNew>> someWrapped => option.Map(someWrapped.Value),
                _ => new None<TNew>(),
            };
    }
}