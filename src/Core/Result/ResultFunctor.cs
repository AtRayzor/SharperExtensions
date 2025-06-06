using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SharperExtensions;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

/// <summary>
/// Provides functional mapping operations for the
/// <see cref="Result{T, TError}" /> type.
/// </summary>
public static partial class Result
{
    /// <summary>
    /// Contains functor operations for <see cref="Result{T, TError}" />, such as
    /// mapping over success and error values.
    /// </summary>
    public static class Functor
    {
        /// <summary>
        /// Maps the success value of a <see cref="Result{T, TError}" /> to a new
        /// value, preserving the error type if the result is an error.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        /// <typeparam name="TNew">The type of the new success value.</typeparam>
        /// <param name="result">The input result to map.</param>
        /// <param name="mapping">A function to transform the success value.</param>
        /// <returns>
        /// A new <see cref="Result{TNew, TError}" /> with the mapped success value or
        /// the original error.
        /// </returns>
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
                { IsOk: true, Value: var value } => Ok<TNew, TError>(mapping(value)),
                { IsError: true, ErrorValue: var error } => Error<TNew, TError>(error)
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TNew, TError> Map<T1, T2, TError, TNew>(
            Result<(T1, T2), TError> result,
            Func<T1, T2, TNew> mapper
        )
            where T1 : notnull
            where T2 : notnull
            where TError : notnull
            where TNew : notnull => Map(result, pair => mapper(pair.Item1, pair.Item2));

        /// <summary>
        /// Maps the error value of a <see cref="Result{T, TError}" /> to a new error
        /// value, preserving the success type if the result is successful.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <typeparam name="TError">The type of the original error value.</typeparam>
        /// <typeparam name="TNewError">The type of the new error value.</typeparam>
        /// <param name="result">The input result to map the error of.</param>
        /// <param name="mapping">A function to transform the error value.</param>
        /// <returns>
        /// A new <see cref="Result{T, TNewError}" /> with the mapped error value or
        /// the original success.
        /// </returns>
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
                { IsError: true, ErrorValue: var error } =>
                    Error<T, TNewError>(mapping(error)),
                { IsOk: true, Value: var value } => Ok<T, TNewError>(value),
            };

        /// <summary>
        /// Matches on a <see cref="Result{T, TError}" />, invoking the appropriate
        /// function depending on whether the result is successful or an error, and
        /// returns the result.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        /// <typeparam name="TNew">The return type of the match functions.</typeparam>
        /// <param name="result">The input result to match on.</param>
        /// <param name="matchOk">Function to invoke if the result is successful.</param>
        /// <param name="matchError">Function to invoke if the result is an error.</param>
        /// <returns>The result of invoking the matching function.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TNew Match<T, TError, TNew>(
            Result<T, TError> result,
            Func<T, TNew> matchOk,
            Func<TError, TNew> matchError
        )
            where TNew : notnull
            where T : notnull
            where TError : notnull =>
            result switch
            {
                { IsOk: true, Value: var value } => matchOk(value),
                { IsError: true, ErrorValue: var error } => matchError(error),
            };
    }
}