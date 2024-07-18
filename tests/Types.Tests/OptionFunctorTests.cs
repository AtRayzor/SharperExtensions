using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using NetFunctional.Types.OptionKind;
using NetFunctional.Types.Traits;
using Functor = NetFunctional.Types.Traits.Functor;

namespace NetFunction.Types.Tests;

public class OptionFunctorTests
{
    private static DummyValue Value =>
        new() { Name = "Jack Black", Email = "jack.black@example.com" };

    private static DummyNewValue NewValue => new() { NameAllCaps = "JACK BLACK" };
    private static OptionType<DummyValue>.Some SomeValue => new(Value);
    private static OptionType<DummyNewValue>.Some SomeNewValue => new(NewValue);

    private IFunctor<OptionType<DummyValue>> SomeOptionFunctor =>
        new Functor<OptionType<DummyValue>>(SomeValue);

    private IFunctor<OptionType<DummyValue>> NoneOptionFunctor =>
        new Functor<OptionType<DummyValue>>(new OptionType<DummyValue>.None());

    [Fact]
    public void Create_PassNonNullValue_SomeOptionsFunctorCreated()
    {
        var optionFunctor = Functor.Option.Create(Value);

        optionFunctor.Should().BeAssignableTo<IFunctor<OptionType<DummyValue>>>();
        optionFunctor.Type.Should().BeEquivalentTo(SomeValue);
    }

    [Fact]
    public void Create_PassNullValue_NoneOptionsFunctorCreated()
    {
        DummyValue? nullableValue = default;
        var optionFunctor = Functor.Option.Create(nullableValue);

        optionFunctor.Should().BeAssignableTo<IFunctor<OptionType<DummyValue>>>();
        optionFunctor.Type.Should().BeOfType<OptionType<DummyValue>.None>();
    }

    [Fact]
    public void Some_PassValue_SomeOptionsFunctorCreated()
    {
        var optionFunctor = Functor.Option.Some(Value);

        optionFunctor.Should().BeAssignableTo<IFunctor<OptionType<DummyValue>>>();
        optionFunctor.Type.Should().BeEquivalentTo(SomeValue);
    }

    [Fact]
    public void None_CallMethod_NoneOptionsFunctorCreated()
    {
        DummyValue? nullableValue = default;
        var optionFunctor = Functor.Option.Create(nullableValue);

        optionFunctor.Should().BeAssignableTo<IFunctor<OptionType<DummyValue>>>();
        optionFunctor.Type.Should().BeOfType<OptionType<DummyValue>.None>();
    }

    [Fact]
    public void Map_InputSomeOption_ReturnNewSomeOption()
    {
        var newOptionFunctor = SomeOptionFunctor.Map(
            value => new DummyNewValue { NameAllCaps = value.Name.ToUpper() }
        );

        newOptionFunctor.Should().BeAssignableTo<IFunctor<OptionType<DummyNewValue>>>();
        newOptionFunctor.Type.Should().BeEquivalentTo(SomeNewValue);
    }

    [Fact]
    public void Map_InputNoneOption_ReturnNewNoneOption()
    {
        var newOptionFunctor = NoneOptionFunctor.Map(
            value => new DummyNewValue { NameAllCaps = value.Name.ToUpper() }
        );

        newOptionFunctor.Should().BeAssignableTo<IFunctor<OptionType<DummyNewValue>>>();
        newOptionFunctor.Type.Should().BeOfType<OptionType<DummyNewValue>.None>();
    }

    [Fact]
    public async Task MapAsync_InputSomeOptionWithSynchronousLambda_ReturnNewSomeOption()
    {
        var optionFunctorTask = Task.FromResult(SomeOptionFunctor);
        var newOptionFunctor = await optionFunctorTask.MapAsync(
            value => new DummyNewValue { NameAllCaps = value.Name.ToUpper() }
        );

        newOptionFunctor.Should().BeAssignableTo<IFunctor<OptionType<DummyNewValue>>>();
        newOptionFunctor.Type.Should().BeEquivalentTo(SomeNewValue);
    }

    [Fact]
    public async Task MapAsync_InputNoneOption_ReturnNewNoneOption()
    {
        var optionFunctorTask = Task.FromResult(NoneOptionFunctor);

        var newOptionFunctor = await optionFunctorTask.MapAsync(
            value => new DummyNewValue { NameAllCaps = value.Name.ToUpper() }
        );

        newOptionFunctor.Should().BeAssignableTo<IFunctor<OptionType<DummyNewValue>>>();
        newOptionFunctor.Type.Should().BeOfType<OptionType<DummyNewValue>.None>();
    }

    [Fact]
    public async Task MapAsync_InputSomeOptionWithAsyncLambda_ReturnNewSomeOption()
    {
        var optionFunctorTask = Task.FromResult(SomeOptionFunctor);
        var newOptionFunctor = await optionFunctorTask.MapAsync(
            value => Task.FromResult(new DummyNewValue { NameAllCaps = value.Name.ToUpper() })
        );

        newOptionFunctor.Should().BeAssignableTo<IFunctor<OptionType<DummyNewValue>>>();
        newOptionFunctor.Type.Should().BeEquivalentTo(SomeNewValue);
    }

    [Fact]
    public async Task MapAsync_InputNoneOptionWithAsyncLambda_ReturnNewNoneOption()
    {
        var optionFunctorTask = Task.FromResult(NoneOptionFunctor);

        var newOptionFunctor = await optionFunctorTask.MapAsync(
            value => Task.FromResult(new DummyNewValue { NameAllCaps = value.Name.ToUpper() })
        );

        newOptionFunctor.Should().BeAssignableTo<IFunctor<OptionType<DummyNewValue>>>();
        newOptionFunctor.Type.Should().BeOfType<OptionType<DummyNewValue>.None>();
    }
}
