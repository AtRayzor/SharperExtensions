using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NetFunctional.Types;

public static partial class Result
{
    public static class Applicative
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TNew, TError> Apply<T, TError, TNew>(Result<T, TError> result,
            Result<Func<T, TNew>, TError> wrappedMapping)
            where T : notnull where TError : notnull where TNew : notnull =>
            wrappedMapping switch
            {
                Ok<Func<T, TNew>, TError> okMapping => Functor.Map(result, okMapping.Value),
                Error<Func<T, TNew>, TError> error => new Error<TNew, TError>(error)
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<TNew, TError>> ApplyAsync<T, TError, TNew>(
            Task<Result<T, TError>> resultTask,
            Task<Result<Func<T, TNew>, TError>> wrappedMappingTask
        )
            where T : notnull where TError : notnull where TNew : notnull =>
            await wrappedMappingTask switch
            {
                Ok<Func<T, TNew>, TError> okMapping => Functor.Map(await resultTask, okMapping.Value),
                Error<Func<T, TNew>, TError> error => new Error<TNew, TError>(error)
            };
    }
}

public static class ApplicativeResultExtensions
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TNew, TError> Apply<T, TError, TNew>(
        this Result<T, TError> result,
        Result<Func<T, TNew>, TError> wrappedMapping) where T : notnull where TError : notnull where TNew : notnull =>
        Result.Applicative.Apply(result, wrappedMapping);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Result<TNew, TError>> ApplyAsync<T, TError, TNew>(
        this Task<Result<T, TError>> resultTask,
        Task<Result<Func<T, TNew>, TError>> wrappedMappingTask
    )
        where T : notnull where TError : notnull where TNew : notnull =>
        Result.Applicative.ApplyAsync(resultTask, wrappedMappingTask);
}