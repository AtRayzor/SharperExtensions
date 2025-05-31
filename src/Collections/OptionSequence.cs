using System.Collections;
using System.Runtime.CompilerServices;

namespace SharperExtensions.Collections;

/// <summary>
/// Provides static methods for creating and manipulating
/// <see cref="OptionSequence{T}" /> instances.
/// </summary>
/// <remarks>
/// This static class offers utility methods to construct option sequences
/// from various input types, facilitating functional-style handling of
/// optional value collections.
/// </remarks>
public static class OptionSequence
{
    /// <summary>
    /// Creates an <see cref="OptionSequence{T}" /> from an
    /// <see cref="IEnumerable{T}" /> of nullable values.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the sequence, which must be
    /// non-nullable.
    /// </typeparam>
    /// <param name="enumerable">
    /// An enumerable collection of nullable values to
    /// create the sequence from.
    /// </param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> containing the provided
    /// values converted to options.
    /// </returns>
    public static OptionSequence<T> Create<T>(IEnumerable<T?> enumerable)
        where T : notnull => new(enumerable.Cast<Option<T>>());

    /// <summary>
    /// Creates an <see cref="OptionSequence{T}" /> from a
    /// <see cref="ReadOnlySpan{T}" /> of <see cref="Option{T}" /> values.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the sequence, which must be
    /// non-nullable.
    /// </typeparam>
    /// <param name="optionSpan">
    /// A read-only span of optional values to create the
    /// sequence from.
    /// </param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> containing the provided
    /// optional values.
    /// </returns>
    public static OptionSequence<T> Create<T>(ReadOnlySpan<Option<T>> optionSpan)
        where T : notnull => new(optionSpan);
}

