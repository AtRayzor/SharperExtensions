using FluentAssertions;
using Monads.OptionKind;
using Monads.Tests.DummyTypes;
using Monads.Traits;

namespace Monads.Tests;

public class OptionMonadTests
{
    private static DummyValue Value => new() { Name = "Jack Black", Email = "jack.black@example.com" };
    private static DummyNewValue NewValue => new() { NameAllCaps = "JACK BLACK" };
    private static OptionType<DummyValue>.Some SomeValue => new(Value);
    private static OptionType<DummyNewValue>.Some SomeNewValue => new(NewValue);

    private IMonad<OptionType<DummyValue>> SomeOptionMonad =>
        new Monad<OptionType<DummyValue>>(SomeValue);

    private IMonad<OptionType<DummyValue>> NoneOptionMonad =>
        new Monad<OptionType<DummyValue>>(new OptionType<DummyValue>.None());

    [Fact]
    public void Create_CallWithNonNullValue_ReturnsSome()
    {
        var optionMonad = Monad.Option.Create(Value);

        optionMonad.Should().BeAssignableTo<IMonad<OptionType<DummyValue>>>();
        optionMonad.Type.Should().BeEquivalentTo(SomeValue);
    }

    [Fact]
    public void Create_CallWithNullValue_ReturnsNone()
    {
        DummyValue? nullableValue = default;
        var optionMonad = Monad.Option.Create(nullableValue);


        optionMonad.Should().BeAssignableTo<IMonad<OptionType<DummyValue>>>();
        optionMonad.Type.Should().BeOfType<OptionType<DummyValue>.None>();
    }

    [Fact]
    public void Some_CallWithValue_ReturnsSome()
    {
        var optionMonad = Monad.Option.Some(Value);

        optionMonad.Should().BeAssignableTo<IMonad<OptionType<DummyValue>>>();
        optionMonad.Type.Should().BeEquivalentTo(SomeValue);
    }

    [Fact]
    public void None_CallMethod_ReturnsNone()
    {
        var optionMonad = Monad.Option.None<DummyValue>();

        optionMonad.Should().BeAssignableTo<IMonad<OptionType<DummyValue>>>();
        optionMonad.Type.Should().BeOfType<OptionType<DummyValue>.None>();
    }

    [Fact]
    public void Bind_CallOnSome_ReturnsNewSomeValue()
    {
        var expected = new OptionType<DummyNewValue>.Some(NewValue);

        var newOptionMonad = SomeOptionMonad.Bind(value =>
            new Monad<OptionType<DummyNewValue>>(new DummyNewValue { NameAllCaps = value.Name.ToUpper() }));

        newOptionMonad.Type.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Bind_CallOnNone_ReturnsNoneValue()
    {
        var newOptionMonad = NoneOptionMonad.Bind(value =>
            new Monad<OptionType<DummyNewValue>>(new DummyNewValue { NameAllCaps = value.Name.ToUpper() }));

        newOptionMonad.Type.Should().BeOfType<OptionType<DummyNewValue>.None>();
    }

    [Fact]
    public async Task BindAsync_CallOnSome_ReturnsNewSomeValue()
    {
        var expected = new OptionType<DummyNewValue>.Some(NewValue);
        var optionMonadTask = Task.FromResult(SomeOptionMonad);
        
        var newOptionMonad = await optionMonadTask.BindAsync(value =>
            Task.FromResult((IMonad<OptionType<DummyNewValue>>)new Monad<OptionType<DummyNewValue>>(
                new DummyNewValue
                    { NameAllCaps = value.Name.ToUpper() })));

        newOptionMonad.Type.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task BindAsync_CallOnNone_ReturnsNoneValue()
    {
        var optionMonadTask = Task.FromResult(NoneOptionMonad);
        
        var newOptionMonad = await optionMonadTask.BindAsync(value =>
            Task.FromResult((IMonad<OptionType<DummyNewValue>>)new Monad<OptionType<DummyNewValue>>(
                new DummyNewValue
                    { NameAllCaps = value.Name.ToUpper() })));

        newOptionMonad.Type.Should().BeOfType<OptionType<DummyNewValue>.None>();
    }
}