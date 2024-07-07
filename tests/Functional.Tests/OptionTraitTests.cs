using System.Collections;
using FluentAssertions;
using Monads.OptionKind;
using Monads.Tests.DummyTypes;
using Monads.Traits;

namespace Monads.Tests;

public class OptionTraitTests
{
    private static DummyValue Value => new() { Name = "Jack Black", Email = "jack.black@example.com" };
    private static DummyNewValue NewValue => new() { NameAllCaps = "JACK BLACK" };
    private static OptionType<DummyValue>.Some SomeValue => new(Value);
    private static OptionType<DummyNewValue>.Some SomeNewValue => new(NewValue);

    private static IFunctor<OptionType<DummyValue>> SomeOptionFunctor =>
        new Functor<OptionType<DummyValue>>(SomeValue);

    private static IFunctor<OptionType<DummyValue>> NoneOptionFunctor =>
        new Functor<OptionType<DummyValue>>(new OptionType<DummyValue>.None());

    private class IsSomeTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [SomeOptionFunctor, true];
            yield return [NoneOptionFunctor, false];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class IsNoneTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [SomeOptionFunctor, false];
            yield return [NoneOptionFunctor, true];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    [Theory]
    [ClassData(typeof(IsSomeTestData))]
    public void IsSome_Tests(IFunctor<OptionType<DummyValue>> option, bool expected)
    {
        option.IsSome().Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(IsNoneTestData))]
    public void IsNone_Tests(IFunctor<OptionType<DummyValue>> option, bool expected)
    {
        option.IsNone().Should().Be(expected);
    }


    [Fact]
    public void TryGetValue_CallOnSome_GetsValue()
    {
        SomeOptionFunctor.TryGetValue(out var value).Should().BeTrue();
        value.Should().BeEquivalentTo(Value);
    }


    [Fact]
    public void TryGetValue_CallOnNone_DoesNotGetValue()
    {
        NoneOptionFunctor.TryGetValue(out var value).Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void AsOption_CallOnFunctor_ConvertToOption()
    {
        SomeOptionFunctor.AsOption()
            .Should().BeAssignableTo<IOption<DummyValue>>();
    }
    
    [Fact]
    public void AsFunctor_CallOnFunctor_ConvertToOption()
    {
        var someOption = OptionFactory<DummyValue>.Construct<Option<DummyValue>>(Value);
        someOption.AsFunctor()
            .Should().BeAssignableTo<IFunctor<OptionType<DummyValue>>>();
    }
    
    [Fact]
    public void AsMonad_CallOnFunctor_ConvertToOption()
    {
        SomeOptionFunctor.AsMonad()
            .Should().BeAssignableTo<IMonad<OptionType<DummyValue>>>();
    }

}