using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Result;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
public static partial class Result
{
    /// <summary>
    /// Provides monadic operations for the
    /// <see cref="Result{T, TError}" /> type.
    /// </summary>
    public static class Monad
    {
        /// <summary>
        /// Binds a function over a <see cref="Result{T, TError}" />, returning a new
        /// <see cref="Result{TNew, TError}" />. If the input is an
        /// <see cref="Ok{T, TError}" />, applies the binder function to its value. If
        /// the input is an <see cref="Error{T, TError}" />, returns an
        /// <see cref="Error{TNew, TError}" /> with the same error.
        /// </summary>
        /// <typeparam name="T">The type of the success value in the input result.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        /// <typeparam name="TNew">The type of the success value in the output result.</typeparam>
        /// <param name="result">The input result to bind over.</param>
        /// <param name="binder">The function to apply if the result is successful.</param>
        /// <returns>
        /// A new <see cref="Result{TNew, TError}" /> after applying the binder or
        /// propagating the error.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TNew, TError> Bind<T, TError, TNew>(
            Result<T, TError> result,
            Func<T, Result<TNew, TError>> binder
        )
            where T : notnull
            where TError : notnull
            where TNew : notnull =>
            result switch
            {
                Ok<T, TError> ok => binder(ok),
                Error<T, TError> error => new Error<TNew, TError>(error),
            };

        /// <summary>
        /// Binds a function over the error of a <see cref="Result{T, TError}" /> ,
        /// returning a new <see cref="Result{T, TNewError}" />. If the input is an
        /// <see cref="Error{T, TError}" />, applies the binder function to its error.
        /// If the input is an <see cref="Ok{T, TError}" /> , returns an
        /// <see cref="Ok{T, TNewError}" /> with the same value.
        /// </summary>
        /// <typeparam name="T">The type of the success value in the result.</typeparam>
        /// <typeparam name="TError">The type of the original error value.</typeparam>
        /// <typeparam name="TNewError">The type of the new error value.</typeparam>
        /// <param name="result">The input result to bind over.</param>
        /// <param name="binder">The function to apply if the result is an error.</param>
        /// <returns>
        /// A new <see cref="Result{T, TNewError}" /> after applying the binder or
        /// preserving the success value.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T, TNewError> BindError<T, TError, TNewError>(
            Result<T, TError> result,
            Func<TError, Result<T, TNewError>> binder
        )
            where T : notnull
            where TError : notnull
            where TNewError : notnull =>
            result switch
            {
                Ok<T, TError> { Value: var value } => new Ok<T, TNewError>(value),
                Error<T, TError> { Err: var error } => binder(error),
            };

        /// <summary>
        /// Flattens a nested <see cref="Result{T, TError}" /> structure, converting a
        /// <see>
        ///     <cref>Result{Result{T, TError}, TError}</cref>
        /// </see>
        /// into a <see cref="Result{T, TError}" />. If the outer result is
        /// <see>
        ///     <cref>Ok{Result{T, TError}, TError}</cref>
        /// </see>
        /// , returns the inner result. If the outer result is
        /// <see>
        ///     <cref>Error{Result{T, TError}, TError}</cref>
        /// </see>
        /// , returns an <see cref="Error{T, TError}" /> with the same error.
        /// </summary>
        /// <typeparam name="T">The type of the success value in the inner result.</typeparam>
        /// <typeparam name="TError">The type of the error value.</typeparam>
        /// <param name="wrappedResult">The nested result to flatten.</param>
        /// <returns>A flattened <see cref="Result{T, TError}" />.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T, TError> Flatten<T, TError>(
            Result<Result<T, TError>, TError> wrappedResult
        )
            where T : notnull
            where TError : notnull =>
            wrappedResult switch
            {
                Ok<Result<T, TError>, TError> result => result.Value,
                Error<Result<T, TError>, TError> error => new Error<T, TError>(error),
            };
    }
}