using NetFunctional.Types.ResultKind;

// ReSharper disable once CheckNamespace
namespace NetFunctional.Types.Traits;

public static partial class Functor
{
    public static class Result
    {
        public static IFunctor<ResultType<T, TE>> Construct<T, TE>(T? value, TE nullError)
            where T : notnull
            where TE : notnull
        {
            return value is not null ? Ok<T, TE>(value) : Error<T, TE>(nullError);
        }

        public static IFunctor<ResultType<T, TE>> Ok<T, TE>(T value)
            where T : notnull
            where TE : notnull
        {
            return ConstructDefault<ResultType<T, TE>>(new ResultType<T, TE>.Ok(value));
        }

        public static IFunctor<ResultType<T, TE>> Error<T, TE>(TE error)
            where T : notnull
            where TE : notnull
        {
            return ConstructDefault<ResultType<T, TE>>(new ResultType<T, TE>.Error(error));
        }

        public static IFunctor<ResultType<T>> Construct<T>(T? value, string nullError)
            where T : notnull
        {
            return value is not null ? Ok(value) : Error<T>(nullError);
        }

        public static IFunctor<ResultType<T>> Ok<T>(T value)
            where T : notnull
        {
            return ConstructDefault<ResultType<T>>(new ResultType<T>.Ok(value));
        }

        public static IFunctor<ResultType<T>> Error<T>(string error)
            where T : notnull
        {
            return ConstructDefault<ResultType<T>>(new ResultType<T>.Error(error));
        }
    }
}

public static partial class Monad
{
    public static class Result
    {
        public static IMonad<ResultType<T, TE>> Construct<T, TE>(T? value, TE nullError)
            where T : notnull
            where TE : notnull
        {
            return value is not null ? Ok<T, TE>(value) : Error<T, TE>(nullError);
        }

        public static IMonad<ResultType<T, TE>> Ok<T, TE>(T value)
            where T : notnull
            where TE : notnull
        {
            return ConstructDefault<ResultType<T, TE>>(new ResultType<T, TE>.Ok(value));
        }

        public static IMonad<ResultType<T, TE>> Error<T, TE>(TE error)
            where T : notnull
            where TE : notnull
        {
            return ConstructDefault<ResultType<T, TE>>(new ResultType<T, TE>.Error(error));
        }

        public static IMonad<ResultType<T>> Construct<T>(T? value, string nullError)
            where T : notnull
        {
            return value is not null ? Ok(value) : Error<T>(nullError);
        }

        public static IMonad<ResultType<T>> Ok<T>(T value)
            where T : notnull
        {
            return ConstructDefault<ResultType<T>>(new ResultType<T>.Ok(value));
        }

        public static IMonad<ResultType<T>> Error<T>(string error)
            where T : notnull
        {
            return ConstructDefault<ResultType<T>>(new ResultType<T>.Error(error));
        }
    }
}

public static partial class Applicative
{
    public static class Result
    {
        public static IApplicative<ResultType<T, TE>> Construct<T, TE>(T? value, TE nullError)
            where T : notnull
            where TE : notnull
        {
            return value is not null ? Ok<T, TE>(value) : Error<T, TE>(nullError);
        }

        public static IApplicative<ResultType<T, TE>> Ok<T, TE>(T value)
            where T : notnull
            where TE : notnull
        {
            return ConstructDefault<ResultType<T, TE>>(new ResultType<T, TE>.Ok(value));
        }

        public static IApplicative<ResultType<T, TE>> Error<T, TE>(TE error)
            where T : notnull
            where TE : notnull
        {
            return ConstructDefault<ResultType<T, TE>>(new ResultType<T, TE>.Error(error));
        }

        public static IApplicative<ResultType<T>> Construct<T>(T? value, string nullError)
            where T : notnull
        {
            return value is not null ? Ok(value) : Error<T>(nullError);
        }

        public static IApplicative<ResultType<T>> Ok<T>(T value)
            where T : notnull
        {
            return ConstructDefault<ResultType<T>>(new ResultType<T>.Ok(value));
        }

        public static IApplicative<ResultType<T>> Error<T>(string error)
            where T : notnull
        {
            return ConstructDefault<ResultType<T>>(new ResultType<T>.Error(error));
        }
    }
}