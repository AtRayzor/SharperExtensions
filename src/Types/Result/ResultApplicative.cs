using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Result;

/// <summary>
/// Provides applicative operations for the <see cref="Result{T, TError}"/> type.
/// </summary>
public static partial class Result
{
    /// <summary>
    /// Contains applicative methods for the <see cref="Result{T, TError}"/> type.
    /// </summary>
    public static class Applicative
    {
        /// <summary>
        /// Applies a wrapped function to a wrapped value, producing a new <see cref="Result{TNew, TError}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the input value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TNew">The type of the output value.</typeparam>
        /// <param name="result">The <see cref="Result{T, TError}"/> containing the input value.</param>
        /// <param name="wrappedMapping">The <see>
        ///         <cref>Result{Func{T, TNew}, TError}</cref>
        ///     </see>
        ///     containing the function to apply.</param>
        /// <returns>
        /// An <see cref="Ok{T, TError}"/> containing the result of applying the function if both inputs are <see cref="Ok{T, TError}"/>,
        /// otherwise an <see cref="Error{T, TError}"/> containing the error.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TNew, TError> Apply<T, TError, TNew>(
            Result<T, TError> result,
            Result<Func<T, TNew>, TError> wrappedMapping
        )
            where T : notnull
            where TError : notnull
            where TNew : notnull =>
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            wrappedMapping switch
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            {
                Ok<Func<T, TNew>, TError> okMapping => Functor.Map(result, okMapping.Value),
                Error<Func<T, TNew>, TError> error => new Error<TNew, TError>(error),
            };
    }
}

/// <summary>
/// Provides extension methods for applicative operations on <see cref="Result{T, TError}"/>.
/// </summary>
public static class ApplicativeResultExtensions
{
    /// <summary>
    /// Applies a wrapped function to a wrapped value, producing a new <see cref="Result{TNew, TError}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <typeparam name="TNew">The type of the output value.</typeparam>
    /// <param name="result">The <see cref="Result{T, TError}"/> containing the input value.</param>
    /// <param name="wrappedMapping">The <see>
    ///         <cref>Result{Func{T, TNew}, TError}</cref>
    ///     </see>
    ///     containing the function to apply.</param>
    /// <returns>
    /// An <see cref="Ok{T, TError}"/> containing the result of applying the function if both inputs are <see cref="Ok{T, TError}"/>,
    /// otherwise an <see cref="Error{T, TError}"/> containing the error.
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TNew, TError> Apply<T, TError, TNew>(
        this Result<T, TError> result,
        Result<Func<T, TNew>, TError> wrappedMapping
    )
        where T : notnull
        where TError : notnull
        where TNew : notnull => Result.Applicative.Apply(result, wrappedMapping);
}
