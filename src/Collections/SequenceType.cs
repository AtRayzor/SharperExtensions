using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SharperExtensions.Collections;

/// <summary>
/// Provides internal implementation methods for creating and managing sequences.
/// </summary>
/// <remarks>
/// This internal static class serves as a helper for sequence creation and manipulation
/// within the Sequence type implementation.
/// </remarks>
public static class SequenceInstance
{
    /// <summary>
    /// Creates a new <see cref="Sequence{T}"/> from the given enumerable collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence, which must be non-nullable.</typeparam>
    /// <param name="enumerable">The collection of elements to create the sequence from.</param>
    /// <returns>A new <see cref="Sequence{T}"/> containing the elements from the input enumerable.</returns>
    public static Sequence<T> Create<T>(IEnumerable<T> enumerable)
        where T : notnull => Sequence<T>.Create(enumerable);

    /// <summary>
    /// Creates a new <see cref="Sequence{T}"/> from the given read-only span.
    /// </summary>
    /// <typeparam name="T">The type of elements in the sequence, which must be non-nullable.</typeparam>
    /// <param name="span">The read-only span of elements to create the sequence from.</param>
    /// <returns>A new <see cref="Sequence{T}"/> containing the elements from the input span.</returns>
    public static Sequence<T> Create<T>(ReadOnlySpan<T> span)
        where T : notnull => Sequence<T>.FromSpan(span);
}

/// <summary>Provides static methods for creating and manipulating sequences.</summary>
/// <remarks>
/// This static class serves as a utility for creating sequences with
/// various input types.
/// </remarks>
public static class Sequence
{
    public static ISequence<T> Create<T>(IEnumerable<T> enumerable)
        where T : notnull => SequenceInstance.Create(enumerable);

    public static ISequence<T> Create<T>(ReadOnlySpan<T> span)
        where T : notnull => SequenceInstance.Create(span);
}

