namespace SharperExtensions.Collections.Tests;

public class OptionSequenceTests
{
    #region Test Data

    private static readonly DummyValue DummyValue1 = new()
        { Name = "John", Email = "john@example.com" };

    private static readonly DummyValue DummyValue2 = new()
        { Name = "Jane", Email = "jane@example.com" };

    private static readonly DummyValue DummyValue3 = new()
        { Name = "Bob", Email = "bob@example.com" };

    #endregion

    #region Creation Tests

    [Fact]
    public void Create_FromNullableEnumerable_WithInts_ShouldCreateCorrectSequence()
    {
        // Arrange
        IEnumerable<string?> nullableInts = ["one", null, "two", null, "three"];

        // Act
        var sequence = OptionSequence.Create(nullableInts);

        // Assert
        sequence.Length.Should().Be(5);
        sequence[0].Should().Be(Option<string>.Some("one"));
        sequence[1].Should().Be(Option<string>.None);
        sequence[2].Should().Be(Option<string>.Some("two"));
        sequence[3].Should().Be(Option<string>.None);
        sequence[4].Should().Be(Option<string>.Some("three"));
    }

    [Fact]
    public void Create_FromNullableEnumerable_WithStrings_ShouldCreateCorrectSequence()
    {
        // Arrange
        IEnumerable<string?> nullableStrings = ["hello", null, "world", null];

        // Act
        var sequence = OptionSequence.Create(nullableStrings);

        // Assert
        sequence.Length.Should().Be(4);
        sequence[0].Should().Be(Option<string>.Some("hello"));
        sequence[1].Should().Be(Option<string>.None);
        sequence[2].Should().Be(Option<string>.Some("world"));
        sequence[3].Should().Be(Option<string>.None);
    }

    [Fact]
    public void
        Create_FromNullableEnumerable_WithDummyValues_ShouldCreateCorrectSequence()
    {
        // Arrange
        IEnumerable<DummyValue?> nullableValues = [DummyValue1, null, DummyValue2];

        // Act
        var sequence = OptionSequence.Create(nullableValues);

        // Assert
        sequence.Length.Should().Be(3);
        sequence[0].Should().Be(Option<DummyValue>.Some(DummyValue1));
        sequence[1].Should().Be(Option<DummyValue>.None);
        sequence[2].Should().Be(Option<DummyValue>.Some(DummyValue2));
    }

    [Fact]
    public void Create_FromOptionSpan_ShouldCreateCorrectSequence()
    {
        // Arrange
        ReadOnlySpan<Option<int>> optionSpan =
        [
            Option<int>.Some(1),
            Option<int>.None,
            Option<int>.Some(3)
        ];

        // Act
        var sequence = OptionSequence.Create(optionSpan);

        // Assert
        sequence.Length.Should().Be(3);
        sequence[0].Should().Be(Option<int>.Some(1));
        sequence[1].Should().Be(Option<int>.None);
        sequence[2].Should().Be(Option<int>.Some(3));
    }

    #endregion

    #region Basic Properties Tests

    [Fact]
    public void Length_EmptySequence_ShouldReturnZero()
    {
        // Arrange
        var sequence = OptionSequence.Create((string?[]) []);

        // Act & Assert
        sequence.Length.Should().Be(0);
    }

    [Fact]
    public void Length_NonEmptySequence_ShouldReturnCorrectCount()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", null, "second"]);

