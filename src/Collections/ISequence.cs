using System.Runtime.CompilerServices;

namespace SharperExtensions.Collections;

/// <summary>
/// Represents an immutable sequence of non-null elements with
/// various manipulation methods.
/// </summary>
/// <typeparam name="T">
/// The type of elements in the sequence, which must be
/// non-null.
/// </typeparam>
/// <remarks>
/// Provides a functional, immutable collection with operations that return
/// new sequences instead of modifying the original.
/// </remarks>
[CollectionBuilder(typeof(Sequence), nameof(Sequence.Create))]
public interface ISequence<T> : IEnumerable<T> where T : notnull
{
    /// <summary>Gets the number of elements in the sequence.</summary>
    /// <value>
    /// An integer representing the total count of elements in the
    /// sequence.
    /// </value>
    int Length { get; }

    /// <summary>
    /// Gets a value indicating whether the sequence contains no
    /// elements.
    /// </summary>
    /// <value><c>true</c> if the sequence is empty; otherwise, <c>false</c>.</value>
    bool IsEmpty { get; }

    /// <summary>
    /// Provides a read-only span view of the sequence's underlying
    /// elements.
    /// </summary>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}" /> representing the sequence's
    /// elements.
    /// </returns>
    ReadOnlySpan<T> AsSpan();

    /// <summary>
    /// Creates a new sequence by adding the specified item to the
    /// current sequence.
    /// </summary>
    /// <param name="item">The item to add to the sequence.</param>
    /// <returns>
    /// A new sequence containing the original elements followed by the
    /// specified item.
    /// </returns>
    ISequence<T> Add(T item);

    /// <summary>
    /// Creates a new sequence by adding all elements from the specified range to
    /// the current sequence.
    /// </summary>
    /// <param name="range">The collection of elements to add to the sequence.</param>
    /// <returns>
    /// A new sequence containing the original elements followed by the elements
    /// from the specified range.
    /// </returns>
    ISequence<T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Creates a new sequence by removing the first occurrence of the specified
    /// item from the current sequence.
    /// </summary>
    /// <param name="item">The item to remove from the sequence.</param>
    /// <returns>
    /// A new sequence with the first occurrence of the specified item
    /// removed.
    /// </returns>
    ISequence<T> Remove(T item);

    /// <summary>
    /// Creates a new sequence by removing the first occurrence of the specified
    /// item from the current sequence using a custom equality comparer.
    /// </summary>
    /// <param name="item">The item to remove from the sequence.</param>
    /// <param name="equalityComparer">
    /// The equality comparer to use when
    /// determining item equality.
    /// </param>
    /// <returns>
    /// A new sequence with the first occurrence of the specified item removed,
    /// based on the provided equality comparer.
    /// </returns>
    ISequence<T> Remove(T item, EqualityComparer<T> equalityComparer);

    /// <summary>
    /// Creates a new empty sequence, removing all elements from the
    /// current sequence.
    /// </summary>
    /// <returns>A new sequence with no elements.</returns>
    ISequence<T> Clear();

    /// <summary>
    /// Creates a new sequence by inserting the specified item at the given index
    /// in the current sequence.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which to insert the item.
    /// </param>
    /// <param name="item">
    /// The item to insert into the sequence.
    /// </param>
    /// <returns>
    /// A new sequence containing the original elements with the specified item
    /// inserted at the given index.
    /// </returns>
    ISequence<T> Insert(int index, T item);

    /// <summary>
    /// Creates a new sequence by inserting a range of elements at the specified
    /// index in the current sequence.
    /// </summary>
    /// <param name="index">
    /// The zero-based index at which to insert the range of
    /// elements.
    /// </param>
    /// <param name="range">
    /// The collection of elements to insert into the
    /// sequence.
    /// </param>
    /// <returns>
    /// A new sequence containing the original elements with the specified range
    /// inserted at the given index.
    /// </returns>
    ISequence<T> InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Creates a new sequence by removing the element at the specified index from
    /// the current sequence.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <returns>A new sequence with the element at the specified index removed.</returns>
    ISequence<T> RemoveAt(int index);

    /// <summary>
    /// Creates a new sequence by removing all elements that match the specified
    /// predicate from the current sequence.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>
    /// A new sequence with all elements that do not satisfy the
    /// predicate.
    /// </returns>
    ISequence<T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Retrieves the value at the specified index or returns None if the
    /// index is out of range.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <returns>
    /// An Option containing the value at the specified index, or None if
    /// the index is invalid.
    /// </returns>
    Option<T> GetValueOrNone(int index);

    /// <summary>
    /// Retrieves the value at the specified index or returns None if the
    /// index is out of range.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <returns>
    /// An Option containing the value at the specified index, or None if
    /// the index is invalid.
    /// </returns>
    Option<T> this[int index] { get; }

    ISequence<T> this[Range range] { get; }

    /// <summary>
    /// Finds the index of the first occurrence of the specified item in
    /// the sequence.
    /// </summary>
    /// <param name="option">The item to locate in the sequence.</param>
    /// <returns>
    /// An Option containing the zero-based index of the first occurrence of the
    /// item, or None if the item is not found.
    /// </returns>
    Option<int> IndexOf(T option);

    /// <summary>
    /// Finds the index of the first occurrence of the specified item in the
    /// sequence using a custom equality comparer.
    /// </summary>
    /// <param name="option">The item to locate in the sequence.</param>
    /// <returns>
    /// An Option containing the zero-based index of the first occurrence of the
    /// item, or None if the item is not found.
    /// </returns>
    Option<int> IndexOf(
        T option,
        EqualityComparer<T> equalityComparer
    );

    /// <summary>
    /// Finds the index of the last occurrence of the specified item in
    /// the sequence.
    /// </summary>
    /// <param name="option">The item to locate in the sequence.</param>
    /// <returns>
    /// An Option containing the zero-based index of the last occurrence of the
    /// item, or None if the item is not found.
    /// </returns>
    Option<int> LastIndexOf(T option);

    /// <summary>
    /// Finds the index of the last occurrence of the specified item in the
    /// sequence using a custom equality comparer.
    /// </summary>
    /// <param name="option">The item to locate in the sequence.</param>
    /// <param name="equalityComparer">
    /// The equality comparer to use when searching
    /// for the item.
    /// </param>
    /// <returns>
    /// An Option containing the zero-based index of the last occurrence of the
    /// item, or None if the item is not found.
    /// </returns>
    Option<int> LastIndexOf(
        T option,
        EqualityComparer<T> equalityComparer
    );

    /// <summary>Finds all indices of the specified item in the sequence.</summary>
    /// <param name="item">The item to locate in the sequence.</param>
    /// <returns>
    /// A sequence of zero-based indices where the item occurs in the
    /// sequence.
    /// </returns>
    ISequence<int> AllIndicesOf(T item);

    /// <summary>
    /// Finds all indices of the specified item in the sequence using a custom
    /// equality comparer.
    /// </summary>
    /// <param name="item">The item to locate in the sequence.</param>
    /// <param name="equalityComparer">
    /// The equality comparer to use when searching
    /// for the item.
    /// </param>
    /// <returns>
    /// A sequence of zero-based indices where the item occurs in the
    /// sequence.
    /// </returns>
    ISequence<int> AllIndicesOf(
        T item,
        EqualityComparer<T> equalityComparer
    );

    /// <summary>
    /// Converts the current instance to an <see cref="Sequence{T}"/>.
    /// </summary>
    /// <returns>A new <see cref="Sequence{T}"/> representing the current sequence.</returns>
    Sequence<T> AsSequence();
}