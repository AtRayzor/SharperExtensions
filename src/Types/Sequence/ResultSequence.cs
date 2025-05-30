using System.Collections;
using System.Runtime.CompilerServices;
using DotNetCoreFunctional.Option;
using DotNetCoreFunctional.Result;

namespace DotNetCoreFunctional.Sequence;

/// <summary>
/// Provides static methods for creating and manipulating
/// <see cref="ResultSequence{T, TError}" /> instances.
/// </summary>
/// <remarks>
/// This static class serves as a factory and utility class for working with
/// sequences of results.
/// </remarks>
public static class ResultSequence
{
    /// <summary>
    /// Creates a new <see cref="ResultSequence{T, TError}" /> from an enumerable
    /// collection of results.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the successful value, which must be
    /// non-nullable.
    /// </typeparam>
    /// <typeparam name="TError">
    /// The type of the error value, which must be
    /// non-nullable.
    /// </typeparam>
    /// <param name="results">
    /// The collection of results to create the sequence
    /// from.
    /// </param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> containing the
    /// provided results.
    /// </returns>
    public static ResultSequence<T, TError> Create<T, TError>(
        IEnumerable<Result<T, TError>> results
    )
        where T : notnull where TError : notnull => new(results);

    /// <summary>
    /// Creates a new <see cref="ResultSequence{T, TError}" /> from a
    /// read-only span of results.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the successful value, which must be
    /// non-nullable.
    /// </typeparam>
    /// <typeparam name="TError">
    /// The type of the error value, which must be
    /// non-nullable.
    /// </typeparam>
    /// <param name="results">
    /// The read-only span of results to create the sequence
    /// from.
    /// </param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> containing the
    /// provided results.
    /// </returns>
    public static ResultSequence<T, TError> Create<T, TError>(
        ReadOnlySpan<Result<T, TError>> results
    )
        where T : notnull where TError : notnull => new(results);
}

