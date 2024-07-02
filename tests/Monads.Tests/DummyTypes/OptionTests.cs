using FluentAssertions;
using Monads.OptionMonad;

namespace Monads.Tests.DummyTypes;

public class OptionTests
{
    private static readonly DummyValue DummyValue = new() { Name = "Jack Black", Email = "jack.black@example.com" };

    private static readonly DummyNewValue DummyNewValue = new() { NameAllCaps = "JACK BLACK" };
    private static readonly DummyNewValue AltDummyNewValue = new() { NameAllCaps = "JIM SLIM" };

    private static Option<DummyNewValue> ValueToNewValueSomeOption(DummyValue value) =>
        new DummyNewValue { NameAllCaps = value.Name.ToUpper() };

    private static Option<DummyNewValue> ValueToNewValueNoneOption(DummyValue value) =>
        new Option<DummyNewValue>.None();

    private static Option<DummyNewValue> NoValueToNewValueSomeOption() =>
        AltDummyNewValue;

    private static Option<DummyNewValue> NoValueToNewValueNoneOption() => new Option<DummyNewValue>.None();

    [Fact]
    public void Some_CallMethod_ReturnsSomeOption()
    {
        var expected = new Option<DummyValue>.Some(DummyValue);

        Option.Some(DummyValue).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void None_CallMethod_ReturnsNoneOption()
    {
        Option.None<DummyValue>().Should().BeOfType<Option<DummyValue>.None>();
    }

    [Fact]
    public void ImplicitOperator_ApplyCastToNonNullValue_CastsToSomeOption()
    {
        var expected = new Option<DummyValue>.Some(DummyValue);

        Option<DummyValue> value = DummyValue;

        value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ImplicitOperator_ApplyCastToNullValue_CastsToNoneOption()
    {
        DummyValue? dummy = default;
        Option<DummyValue> value = dummy;

        value.Should().BeOfType<Option<DummyValue>.None>();
    }

    [Fact]
    public void Map_CallOnSomeOption_ReturnsNewSomeOption()
    {
        Option<DummyNewValue>.Some expected = DummyNewValue;

        Option<DummyValue> option = DummyValue;
        option.Map(ValueToNewValueSomeOption)
            .Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_CallOnSomeOption_ReturnsNewNoneOption()
    {
        Option<DummyValue> option = DummyValue;
        option.Map(ValueToNewValueNoneOption)
            .Should().BeOfType<Option<DummyNewValue>.None>();
    }

    [Fact]
    public void Map_CallOnNone_ReturnsNewSomeOption()
    {
        Option<DummyNewValue>.Some expected = AltDummyNewValue;

        var option = new Option<DummyValue>.None();
        option.Map(ValueToNewValueSomeOption, NoValueToNewValueSomeOption)
            .Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_CallOnNone_ReturnsNewNoneOption()
    {
        var option = new Option<DummyValue>.None();
        option.Map(ValueToNewValueSomeOption, NoValueToNewValueNoneOption)
            .Should().BeOfType<Option<DummyNewValue>.None>();
    }

    [Fact]
    public void GetValueIfSome_CallOnSome_OutputsValue()
    {
        Option<DummyValue> option = DummyValue;
        option.GetValueIfSome(out var value).Should().BeTrue();
        value.Should().BeEquivalentTo(DummyValue);
    }

    [Fact]
    public void ThrowIfNone_CallOnSome_DoesNotThrow()
    {
        Option<DummyValue> option = DummyValue;
        option.Invoking(o => o.ThrowIfNone<InvalidDataException>())
            .Should()
            .NotThrow<InvalidDataException>();
    }

    [Fact]
    public void ThrowIfNone_CallOnNone_ThrowsException()
    {
        var message = "exception was thrown";
        var option = new Option<DummyValue>.None();
        option.Invoking(o => o.ThrowIfNone<InvalidDataException>(message))
            .Should()
            .Throw<InvalidDataException>()
            .WithMessage(message);
    }

    [Fact]
    public void ExecuteIfSome_CallOnSome_ExecutesAction()
    {
        string? name = default;
        Option<DummyValue> option = DummyValue;
        option.ExecuteIfSome(value => { name = value.Name; });
        name.Should().Be("Jack Black");
    }

    [Fact]
    public void ExecuteIfSome_CallOnNone_DoesNotExecuteAction()
    {
        string? name = default;
        Option<DummyValue> option = new Option<DummyValue>.None();
        option.ExecuteIfSome(value => { name = value.Name; });
        name.Should().BeNull();
    }

    [Fact]
    public void ExecuteActionOrThrow_CallOnSome_ExecutesAction()
    {
        Option<DummyValue> option = DummyValue;
        option.Invoking(o =>
            {
                string? name = default;
                o.ExecuteOrThrow<InvalidDataException>(value => { name = value.Name; });
                name.Should().Be("Jack Black");
            })
            .Should()
            .NotThrow<InvalidDataException>();
    }

    [Fact]
    public void ExecuteActionOrThrow_CallOnSome_ThrowsException()
    {
        var message = "exception was thrown";
        Option<DummyValue> option = new Option<DummyValue>.None();
        option.Invoking(o => { o.ExecuteOrThrow<InvalidDataException>(value => { }, message); })
            .Should()
            .Throw<InvalidDataException>()
            .WithMessage(message);
    }
}