        // Act & Assert
        sequence.Length.Should().Be(3);
    }

    [Fact]
    public void IsEmpty_EmptySequence_ShouldReturnTrue()
    {
        // Arrange
        var sequence = OptionSequence.Create(Array.Empty<string?>());

        // Act & Assert
        sequence.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_NonEmptySequence_ShouldReturnFalse()
    {
        // Arrange
        var sequence = OptionSequence.Create(new string?[] { "test" });

        // Act & Assert
        sequence.IsEmpty.Should().BeFalse();
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_OptionValue_ShouldAddToSequence()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2]);

        // Act
        var result = sequence.Add(Option<int>.Some(3));

        // Assert
        result.Length.Should().Be(3);
        result[2].Should().Be(Option<int>.Some(3));
    }

    [Fact]
    public void Add_Value_ShouldWrapInSomeAndAdd()
    {
        // Arrange
        var sequence = OptionSequence.Create(["hello"]);

        // Act
        var result = sequence.Add("world");

        // Assert
        result.Length.Should().Be(2);
        result[1].Should().Be(Option<string>.Some("world"));
    }

    [Fact]
    public void Add_DummyValue_ShouldAddCorrectly()
    {
        // Arrange
        var sequence = OptionSequence.Create([DummyValue1]);

        // Act
        var result = sequence.Add(DummyValue2);

        // Assert
        result.Length.Should().Be(2);
        result[1].Should().Be(Option<DummyValue>.Some(DummyValue2));
    }

    [Fact]
    public void AddNone_ShouldAddNoneValue()
    {
        // Arrange
        var sequence = OptionSequence.Create([1]);

        // Act
        var result = sequence.AddNone();

        // Assert
        result.Length.Should().Be(2);
        result[1].Should().Be(Option<int>.None);
    }

    [Fact]
    public void AddRange_OptionValues_ShouldAddAllValues()
    {
        // Arrange
        var sequence = OptionSequence.Create([1]);
        var range = new[] { Option<int>.Some(2), Option<int>.None, Option<int>.Some(3) };

        // Act
        var result = sequence.AddRange(range);

        // Assert
        result.Length.Should().Be(4);
        result[1].Should().Be(Option<int>.Some(2));
        result[2].Should().Be(Option<int>.None);
        result[3].Should().Be(Option<int>.Some(3));
    }

    #endregion

    #region Remove Tests

    [Fact]
    public void Remove_OptionValue_ShouldRemoveFirstOccurrence()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 1, 3]);

        // Act
        var result = sequence.Remove(Option<int>.Some(1));

        // Assert
        result.Length.Should().Be(2);
        result[0].Should().Be(Option<int>.Some(2));
        result[1].Should().Be(Option<int>.Some(3));
    }

    [Fact]
    public void Remove_Value_ShouldRemoveWrappedValue()
    {
        // Arrange
        var sequence = OptionSequence.Create(["hello", "world", "hello"]);

        // Act
        var result = sequence.Remove("hello");

        // Assert
        result.Length.Should().Be(1);
        result[0].Should().Be(Option<string>.Some("world"));
    }

    [Fact]
    public void Remove_DummyValue_ShouldRemoveCorrectly()
    {
        // Arrange
        var sequence = OptionSequence.Create([DummyValue1, DummyValue2, DummyValue1]);

        // Act
        var result = sequence.Remove(DummyValue1);

        // Assert
        result.Length.Should().Be(1);
        result[0].Should().BeEquivalentTo(Option<DummyValue>.Some(DummyValue2));
    }

    [Fact]
    public void Clear_ShouldReturnEmptySequence()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 3]);

        // Act
        var result = sequence.Clear();

        // Assert
        result.Length.Should().Be(0);
        result.IsEmpty.Should().BeTrue();
    }

    #endregion

    #region Insert Tests

    [Fact]
    public void Insert_OptionValue_ShouldInsertAtCorrectIndex()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 3]);

        // Act
        var result = sequence.Insert(1, Option<int>.Some(2));

        // Assert
        result.Length.Should().Be(3);
        result[0].Should().Be(Option<int>.Some(1));
        result[1].Should().Be(Option<int>.Some(2));
        result[2].Should().Be(Option<int>.Some(3));
    }

    [Fact]
    public void Insert_Value_ShouldWrapAndInsert()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", "third"]);

        // Act
        var result = sequence.Insert(1, "second");

        // Assert
        result.Length.Should().Be(3);
        result[0].Should().Be(Option<string>.Some("first"));
        result[1].Should().Be(Option<string>.Some("second"));
        result[2].Should().Be(Option<string>.Some("third"));
    }

    [Fact]
    public void InsertRange_OptionValues_ShouldInsertAllAtIndex()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 4]);
        var range = new[] { Option<int>.Some(2), Option<int>.None, Option<int>.Some(3) };

        // Act
        var result = sequence.InsertRange(1, range);

        // Assert
        result.Length.Should().Be(5);
        result[0].Should().Be(Option<int>.Some(1));
        result[1].Should().Be(Option<int>.Some(2));
        result[2].Should().Be(Option<int>.None);
        result[3].Should().Be(Option<int>.Some(3));
        result[4].Should().Be(Option<int>.Some(4));
    }

    [Fact]
    public void InsertRange_Values_ShouldWrapAndInsertAll()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", "last"]);
        var values = new[] { "second", "third" };

        // Act
        var result = sequence.InsertRange(1, values);

        // Assert
        result.Length.Should().Be(4);
        result[0].Should().Be(Option<string>.Some("first"));
        result[1].Should().Be(Option<string>.Some("second"));
        result[2].Should().Be(Option<string>.Some("third"));
        result[3].Should().Be(Option<string>.Some("last"));
    }

    #endregion

    #region RemoveAt Tests

    [Fact]
    public void RemoveAt_ValidIndex_ShouldRemoveElement()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 3]);

        // Act
        var result = sequence.RemoveAt(1);

        // Assert
        result.Length.Should().Be(2);
        result[0].Should().Be(Option<int>.Some(1));
        result[1].Should().Be(Option<int>.Some(3));
    }

    [Fact]
    public void RemoveAt_FirstIndex_ShouldRemoveFirstElement()
    {
        // Arrange
        var sequence = OptionSequence.Create([1.1, 2.2, 3.3]);

        // Act
        var result = sequence.RemoveAt(0);

        // Assert
        result.Length.Should().Be(2);
        result[0].Should().Be(Option<double>.Some(2.2));
        result[1].Should().Be(Option<double>.Some(3.3));
    }

    [Fact]
    public void RemoveAt_LastIndex_ShouldRemoveLastElement()
    {
        // Arrange
        var sequence = OptionSequence.Create(["a", "b", "c"]);

        // Act
        var result = sequence.RemoveAt(2);

        // Assert
        result.Length.Should().Be(2);
        result[0].Should().Be(Option<string>.Some("a"));
        result[1].Should().Be(Option<string>.Some("b"));
    }

    #endregion

    #region RemoveAll Tests

    [Fact]
    public void RemoveAll_OptionPredicate_ShouldRemoveMatchingElements()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", null, "second", null, "third"]);

        // Act
        var result = sequence.RemoveAll(option => option.IsNone);

        // Assert
        result.Length.Should().Be(3);
        result[0].Should().Be(Option<string>.Some("first"));
        result[1].Should().Be(Option<string>.Some("second"));
        result[2].Should().Be(Option<string>.Some("third"));
    }

    [Fact]
    public void RemoveAll_ValuePredicate_ShouldRemoveMatchingValues()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 3, 4, 5]);

        // Act
        var result = sequence.RemoveAll(value => value % 2 == 0);

        // Assert
        result.Length.Should().Be(3);
        result[0].Should().Be(Option<int>.Some(1));
        result[1].Should().Be(Option<int>.Some(3));
        result[2].Should().Be(Option<int>.Some(5));
    }

    [Fact]
    public void RemoveAll_DummyValuePredicate_ShouldRemoveMatchingValues()
    {
        // Arrange
        var sequence = OptionSequence.Create([DummyValue1, DummyValue2, DummyValue3]);

        // Act
        var result = sequence.RemoveAll(value => value.Name.StartsWith("J"));

        // Assert
        result.Length.Should().Be(1);
        result[0].Should().Be(Option<DummyValue>.Some(DummyValue3));
    }

    [Fact]
    public void RemoveNone_ShouldRemoveAllNoneValues()
    {
        // Arrange
        var sequence = OptionSequence.Create(["hello", null, "world", null, "test"]);

        // Act
        var result = sequence.RemoveNone();

        // Assert
        result.Length.Should().Be(3);
        result[0].Should().Be(Option<string>.Some("hello"));
        result[1].Should().Be(Option<string>.Some("world"));
        result[2].Should().Be(Option<string>.Some("test"));
    }

    #endregion

    #region Indexer and Access Tests

    [Fact]
    public void GetValueOrNone_ValidIndex_ShouldReturnValue()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", null, "second"]);

        // Act & Assert
        sequence.GetValueOrNone(0).Should().Be(Option<string>.Some("first"));
        sequence.GetValueOrNone(1).Should().Be(Option<string>.None);
        sequence.GetValueOrNone(2).Should().Be(Option<string>.Some("second"));
    }

    [Fact]
    public void GetValueOrNone_InvalidIndex_ShouldReturnNone()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2]);

        // Act & Assert
        sequence.GetValueOrNone(-1).Should().Be(Option<int>.None);
        sequence.GetValueOrNone(5).Should().Be(Option<int>.None);
    }

    [Fact]
    public void Indexer_Range_ShouldReturnSubsequence()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 3, 4, 5]);

        // Act
        var result = sequence[1..4];

        // Assert
        result.Length.Should().Be(3);
        result[0].Should().Be(Option<int>.Some(2));
        result[1].Should().Be(Option<int>.Some(3));
        result[2].Should().Be(Option<int>.Some(4));
    }

    [Fact]
    public void Indexer_RangeFromEnd_ShouldReturnCorrectSubsequence()
    {
        // Arrange
        var sequence = OptionSequence.Create(["a", "b", "c", "d"]);

        // Act
        var result = sequence[^2..];

        // Assert
        result.Length.Should().Be(2);
        result[0].Should().Be(Option<string>.Some("c"));
        result[1].Should().Be(Option<string>.Some("d"));
    }

    #endregion

    #region IndexOf Tests

    [Fact]
    public void IndexOf_OptionValue_ExistingValue_ShouldReturnCorrectIndex()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", null, "third", "first"]);

        // Act
        var result = sequence.IndexOf(Option<string>.Some("third"));

        // Assert
        result.Should().Be(Option<int>.Some(2));
    }

    [Fact]
    public void IndexOf_OptionValue_NonExistingValue_ShouldReturnNone()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 3]);

        // Act
        var result = sequence.IndexOf(Option<int>.Some(5));

        // Assert
        result.Should().Be(Option<int>.None);
    }

    [Fact]
    public void IndexOf_Value_ExistingValue_ShouldReturnCorrectIndex()
    {
        // Arrange
        var sequence = OptionSequence.Create(["hello", "world", "hello"]);

        // Act
        var result = sequence.IndexOf("world");

        // Assert
        result.Should().Be(Option<int>.Some(1));
    }

    [Fact]
    public void IndexOf_DummyValue_ShouldReturnCorrectIndex()
    {
        // Arrange
        var sequence = OptionSequence.Create(
            new DummyValue?[] { DummyValue1, DummyValue2, DummyValue3 }
        );

        // Act
        var result = sequence.IndexOf(DummyValue2);

        // Assert
        result.Should().Be(Option<int>.Some(1));
    }

    [Fact]
    public void IndexOfNone_ExistingNone_ShouldReturnCorrectIndex()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", null, "second", null]);

        // Act
        var result = sequence.IndexOfNone();

        // Assert
        result.Should().Be(Option<int>.Some(1));
    }

    [Fact]
    public void IndexOfNone_NoNoneValues_ShouldReturnNone()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 3]);

        // Act
        var result = sequence.IndexOfNone();

        // Assert
        result.Should().Be(Option<int>.None);
    }

    #endregion

    #region LastIndexOf Tests

    [Fact]
    public void LastIndexOf_OptionValue_ExistingValue_ShouldReturnLastIndex()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 1, 3, 1]);

        // Act
        var result = sequence.LastIndexOf(Option<int>.Some(1));

        // Assert
        result.Should().Be(Option<int>.Some(4));
    }

    [Fact]
    public void LastIndexOf_Value_ExistingValue_ShouldReturnLastIndex()
    {
        // Arrange
        var sequence =
            OptionSequence.Create(["test", "hello", "test", "world"]);

        // Act
        var result = sequence.LastIndexOf("test");

        // Assert
        result.Should().Be(Option<int>.Some(2));
    }

    [Fact]
    public void LastIndexOf_DummyValue_ShouldReturnCorrectIndex()
    {
        // Arrange
        var sequence = OptionSequence.Create([DummyValue1, DummyValue2, DummyValue1]);

        // Act
        var result = sequence.LastIndexOf(DummyValue1);

        // Assert
        result.Should().Be(Option<int>.Some(2));
    }

    [Fact]
    public void LastIndexOfNone_ExistingNone_ShouldReturnLastIndex()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", null, "second", null, "third"]);

        // Act
        var result = sequence.LastIndexOfNone();

        // Assert
        result.Should().Be(Option<int>.Some(3));
    }

    #endregion

    #region AllIndicesOf Tests

    [Fact]
    public void AllIndicesOf_OptionValue_ShouldReturnAllIndices()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 1, 3, 1, 2]);

        // Act
        var result = sequence.AllIndicesOf(Option<int>.Some(1));

        // Assert
        result.Length.Should().Be(3);
        result[0].Value.Should().Be(0);
        result[1].Value.Should().Be(2);
        result[2].Value.Should().Be(4);
    }

    [Fact]
    public void AllIndicesOf_Value_ShouldReturnAllIndices()
    {
        // Arrange
        var sequence = OptionSequence.Create(["a", "b", "a", "c", "a"]);

        // Act
        var result = sequence.AllIndicesOf("a");

        // Assert
        result.Length.Should().Be(3);
        result[0].Value.Should().Be(0);
        result[1].Value.Should().Be(2);
        result[2].Value.Should().Be(4);
    }

    [Fact]
    public void AllIndicesOf_DummyValue_ShouldReturnAllIndices()
    {
        // Arrange
        var sequence = OptionSequence.Create(
            [DummyValue1, DummyValue2, DummyValue1, DummyValue3, DummyValue1]
        );

        // Act
        var result = sequence.AllIndicesOf(DummyValue1);

        // Assert
        result.Length.Should().Be(3);
        result[0].Value.Should().Be(0);
        result[1].Value.Should().Be(2);
        result[2].Value.Should().Be(4);
    }

    [Fact]
    public void AllIndicesOfNone_ShouldReturnAllNoneIndices()
    {
        // Arrange
        var sequence =
            OptionSequence.Create(["first", null, "third", null, "fifth", null]);

        // Act
        var result = sequence.AllIndicesOfNone();

        // Assert
        result.Length.Should().Be(3);
        result[0].Value.Should().Be(1);
        result[1].Value.Should().Be(3);
        result[2].Value.Should().Be(5);
    }

    [Fact]
    public void AllIndicesOf_NonExistingValue_ShouldReturnEmptySequence()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 3]);

        // Act
        var result = sequence.AllIndicesOf(5);

        // Assert
        result.Length.Should().Be(0);
        result.IsEmpty.Should().BeTrue();
    }

    #endregion

    #region AsSpan Tests

    [Fact]
    public void AsSpan_ShouldReturnCorrectSpan()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", null, "third"]);

        // Act
        var span = sequence.AsSpan();

        // Assert
        span.Length.Should().Be(3);
        span[0].Should().Be(Option<string>.Some("first"));
        span[1].Should().Be(Option<string>.None);
        span[2].Should().Be(Option<string>.Some("third"));
    }

    [Fact]
    public void AsSpan_EmptySequence_ShouldReturnEmptySpan()
    {
        // Arrange
        var sequence = OptionSequence.Create(Array.Empty<string?>());

        // Act
        var span = sequence.AsSpan();

        // Assert
        span.Length.Should().Be(0);
        span.IsEmpty.Should().BeTrue();
    }

    #endregion

    #region AsSequence Tests

    [Fact]
    public void AsSequence_ShouldReturnUnderlyingSequence()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", null, "third"]);

        // Act
        var innerSequence = sequence.AsSequence();

        // Assert
        innerSequence.Should().BeOfType<Sequence<Option<string>>>();
        innerSequence.Length.Should().Be(3);
        innerSequence[0].Value.Should().Be(Option<string>.Some("first"));
        innerSequence[1].Value.Should().Be(Option<string>.None);
        innerSequence[2].Value.Should().Be(Option<string>.Some("third"));
    }

    #endregion

    #region Enumeration Tests

    [Fact]
    public void GetEnumerator_ShouldEnumerateAllElements()
    {
        // Arrange
        var sequence = OptionSequence.Create(["first", null, "third"]);

        // Act
        var elements = sequence.ToList();

        // Assert
        elements.Should().HaveCount(3);
        elements[0].Should().Be(Option<string>.Some("first"));
        elements[1].Should().Be(Option<string>.None);
        elements[2].Should().Be(Option<string>.Some("third"));
    }

    [Fact]
    public void GetEnumerator_EmptySequence_ShouldEnumerateNothing()
    {
        // Arrange
        var sequence = OptionSequence.Create((string?[]) []);

        // Act
        var elements = sequence.ToList();

        // Assert
        elements.Should().BeEmpty();
    }

    [Fact]
    public void GetEnumerator_WithDummyValues_ShouldEnumerateCorrectly()
    {
        // Arrange
        var sequence =
            OptionSequence.Create([DummyValue1, null, DummyValue2]);

        // Act
        var elements = sequence.ToList();

        // Assert
        elements.Should().HaveCount(3);
        elements[0].Should().Be(Option<DummyValue>.Some(DummyValue1));
        elements[1].Should().Be(Option<DummyValue>.None);
        elements[2].Should().Be(Option<DummyValue>.Some(DummyValue2));
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void EmptySequence_AllOperations_ShouldHandleGracefully()
    {
        // Arrange
        var sequence = OptionSequence.Create((string?[]) []);

        // Act & Assert
        sequence.Length.Should().Be(0);
        sequence.IsEmpty.Should().BeTrue();
        sequence.GetValueOrNone(0).Should().Be(Option<string>.None);
        sequence.IndexOf("first").Should().Be(Option<int>.None);
        sequence.LastIndexOf("first").Should().Be(Option<int>.None);
        sequence.AllIndicesOf("first").Should().BeEmpty();
        sequence.RemoveNone().Should().BeEmpty();
    }

    [Fact]
    public void SingleElementSequence_ShouldWorkCorrectly()
    {
        // Arrange
        var sequence = OptionSequence.Create([42]);

        // Act & Assert
        sequence.Length.Should().Be(1);
        sequence.IsEmpty.Should().BeFalse();
        sequence[0].Should().Be(Option<int>.Some(42));
        sequence.IndexOf(42).Should().Be(Option<int>.Some(0));
        sequence.LastIndexOf(42).Should().Be(Option<int>.Some(0));
        sequence.AllIndicesOf(42).Should().HaveCount(1).And.Contain(0);
    }

    [Fact]
    public void SingleNoneElementSequence_ShouldWorkCorrectly()
    {
        // Arrange
        var sequence = OptionSequence.Create((string?[]) [null]);

        // Act & Assert
        sequence.Length.Should().Be(1);
        sequence.IsEmpty.Should().BeFalse();
        sequence[0].Should().Be(Option<string>.None);
        sequence.IndexOfNone().Should().Be(Option<int>.Some(0));
        sequence.LastIndexOfNone().Should().Be(Option<int>.Some(0));
        sequence.AllIndicesOfNone().Should().HaveCount(1).And.Contain(0);
        sequence.RemoveNone().Should().BeEmpty();
    }

    #endregion

    #region Complex Scenario Tests

    [Fact]
    public void ComplexScenario_MixedOperations_WithInts()
    {
        // Arrange
        var sequence = OptionSequence.Create(
            [
                "first", null, "third", "fourth", null, "sixth"
            ]
        );

        // Act
        var result = sequence
            .Add("seventh")
            .AddNone()
            .Remove("fourth")
            .RemoveNone()
            .Insert(2, "second");

        // Assert
        result.Length.Should().Be(5);
        result[0].Should().Be(Option<string>.Some("first"));
        result[1].Should().Be(Option<string>.Some("third"));
        result[2].Should().Be(Option<string>.Some("second"));
        result[3].Should().Be(Option<string>.Some("sixth"));
        result[4].Should().Be(Option<string>.Some("seventh"));
    }

    [Fact]
    public void ComplexScenario_MixedOperations_WithStrings()
    {
        // Arrange
        var sequence =
            OptionSequence.Create(["hello", null, "world", null]);

        // Act
        var result = sequence
            .AddRange([Option<string>.Some("test"), Option<string>.None])
            .RemoveAll(option => option.IsSome && option.Value!.Length > 4)
            .InsertRange(1, ["foo", "bar"]);

        // Assert
        result.Length.Should().Be(6);
        result[0].Should().Be(Option<string>.None);
        result[1].Should().Be(Option<string>.Some("foo"));
        result[2].Should().Be(Option<string>.Some("bar"));
        result[3].Should().Be(Option<string>.None);
        result[4].Should().Be(Option<string>.Some("test"));
        result[5].Should().Be(Option<string>.None);
    }

    [Fact]
    public void ComplexScenario_MixedOperations_WithDummyValues()
    {
        // Arrange
        var sequence = OptionSequence.Create(
            new DummyValue?[] { DummyValue1, null, DummyValue2, DummyValue3 }
        );

        // Act
        var result = sequence
            .RemoveAll(value => value.Email.Contains("jane"))
            .Add(new DummyValue { Name = "Alice", Email = "alice@example.com" })
            .RemoveAt(1);

        // Assert
        result.Length.Should().Be(3);
        result[0].Should().Be(Option<DummyValue>.Some(DummyValue1));
        result[1].Should().Be(Option<DummyValue>.Some(DummyValue3));
        result[2].IsSome.Should().BeTrue();
        result[2].Value!.Name.Should().Be("Alice");
        result[2].Value!.Email.Should().Be("alice@example.com");
    }

    #endregion

    #region Equality Comparer Tests

    [Fact]
    public void Remove_WithEqualityComparer_ShouldUseCustomComparer()
    {
        // Arrange
        var sequence = OptionSequence.Create(["Hello", "WORLD", "hello"]);
        var comparer = EqualityComparer<Option<string>>.Create(
            (x, y) =>
                x.IsSome
                && y.IsSome
                && string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase),
            obj => obj.IsSome ? obj.Value!.ToUpperInvariant().GetHashCode() : 0
        );

        // Act
        var result = sequence.Remove(Option<string>.Some("HELLO"), comparer);

        // Assert
        result.Length.Should().Be(1);
        result[0].Should().Be(Option<string>.Some("WORLD"));
    }

    [Fact]
    public void IndexOf_WithEqualityComparer_ShouldUseCustomComparer()
    {
        // Arrange
        var sequence = OptionSequence.Create(["Hello", "WORLD", "hello"]);
        var comparer = EqualityComparer<Option<string>>.Create(
            (x, y) =>
                x.IsSome
                && y.IsSome
                && string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase),
            obj => obj.IsSome ? obj.Value!.ToUpperInvariant().GetHashCode() : 0
        );

        // Act
        var result = sequence.IndexOf(Option<string>.Some("HELLO"), comparer);

        // Assert
        result.Should().Be(Option<int>.Some(0));
    }

    [Fact]
    public void LastIndexOf_WithEqualityComparer_ShouldUseCustomComparer()
    {
        // Arrange
        var sequence = OptionSequence.Create(["Hello", "WORLD", "hello"]);
        var comparer = EqualityComparer<Option<string>>.Create(
            (x, y) =>
                x.IsSome
                && y.IsSome
                && string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase),
            obj => obj.IsSome ? obj.Value!.ToUpperInvariant().GetHashCode() : 0
        );

        // Act
        var result = sequence.LastIndexOf(Option<string>.Some("HELLO"), comparer);

        // Assert
        result.Should().Be(Option<int>.Some(2));
    }

    [Fact]
    public void AllIndicesOf_WithEqualityComparer_ShouldUseCustomComparer()
    {
        // Arrange
        var sequence =
            OptionSequence.Create(["Hello", "WORLD", "hello", "HELLO"]);
        var comparer = EqualityComparer<Option<string>>.Create(
            (x, y) =>
                x.IsSome
                && y.IsSome
                && string.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase),
            obj => obj.IsSome ? obj.Value!.ToUpperInvariant().GetHashCode() : 0
        );

        // Act
        var result = sequence.AllIndicesOf(Option<string>.Some("HELLO"), comparer);

        // Assert
        result.Length.Should().Be(3);
        result[0].Value.Should().Be(0);
        result[1].Value.Should().Be(2);
        result[2].Value.Should().Be(3);
    }

    #endregion

    #region Performance and Large Data Tests

    [Fact]
    public void LargeSequence_BasicOperations_ShouldPerformCorrectly()
    {
        // Arrange
        var largeArray = Enumerable
            .Range(0, 1000)
            .Select(v => v % 3 != 0 ? $"{v}" : null )
            .ToArray();
        var sequence = OptionSequence.Create(largeArray);

        // Act & Assert
        sequence.Length.Should().Be(1000);
        sequence.RemoveNone().Length.Should().Be(666); // Approximately 2/3 of elements
        sequence
            .AllIndicesOfNone()
            .Length
            .Should()
            .Be(334); // Approximately 1/3 of elements
        sequence.IndexOf("500").Should().Be(Option<int>.Some(500));
        sequence.LastIndexOf("999").Should().Be(Option<int>.None);
    }

    [Fact]
    public void LargeSequence_WithDummyValues_ShouldPerformCorrectly()
    {
        // Arrange
        var largeDummyArray = Enumerable
            .Range(0, 100)
            .Select(i => i % 5 == 0
                ? null
                : new DummyValue
                {
                    Name = $"User{i}",
                    Email = $"user{i}@example.com"
                }
            )
            .ToArray();
        var sequence = OptionSequence.Create(largeDummyArray);

        // Act & Assert
        sequence.Length.Should().Be(100);
        sequence.RemoveNone().Length.Should().Be(80);
        sequence.AllIndicesOfNone().Length.Should().Be(20);

        var userToFind = new DummyValue { Name = "User50", Email = "user50@example.com" };
        sequence.IndexOf(userToFind).Should().Be(Option<int>.None); // Different reference
    }

    #endregion

    #region Type Safety Tests

    [Fact]
    public void TypeSafety_IntSequence_ShouldOnlyAcceptInts()
    {
        // Arrange
        var sequence = OptionSequence.Create([1, 2, 3]);

        // Act & Assert - These should compile without issues
        var result1 = sequence.Add(4);
        var result2 = sequence.Remove(2);
        var result3 = sequence.Insert(1, 5);

        result1.Length.Should().Be(4);
        result2.Length.Should().Be(2);
        result3.Length.Should().Be(4);
    }

    [Fact]
    public void TypeSafety_DummyValueSequence_ShouldOnlyAcceptDummyValues()
    {
        // Arrange
        var sequence =
            OptionSequence.Create(new DummyValue?[] { DummyValue1, DummyValue2 });

        // Act & Assert - These should compile without issues
        var result1 = sequence.Add(DummyValue3);
        var result2 = sequence.Remove(DummyValue1);
        var newDummy = new DummyValue { Name = "Test", Email = "test@example.com" };
        var result3 = sequence.Insert(1, newDummy);

        result1.Length.Should().Be(3);
        result2.Length.Should().Be(1);
        result3.Length.Should().Be(3);
    }

    #endregion

    #region Immutability Tests

    [Fact]
    public void ImmutabilityTest_OriginalSequenceUnchanged_AfterOperations()
    {
        // Arrange
        var original = OptionSequence.Create([1, 2, 3]);

        // Act
        var modified = original
            .Add(4)
            .Remove(2)
            .Insert(0, 0);

        // Assert
        original.Length.Should().Be(3);
        original[0].Should().Be(Option<int>.Some(1));
        original[1].Should().Be(Option<int>.Some(2));
        original[2].Should().Be(Option<int>.Some(3));

        modified.Length.Should().Be(4);
        modified[0].Should().Be(Option<int>.Some(0));
        modified[1].Should().Be(Option<int>.Some(1));
        modified[2].Should().Be(Option<int>.Some(3));
        modified[3].Should().Be(Option<int>.Some(4));
    }

    [Fact]
    public void ImmutabilityTest_DummyValues_OriginalUnchanged()
    {
        // Arrange
        var original =
            OptionSequence.Create([DummyValue1, DummyValue2]);

        // Act
        var modified = original
            .Add(DummyValue3)
            .RemoveAt(0);

        // Assert
        original.Length.Should().Be(2);
        original[0].Should().Be(Option<DummyValue>.Some(DummyValue1));
        original[1].Should().Be(Option<DummyValue>.Some(DummyValue2));

        modified.Length.Should().Be(2);
        modified[0].Should().Be(Option<DummyValue>.Some(DummyValue2));
        modified[1].Should().Be(Option<DummyValue>.Some(DummyValue3));
    }

    #endregion
}