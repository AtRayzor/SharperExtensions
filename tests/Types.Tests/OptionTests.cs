using System.Collections;
using DotNetCoreFunctional.Option;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using NetFunctional.Types;
using Xunit;

namespace NetFunction.Types.Tests;

public class OptionTests
{
    [Theory]
    [ClassData(typeof(IsSomeTestCases))]
    public void IsSome_Tests(Option<DummyValue> option, bool expected)
    {
        Option.IsSome(option).Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(IsNoneTestCases))]
    public void IsNone_Tests(Option<DummyValue> option, bool expected)
    {
        Option.IsNone(option).Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(GetValueOrdDefaultTestCases))]
    public void GetValueOrDefaultTests(Option<DummyValue> option, DummyValue? value)
    {
        Option.Unsafe.GetValueOrDefault(option).Should().Be(value);
    }

    [Fact]
    public void TryGetValue_CallWithSome_ReturnsTrueAndOutputsValue()
    {
        Option.Unsafe.TryGetValue(OptionTestData.SomeValue, out var value).Should().BeTrue();
        value.Should().BeEquivalentTo(OptionTestData.Value);
    }

    [Fact]
    public void TryGetValue_CallWithNone_ReturnsFalseAndOutputsValue()
    {
        Option.Unsafe.TryGetValue(OptionTestData.NoneValue, out var value).Should().BeFalse();
        value.Should().BeNull();
    }
}

file class IsSomeTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [OptionTestData.SomeValue, true];
        yield return [OptionTestData.NoneValue, false];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class IsNoneTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [OptionTestData.NoneValue, true];
        yield return [OptionTestData.SomeValue, false];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class GetValueOrdDefaultTestCases : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return [OptionTestData.SomeValue, OptionTestData.Value];
        yield return [OptionTestData.NoneValue, default];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