/// <summary>
/// Represents an immutable sequence of elements of type
/// <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">
/// The type of elements in the sequence, which must be
/// non-nullable.
/// </typeparam>
/// <remarks>
/// This readonly struct provides an efficient, immutable collection type with
/// collection builder support.
/// </remarks>
[CollectionBuilder(typeof(SequenceInstance), nameof(SequenceInstance.Create))]
public readonly struct Sequence<T> : ISequence<T>
    where T : notnull
{
    private T[]? Items { get; }

    /// <summary>Gets the number of elements in the sequence.</summary>
    /// <value>The total number of elements in the sequence.</value>
    public int Length => Items?.Length ?? 0;

    /// <summary>Determines whether the sequence is empty.</summary>
    /// <returns>
    /// <see langword="true" /> if the sequence contains no elements; otherwise,
    /// <see langword="false" />.
    /// </returns>
    [MemberNotNullWhen(false, "Items")]
    public bool IsEmpty => Items is null;

    private Sequence(T[]? items)
    {
        Items = items;
    }

    internal Sequence(IEnumerable<T>? enumerable)
    {
        if (enumerable?.ToArray() is not { Length: > 0 } items)
        {
            return;
        }

        Items = items;
    }

    private Sequence(ReadOnlySpan<T> span)
    {
        Items = !span.IsEmpty ? span.ToArray() : null;
    }

    /// <summary>
    /// Creates a new <see cref="Sequence{T}" /> from the given
    /// enumerable collection.
    /// </summary>
    /// <param name="enumerable">
    /// The collection of elements to create the sequence
    /// from. Can be null.
    /// </param>
    /// <returns>
    /// A new <see cref="Sequence{T}" /> containing the elements from the
    /// enumerable.
    /// </returns>
    public static Sequence<T> Create(IEnumerable<T>? enumerable) => new(enumerable);

    /// <summary>Creates a new <see cref="Sequence{T}" /> from the given span.</summary>
    /// <param name="span">The span of elements to create the sequence from.</param>
    /// <returns>
    /// A new <see cref="Sequence{T}" /> containing the elements from the
    /// span.
    /// </returns>
    public static Sequence<T> FromSpan(Span<T> span) => new(span);

    /// <summary>
    /// Creates a new <see cref="Sequence{T}" /> from the given read-only
    /// span.
    /// </summary>
    /// <param name="span">
    /// The read-only span of elements to create the sequence
    /// from.
    /// </param>
    /// <returns>
    /// A new <see cref="Sequence{T}" /> containing the elements from the
    /// read-only span.
    /// </returns>
    public static Sequence<T> FromSpan(ReadOnlySpan<T> span) => new(span);

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        if (Items is null)
        {
            return Empty();
        }

        var items = Items;
        return Unsafe.As<T[], IEnumerable<T>>(ref items).GetEnumerator();

        static IEnumerator<T> Empty()
        {
            yield break;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private ReadOnlySpan<T> EnumerableToSpan(IEnumerable<T> enumerable) =>
        Unsafe.As<IEnumerable<T>, T[]>(ref enumerable).AsSpan();

    /// <inheritdoc />
    public ReadOnlySpan<T> AsSpan() =>
        Items is not null ? Items.AsSpan() : [];

    /// <summary>
    /// Creates a new <see cref="Sequence{T}" /> with the specified item added to
    /// the end of the current sequence.
    /// </summary>
    /// <param name="item">The item to add to the sequence.</param>
    /// <returns>
    /// A new <see cref="Sequence{T}" /> containing all the original
    /// elements plus the new item.
    /// </returns>
    public Sequence<T> Add(T item)
    {
        var sequenceSpan = AsSpan();
        Span<T> destination = new T[Length + 1];
        sequenceSpan.CopyTo(destination);
        destination[^1] = item;

        return FromSpan(destination);
    }

    /// <summary>
    /// Creates a new <see cref="Sequence{T}" /> with the specified range of items
    /// added to the end of the current sequence.
    /// </summary>
    /// <param name="range">The range of items to add to the sequence.</param>
    /// <returns>
    /// A new <see cref="Sequence{T}" /> containing all the original elements plus
    /// the new range of items.
    /// </returns>
    public Sequence<T> AddRange(IEnumerable<T> range)
    {
        Span<T> items = [.. range];
        var sequenceSpan = AsSpan();
        var destinationLength = sequenceSpan.Length + items.Length;
        Span<T> destination = new T[destinationLength];
        sequenceSpan.CopyTo(destination);
        items.CopyTo(destination[sequenceSpan.Length ..]);

        return FromSpan(destination);
    }

    /// <summary>
    /// Removes the first occurrence of the specified item from the sequence using
    /// the default equality comparer.
    /// </summary>
    /// <param name="item">The item to remove from the sequence.</param>
    /// <returns>
    /// A new <see cref="Sequence{T}" /> with the first occurrence of the
    /// item removed.
    /// </returns>
    public Sequence<T> Remove(T item) => Remove(item, EqualityComparer<T>.Default);

    /// <summary>
    /// Removes the first occurrence of the specified item from the sequence using
    /// the provided equality comparer.
    /// </summary>
    /// <param name="item">The item to remove from the sequence.</param>
    /// <param name="equalityComparer">
    /// The equality comparer to use when comparing
    /// items.
    /// </param>
    /// <returns>
    /// A new <see cref="Sequence{T}" /> with the first occurrence of the
    /// item removed.
    /// </returns>
    public Sequence<T> Remove(T item, EqualityComparer<T> equalityComparer)
    {
        var sequenceSpan = AsSpan();
        Span<T> destination = new T[sequenceSpan.Length];
        var itemHash = equalityComparer.GetHashCode(item);
        var startIndex = 0;
        var endIndex = 0;
        var destinationIndex = 0;

        for (var i = 0; i < sequenceSpan.Length; i++)
        {
            var currentItem = sequenceSpan[i];

            if (
                equalityComparer.GetHashCode(currentItem) != itemHash
                || !equalityComparer.Equals(currentItem, item)
            )
            {
                continue;
            }

            endIndex = i + 1;
            var currentSlice = sequenceSpan[startIndex .. endIndex];
            currentSlice.CopyTo(destination[destinationIndex..]);
            destinationIndex += currentSlice.Length;
            startIndex = endIndex;
        }

        if (endIndex != sequenceSpan.Length)
        {
            var currentSlice = sequenceSpan[endIndex ..];
            currentSlice.CopyTo(destination[destinationIndex..]);
            destinationIndex += currentSlice.Length;
        }

        return FromSpan(destination[..destinationIndex]);
    }

    /// <summary>Creates an empty <see cref="Sequence{T}" />.</summary>
    /// <returns>An empty sequence of type <typeparamref name="T" />.</returns>
    public Sequence<T> Clear() => [];

    /// <summary>Inserts an item at the specified index in the sequence.</summary>
    /// <param name="index">
    /// The zero-based index at which the item should be
    /// inserted.
    /// </param>
    /// <param name="item">The item to insert into the sequence.</param>
    /// <returns>
    /// A new <see cref="Sequence{T}" /> with the item inserted at the
    /// specified index.
    /// </returns>
    public Sequence<T> Insert(int index, T item)
    {
        if (index is 0 && Length is 0)
        {
            return [item];
        }

        if (index >= Length)
        {
            return this;
        }

        var sequenceSpan = AsSpan();
        Span<T> destination = new T[Length + 1];
        sequenceSpan[..index].CopyTo(destination);
        destination[index] = item;
        sequenceSpan[index..].CopyTo(destination[(index + 1)..]);

        return FromSpan(destination);
    }

    /// <summary>Inserts a range of items at the specified index in the sequence.</summary>
    /// <param name="index">
    /// The zero-based index at which the range should be
    /// inserted.
    /// </param>
    /// <param name="range">The collection of items to insert into the sequence.</param>
    /// <returns>
    /// A new <see cref="Sequence{T}" /> with the range of items inserted at the
    /// specified index.
    /// </returns>
    public Sequence<T> InsertRange(int index, IEnumerable<T> range)
    {
        if (index is 0 && this is [])
        {
            return new Sequence<T>(range);
        }

        if (index >= Length)
        {
            return this;
        }

        var rangeSpan = EnumerableToSpan(range);
        var sequenceSpan = AsSpan();
        var length = Length + rangeSpan.Length;
        Span<T> destination = new T[length];
        sequenceSpan[..index].CopyTo(destination);
        rangeSpan.CopyTo(destination[index..]);
        sequenceSpan[index..].CopyTo(destination[(index + rangeSpan.Length)..]);

        return FromSpan(destination);
    }

    /// <summary>Removes the item at the specified index from the sequence.</summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    /// <returns>
    /// A new <see cref="Sequence{T}" /> with the item at the specified
    /// index removed.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the index is less than 0 or greater than or equal to the
    /// length of the sequence.
    /// </exception>
    public Sequence<T> RemoveAt(int index)
    {
        if (this is [])
        {
            return this;
        }

        if ((Length, index) is (1, 0))
        {
            return [];
        }

        var sequenceSpan = AsSpan();
        Span<T> destination = new T[Length - 1];
        sequenceSpan[..index].CopyTo(destination);
        sequenceSpan[(index + 1) ..].CopyTo(destination[(index - 1)..]);

        return FromSpan(destination);
    }

    /// <summary>
    /// Removes all items from the sequence that match the given
    /// predicate.
    /// </summary>
    /// <param name="predicate">
    /// A function that determines whether an item should
    /// be removed.
    /// </param>
    /// <returns>A new <see cref="Sequence{T}" /> with matching items removed.</returns>
    public Sequence<T> RemoveAll(Predicate<T> predicate)
    {
        var sequenceSpan = AsSpan();
        Span<T> destination = new T[Length];
        var index = 0;

        foreach (var item in sequenceSpan)
        {
            if (predicate(item))
            {
                continue;
            }

            destination[index++] = item;
        }

        return FromSpan(destination[..index]);
    }

    /// <inheritdoc />
    public Option<T> GetValueOrNone(int index)
    {
        if (IsEmpty || index >= Length)
        {
            return Option<T>.None;
        }

        return Items![index];
    }

    /// <inheritdoc />
    public Option<T> this[int index] => GetValueOrNone(index);

    public Sequence<T> this[Range range] =>
        !IsEmpty ? new Sequence<T>(Items[range]) : [];

    /// <inheritdoc />
    public Option<int> IndexOf(T option) => IndexOf(option, EqualityComparer<T>.Default);

    /// <inheritdoc />
    public Option<int> IndexOf(T option, EqualityComparer<T> equalityComparer)
    {
        if (IsEmpty)
        {
            return Option<int>.None;
        }

        var itemHashCode = equalityComparer.GetHashCode(option);

        for (var i = 0; i < Length; i++)
        {
            var value = Items[i];

            if (
                equalityComparer.GetHashCode(value) != itemHashCode
                || !equalityComparer.Equals(option, value)
            )
            {
                continue;
            }

            return i;
        }

        return Option<int>.None;
    }

    /// <inheritdoc />
    public Option<int> LastIndexOf(T option) =>
        LastIndexOf(option, EqualityComparer<T>.Default);

    /// <inheritdoc />
    public Option<int> LastIndexOf(T option, EqualityComparer<T> equalityComparer)
    {
        var lastIndex = Option<int>.None;

        if (IsEmpty)
        {
            return lastIndex;
        }

        var itemHash = equalityComparer.GetHashCode(option);

        for (var i = 0; i < Length; i++)
        {
            var value = Items[i];

            if (
                equalityComparer.GetHashCode(value) != itemHash
                || !equalityComparer.Equals(value, option))
            {
                continue;
            }

            lastIndex = i;
        }

        return lastIndex;
    }

    /// <summary>
    /// Finds all indices of the specified item in the sequence using the default
    /// equality comparer.
    /// </summary>
    /// <param name="item">The item to search for in the sequence.</param>
    /// <returns>
    /// A <see cref="Sequence{int}" /> containing the indices of all
    /// occurrences of the item.
    /// </returns>
    public Sequence<int> AllIndicesOf(T item) =>
        AllIndicesOf(item, EqualityComparer<T>.Default);

    /// <summary>
    /// Finds all indices of the specified item in the sequence using the provided
    /// equality comparer.
    /// </summary>
    /// <param name="item">The item to search for in the sequence.</param>
    /// <param name="equalityComparer">
    /// The equality comparer to use for comparing
    /// items.
    /// </param>
    /// <returns>
    /// A <see cref="Sequence{int}" /> containing the indices of all
    /// occurrences of the item.
    /// </returns>
    public Sequence<int> AllIndicesOf(T item, EqualityComparer<T> equalityComparer)
    {
        return IsEmpty
            ? []
            : new Sequence<int>(YieldAllIndices(item, Items, equalityComparer));

        static IEnumerable<int> YieldAllIndices(
            T item,
            T[] itemsArray,
            EqualityComparer<T> equalityComparer
        )
        {
            var itemHash = equalityComparer.GetHashCode(item);

            for (var i = 0; i < itemsArray.Length; i++)
            {
                var value = itemsArray[i];

                if (
                    equalityComparer.GetHashCode(value) != itemHash
                    || !equalityComparer.Equals(value, item)
                )
                {
                    continue;
                }

                yield return i;
            }
        }
    }

    /// <inheritdoc />
    public Sequence<T> AsSequence() => this;

    ISequence<T> ISequence<T>.Add(T item) => Add(item);

    ISequence<T> ISequence<T>.AddRange(IEnumerable<T> range) => AddRange(range);

    ISequence<T> ISequence<T>.Remove(T item) => Remove(item);

    ISequence<T> ISequence<T>.Remove(T item, EqualityComparer<T> equalityComparer) =>
        Remove(item);

    ISequence<T> ISequence<T>.Clear() => Clear();

    ISequence<T> ISequence<T>.Insert(int index, T item) => Insert(index, item);

    ISequence<T> ISequence<T>.InsertRange(int index, IEnumerable<T> range) =>
        InsertRange(index, range);

    ISequence<T> ISequence<T>.RemoveAt(int index) => RemoveAt(index);

    ISequence<T> ISequence<T>.RemoveAll(Predicate<T> predicate) => RemoveAll(predicate);

    ISequence<T> ISequence<T>.this[Range range] => this[range];

    ISequence<int> ISequence<T>.AllIndicesOf(T item, EqualityComparer<T> equalityComparer)
        => AllIndicesOf(item, equalityComparer);

    ISequence<int> ISequence<T>.AllIndicesOf(T item) => AllIndicesOf(item);
}