/// <summary>
/// Represents a sequence of optional values of type
/// <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">
/// The type of elements in the sequence, which must be
/// non-nullable.
/// </typeparam>
/// <remarks>
/// Provides a collection-like structure for handling sequences of optional
/// values with additional functional programming methods.
/// </remarks>
[CollectionBuilder(typeof(OptionSequence), nameof(OptionSequence.Create))]
public readonly struct OptionSequence<T> : ISequence<Option<T>>
    where T : notnull
{
    private readonly Sequence<Option<T>> _innerSequence;

    public IEnumerator<Option<T>> GetEnumerator() => _innerSequence.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    internal OptionSequence(IEnumerable<Option<T>> options)
    {
        _innerSequence = SequenceInstance.Create(options);
    }

    internal OptionSequence(Sequence<Option<T>> options)
    {
        _innerSequence = options;
    }

    internal OptionSequence(ReadOnlySpan<Option<T>> options)
    {
        _innerSequence = [..options];
    }

    /// <summary>
    /// Gets the number of elements in the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <value>The total count of elements in the sequence.</value>
    public int Length => _innerSequence.Length;

    /// <summary>
    /// Determines whether the <see cref="OptionSequence{T}" /> contains
    /// no elements.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if the sequence is empty; otherwise,
    /// <see langword="false" />.
    /// </value>
    public bool IsEmpty => _innerSequence.IsEmpty;

    /// <summary>
    /// Converts the <see cref="OptionSequence{T}" /> to a read-only span
    /// of optional values.
    /// </summary>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}" /> containing the optional values
    /// in the sequence.
    /// </returns>
    public ReadOnlySpan<Option<T>> AsSpan() => _innerSequence.AsSpan();

    /// <inheritdoc />
    public Sequence<Option<T>> AsSequence() => _innerSequence;

    /// <summary>Adds an optional value to the <see cref="OptionSequence{T}" />.</summary>
    /// <param name="option">The optional value to add to the sequence.</param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the added optional
    /// value.
    /// </returns>
    public OptionSequence<T> Add(Option<T> option) => new(_innerSequence.Add(option));

    /// <summary>
    /// Adds a value to the <see cref="OptionSequence{T}" /> by wrapping it in an
    /// <see cref="Option{T}.Some" /> instance.
    /// </summary>
    /// <param name="value">The value to add to the sequence.</param>
    /// <returns>A new <see cref="OptionSequence{T}" /> with the added value.</returns>
    public OptionSequence<T> Add(T value) => Add(Option<T>.Some(value));

    /// <summary>
    /// Adds a <see cref="Option{T}.None" /> value to the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the added
    /// <see cref="Option{T}.None" /> value.
    /// </returns>
    public OptionSequence<T> AddNone() => Add(Option<T>.None);

    /// <summary>
    /// Adds a range of optional values to the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="range">
    /// The collection of optional values to add to the
    /// sequence.
    /// </param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the added optional
    /// values.
    /// </returns>
    public OptionSequence<T> AddRange(IEnumerable<Option<T>> range) =>
        new(_innerSequence.AddRange(range));

    /// <summary>
    /// Removes a specific optional value from the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="option">The optional value to remove from the sequence.</param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the specified
    /// optional value removed.
    /// </returns>
    public OptionSequence<T> Remove(Option<T> option) =>
        new(_innerSequence.Remove(option));

    /// <summary>
    /// Removes a specific value from the <see cref="OptionSequence{T}" /> by
    /// wrapping it in an <see cref="Option{T}.Some" /> instance.
    /// </summary>
    /// <param name="value">The value to remove from the sequence.</param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the specified value
    /// removed.
    /// </returns>
    public OptionSequence<T> Remove(T value) => Remove(Option<T>.Some(value));

    public OptionSequence<T> Remove(
        Option<T> option,
        EqualityComparer<Option<T>> equalityComparer
    ) => new(_innerSequence.Remove(option, equalityComparer));

    /// <summary>Removes all elements from the <see cref="OptionSequence{T}" />.</summary>
    /// <returns>A new <see cref="OptionSequence{T}" /> with no elements.</returns>
    public OptionSequence<T> Clear() => new(_innerSequence.Clear());

    /// <summary>
    /// Inserts an optional value at the specified index in the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which to insert the optional
    /// value.
    /// </param>
    /// <param name="option">The optional value to insert into the sequence.</param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the optional value inserted at
    /// the specified index.
    /// </returns>
    public OptionSequence<T> Insert(int index, Option<T> option) =>
        new(_innerSequence.Insert(index, option));

    /// <summary>
    /// Inserts a value at the specified index in the
    /// <see cref="OptionSequence{T}" /> by wrapping it in an
    /// <see cref="Option{T}.Some" /> instance.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert the value.</param>
    /// <param name="value">The value to insert into the sequence.</param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the value inserted at
    /// the specified index.
    /// </returns>
    public OptionSequence<T> Insert(int index, T value) =>
        Insert(index, Option<T>.Some(value));

    /// <summary>
    /// Inserts a range of optional values at the specified index in the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which to insert the optional
    /// values.
    /// </param>
    /// <param name="range">
    /// The collection of optional values to insert into the
    /// sequence.
    /// </param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the optional values inserted
    /// at the specified index.
    /// </returns>
    public OptionSequence<T> InsertRange(int index, IEnumerable<Option<T>> range) =>
        new(_innerSequence.InsertRange(index, range));

    /// <summary>
    /// Inserts a range of values at the specified index in the
    /// <see cref="OptionSequence{T}" /> by wrapping each value in an
    /// <see cref="Option{T}.Some" /> instance.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert the values.</param>
    /// <param name="values">The collection of values to insert into the sequence.</param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the values inserted
    /// at the specified index.
    /// </returns>
    public OptionSequence<T> InsertRange(int index, IEnumerable<T> values) =>
        InsertRange(index, values.Select(Option<T>.Some));

    /// <summary>
    /// Removes the element at the specified index from the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the element at the
    /// specified index removed.
    /// </returns>
    public OptionSequence<T> RemoveAt(int index) => new(_innerSequence.RemoveAt(index));

    /// <summary>
    /// Removes all elements from the <see cref="OptionSequence{T}" /> that
    /// satisfy the given predicate.
    /// </summary>
    /// <param name="predicate">
    /// A function that determines whether an optional
    /// element should be removed.
    /// </param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with elements removed
    /// based on the predicate.
    /// </returns>
    public OptionSequence<T> RemoveAll(Predicate<Option<T>> predicate) =>
        new(_innerSequence.RemoveAll(predicate));

    /// <summary>
    /// Removes all elements from the <see cref="OptionSequence{T}" /> that
    /// satisfy the given predicate.
    /// </summary>
    /// <param name="predicate">
    /// A function that determines whether an element
    /// should be removed.
    /// </param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with elements removed
    /// based on the predicate.
    /// </returns>
    public OptionSequence<T> RemoveAll(Predicate<T> predicate)
    {
        Predicate<Option<T>> optionPredicate = option =>
            option.Match(
                value => predicate(value),
                () => false
            );

        return new OptionSequence<T>(_innerSequence.RemoveAll(optionPredicate));
    }

    /// <summary>
    /// Removes all <see cref="Option{T}.None" /> elements from the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with all
    /// <see cref="Option{T}.None" /> elements removed.
    /// </returns>
    public OptionSequence<T> RemoveNone() => RemoveAll(option => option.IsNone);

    /// <summary>
    /// Gets the value at the specified index in the
    /// <see cref="OptionSequence{T}" /> as an <see cref="Option{T}" />, or
    /// returns <see cref="Option{T}.None" /> if the index is out of range.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <returns>
    /// An <see cref="Option{T}" /> containing the value at the specified index,
    /// or <see cref="Option{T}.None" /> if the index is out of range.
    /// </returns>
    public Option<T> GetValueOrNone(int index) => GetValueOrNoneNested(index).Flatten();

    private Option<Option<T>> GetValueOrNoneNested(int index) =>
        _innerSequence.GetValueOrNone(index);

    /// <summary>
    /// Gets the value at the specified index in the
    /// <see cref="OptionSequence{T}" /> as an <see cref="Option{T}" />.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <returns>
    /// An <see cref="Option{T}" /> containing the value at the specified index,
    /// or <see cref="Option{T}.None" /> if the index is out of range.
    /// </returns>
    public Option<T> this[int index] => GetValueOrNone(index);

    /// <summary>
    /// Gets a new <see cref="OptionSequence{T}" /> containing the elements within
    /// the specified range.
    /// </summary>
    /// <param name="range">
    /// The range of elements to retrieve from the
    /// <see cref="OptionSequence{T}" />.
    /// </param>
    /// <returns>
    /// A new <see cref="OptionSequence{T}" /> with the elements
    /// specified by the range.
    /// </returns>
    public OptionSequence<T> this[Range range] => new(_innerSequence[range]);

    /// <summary>
    /// Gets the index of the first occurrence of the specified
    /// <see cref="Option{T}" /> in the <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="option">
    /// The <see cref="Option{T}" /> to locate in the
    /// sequence.
    /// </param>
    /// <returns>
    /// An <see cref="Option{int}" /> containing the index of the first occurrence
    /// of the option, or <see cref="Option{int}.None" /> if the option is not
    /// found.
    /// </returns>
    public Option<int> IndexOf(Option<T> option) => _innerSequence.IndexOf(option);

    /// <summary>
    /// Gets the index of the first occurrence of the specified value in the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="value">The value to locate in the sequence.</param>
    /// <returns>
    /// An <see cref="Option{int}" /> containing the index of the first occurrence
    /// of the value, or <see cref="Option{int}.None" /> if the value is not
    /// found.
    /// </returns>
    public Option<int> IndexOf(T value) => IndexOf(Option<T>.Some(value));

    /// <summary>
    /// Gets the index of the first occurrence of <see cref="Option{T}.None" /> in
    /// the <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <returns>
    /// An <see cref="Option{int}" /> containing the index of the first
    /// <see cref="Option{T}.None" /> element, or <see cref="Option{int}.None" />
    /// if no such element exists.
    /// </returns>
    public Option<int> IndexOfNone() => IndexOf(Option<T>.None);

    /// <summary>
    /// Gets the index of the first occurrence of the specified
    /// <see cref="Option{T}" /> in the <see cref="OptionSequence{T}" /> using the
    /// provided equality comparer.
    /// </summary>
    /// <param name="option">
    /// The <see cref="Option{T}" /> to locate in the
    /// sequence.
    /// </param>
    /// <param name="equalityComparer">
    /// The <see cref="EqualityComparer{T}" /> to
    /// use when comparing options.
    /// </param>
    /// <returns>
    /// An <see cref="Option{int}" /> containing the index of the first occurrence
    /// of the option, or <see cref="Option{int}.None" /> if the option is not
    /// found.
    /// </returns>
    public Option<int> IndexOf(
        Option<T> option,
        EqualityComparer<Option<T>> equalityComparer
    ) => _innerSequence.IndexOf(option, equalityComparer);

    /// <summary>
    /// Gets the index of the last occurrence of the specified
    /// <see cref="Option{T}" /> in the <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="option">
    /// The <see cref="Option{T}" /> to locate in the
    /// sequence.
    /// </param>
    /// <returns>
    /// An <see cref="Option{int}" /> containing the index of the last occurrence
    /// of the option, or <see cref="Option{int}.None" /> if the option is not
    /// found.
    /// </returns>
    public Option<int> LastIndexOf(Option<T> option) =>
        _innerSequence.LastIndexOf(option);

    /// <summary>
    /// Gets the index of the last occurrence of the specified value in the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="value">The value to locate in the sequence.</param>
    /// <returns>
    /// An <see cref="Option{int}" /> containing the index of the last occurrence
    /// of the value, or <see cref="Option{int}.None" /> if the value is not
    /// found.
    /// </returns>
    public Option<int> LastIndexOf(T value) => LastIndexOf(Option<T>.Some(value));

    /// <summary>
    /// Gets the index of the last occurrence of <see cref="Option{T}.None" /> in
    /// the <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <returns>
    /// An <see cref="Option{int}" /> containing the index of the last
    /// <see cref="Option{T}.None" /> element, or <see cref="Option{int}.None" />
    /// if no such element exists.
    /// </returns>
    public Option<int> LastIndexOfNone() => LastIndexOf(Option<T>.None);

    /// <summary>
    /// Gets the index of the last occurrence of the specified
    /// <see cref="Option{T}" /> in the <see cref="OptionSequence{T}" /> using the
    /// provided equality comparer.
    /// </summary>
    /// <param name="option">
    /// The <see cref="Option{T}" /> to locate in the
    /// sequence.
    /// </param>
    /// <param name="equalityComparer">
    /// The <see cref="EqualityComparer{T}" /> to
    /// use when comparing options.
    /// </param>
    /// <returns>
    /// An <see cref="Option{int}" /> containing the index of the last occurrence
    /// of the option, or <see cref="Option{int}.None" /> if the option is not
    /// found.
    /// </returns>
    public Option<int> LastIndexOf(
        Option<T> option,
        EqualityComparer<Option<T>> equalityComparer
    ) => _innerSequence.LastIndexOf(option, equalityComparer);

    /// <summary>
    /// Gets all indices of the specified <see cref="Option{T}" /> in the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="option">
    /// The <see cref="Option{T}" /> to locate in the
    /// sequence.
    /// </param>
    /// <returns>
    /// A <see cref="Sequence{int}" /> containing the indices of all
    /// occurrences of the option.
    /// </returns>
    public Sequence<int> AllIndicesOf(Option<T> option) =>
        _innerSequence.AllIndicesOf(option);

    /// <summary>
    /// Gets all indices of the specified value in the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <param name="value">The value to locate in the sequence.</param>
    /// <returns>
    /// A <see cref="Sequence{int}" /> containing the indices of all
    /// occurrences of the value.
    /// </returns>
    public Sequence<int> AllIndicesOf(T value) => AllIndicesOf(Option<T>.Some(value));

    /// <summary>
    /// Gets all indices of <see cref="Option{T}.None" /> in the
    /// <see cref="OptionSequence{T}" />.
    /// </summary>
    /// <returns>
    /// A <see cref="Sequence{int}" /> containing the indices of all
    /// <see cref="Option{T}.None" /> elements.
    /// </returns>
    public Sequence<int> AllIndicesOfNone() => AllIndicesOf(Option<T>.None);

    /// <summary>
    /// Gets all indices of the specified <see cref="Option{T}" /> in the
    /// <see cref="OptionSequence{T}" /> using the provided equality comparer.
    /// </summary>
    /// <param name="option">
    /// The <see cref="Option{T}" /> to locate in the
    /// sequence.
    /// </param>
    /// <param name="equalityComparer">
    /// The <see cref="EqualityComparer{T}" /> to
    /// use when comparing options.
    /// </param>
    /// <returns>
    /// A <see cref="Sequence{int}" /> containing the indices of all
    /// occurrences of the option.
    /// </returns>
    public Sequence<int> AllIndicesOf(
        Option<T> option,
        EqualityComparer<Option<T>> equalityComparer
    ) => _innerSequence.AllIndicesOf(option, equalityComparer);

    ISequence<Option<T>> ISequence<Option<T>>.Add(Option<T> option) => Add(option);

    ISequence<Option<T>> ISequence<Option<T>>.AddRange(IEnumerable<Option<T>> range) =>
        AddRange(range);

    ISequence<Option<T>> ISequence<Option<T>>.Remove(Option<T> option) => Remove(option);

    ISequence<Option<T>> ISequence<Option<T>>.Remove(
        Option<T> option,
        EqualityComparer<Option<T>> equalityComparer
    ) =>
        Remove(option);

    ISequence<Option<T>> ISequence<Option<T>>.Clear() => Clear();

    ISequence<Option<T>> ISequence<Option<T>>.Insert(int index, Option<T> option) =>
        Insert(index, option);

    ISequence<Option<T>> ISequence<Option<T>>.InsertRange(
        int index,
        IEnumerable<Option<T>> range
    ) =>
        InsertRange(index, range);

    ISequence<Option<T>> ISequence<Option<T>>.RemoveAt(int index) => RemoveAt(index);

    ISequence<Option<T>> ISequence<Option<T>>.RemoveAll(Predicate<Option<T>> predicate) =>
        _innerSequence.RemoveAll(predicate);

    Option<Option<T>> ISequence<Option<T>>.GetValueOrNone(int index) =>
        GetValueOrNone(index);

    Option<Option<T>> ISequence<Option<T>>.this[int index] => this[index];

    ISequence<Option<T>> ISequence<Option<T>>.this[Range range] => this[range];

    ISequence<int> ISequence<Option<T>>.AllIndicesOf(
        Option<T> option,
        EqualityComparer<Option<T>> equalityComparer
    )
        => AllIndicesOf(option, equalityComparer);

    ISequence<int> ISequence<Option<T>>.AllIndicesOf(Option<T> option) =>
        AllIndicesOf(option);
}