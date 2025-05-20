using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Option;

public static partial class Option
{
    public static class Monad
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Bind<T, TNew>(
            Option<T> option, Func<T, Option<TNew>> binder
        )
            where T : notnull
            where TNew : notnull =>
            option switch
            {
                Some<T> some => binder(some.Value),
                _ => new None<TNew>(),
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<T> Flatten<T>(Option<Option<T>> wrappedOption)
            where T : notnull
        {
            return wrappedOption switch
            {
                Some<Option<T>> wrapped => wrapped.Value,
                _ => new None<T>(),
            };
        }
    }
}