using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Option;

public static partial class OptionExtensions
{
    extension<T>(Option<T> option) where T : notnull
    {
        public bool IsSome => Option.IsSome(option);

        public bool IsNone => Option.IsNone(option);

        public T? ValueOrDefault => Option.Unsafe.GetValueOrDefault(option);

        public Option<T> Or(Option<T> fallback) => Option.Or(option, fallback);

        public Option<T> Or(T fallback) => Option.Or(option, fallback);

        public T ValueOr(T fallback) => Option.GetValueOr(option, fallback);

        public bool TryGetValue([MaybeNullWhen(false)] out T value) =>
            Option.Unsafe.TryGetValue(option, out value);

        public T GetValueOrThrow<TException>(string message)
            where TException : Exception =>
                Option.Unsafe.GetValueOrThrow<T, TException>(option, message);

        public T GetValueOrThrow<TException>(params object[] constructorArgs)
            where TException : Exception =>
                Option.Unsafe.GetValueOrThrow<T, TException>(option, constructorArgs);

        public void IfSome(Action<T> action) => Option.Unsafe.IfSome(option, action);

        public void IfNone(Action action) => Option.Unsafe.IfNone(option, action);

        public void Do(Action<T> someAction, Action noneAction) =>
            Option.Unsafe.Do(option, someAction, noneAction);

        public Option<TNew> Map<TNew>(Func<T, TNew> mapper)
            where TNew : notnull => Option.Functor.Map(option, mapper);

        public TResult Match<TResult>(
            Func<T, TResult> matchSome,
            Func<TResult> matchNone
        ) 
            where TResult : notnull =>
                Option.Functor.Match(option, matchSome, matchNone);

        public Option<TNew> Bind<TNew>(Func<T, Option<TNew>> binder)
            where TNew : notnull => Option.Monad.Bind(option, binder);

        public Option<TNew> Apply<TNew>(Option<Func<T, TNew>> wrappedMapper)
            where TNew : notnull => Option.Applicative.Apply(option, wrappedMapper);
    }
    
    extension<T>(Option<Option<T>> nestedOption) 
        where T : notnull
    {
            public Option<T> Flatten() => Option.Monad.Flatten(nestedOption);
    }
}