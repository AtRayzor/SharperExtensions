using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using NetFunction.Types.Tests.DummyTypes;
using NetFunctional.Types;

namespace NetFunction.Types.Tests;

public static class ResultTestMethods
{
    public static DummyNewValue TestMapping(DummyValue value) => new() { NameAllCaps = value.Name.ToUpper() };

    public static DummyNewError TestErrorMapping(DummyError value) =>
        new() { Message = $"new {value.Message}", Count = 2 };

    public static Result<DummyNewValue, DummyError> TestBinder(DummyValue value)
        => new Ok<DummyNewValue, DummyError>(
            new DummyNewValue { NameAllCaps = value.Name.ToUpper() }
        );

    public static Task<Result<DummyNewValue, DummyError>> AsyncTestBinder(DummyValue value)
        => Task.FromResult<Result<DummyNewValue, DummyError>>(
            new Ok<DummyNewValue, DummyError>(
                new DummyNewValue { NameAllCaps = value.Name.ToUpper() }
            ));
}