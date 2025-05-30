using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DotNetCoreFunctional.Option;

namespace DotNetCoreFunctional.Sequence;

/// <summary>
/// Represents an immutable, equatable sequence of elements of type
/// <typeparamref name="T" /> .
/// </summary>
/// <typeparam name="T">
/// The type of elements in the sequence, which must
/// implement <see cref="IEquatable{T}" />.
/// </typeparam>
/// <remarks>
/// Provides value-based equality and collection-like operations while
/// maintaining immutability.
/// </remarks>
[CollectionBuilder(typeof(EquatableSequence), nameof(EquatableSequence.Create))]
public readonly struct EquatableSequence<T>
    : ISequence<T>,
        IEquatable<EquatableSequence<T>> where T : IEquatable<T>
{
    private readonly Sequence<T> _innerSequence;

    internal EquatableSequence(Sequence<T> sequence)
    {
        _innerSequence = sequence;
    }

    internal EquatableSequence(IEnumerable<T> enumerable)
    {
        _innerSequence = new Sequence<T>(enumerable);
    }

    internal EquatableSequence(ReadOnlySpan<T> items)
    {
        _innerSequence = Sequence<T>.FromSpan(items);
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _innerSequence.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Length => _innerSequence.Length;
    public bool IsEmpty => _innerSequence.IsEmpty;

    /// <inheritdoc />
    public ReadOnlySpan<T> AsSpan() => _innerSequence.AsSpan();

    public EquatableSequence<T> Add(T item) => new(_innerSequence.Add(item));

    /// <summary>
    /// Creates a new <see cref="EquatableSequence{T}" /> by adding a range of
    /// elements to the current sequence.
    /// </summary>
    /// <param name="range">The collection of elements to add to the sequence.</param>
    /// <returns>
    /// A new <see cref="EquatableSequence{T}" /> containing the original elements
    /// and the added range.
    /// </returns>
    public EquatableSequence<T> AddRange(IEnumerable<T> range) =>
        new(_innerSequence.AddRange(range));

    /// <summary>
    /// Removes the first occurrence of the specified item from the
    /// sequence.
    /// </summary>
    /// <param name="item">The item to remove from the sequence.</param>
    /// <returns>
    /// A new <see cref="EquatableSequence{T}" /> with the first
    /// occurrence of the item removed.
    /// </returns>
    public EquatableSequence<T> Remove(T item) => new(_innerSequence.Remove(item));

    /// <summary>
    /// Removes the first occurrence of the specified item from the sequence using
    /// a custom equality comparer.
    /// </summary>
    /// <param name="item">The item to remove from the sequence.</param>
    /// <param name="equalityComparer">
    /// The equality comparer to use when comparing
    /// items.
    /// </param>
    /// <returns>
    /// A new <see cref="EquatableSequence{T}" /> with the first
    /// occurrence of the item removed.
    /// </returns>
    public EquatableSequence<T> Remove(T item, EqualityComparer<T> equalityComparer) =>
        new(_innerSequence.Remove(item, equalityComparer));

    /// <summary>
    /// Creates a new empty <see cref="EquatableSequence{T}" />, removing all
    /// elements from the current sequence.
    /// </summary>
    /// <returns>A new <see cref="EquatableSequence{T}" /> with no elements.</returns>
    public EquatableSequence<T> Clear() => [];

    /// <summary>Inserts an item at the specified index in the sequence.</summary>
    /// <param name="index">
    /// The zero-based index at which the item should be
    /// inserted.
    /// </param>
    /// <param name="item">The item to insert into the sequence.</param>
    /// <returns>
    /// A new <see cref="EquatableSequence{T}" /> with the item inserted
    /// at the specified index.
    /// </returns>
    public EquatableSequence<T> Insert(int index, T item) =>
        new(_innerSequence.Insert(index, item));

    /// <summary>
    /// Inserts a range of elements at the specified index in the
    /// sequence.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which the range should be
    /// inserted.
    /// </param>
    /// <param name="range">
    /// The collection of elements to insert into the
    /// sequence.
    /// </param>
    /// <returns>
    /// A new <see cref="EquatableSequence{T}" /> with the range inserted at the
    /// specified index.
    /// </returns>
    public EquatableSequence<T> InsertRange(int index, IEnumerable<T> range) =>
        new(_innerSequence.InsertRange(index, range));

    /// <summary>Removes the element at the specified index from the sequence.</summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <returns>
    /// A new <see cref="EquatableSequence{T}" /> with the element at the
    /// specified index removed.
    /// </returns>
    public EquatableSequence<T> RemoveAt(int index) =>
        new(_innerSequence.RemoveAt(index));

    /// <summary>
    /// Removes all elements that match the conditions defined by the
    /// specified predicate.
    /// </summary>
    /// <param name="predicate">
    /// The predicate that defines the conditions of the
    /// elements to remove.
    /// </param>
    /// <returns>
    /// A new <see cref="EquatableSequence{T}" /> with elements removed
    /// based on the predicate.
    /// </returns>
    public EquatableSequence<T> RemoveAll(Predicate<T> predicate) =>
        new(_innerSequence.RemoveAll(predicate));

    /// <inheritdoc />
    public Option<T> GetValueOrNone(int index) => _innerSequence.GetValueOrNone(index);

    /// <inheritdoc />
    public Option<T> this[int index] => _innerSequence[index];

    public EquatableSequence<T> this[Range range] => new(_innerSequence[range]);

    /// <inheritdoc />
    public Option<int> IndexOf(T option) => _innerSequence.IndexOf(option);

    /// <inheritdoc />
    public Option<int> IndexOf(T option, EqualityComparer<T> equalityComparer) =>
        _innerSequence.IndexOf(option, equalityComparer);

    /// <inheritdoc />
    public Option<int> LastIndexOf(T option) => _innerSequence.LastIndexOf(option);

    /// <inheritdoc />
    public Option<int> LastIndexOf(T option, EqualityComparer<T> equalityComparer) =>
        _innerSequence.LastIndexOf(option, equalityComparer);

    public ISequence<int> AllIndicesOf(T item) => _innerSequence.AllIndicesOf(item);

    public ISequence<int> AllIndicesOf(T item, EqualityComparer<T> equalityComparer) =>
        _innerSequence.AllIndicesOf(item, equalityComparer);

    /// <inheritdoc />
    public Sequence<T> AsSequence() => _innerSequence;

    ISequence<T> ISequence<T>.Add(T item) => Add(item);

    ISequence<T> ISequence<T>.AddRange(IEnumerable<T> range) => AddRange(range);

    ISequence<T> ISequence<T>.Remove(T item) => Remove(item);

    ISequence<T> ISequence<T>.Remove(T item, EqualityComparer<T> equalityComparer) =>
        Remove(item, equalityComparer);

    ISequence<T> ISequence<T>.Clear() => Clear();

    ISequence<T> ISequence<T>.Insert(int index, T item) => Insert(index, item);

    ISequence<T> ISequence<T>.InsertRange(int index, IEnumerable<T> range) =>
        InsertRange(index, range);

    ISequence<T> ISequence<T>.RemoveAt(int index) => RemoveAt(index);

    ISequence<T> ISequence<T>.RemoveAll(Predicate<T> predicate) =>
        RemoveAll(predicate);

    ISequence<T> ISequence<T>.this[Range range] => this[range];

    ISequence<int> ISequence<T>.AllIndicesOf(T item) => AllIndicesOf(item);

    ISequence<int> ISequence<T>.AllIndicesOf(
        T item,
        EqualityComparer<T> equalityComparer
    ) => AllIndicesOf(item, equalityComparer);

    /// <inheritdoc />
    public bool Equals(EquatableSequence<T> other)
    {
        return AsSpan().SequenceEqual(other.AsSpan());
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is EquatableSequence<T> other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var item in _innerSequence)
        {
            hash.Add(item);
        }

        return hash.ToHashCode();
    }
}

