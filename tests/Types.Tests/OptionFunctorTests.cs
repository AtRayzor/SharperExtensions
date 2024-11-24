using System.Collections;
using DotNetCoreFunctional.Option;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using Xunit;

namespace NetFunction.Types.Tests;

public class OptionFunctorTests
{
    [Theory]
    [ClassData(typeof(MapTestCases))]
    public void MapTests(
        Option<DummyValue> option,
        Func<DummyValue, DummyNewValue> mapping,
        Option<DummyNewValue> expected,
        Type expectedType
    )
    {
        Option
            .Functor.Map(option, mapping)
            .Should()
            .BeOfType(expectedType)
            .And.BeEquivalentTo(expected);
    }
}

file static class TestFunctions
{
    public static DummyNewValue Mapping(DummyValue value)
    {
        return new DummyNewValue { NameAllCaps = value.Name.ToUpper() };
    }
}

file class MapTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            OptionTestData.SomeValue,
            (Func<DummyValue, DummyNewValue>)TestFunctions.Mapping,
            OptionTestData.SomeNewValue,
            typeof(Some<DummyNewValue>),
        ];
        yield return
        [
            OptionTestData.NoneValue,
            (Func<DummyValue, DummyNewValue>)TestFunctions.Mapping,
            OptionTestData.NoneNewValue,
            typeof(None<DummyNewValue>),
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
