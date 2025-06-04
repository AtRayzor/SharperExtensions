using System.Diagnostics.CodeAnalysis;

namespace SharperExtensions;

public static class OptionExtensions
{
    extension<T>(T? value)
        where T : notnull
    {
        public Option<T> ToOption() => Option.Return(value);

        public Option<TNew> CastToOption<TNew>()
            where TNew : notnull => Option.CastToOption<T, TNew>(value);
    }

    extension<TSource>(TSource? source)
    {
        public Option<T> ToOption<T>(Func<TSource?, T?> factory) where T : notnull =>
            Option.Create(source, factory);
    }

    extension<T>(Option<T> option) where T : notnull
    {

        public T? ValueOrDefault => Option.Unsafe.GetValueOrDefault(option);

        public Option<T> Or(Option<T> fallback) => Option.Or(option, fallback);

        public Option<T> Or(T fallback) => Option.Or(option, fallback);

        public Option<T> Or(Func<T> fallbackFunc) => Option.Or(option, fallbackFunc);

        public Option<T> OrApply(Option<Func<T>> wrappedFallbackFunc) =>
            Option.OrApply(option, wrappedFallbackFunc);

        public T ValueOr(T fallback) => Option.GetValueOr(option, fallback);

        public Option<(T, T2)> Combine<T2>(Option<T2> option2)
            where T2 : notnull => Option.Combine<T, T2>(option, option2);

        public Option<(T, T2, T3)> Combine<T2, T3>(
            Option<T2> option2,
            Option<T3> option3
        )
            where T2 : notnull
            where T3 : notnull => Option.Combine(option, option2, option3);

        public Option<(T, T2, T3, T4)> Combine<T1, T2, T3, T4>(
            Option<T2> option2,
            Option<T3> option3,
            Option<T4> option4
        )
            where T2 : notnull
            where T3 : notnull
            where T4 : notnull => Option.Combine(option, option2, option3, option4);

        public Option<TNew> Cast<TNew>() where TNew : notnull =>
            Option.Cast<T, TNew>(option);

        public bool TryGetValue([MaybeNullWhen(false)] out T value) =>
            Option.Unsafe.TryGetValue(option, out value);

        public T GetValueOrThrow<TException>() where TException : Exception =>
            Option.Unsafe.GetValueOrThrow<T, TException>(option);

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
    where T :
    notnull
    {
        public Option<T> Flatten() => Option.Monad.Flatten(nestedOption);
    }

    extension<T1, T2>(Option<(T1, T2)> tupleOption)
    where T1 :
    notnull
        where T2 :
    notnull
    {
        public Option<(T1, T2, T3)> Combine<T3>(Option<T3> option3)
            where T3 : notnull => Option.Combine(tupleOption, option3);

        public Option<TNew> Map<TNew>(Func<T1, T2, TNew> mapper)
            where TNew : notnull => Option.Functor.Map(tupleOption, mapper);

        public Option<TNew> Bind<TNew>(Func<T1, T2, Option<TNew>> binder)
            where TNew : notnull => Option.Monad.Bind(tupleOption, binder);
    }

    extension<T1, T2, T3>(Option<(T1, T2, T3)> tupleOption)
    where T1 :
    notnull
        where T2 :
    notnull
        where T3 :
    notnull
    {
        public Option<(T1, T2, T3, T4)> Combine<T4>(Option<T4> option3)
            where T4 : notnull => Option.Combine(tupleOption, option3);

        public Option<TNew> Map<TNew>(Func<T1, T2, T3, TNew> mapper)
            where TNew : notnull => Option.Functor.Map(tupleOption, mapper);

        public Option<TNew> Bind<TNew>(Func<T1, T2, T3, Option<TNew>> bind)
            where TNew : notnull => Option.Monad.Bind(tupleOption, bind);
    }

    extension<T1, T2, T3, T4>(Option<(T1, T2, T3, T4)> tupleOption)
    where T1 :
    notnull
        where T2 :
    notnull
        where T3 :
    notnull
        where T4 :
    notnull
    {
        public Option<TNew> Map<TNew>(Func<T1, T2, T3, T4, TNew> mapper)
            where TNew : notnull => Option.Functor.Map(tupleOption, mapper);

        public Option<TNew> Bind<TNew>(Func<T1, T2, T3, T4, Option<TNew>> bind)
            where TNew : notnull => Option.Monad.Bind(tupleOption, bind);
    }

}