namespace SharperExtensions.Collections.Tests;

public class SequenceTests
{
    #region Creation Tests

    [Fact]
    public void Create_WithNullEnumerable_ShouldCreateEmptySequence()
    {
        var sequence = Sequence<int>.Create(null);

        sequence.IsEmpty.Should().BeTrue();
        sequence.Length.Should().Be(0);
    }

    [Fact]
    public void Create_WithEmptyEnumerable_ShouldCreateEmptySequence()
    {
        var sequence = Sequence<string>.Create([]);

        sequence.IsEmpty.Should().BeTrue();
        sequence.Length.Should().Be(0);
    }

    [Fact]
    public void Create_WithEnumerable_ShouldCreateSequenceWithElements()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        var sequence = Sequence<int>.Create(items);

        sequence.IsEmpty.Should().BeFalse();
        sequence.Length.Should().Be(5);
        sequence.Should().BeEquivalentTo(items);
    }

    [Fact]
    public void Create_WithDummyValues_ShouldCreateSequenceWithElements()
    {
        var dummies = new[]
        {
            new DummyValue { Name = "John", Email = "john@example.com" },
            new DummyValue { Name = "Jane", Email = "jane@example.com" }
        };
        var sequence = Sequence<DummyValue>.Create(dummies);

        sequence.IsEmpty.Should().BeFalse();
        sequence.Length.Should().Be(2);
        sequence.Should().BeEquivalentTo(dummies);
    }

    [Fact]
    public void FromSpan_WithEmptySpan_ShouldCreateEmptySequence()
    {
        var sequence = Sequence<double>.FromSpan(ReadOnlySpan<double>.Empty);

        sequence.IsEmpty.Should().BeTrue();
        sequence.Length.Should().Be(0);
    }

    [Fact]
    public void FromSpan_WithSpan_ShouldCreateSequenceWithElements()
    {
        var items = new[] { 1.1, 2.2, 3.3 };
        var sequence = Sequence<double>.FromSpan(items.AsSpan());

        sequence.IsEmpty.Should().BeFalse();
        sequence.Length.Should().Be(3);
        sequence.Should().BeEquivalentTo(items);
    }

    [Fact]
    public void StaticCreate_WithEnumerable_ShouldCreateSequence()
    {
        var items = new[] { "hello", "world" };
        var sequence = Sequence.Create(items);

        sequence.Length.Should().Be(2);
        sequence.Should().BeEquivalentTo(items);
    }

    [Fact]
    public void StaticCreate_WithSpan_ShouldCreateSequence()
    {
        var items = new[] { 10, 20, 30 };
        var sequence = Sequence.Create(items.AsSpan());

        sequence.Length.Should().Be(3);
        sequence.Should().BeEquivalentTo(items);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_ToEmptySequence_ShouldCreateSequenceWithOneElement()
    {
        var sequence = Sequence<int>.Create(null);
        var result = sequence.Add(42);

        result.Length.Should().Be(1);
        result[0].Value.Should().Be(42);
    }

    [Fact]
    public void Add_ToExistingSequence_ShouldAppendElement()
    {
        var sequence = Sequence<string>.Create(new[] { "hello" });
        var result = sequence.Add("world");

        result.Length.Should().Be(2);
        result[0].Value.Should().Be("hello");
        result[1].Value.Should().Be("world");
    }

    [Fact]
    public void Add_DummyValue_ShouldAppendElement()
    {
        var dummy1 = new DummyValue { Name = "John", Email = "john@example.com" };
        var dummy2 = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var sequence = Sequence<DummyValue>.Create(new[] { dummy1 });

        var result = sequence.Add(dummy2);

        result.Length.Should().Be(2);
        result[0].Value.Should().BeEquivalentTo(dummy1);
        result[1].Value.Should().BeEquivalentTo(dummy2);
    }

    #endregion

    #region AddRange Tests

    [Fact]
    public void AddRange_ToEmptySequence_ShouldCreateSequenceWithAllElements()
    {
        var sequence = Sequence<int>.Create(null);
        var range = new[] { 1, 2, 3 };
        var result = sequence.AddRange(range);

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo(range);
    }

    [Fact]
    public void AddRange_ToExistingSequence_ShouldAppendAllElements()
    {
        var sequence = Sequence<string>.Create(new[] { "hello" });
        var range = new[] { "world", "!" };
        var result = sequence.AddRange(range);

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo(new[] { "hello", "world", "!" });
    }

    #endregion

    #region Remove Tests

    [Fact]
    public void Remove_ExistingElement_ShouldRemoveFirstOccurrence()
    {
        var sequence = Sequence<int>.Create([1, 2, 3, 2, 4]);
        var result = sequence.Remove(2);

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo([1, 3, 4]);
    }

    [Fact]
    public void Remove_NonExistingElement_ShouldReturnUnchangedSequence()
    {
        var sequence = Sequence<int>.Create([1, 2, 3]);
        var result = sequence.Remove(5);

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo([1, 2, 3]);
    }

    [Fact]
    public void Remove_DummyValue_ShouldRemoveMatchingElement()
    {
        var dummy1 = new DummyValue { Name = "John", Email = "john@example.com" };
        var dummy2 = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var dummy3 = new DummyValue { Name = "Sam", Email = "sam@example.com" };
        var dummy4 = new DummyValue { Name = "Jack", Email = "jack@example.com" };
        var dummy5 = new DummyValue { Name = "Sarah", Email = "sara@example.com" };
        var dummy6 = new DummyValue { Name = "Dan", Email = "dan@example.com" };
        var sequence =
            Sequence<DummyValue>.Create([dummy1, dummy2, dummy3, dummy4, dummy5, dummy6]);

        var result = sequence.Remove(dummy4);

        result.Length.Should().Be(5);
        result.Should().BeEquivalentTo([dummy1, dummy2, dummy3, dummy5, dummy6]);
    }

    [Fact]
    public void Remove_WithCustomEqualityComparer_ShouldUseComparer()
    {
        var sequence = Sequence<string>.Create(["hello", "WORLD", "hello"]);
        var result = sequence.Remove(
            "hello",
            EqualityComparer<string>.Create(
                (x, y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase),
                x => x?.ToUpperInvariant()?.GetHashCode() ?? 0
            )
        );

        result.Length.Should().Be(1);
        result.Should().BeEquivalentTo("WORLD");
    }

    #endregion

    #region Clear Tests

    [Fact]
    public void Clear_ShouldReturnEmptySequence()
    {
        var sequence = Sequence<int>.Create([1, 2, 3]);
        var result = sequence.Clear();

        result.IsEmpty.Should().BeTrue();
        result.Length.Should().Be(0);
    }

    #endregion

    #region Insert Tests

    [Fact]
    public void Insert_AtBeginning_ShouldInsertElement()
    {
        var sequence = Sequence<int>.Create(new[] { 2, 3, 4 });
        var result = sequence.Insert(0, 1);

        result.Length.Should().Be(4);
        result.Should().BeEquivalentTo(new[] { 1, 2, 3, 4 });
    }

    [Fact]
    public void Insert_InMiddle_ShouldInsertElement()
    {
        var sequence = Sequence<string>.Create(new[] { "a", "c", "d" });
        var result = sequence.Insert(1, "b");

        result.Length.Should().Be(4);
        result.Should().BeEquivalentTo(new[] { "a", "b", "c", "d" });
    }

    [Fact]
    public void Insert_InEmptySequence_ShouldCreateSequenceWithElement()
    {
        var sequence = Sequence<double>.Create(null);
        var result = sequence.Insert(0, 3.14);

        result.Length.Should().Be(1);
        result[0].Value.Should().Be(3.14);
    }

    [Fact]
    public void Insert_IndexOutOfRange_ShouldReturnUnchangedSequence()
    {
        var sequence = Sequence<int>.Create(new[] { 1, 2, 3 });
        var result = sequence.Insert(10, 99);

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    #endregion

    #region InsertRange Tests

    [Fact]
    public void InsertRange_AtBeginning_ShouldInsertAllElements()
    {
        var sequence = Sequence<int>.Create(new[] { 4, 5 });
        var range = new[] { 1, 2, 3 };
        var result = sequence.InsertRange(0, range);

        result.Length.Should().Be(5);
        result.Should().BeEquivalentTo(new[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void InsertRange_InEmptySequence_ShouldCreateSequenceWithRange()
    {
        var sequence = Sequence<string>.Create(null);
        var range = new[] { "hello", "world" };
        var result = sequence.InsertRange(0, range);

        result.Length.Should().Be(2);
        result.Should().BeEquivalentTo(range);
    }

    #endregion

    #region RemoveAt Tests

    [Fact]
    public void RemoveAt_ValidIndex_ShouldRemoveElement()
    {
        var sequence = Sequence<int>.Create(new[] { 1, 2, 3, 4 });
        var result = sequence.RemoveAt(1);

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo(new[] { 1, 3, 4 });
    }

    [Fact]
    public void RemoveAt_SingleElementSequence_ShouldReturnEmptySequence()
    {
        var sequence = Sequence<string>.Create(new[] { "only" });
        var result = sequence.RemoveAt(0);

        result.IsEmpty.Should().BeTrue();
        result.Length.Should().Be(0);
    }

    [Fact]
    public void RemoveAt_EmptySequence_ShouldReturnEmptySequence()
    {
        var sequence = Sequence<int>.Create(null);
        var result = sequence.RemoveAt(0);

        result.IsEmpty.Should().BeTrue();
    }

    #endregion

    #region RemoveAll Tests

    [Fact]
    public void RemoveAll_WithPredicate_ShouldRemoveMatchingElements()
    {
        var sequence = Sequence<int>.Create(new[] { 1, 2, 3, 4, 5, 6 });
        var result = sequence.RemoveAll(x => x % 2 == 0);

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo(new[] { 1, 3, 5 });
    }

    [Fact]
    public void RemoveAll_NoMatches_ShouldReturnUnchangedSequence()
    {
        var sequence = Sequence<string>.Create(new[] { "hello", "world" });
        var result = sequence.RemoveAll(x => x.Length > 10);

        result.Length.Should().Be(2);
        result.Should().BeEquivalentTo("hello", "world");
    }

    [Fact]
    public void RemoveAll_DummyValues_ShouldRemoveMatchingElements()
    {
        var dummies = new[]
        {
            new DummyValue { Name = "John", Email = "john@example.com" },
            new DummyValue { Name = "Jane", Email = "jane@example.com" },
            new DummyValue { Name = "Bob", Email = "bob@test.com" }
        };
        var sequence = Sequence<DummyValue>.Create(dummies);

        var result = sequence.RemoveAll(x => x.Email.Contains("example"));

        result.Length.Should().Be(1);
        result[0].Value?.Name.Should().Be("Bob");
    }

    #endregion

    #region Indexer and GetValueOrNone Tests

    [Fact]
    public void Indexer_ValidIndex_ShouldReturnSomeWithValue()
    {
        var sequence = Sequence<int>.Create(new[] { 10, 20, 30 });
        var result = sequence[1];

        result.IsSome.Should().BeTrue();
        result.Value.Should().Be(20);
    }

    [Fact]
    public void Indexer_InvalidIndex_ShouldReturnNone()
    {
        var sequence = Sequence<int>.Create(new[] { 10, 20, 30 });
        var result = sequence[5];

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void Indexer_EmptySequence_ShouldReturnNone()
    {
        var sequence = Sequence<string>.Create(null);
        var result = sequence[0];

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void RangeIndexer_ValidRange_ShouldReturnSubsequence()
    {
        var sequence = Sequence<int>.Create([1, 2, 3, 4, 5]);
        var result = sequence[1..4];

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo([2, 3, 4]);
    }

    [Fact]
    public void RangeIndexer_EmptySequence_ShouldReturnEmptySequence()
    {
        var sequence = Sequence<int>.Create(null);
        var result = sequence[..2];

        result.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void GetValueOrNone_ValidIndex_ShouldReturnSomeWithValue()
    {
        var dummy = new DummyValue { Name = "Test", Email = "test@example.com" };
        var sequence = Sequence<DummyValue>.Create(new[] { dummy });
        var result = sequence.GetValueOrNone(0);

        result.IsSome.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dummy);
    }

    #endregion

    #region IndexOf Tests

    [Fact]
    public void IndexOf_ExistingElement_ShouldReturnSomeWithIndex()
    {
        var sequence = Sequence<int>.Create(new[] { 10, 20, 30, 20 });
        var result = sequence.IndexOf(20);

        result.IsSome.Should().BeTrue();
        result.Value.Should().Be(1);
    }

    [Fact]
    public void IndexOf_NonExistingElement_ShouldReturnNone()
    {
        var sequence = Sequence<string>.Create(new[] { "hello", "world" });
        var result = sequence.IndexOf("test");

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void IndexOf_EmptySequence_ShouldReturnNone()
    {
        var sequence = Sequence<int>.Create(null);
        var result = sequence.IndexOf(42);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void IndexOf_WithCustomEqualityComparer_ShouldUseComparer()
    {
        var sequence = Sequence<string>.Create(new[] { "Hello", "WORLD", "test" });
        var comparer = EqualityComparer<string>.Create(
            (x, y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase),
            x => x?.ToUpperInvariant()?.GetHashCode() ?? 0
        );

        var result = sequence.IndexOf("world", comparer);

        result.IsSome.Should().BeTrue();
        result.Value.Should().Be(1);
    }

    [Fact]
    public void IndexOf_DummyValue_ShouldFindMatchingElement()
    {
        var dummy1 = new DummyValue { Name = "John", Email = "john@example.com" };
        var dummy2 = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var sequence = Sequence<DummyValue>.Create(new[] { dummy1, dummy2 });

        var result = sequence.IndexOf(dummy2);

        result.IsSome.Should().BeTrue();
        result.Value.Should().Be(1);
    }

    #endregion

    #region LastIndexOf Tests

    [Fact]
    public void LastIndexOf_ExistingElement_ShouldReturnSomeWithLastIndex()
    {
        var sequence = Sequence<int>.Create(new[] { 10, 20, 30, 20, 40 });
        var result = sequence.LastIndexOf(20);

        result.IsSome.Should().BeTrue();
        result.Value.Should().Be(3);
    }

    [Fact]
    public void LastIndexOf_NonExistingElement_ShouldReturnNone()
    {
        var sequence = Sequence<double>.Create(new[] { 1.1, 2.2, 3.3 });
        var result = sequence.LastIndexOf(4.4);

        result.IsNone.Should().BeTrue();
    }

    [Fact]
    public void LastIndexOf_SingleOccurrence_ShouldReturnThatIndex()
    {
        var sequence = Sequence<string>.Create(new[] { "unique", "test", "value" });
        var result = sequence.LastIndexOf("test");

        result.IsSome.Should().BeTrue();
        result.Value.Should().Be(1);
    }

    [Fact]
    public void LastIndexOf_WithCustomEqualityComparer_ShouldUseComparer()
    {
        var sequence =
            Sequence<string>.Create(new[] { "Hello", "WORLD", "hello", "TEST" });
        var comparer = EqualityComparer<string>.Create(
            (x, y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase),
            x => x?.ToUpperInvariant()?.GetHashCode() ?? 0
        );

        var result = sequence.LastIndexOf("HELLO", comparer);

        result.IsSome.Should().BeTrue();
        result.Value.Should().Be(2);
    }

    #endregion

    #region AllIndicesOf Tests

    [Fact]
    public void AllIndicesOf_MultipleOccurrences_ShouldReturnAllIndices()
    {
        var sequence = Sequence<int>.Create(new[] { 1, 2, 3, 2, 4, 2, 5 });
        var result = sequence.AllIndicesOf(2);

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo(new[] { 1, 3, 5 });
    }

    [Fact]
    public void AllIndicesOf_NoOccurrences_ShouldReturnEmptySequence()
    {
        var sequence = Sequence<string>.Create(new[] { "hello", "world", "test" });
        var result = sequence.AllIndicesOf("missing");

        result.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void AllIndicesOf_EmptySequence_ShouldReturnEmptySequence()
    {
        var sequence = Sequence<int>.Create(null);
        var result = sequence.AllIndicesOf(42);

        result.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void AllIndicesOf_WithCustomEqualityComparer_ShouldUseComparer()
    {
        var sequence =
            Sequence<string>.Create(["hello", "WORLD", "hello", "TEST", "hello"]);
        var comparer = EqualityComparer<string>.Create(
            (x, y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase),
            x => x?.ToUpperInvariant()?.GetHashCode() ?? 0
        );

        var result = sequence.AllIndicesOf("hello", comparer);

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo([0, 2, 4]);
    }

    [Fact]
    public void AllIndicesOf_DummyValues_ShouldFindAllMatches()
    {
        var dummy1 = new DummyValue { Name = "John", Email = "john@example.com" };
        var dummy2 = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var sequence =
            Sequence<DummyValue>.Create(new[] { dummy1, dummy2, dummy1, dummy2, dummy1 });

        var result = sequence.AllIndicesOf(dummy1);

        result.Length.Should().Be(3);
        result.Should().BeEquivalentTo([0, 2, 4]);
    }

    #endregion

    #region AsSpan Tests

    [Fact]
    public void AsSpan_NonEmptySequence_ShouldReturnSpanWithElements()
    {
        var items = new[] { 1, 2, 3, 4, 5 };
        var sequence = Sequence<int>.Create(items);
        var span = sequence.AsSpan();

        span.Length.Should().Be(5);
        span.ToArray().Should().BeEquivalentTo(items);
    }

    [Fact]
    public void AsSpan_EmptySequence_ShouldReturnEmptySpan()
    {
        var sequence = Sequence<string>.Create(null);
        var span = sequence.AsSpan();

        span.IsEmpty.Should().BeTrue();
        span.Length.Should().Be(0);
    }

    #endregion

    #region Enumeration Tests

    [Fact]
    public void GetEnumerator_NonEmptySequence_ShouldEnumerateAllElements()
    {
        var items = new[] { "a", "b", "c" };
        var sequence = Sequence<string>.Create(items);
        var enumerated = new List<string>();

        foreach (var item in sequence)
        {
            enumerated.Add(item);
        }

        enumerated.Should().BeEquivalentTo(items);
    }

    [Fact]
    public void GetEnumerator_EmptySequence_ShouldNotEnumerateAnyElements()
    {
        var sequence = Sequence<int>.Create(null);
        var enumerated = new List<int>();

        foreach (var item in sequence)
        {
            enumerated.Add(item);
        }

        enumerated.Should().BeEmpty();
    }

    [Fact]
    public void GetEnumerator_DummyValues_ShouldEnumerateAllElements()
    {
        var dummies = new[]
        {
            new DummyValue { Name = "John", Email = "john@example.com" },
            new DummyValue { Name = "Jane", Email = "jane@example.com" }
        };
        var sequence = Sequence<DummyValue>.Create(dummies);
        var enumerated = sequence.ToList();

        enumerated.Should().BeEquivalentTo(dummies);
    }

    #endregion

    #region AsSequence Tests

    [Fact]
    public void AsSequence_ShouldReturnSelf()
    {
        var sequence = Sequence<int>.Create([1, 2, 3]);
        var result = sequence.AsSequence();

        result.Should().BeEquivalentTo(sequence);
        result.Length.Should().Be(sequence.Length);
    }

    #endregion

    #region Collection Builder Tests

    [Fact]
    public void CollectionBuilder_ShouldCreateSequenceFromCollectionExpression()
    {
        // This tests the CollectionBuilder attribute functionality
        Sequence<int> sequence = [1, 2, 3, 4, 5];

        sequence.Length.Should().Be(5);
        sequence.Should().BeEquivalentTo([1, 2, 3, 4, 5]);
    }

    [Fact]
    public void CollectionBuilder_EmptyCollection_ShouldCreateEmptySequence()
    {
        Sequence<string> sequence = [];

        sequence.IsEmpty.Should().BeTrue();
        sequence.Length.Should().Be(0);
    }

    [Fact]
    public void CollectionBuilder_WithDummyValues_ShouldCreateSequence()
    {
        var dummy1 = new DummyValue { Name = "John", Email = "john@example.com" };
        var dummy2 = new DummyValue { Name = "Jane", Email = "jane@example.com" };

        Sequence<DummyValue> sequence = [dummy1, dummy2];

        sequence.Length.Should().Be(2);
        sequence[0].Value.Should().BeEquivalentTo(dummy1);
        sequence[1].Value.Should().BeEquivalentTo(dummy2);
    }

    #endregion

    #region Edge Cases and Error Handling

    [Fact]
    public void Length_EmptySequence_ShouldReturnZero()
    {
        var sequence = Sequence<int>.Create(null);
        sequence.Length.Should().Be(0);
    }

    [Fact]
    public void IsEmpty_EmptySequence_ShouldReturnTrue()
    {
        var sequence = Sequence<double>.Create(Array.Empty<double>());
        sequence.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_NonEmptySequence_ShouldReturnFalse()
    {
        var sequence = Sequence<string>.Create(new[] { "test" });
        sequence.IsEmpty.Should().BeFalse();
    }

    [Fact]
    public void ChainedOperations_ShouldWorkCorrectly()
    {
        var sequence = Sequence<int>
            .Create([1, 2, 3])
            .Add(4)
            .AddRange([5, 6])
            .Remove(2)
            .Insert(0, 8);

        sequence.Length.Should().Be(6);
        sequence.Should().BeEquivalentTo([8, 1, 3, 4, 5, 6]);
    }

    [Fact]
    public void ChainedOperations_WithDummyValues_ShouldWorkCorrectly()
    {
        var dummy1 = new DummyValue { Name = "John", Email = "john@example.com" };
        var dummy2 = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var dummy3 = new DummyValue { Name = "Bob", Email = "bob@example.com" };
        var dummy4 = new DummyValue { Name = "Frank", Email = "Frank@example.com" };
        var dummy5 = new DummyValue { Name = "Phil", Email = "Phil@example.com" };
        var dummy6 = new DummyValue { Name = "Tucker", Email = "Tucker@example.com" };

        var sequence = Sequence<DummyValue>
            .Create([dummy1, dummy2, dummy3, dummy4])
            .Add(dummy5)
            .Insert(3, dummy6)
            .RemoveAt(1);

        sequence.Length.Should().Be(5);
        sequence.Should().BeEquivalentTo([dummy1, dummy3, dummy4, dummy6, dummy5]);
    }

    #endregion

    #region Interface Implementation Tests

    [Fact]
    public void ISequence_Add_ShouldReturnISequence()
    {
        ISequence<int> sequence = Sequence<int>.Create(new[] { 1, 2, 3 });
        var result = sequence.Add(4);

        result.Should().BeAssignableTo<ISequence<int>>();
        result.Length.Should().Be(4);
    }

    [Fact]
    public void ISequence_Remove_ShouldReturnISequence()
    {
        ISequence<string> sequence = Sequence<string>.Create(new[] { "hello", "world" });
        var result = sequence.Remove("hello");

        result.Should().BeAssignableTo<ISequence<string>>();
        result.Length.Should().Be(1);
    }

    [Fact]
    public void ISequence_Clear_ShouldReturnEmptyISequence()
    {
        ISequence<double> sequence = Sequence<double>.Create(new[] { 1.1, 2.2, 3.3 });
        var result = sequence.Clear();

        result.Should().BeAssignableTo<ISequence<double>>();
        result.Length.Should().Be(0);
    }

    #endregion
}