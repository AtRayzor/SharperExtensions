using System.Collections;

namespace SharperExtensions.Core.Tests;

public class OptionOperationTests
{
    [Theory]
    [ClassData(typeof(MapTestCases))]
    internal void MapTests(
        Option<DummyValue> option,
        Func<DummyValue, DummyNewValue> mapping,
        Option<DummyNewValue> expected,
        OptionType expectedType
    )
    {
        Option
            .Functor
            .Map(option, mapping)
            .Should()
            .Satisfy<Option<DummyNewValue>>(o => o.Type.Should().Be(expectedType))
            .And
            .Satisfy<Option<DummyNewValue>>(o =>
                o.Value.Should().BeEquivalentTo(expected.Value)
            );
    }

    [Theory]
    [ClassData(typeof(BindTestCases))]
    internal void BindTests(
        Option<DummyValue> option,
        Func<DummyValue, Option<DummyNewValue>> binder,
        Option<DummyNewValue> expected,
        OptionType expectedType
    )
    {
        Option
            .Monad
            .Bind(option, binder)
            .Should()
            .Satisfy<Option<DummyNewValue>>(o => o.Type.Should().Be(expectedType))
            .And
            .Satisfy<Option<DummyNewValue>>(o =>
                o.Value.Should().BeEquivalentTo(expected.Value)
            );
    }

    [Theory]
    [ClassData(typeof(ApplyTestCases))]
    internal void ApplyTests(
        Option<DummyValue> option,
        Option<Func<DummyValue, DummyNewValue>> wrappedMapper,
        Option<DummyNewValue> expected,
        OptionType expectedType
    )
    {
        Option
            .Applicative
            .Apply(option, wrappedMapper)
            .Should()
            .Satisfy<Option<DummyNewValue>>(o => o.Type.Should().Be(expectedType))
            .And
            .Satisfy<Option<DummyNewValue>>(o =>
                o.Value.Should().BeEquivalentTo(expected.Value)
            );
    }
}

file static class TestFunctions
{
    public static DummyNewValue Mapping(DummyValue value)
    {
        return new DummyNewValue { NameAllCaps = value.Name.ToUpper() };
    }

    public static Option<DummyNewValue> Binder(DummyValue value)
    {
        return new DummyNewValue { NameAllCaps = value.Name.ToUpper() };
    }
}

file class MapTestCases : IEnumerable<
    TheoryDataRow<Option<DummyValue>,
        Func<DummyValue, DummyNewValue>,
        Option<DummyNewValue>,
        OptionType
    >
>
{
    public IEnumerator<
        TheoryDataRow<
            Option<DummyValue>,
            Func<DummyValue, DummyNewValue>,
            Option<DummyNewValue>,
            OptionType
        >
    > GetEnumerator()
    {
        yield return new
            TheoryDataRow<
                Option<DummyValue>,
                Func<DummyValue, DummyNewValue>,
                Option<DummyNewValue>,
                OptionType
            >(
                OptionTestData.SomeValue,
                (Func<DummyValue, DummyNewValue>)TestFunctions.Mapping,
                OptionTestData.SomeNewValue,
                OptionType.Some
            );
        yield return new
            TheoryDataRow<
                Option<DummyValue>,
                Func<DummyValue, DummyNewValue>,
                Option<DummyNewValue>,
                OptionType
            >(
                OptionTestData.NoneValue,
                (Func<DummyValue, DummyNewValue>)TestFunctions.Mapping,
                OptionTestData.NoneNewValue,
                OptionType.None
            );
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class BindTestCases : IEnumerable<
    TheoryDataRow<Option<DummyValue>,
        Func<DummyValue, Option<DummyNewValue>>,
        Option<DummyNewValue>,
        OptionType
    >
>
{
    public IEnumerator<
        TheoryDataRow<
            Option<DummyValue>,
            Func<DummyValue, Option<DummyNewValue>>,
            Option<DummyNewValue>,
            OptionType
        >
    > GetEnumerator()
    {
        yield return new TheoryDataRow<
            Option<DummyValue>,
            Func<DummyValue, Option<DummyNewValue>>,
            Option<DummyNewValue>,
            OptionType
        >(
            OptionTestData.SomeValue,
            (Func<DummyValue, Option<DummyNewValue>>)TestFunctions.Binder,
            OptionTestData.SomeNewValue,
            OptionType.Some
        );
        yield return new TheoryDataRow<
            Option<DummyValue>,
            Func<DummyValue, Option<DummyNewValue>>,
            Option<DummyNewValue>,
            OptionType
        >(
            OptionTestData.NoneValue,
            (Func<DummyValue, Option<DummyNewValue>>)TestFunctions.Binder,
            OptionTestData.NoneNewValue,
            OptionType.None
        );
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class ApplyTestCases : IEnumerable<
    TheoryDataRow<
        Option<DummyValue>,
        Option<Func<DummyValue, DummyNewValue>>,
        Option<DummyNewValue>,
        OptionType
    >
>
{
    public IEnumerator<
        TheoryDataRow<
            Option<DummyValue>,
            Option<Func<DummyValue, DummyNewValue>>,
            Option<DummyNewValue>,
            OptionType
        >
    > GetEnumerator()
    {
        yield return new TheoryDataRow<
            Option<DummyValue>,
            Option<Func<DummyValue, DummyNewValue>>,
            Option<DummyNewValue>,
            OptionType
        >(
            OptionTestData.SomeValue,
            Option<Func<DummyValue, DummyNewValue>>.Some(TestFunctions.Mapping),
            OptionTestData.SomeNewValue,
            OptionType.Some
        );
        yield return new TheoryDataRow<
            Option<DummyValue>,
            Option<Func<DummyValue, DummyNewValue>>,
            Option<DummyNewValue>,
            OptionType
        >(
            OptionTestData.NoneValue,
            Option<Func<DummyValue, DummyNewValue>>.Some(TestFunctions.Mapping),
            OptionTestData.NoneNewValue,
            OptionType.None
        );
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}