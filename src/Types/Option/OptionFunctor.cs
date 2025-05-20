using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Option;

public static partial class Option
{
    public static class Functor
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Map<T, TNew>(Option<T> option, Func<T, TNew> mapping)
            where T : notnull
            where TNew : notnull =>
            option switch
            {
                Some<T> some => Some(mapping(some.Value)),
                _ => None<TNew>(),
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TNew Match<T, TNew>(
            Option<T> option,
            Func<T, TNew> someMapping,
            Func<TNew> noneMapping
        )
            where T : notnull
            where TNew : notnull =>
            option switch
            {
                Some<T> some => new Some<TNew>(someMapping(some.Value)),
                _ => new Some<TNew>(noneMapping()),
            };

    }
}