/// <summary>
/// Provides static methods for creating and working with
/// <see cref="EquatableSequence{T}" /> instances.
/// </summary>
public static class EquatableSequence
{
    /// <summary>
    /// Creates a new <see cref="EquatableSequence{T}" /> from the given
    /// enumerable collection.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the sequence, which must
    /// implement <see cref="IEquatable{T}" />.
    /// </typeparam>
    /// <param name="enumerable">
    /// The collection of elements to initialize the
    /// sequence with.
    /// </param>
    /// <returns>
    /// A new <see cref="EquatableSequence{T}" /> containing the elements from the
    /// input enumerable.
    /// </returns>
    public static EquatableSequence<T> Create<T>(IEnumerable<T> enumerable)
        where T : IEquatable<T> => new(enumerable);

    /// <summary>
    /// Creates a new <see cref="EquatableSequence{T}" /> from the given read-only
    /// span of elements.
    /// </summary>
    /// <typeparam name="T">
    /// The type of elements in the sequence, which must
    /// implement <see cref="IEquatable{T}" />.
    /// </typeparam>
    /// <param name="items">
    /// The read-only span of elements to initialize the
    /// sequence with.
    /// </param>
    /// <returns>
    /// A new <see cref="EquatableSequence{T}" /> containing the elements
    /// from the input span.
    /// </returns>
    public static EquatableSequence<T> Create<T>(ReadOnlySpan<T> items)
        where T : IEquatable<T> => new();
}