using FluentAssertions;
using Monads.OptionKind;
using Monads.Tests.DummyTypes;
using Monads.Traits;

namespace Monads.Tests;

public class ConstructOptionsTest
{
    private static DummyValue Value => new() { Name = "Jack Black", Email = "jack.black@example.com" };
    public static DummyNewValue? NullValue => null;
    private static OptionType<DummyValue>.Some SomeValue => new(Value);


    [Fact]
    public static void FunctorCreate_CallWithNonNullValue_ReturnsSome()
    {
        var expected = new Functor<OptionType<DummyValue>>(SomeValue);
        Functor.Option.Create(Value).Should().BeEquivalentTo(expected);
    }


    [Fact]
    public static void FunctorCreate_CallWithNullValue_ReturnsNone()
    {
        var expected = new Functor<OptionType<DummyValue>>(new OptionType<DummyValue>.None());
        Functor.Option.Create(NullValue).Should().BeEquivalentTo(expected);
    }
    
    
    [Fact]
    public static void MonadCreate_CallWithNonNullValue_ReturnsSome()
    {
        var expected = new Monad<OptionType<DummyValue>>(SomeValue);
        Monad.Option.Create(Value).Should().BeEquivalentTo(expected);
    }


    [Fact]
    public static void MonadCreate_CallWithNullValue_ReturnsNone()
    {
        var expected = new Monad<OptionType<DummyValue>>(new OptionType<DummyValue>.None());
        Monad.Option.Create(NullValue).Should().BeEquivalentTo(expected);
    }
    
    
    [Fact]
    public static void OptionCreate_CallWithNonNullValue_ReturnsSome()
    {
        var expected = new Option<DummyValue>(SomeValue); 
        Option.Create(Value).Should().BeEquivalentTo(expected);
    }


    [Fact]
    public static void OptionCreate_CallWithNullValue_ReturnsNone()
    {
        var expected = new Option<DummyValue>(new OptionType<DummyValue>.None());
        Option.Create(NullValue).Should().BeEquivalentTo(expected);
    }
}