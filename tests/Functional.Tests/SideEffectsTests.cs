using FluentAssertions;
using Monads.OptionKind;
using Monads.Tests.DummyTypes;
using Monads.Traits;

namespace Monads.Tests;

public class SideEffectsTests
{
    private static DummyValue Value => new() { Name = "Jack Black", Email = "jack.black@example.com" };
    private static DummyNewValue NewValue => new() { NameAllCaps = "JACK BLACK" };
    private static OptionType<DummyValue>.Some SomeValue => new(Value);
    private static OptionType<DummyNewValue>.Some SomeNewValue => new(NewValue);

    private IOptionWithSideEffects<DummyValue> SomeOptionWithSideEffects =>
        new OptionWithSideEffects<DummyValue>(SomeValue);

    private IOptionWithSideEffects<DummyValue> NoneOptionWithSideEffects =>
        new OptionWithSideEffects<DummyValue>(new OptionType<DummyValue>.None());

    [Fact]
    public void ThrowIfNone_CallOnSomeOption_DoesNotThrow()
    {
        SomeOptionWithSideEffects.Invoking(o => o.ThrowIfNone<DummyValue, InvalidDataException>())
            .Should().NotThrow<InvalidDataException>();
    }

    [Fact]
    public void ThrowIfNone_CallOnNoneOptionWithMessage_ThrowsException()
    {
        NoneOptionWithSideEffects.Invoking(o => o.ThrowIfNone<DummyValue, InvalidDataException>("exception thrown"))
            .Should()
            .Throw<InvalidDataException>()
            .WithMessage("exception thrown");
    }

    [Fact]
    public void GetValueOrThrow_CallOnSomeOption_DoesNotThrowButReturnsValue()
    {
        SomeOptionWithSideEffects.Invoking(
                o =>
                    o.GetValueOrThrow<DummyValue, InvalidDataException>()
                        .Should().BeEquivalentTo(SomeValue))
            .Should().NotThrow<InvalidDataException>();
    }

    [Fact]
    public void GetValueOrThrow_CallOnNone_ThrowsException()
    {
        NoneOptionWithSideEffects.Invoking(
                o =>
                    o.GetValueOrThrow<DummyValue, InvalidDataException>())
            .Should().Throw<InvalidDataException>();
    }

    [Fact]
    public void DoOrThrow_CalledOnSome_ActionExecuted()
    {
        SomeOptionWithSideEffects.Invoking(
                o =>
                {
                    string? nullableName = default;
                    o.DoOrThrow<DummyValue, InvalidDataException>(
                        value => nullableName = value.Name
                    );
                    nullableName.Should().Be("Jack Black");
                }
            )
            .Should().NotThrow<InvalidDataException>();
    }
    
    [Fact]
    public void DoOrThrow_CalledOnNone_ThrowsException()
    {
        NoneOptionWithSideEffects.Invoking(
                o =>
                {
                    string? nullableName = default;
                    o.DoOrThrow<DummyValue, InvalidDataException>(
                        value => nullableName = value.Name
                    );
                    nullableName.Should().BeNull();
                }
            )
            .Should().Throw<InvalidDataException>();
    }
}