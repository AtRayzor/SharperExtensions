using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using NetFunctional.Types;
using Xunit;

namespace NetFunction.Types.Tests;

public class OptionExceptionTests
{
    private const string ExceptionMessage = "error message";

    [Fact]
    public void GetValueOrThrow_CallWithSome_ReturnValueDoesNotThrow()
    {
        OptionTestData
            .SomeValue
            .Invoking(
                sv =>
                    sv.GetValueOrThrow<DummyValue, InvalidDataException>(ExceptionMessage)
                        .Should()
                        .BeEquivalentTo(OptionTestData.Value)
            )
            .Should()
            .NotThrow<InvalidDataException>();
    }

    [Fact]
    public void GetValueOrThrow_CallWithNone_ThrowsException()
    {
        OptionTestData
            .NoneValue
            .Invoking(
                sv =>
                    sv.GetValueOrThrow<DummyValue, InvalidDataException>(ExceptionMessage)
                        .Should()
                        .BeEquivalentTo(OptionTestData.Value)
            )
            .Should()
            .Throw<InvalidDataException>()
            .WithMessage(ExceptionMessage);
    }
}
