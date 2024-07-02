using FluentAssertions;
using Monads.OptionMonad;

namespace Monads.Tests.DummyTypes;

public class OptionMonadLawTests
{
    private static readonly DummyValue DummyValue = new() { Name = "Jack Black", Email = "jack.black@example.com" };
    private static Option<DummyNewValue> ValueToNewValueSomeOption(DummyValue value) =>
        new DummyNewValue { NameAllCaps = value.Name.ToUpper() };

    private static Option<DummyNewerValue> NewValueToNewerValueSomeOption(DummyNewValue value) =>
        new DummyNewerValue { NameLowercase = value.NameAllCaps.ToLower() };

    private static Option<DummyNewValue> NoValueToNewValueNoneOption() => new Option<DummyNewValue>.None();
    private static Option<DummyNewerValue> NoValueToNewerValueNoneOption() => new Option<DummyNewerValue>.None();


    [Fact]
    public void Associative_SomeOptionInput()
    {
        Option<DummyValue> option = DummyValue;
        var mappedLeftFirst = option
            .Map(ValueToNewValueSomeOption)
            .Map(NewValueToNewerValueSomeOption);

        var mappedRightFirst =
            option.Map(value => ValueToNewValueSomeOption(value).Map(NewValueToNewerValueSomeOption));

        mappedLeftFirst.Should().BeEquivalentTo((Option<DummyNewerValue>.Some)mappedRightFirst);
    }

    [Fact]
    public void Associative_NoneOptionInput()
    {
        Option<DummyValue> option = new Option<DummyValue>.None();
        var mappedLeftFirst = option
            .Map(ValueToNewValueSomeOption, NoValueToNewValueNoneOption)
            .Map(NewValueToNewerValueSomeOption, NoValueToNewerValueNoneOption);

        var mappedRightFirst =
            option.Map(value => ValueToNewValueSomeOption(value)
                    .Map(NewValueToNewerValueSomeOption),
                () => NoValueToNewValueNoneOption().Map(
                    NewValueToNewerValueSomeOption,
                    NoValueToNewerValueNoneOption
                )
            );

        mappedLeftFirst.Should().BeOfType<Option<DummyNewerValue>.None>();
        mappedRightFirst.Should().BeOfType<Option<DummyNewerValue>.None>();
    }

    [Fact]
    public void LeftIdentity()
    {
        Option.Some(DummyValue)
            .Map(ValueToNewValueSomeOption)
            .Should()
            .BeEquivalentTo(
                (Option<DummyNewValue>.Some)ValueToNewValueSomeOption(DummyValue)
            );
    }
    
    [Fact]
    public void RightIdentity()
    {
        Option<DummyValue> option = DummyValue;
        
            option.Map(Option.Some)
            .Should()
            .BeEquivalentTo((Option<DummyValue>.Some)option);
    }
}