using Monads.ResultMonad;

// ReSharper disable once CheckNamespace
namespace Monads.Traits;

public static partial class Functor
{
    public static class Result
    {
        public static IFunctor<ResultType<T, TE>> Construct<T, TE, TFunctor>(T? value, TE nullError)
            where T : notnull where TE : notnull where TFunctor : IFunctor<ResultType<T, TE>> =>
            value is not null ? Ok<T, TE, TFunctor>(value) : Error<T, TE, TFunctor>(nullError);

        public static IFunctor<ResultType<T, TE>> Ok<T, TE, TFunctor>(T value)
            where T : notnull where TE : notnull where TFunctor : IFunctor<ResultType<T, TE>>
            => ConstructDefault<ResultType<T, TE>>(new ResultType<T, TE>.Ok(value));

        public static IFunctor<ResultType<T, TE>> Error<T, TE, TFunctor>(TE error)
            where T : notnull where TE : notnull where TFunctor : IFunctor<ResultType<T, TE>>
            => ConstructDefault<ResultType<T, TE>>(new ResultType<T, TE>.Error(error));


        public static IFunctor<ResultType<T>> Construct<T, TFunctor>(T? value, string nullError)
            where T : notnull where TFunctor : IFunctor<ResultType<T>> =>
            value is not null ? Ok<T, TFunctor>(value) : Error<T, TFunctor>(nullError);

        public static IFunctor<ResultType<T>> Ok<T, TFunctor>(T value)
            where T : notnull where TFunctor : IFunctor<ResultType<T>>
            => Functor.ConstructDefault<ResultType<T>>(new ResultType<T>.Ok(value));

        public static IFunctor<ResultType<T>> Error<T, TFunctor>(string error)
            where T : notnull where TFunctor : IFunctor<ResultType<T>>
            => Functor.ConstructDefault<ResultType<T>>(new ResultType<T>.Error(error));
    }
}

public static partial class Monad
{
}