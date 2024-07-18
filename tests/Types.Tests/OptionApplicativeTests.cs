using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using NetFunctional.Types.OptionKind;
using NetFunctional.Types.Traits;

namespace NetFunction.Types.Tests;

public class OptionApplicativeTests
{
    [Fact]
    public void Apply_CallOnSomeOptionWithSomeMapping_ReturnsSome()
    {
        (
            TestData.ValueOptionApplicative.Apply(TestData.SomeApplicativeMapping).Type
            as OptionType<DummyNewValue>.Some
        )
            .Should()
            .BeEquivalentTo(TestData.NewValueOption);
    }

    [Fact]
    public void Apply_CallOnSomeOptionWithNoneMapping_ReturnsNone()
    {
        (
            TestData.ValueOptionApplicative.Apply(TestData.NoneApplicativeMapping).Type
            as OptionType<DummyNewValue>.None
        )
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void Apply_CallOnNoneOptionWithSomeMapping_ReturnsNone()
    {
        (
            TestData.NoneOptionApplicative.Apply(TestData.SomeApplicativeMapping).Type
            as OptionType<DummyNewValue>.None
        )
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void Apply_CallOnNoneOptionWithNoneMapping_ReturnsNone()
    {
        (
            TestData.NoneOptionApplicative.Apply(TestData.NoneApplicativeMapping).Type
            as OptionType<DummyNewValue>.None
        )
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task ApplyAsync_CallOnSomeOptionWithSomeMapping_ReturnsSome()
    {
        (
            (
                await TestData
                    .ValueOptionApplicativeAsync
                    .ApplyAsync(TestData.SomeApplicativeMappingAsync)
            ).Type as OptionType<DummyNewValue>.Some
        )
            .Should()
            .BeEquivalentTo(TestData.NewValueOption);
    }

    [Fact]
    public async Task ApplyAsync_CallOnSomeOptionWithNoneMapping_ReturnsNone()
    {
        (
            (
                await TestData
                    .ValueOptionApplicativeAsync
                    .ApplyAsync(TestData.NoneApplicativeMappingAsync)
            ).Type as OptionType<DummyNewValue>.None
        )
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task ApplyAsync_CallOnNoneOptionWithSomeMapping_ReturnsNone()
    {
        (
            (
                await TestData
                    .NoneOptionApplicativeAsync
                    .ApplyAsync(TestData.SomeApplicativeMappingAsync)
            ).Type as OptionType<DummyNewValue>.None
        )
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task ApplyAsync_CallOnNoneOptionWithNoneMapping_ReturnsNone()
    {
        (
            (
                await TestData
                    .NoneOptionApplicativeAsync
                    .ApplyAsync(TestData.NoneApplicativeMappingAsync)
            ).Type as OptionType<DummyNewValue>.None
        )
            .Should()
            .NotBeNull();
    }
}

file static class TestData
{
    public static DummyValue Value =>
        new() { Name = "Jack Black", Email = "jack.black@example.com" };

    public static DummyNewValue NewValue => new() { NameAllCaps = "JACK BLACK" };

    public static OptionType<DummyValue>.Some ValueOption => new(Value);
    public static OptionType<DummyValue>.None NoneOption => new();
    public static OptionType<DummyNewValue>.Some NewValueOption => new(NewValue);
    public static OptionType<DummyNewValue>.None NewNoneOption => new();

    public static IApplicative<OptionType<DummyValue>> ValueOptionApplicative =>
        new Applicative<OptionType<DummyValue>>(ValueOption);

    public static IApplicative<OptionType<DummyValue>> NoneOptionApplicative =>
        new Applicative<OptionType<DummyValue>>(NoneOption);

    public static IApplicative<
        OptionType<Func<DummyValue, DummyNewValue>>
    > SomeApplicativeMapping =>
        new Applicative<OptionType<Func<DummyValue, DummyNewValue>>>(
            new OptionType<Func<DummyValue, DummyNewValue>>.Some(TestFunctions.Mapping)
        );

    public static IApplicative<
        OptionType<Func<DummyValue, DummyNewValue>>
    > NoneApplicativeMapping =>
        new Applicative<OptionType<Func<DummyValue, DummyNewValue>>>(
            new OptionType<Func<DummyValue, DummyNewValue>>.None()
        );

    public static Task<IApplicative<OptionType<DummyValue>>> ValueOptionApplicativeAsync =>
        Task.FromResult(ValueOptionApplicative);

    public static Task<IApplicative<OptionType<DummyValue>>> NoneOptionApplicativeAsync =>
        Task.FromResult(NoneOptionApplicative);

    public static Task<
        IApplicative<OptionType<Func<DummyValue, DummyNewValue>>>
    > SomeApplicativeMappingAsync => Task.FromResult(SomeApplicativeMapping);

    public static Task<
        IApplicative<OptionType<Func<DummyValue, DummyNewValue>>>
    > NoneApplicativeMappingAsync => Task.FromResult(NoneApplicativeMapping);
}

file static class TestFunctions
{
    public static DummyNewValue Mapping(DummyValue value)
    {
        return new DummyNewValue { NameAllCaps = value.Name.ToUpper() };
    }
}