/// <summary>
/// Represents a sequence of <see cref="Result{T, TError}" /> with collection
/// builder support.
/// </summary>
/// <typeparam name="T">
/// The type of the successful value, which must be
/// non-nullable.
/// </typeparam>
/// <typeparam name="TError">
/// The type of the error value, which must be
/// non-nullable.
/// </typeparam>
/// <remarks>
/// Provides a strongly-typed, immutable sequence of results that can be
/// created and manipulated using collection builder syntax.
/// </remarks>
[CollectionBuilder(typeof(ResultSequence), nameof(ResultSequence.Create))]
public readonly struct ResultSequence<T, TError> : ISequence<Result<T, TError>>
    where T : notnull
    where TError : notnull
{
    private readonly Sequence<Result<T, TError>> _innerSequence;

    /// <inheritdoc />
    public int Length => _innerSequence.Length;

    /// <inheritdoc />
    public bool IsEmpty => _innerSequence.IsEmpty;

    internal ResultSequence(Sequence<Result<T, TError>> results)
    {
        _innerSequence = results;
    }

    internal ResultSequence(IEnumerable<Result<T, TError>> results)
    {
        _innerSequence = Sequence<Result<T, TError>>.Create(results);
    }

    internal ResultSequence(ReadOnlySpan<Result<T, TError>> results)
    {
        _innerSequence = Sequence<Result<T, TError>>.FromSpan(results);
    }

    /// <inheritdoc />
    public IEnumerator<Result<T, TError>> GetEnumerator() =>
        _innerSequence.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public ReadOnlySpan<Result<T, TError>> AsSpan() => _innerSequence.AsSpan();

    /// <summary>
    /// Adds a new <see cref="Result{T, TError}" /> to the current
    /// <see cref="ResultSequence{T, TError}" />.
    /// </summary>
    /// <param name="result">The result to add to the sequence.</param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the added
    /// result.
    /// </returns>
    public ResultSequence<T, TError> Add(Result<T, TError> result) =>
        new(_innerSequence.Add(result));

    /// <summary>
    /// Adds a new successful result with the specified value to the current
    /// <see cref="ResultSequence{T, TError}" />.
    /// </summary>
    /// <param name="value">
    /// The value to wrap in a successful result and add to
    /// the sequence.
    /// </param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the added
    /// successful result.
    /// </returns>
    public ResultSequence<T, TError> AddOk(T value) => Add(Result<T, TError>.Ok(value));

    /// <summary>
    /// Adds a new error result with the specified error to the current
    /// <see cref="ResultSequence{T, TError}" />.
    /// </summary>
    /// <param name="error">
    /// The error to wrap in an error result and add to the
    /// sequence.
    /// </param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the added
    /// error result.
    /// </returns>
    public ResultSequence<T, TError> AddError(TError error) =>
        Add(Result<T, TError>.Error(error));

    public ResultSequence<T, TError> AddRange(IEnumerable<Result<T, TError>> result) =>
        new(_innerSequence.AddRange(result));

    /// <summary>
    /// Adds a new sequence of values as successful results to the current
    /// <see cref="ResultSequence{T, TError}" />.
    /// </summary>
    /// <param name="values">
    /// The sequence of values to wrap in successful results
    /// and add to the sequence.
    /// </param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the added
    /// successful results.
    /// </returns>
    public ResultSequence<T, TError> AddValues(Sequence<T> values)
    {
        Span<Result<T, TError>> destination = new Result<T, TError>[values.Length];
        var valueSpan = values.AsSpan();

        for (var i = 0; i < valueSpan.Length; i++)
        {
            destination[i] = Result<T, TError>.Ok(valueSpan[i]);
        }

        return new ResultSequence<T, TError>(
            _innerSequence.AddRange(destination.ToArray())
        );
    }

    /// <summary>
    /// Adds a new sequence of errors as error results to the current
    /// <see cref="ResultSequence{T, TError}" />.
    /// </summary>
    /// <param name="errors">
    /// The sequence of errors to wrap in error results and
    /// add to the sequence.
    /// </param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the added
    /// error results.
    /// </returns>
    public ResultSequence<T, TError> AddErrors(Sequence<TError> errors)
    {
        Span<Result<T, TError>> destination = new Result<T, TError>[errors.Length];
        var errorSpan = errors.AsSpan();

        for (var i = 0; i < errorSpan.Length; i++)
        {
            destination[i] = Result<T, TError>.Error(errorSpan[i]);
        }

        return new ResultSequence<T, TError>(
            _innerSequence.AddRange(destination.ToArray())
        );
    }

    /// <summary>
    /// Removes the specified result from the current
    /// <see cref="ResultSequence{T, TError}" />.
    /// </summary>
    /// <param name="result">The result to remove from the sequence.</param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the result
    /// removed.
    /// </returns>
    public ResultSequence<T, TError> Remove(Result<T, TError> result) =>
        new(_innerSequence.Remove(result));

    /// <summary>
    /// Removes the specified result from the current
    /// <see cref="ResultSequence{T, TError}" /> using a custom equality comparer.
    /// </summary>
    /// <param name="result">The result to remove from the sequence.</param>
    /// <param name="equalityComparer">
    /// The equality comparer to use when comparing
    /// results.
    /// </param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the result
    /// removed.
    /// </returns>
    public ResultSequence<T, TError> Remove(
        Result<T, TError> result,
        EqualityComparer<Result<T, TError>> equalityComparer
    ) => new(_innerSequence.Remove(result, equalityComparer));

    /// <summary>
    /// Removes all results from the current
    /// <see cref="ResultSequence{T, TError}" />.
    /// </summary>
    /// <returns>A new <see cref="ResultSequence{T, TError}" /> with no results.</returns>
    public ResultSequence<T, TError> Clear() => new(_innerSequence.Clear());

    /// <summary>
    /// Inserts a specific result into the current
    /// <see cref="ResultSequence{T, TError}" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert the result.</param>
    /// <param name="result">The result to insert into the sequence.</param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the result inserted at
    /// the specified index.
    /// </returns>
    public ResultSequence<T, TError> Insert(int index, Result<T, TError> result) =>
        new(_innerSequence.Insert(index, result));

    /// <summary>
    /// Inserts a specific value into the current
    /// <see cref="ResultSequence{T, TError}" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert the value.</param>
    /// <param name="value">The value to insert into the sequence.</param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the value inserted at
    /// the specified index.
    /// </returns>
    public ResultSequence<T, TError> Insert(int index, T value) =>
        new(_innerSequence.Insert(index, Result<T, TError>.Ok(value)));

    /// <summary>
    /// Inserts a specific error into the current
    /// <see cref="ResultSequence{T, TError}" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert the error.</param>
    /// <param name="error">The error to insert into the sequence.</param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the error inserted at
    /// the specified index.
    /// </returns>
    public ResultSequence<T, TError> Insert(int index, TError error) =>
        new(_innerSequence.Insert(index, Result<T, TError>.Error(error)));

    /// <summary>
    /// Inserts a range of results into the current
    /// <see cref="ResultSequence{T, TError}" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert the results.</param>
    /// <param name="results">
    /// The collection of results to insert into the
    /// sequence.
    /// </param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the results inserted
    /// at the specified index.
    /// </returns>
    public ResultSequence<T, TError> InsertRange(
        int index,
        IEnumerable<Result<T, TError>> results
    ) => new(_innerSequence.InsertRange(index, results));

    public ResultSequence<T, TError> InsertRange<TSequence>(
        int index,
        TSequence values
    )
        where TSequence : ISequence<T>
    {
        var valueSpan = values.AsSpan();
        Span<Result<T, TError>> destination = new Result<T, TError>[valueSpan.Length];

        for (var i = 0; i < valueSpan.Length; i++)
        {
            destination[i] = Result<T, TError>.Ok(valueSpan[i]);
        }

        return new ResultSequence<T, TError>(
            _innerSequence.InsertRange(index, destination.ToArray())
        );
    }

    public ResultSequence<T, TError> RemoveAll(Predicate<Result<T, TError>> predicate) =>
        new(_innerSequence.RemoveAll(predicate));

    /// <summary>
    /// Removes all elements from the current
    /// <see cref="ResultSequence{T, TError}" /> that match the given predicate.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with elements removed that
    /// satisfy the predicate.
    /// </returns>
    public ResultSequence<T, TError> RemoveAll(Predicate<T> predicate) =>
        RemoveAll(result => result.Match(value => predicate(value), _ => false));

    /// <summary>
    /// Removes all error elements from the current
    /// <see cref="ResultSequence{T, TError}" /> that match the given predicate.
    /// </summary>
    /// <param name="predicate">A function to test each error for a condition.</param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with error elements removed
    /// that satisfy the predicate.
    /// </returns>
    public ResultSequence<T, TError> RemoveAllErrors(Predicate<TError> predicate) =>
        RemoveAll(result => result.Match(_ => false, error => predicate(error)));

    /// <summary>
    /// Retrieves the value at the specified index from the inner sequence as an
    /// optional result.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <returns>
    /// An <see cref="Option{T}" /> containing the
    /// <see cref="Result{T, TError}" /> at the specified index, or an empty
    /// option if the index is out of range.
    /// </returns>
    private Option<Result<T, TError>> GetValueOrNone(int index) =>
        _innerSequence.GetValueOrNone(index);

    /// <summary>
    /// Retrieves the value at the specified index from the sequence, or returns
    /// an error if the index is out of range.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <param name="error">The error to return if the index is out of range.</param>
    /// <returns>
    /// A <see cref="Result{T, TError}" /> containing either the value at the
    /// specified index or the provided error.
    /// </returns>
    public Result<T, TError> GetValueOr(int index, TError error)
    {
        return GetValueOrNone(index)
            .Match(
                result => result,
                () => Result<T, TError>.Error(error)
            );
    }

    public ResultSequence<T, TError> this[Range range] => new(_innerSequence[range]);

    /// <summary>
    /// Removes the element at the specified index from the current
    /// <see cref="ResultSequence{T, TError}" />.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the element at the
    /// specified index removed.
    /// </returns>
    public ResultSequence<T, TError> RemoveAt(int index) =>
        new(_innerSequence.RemoveAt(index));

    /// <summary>
    /// Inserts a range of error values into the current
    /// <see cref="ResultSequence{T, TError}" /> at the specified index.
    /// </summary>
    /// <typeparam name="TSequence">
    /// The type of sequence containing the error
    /// values.
    /// </typeparam>
    /// <param name="index">
    /// The zero-based index at which to insert the error
    /// values.
    /// </param>
    /// <param name="errors">The sequence of error values to insert.</param>
    /// <returns>
    /// A new <see cref="ResultSequence{T, TError}" /> with the error values
    /// inserted at the specified index.
    /// </returns>
    public ResultSequence<T, TError> InsertErrorRange<TSequence>(
        int index,
        TSequence errors
    )
        where TSequence : ISequence<TError>
    {
        var errorSpan = errors.AsSpan();
        Span<Result<T, TError>> destination = new Result<T, TError>[errorSpan.Length];

        for (var i = 0; i < errorSpan.Length; i++)
        {
            destination[i] = Result<T, TError>.Error(errorSpan[i]);
        }

        return new ResultSequence<T, TError>(
            _innerSequence.InsertRange(index, destination.ToArray())
        );
    }

    /// <summary>
    /// Finds the index of the first occurrence of the specified result
    /// in the sequence.
    /// </summary>
    /// <param name="result">The result to locate in the sequence.</param>
    /// <returns>
    /// An <see cref="Option{T}" /> containing the zero-based index of the first
    /// occurrence of the result, or <see cref="Option{T}.None" /> if the result
    /// is not found.
    /// </returns>
    public Option<int> IndexOf(Result<T, TError> result) =>
        _innerSequence.IndexOf(result);

    /// <summary>
    /// Finds the index of the first occurrence of the specified value in
    /// the sequence.
    /// </summary>
    /// <param name="value">The value to locate in the sequence.</param>
    /// <returns>
    /// An <see cref="Option{T}" /> containing the zero-based index of the first
    /// occurrence of the value, or <see cref="Option{T}.None" /> if the value is
    /// not found.
    /// </returns>
    public Option<int> IndexOf(T value) => IndexOf(Result<T, TError>.Ok(value));

    /// <summary>
    /// Finds the index of the first occurrence of the specified error in
    /// the sequence.
    /// </summary>
    /// <param name="error">The error to locate in the sequence.</param>
    /// <returns>
    /// An <see cref="Option{T}" /> containing the zero-based index of the first
    /// occurrence of the error, or <see cref="Option{T}.None" /> if the error is
    /// not found.
    /// </returns>
    public Option<int> IndexOfError(TError error) =>
        IndexOf(Result<T, TError>.Error(error));

    public Option<int> IndexOf(
        Result<T, TError> result,
        EqualityComparer<Result<T, TError>> equalityComparer
    ) => _innerSequence.IndexOf(result, equalityComparer);

    /// <summary>
    /// Finds the index of the last occurrence of the specified result in
    /// the sequence.
    /// </summary>
    /// <param name="result">The result to locate in the sequence.</param>
    /// <returns>
    /// An <see cref="Option{T}" /> containing the zero-based index of the last
    /// occurrence of the result, or <see cref="Option{T}.None" /> if the result
    /// is not found.
    /// </returns>
    public Option<int> LastIndexOf(Result<T, TError> result) =>
        _innerSequence.LastIndexOf(result);

    /// <summary>
    /// Finds the index of the last occurrence of the specified value in
    /// the sequence.
    /// </summary>
    /// <param name="value">The value to locate in the sequence.</param>
    /// <returns>
    /// An <see cref="Option{T}" /> containing the zero-based index of the last
    /// occurrence of the value, or <see cref="Option{T}.None" /> if the value is
    /// not found.
    /// </returns>
    public Option<int> LastIndexOf(T value) => LastIndexOf(Result<T, TError>.Ok(value));

    /// <summary>
    /// Finds the index of the last occurrence of the specified error in
    /// the sequence.
    /// </summary>
    /// <param name="error">The error to locate in the sequence.</param>
    /// <returns>
    /// An <see cref="Option{T}" /> containing the zero-based index of the last
    /// occurrence of the error, or <see cref="Option{T}.None" /> if the error is
    /// not found.
    /// </returns>
    public Option<int> LastIndexOfError(TError error) =>
        LastIndexOf(Result<T, TError>.Error(error));

    /// <summary>
    /// Finds the index of the last occurrence of the specified result in the
    /// sequence using a custom equality comparer.
    /// </summary>
    /// <param name="result">The result to locate in the sequence.</param>
    /// <param name="equalityComparer">
    /// The equality comparer to use for comparing
    /// results.
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}" /> containing the zero-based index of the last
    /// occurrence of the result, or <see cref="Option{T}.None" /> if the result
    /// is not found.
    /// </returns>
    public Option<int> LastIndexOf(
        Result<T, TError> result,
        EqualityComparer<Result<T, TError>> equalityComparer
    ) => _innerSequence.LastIndexOf(result, equalityComparer);

    /// <summary>Finds all indices of the specified result in the sequence.</summary>
    /// <param name="result">The result to locate in the sequence.</param>
    /// <returns>
    /// A <see cref="Sequence{T}" /> containing the zero-based indices of all
    /// occurrences of the result.
    /// </returns>
    public Sequence<int> AllIndicesOf(Result<T, TError> result) =>
        _innerSequence.AllIndicesOf(result);

    /// <summary>Finds all indices of the specified value in the sequence.</summary>
    /// <param name="value">The value to locate in the sequence.</param>
    /// <returns>
    /// A <see cref="Sequence{T}" /> containing the zero-based indices of all
    /// occurrences of the value.
    /// </returns>
    public Sequence<int> AllIndicesOf(T value) =>
        AllIndicesOf(Result<T, TError>.Ok(value));

    /// <summary>Finds all indices of the specified error in the sequence.</summary>
    /// <param name="error">The error to locate in the sequence.</param>
    /// <returns>
    /// A <see cref="Sequence{T}" /> containing the zero-based indices of all
    /// occurrences of the error.
    /// </returns>
    public Sequence<int> AllIndicesOfError(TError error) =>
        AllIndicesOf(Result<T, TError>.Error(error));

    /// <summary>
    /// Finds all indices of the specified result in the sequence using a custom
    /// equality comparer.
    /// </summary>
    /// <param name="result">The result to locate in the sequence.</param>
    /// <param name="equalityComparer">
    /// The equality comparer to use for comparing
    /// results.
    /// </param>
    /// <returns>
    /// A <see cref="Sequence{T}" /> containing the zero-based indices of all
    /// occurrences of the result.
    /// </returns>
    public Sequence<int> AllIndicesOf(
        Result<T, TError> result,
        EqualityComparer<Result<T, TError>> equalityComparer
    ) =>
        _innerSequence.AllIndicesOf(result, equalityComparer);

    /// <inheritdoc />
    public Sequence<Result<T, TError>> AsSequence() => _innerSequence;

    ISequence<Result<T, TError>> ISequence<Result<T, TError>>.
        Add(Result<T, TError> item) => Add(item);

    ISequence<Result<T, TError>> ISequence<Result<T, TError>>.AddRange(
        IEnumerable<Result<T, TError>> range
    ) => AddRange(range);

    ISequence<Result<T, TError>> ISequence<Result<T, TError>>.Remove(
        Result<T, TError> item
    ) => Remove(item);

    ISequence<Result<T, TError>> ISequence<Result<T, TError>>.Remove(
        Result<T, TError> item,
        EqualityComparer<Result<T, TError>> equalityComparer
    ) => Remove(item, equalityComparer);

    ISequence<Result<T, TError>> ISequence<Result<T, TError>>.Clear() => Clear();

    ISequence<Result<T, TError>> ISequence<Result<T, TError>>.Insert(
        int index,
        Result<T, TError> item
    ) => Insert(index, item);

    ISequence<Result<T, TError>> ISequence<Result<T, TError>>.InsertRange(
        int index,
        IEnumerable<Result<T, TError>> range
    ) => InsertRange(index, range);

    ISequence<Result<T, TError>> ISequence<Result<T, TError>>.RemoveAt(int index) =>
        RemoveAt(index);

    ISequence<Result<T, TError>> ISequence<Result<T, TError>>.RemoveAll(
        Predicate<Result<T, TError>> predicate
    ) => RemoveAll(predicate);

    Option<Result<T, TError>> ISequence<Result<T, TError>>.GetValueOrNone(int index) =>
        GetValueOrNone(index);

    Option<Result<T, TError>> ISequence<Result<T, TError>>.this[int index] =>
        _innerSequence[index];

    ISequence<Result<T, TError>> ISequence<Result<T, TError>>.this[Range range] =>
        this[range];

    ISequence<int> ISequence<Result<T, TError>>.AllIndicesOf(Result<T, TError> item) =>
        AllIndicesOf(item);

    ISequence<int> ISequence<Result<T, TError>>.AllIndicesOf(
        Result<T, TError> item,
        EqualityComparer<Result<T, TError>> equalityComparer
    ) => AllIndicesOf(item, equalityComparer);
}