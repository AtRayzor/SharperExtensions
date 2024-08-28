using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Result;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

public static partial class Result
{
    public static class Functor
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TNew, TError> Map<T, TError, TNew>(
            Result<T, TError> result,
            Func<T, TNew> mapping
        )
            where T : notnull
            where TError : notnull
            where TNew : notnull =>
            result switch
            {
                Ok<T, TError> ok => new Ok<TNew, TError>(mapping(ok)),
                Error<T, TError> error => new Error<TNew, TError>(error)
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T, TNewError> MapError<T, TError, TNewError>(
            Result<T, TError> result,
            Func<TError, TNewError> mapping
        )
            where T : notnull
            where TError : notnull
            where TNewError : notnull =>
            result switch
            {
                Error<T, TError> error => new Error<T, TNewError>(mapping(error)),
                Ok<T, TError> ok => new Ok<T, TNewError>(ok)
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TNew, TNewError> Match<T, TError, TNew, TNewError>(
            Result<T, TError> result,
            Func<T, TNew> matchOk,
            Func<TError, TNewError> matchError
        )
            where TNew : notnull
            where TNewError : notnull
            where T : notnull
            where TError : notnull =>
            result switch
            {
                Ok<T, TError> ok => new Ok<TNew, TNewError>(matchOk(ok)),
                Error<T, TError> error => new Error<TNew, TNewError>(matchError(error))
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<TNew, TError>> MapAsync<T, TError, TNew>(
            Task<Result<T, TError>> resultTask,
            Func<T, TNew> mapping
        )
            where T : notnull
            where TError : notnull
            where TNew : notnull => Map(await resultTask, mapping);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T, TNewError>> MapErrorAsync<T, TError, TNewError>(
            Task<Result<T, TError>> resultTask,
            Func<TError, TNewError> mapping
        )
            where T : notnull
            where TError : notnull
            where TNewError : notnull => MapError(await resultTask, mapping);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<TNew, TNewError>> MatchAsync<T, TError, TNew, TNewError>(
            Task<Result<T, TError>> resultTask,
            Func<T, TNew> matchOk,
            Func<TError, TNewError> matchError
        )
            where TNew : notnull
            where TNewError : notnull
            where T : notnull
            where TError : notnull => Match(await resultTask, matchOk, matchError);
    }
}

public static class ResultFunctorExtensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TNew, TError> Map<T, TError, TNew>(
        this Result<T, TError> result,
        Func<T, TNew> mapping
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull => Result.Functor.Map(result, mapping);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TNewError> MapError<T, TError, TNewError>(
        this Result<T, TError> result,
        Func<TError, TNewError> mapping
    )
        where T : notnull
        where TError : notnull
        where TNewError : notnull => Result.Functor.MapError(result, mapping);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TNew, TError>> MapAsync<T, TError, TNew>(
        this Task<Result<T, TError>> resultTask,
        Func<T, TNew> mapping
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull => Result.Functor.MapAsync(resultTask, mapping);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<T, TNewError>> MapErrorAsync<T, TError, TNewError>(
        this Task<Result<T, TError>> resultTask,
        Func<TError, TNewError> mapping
    )
        where T : notnull
        where TError : notnull
        where TNewError : notnull => Result.Functor.MapErrorAsync(resultTask, mapping);
}
