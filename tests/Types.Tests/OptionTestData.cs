using DotNetCoreFunctional.Option;
using NetFunction.Types.Tests.DummyTypes;

namespace NetFunction.Types.Tests;

internal static class OptionTestData
{
    public static readonly DummyValue Value =
        new() { Name = "Jack Black", Email = "jack.black@example.com" };

    public static readonly DummyNewValue NewValue = new() { NameAllCaps = "JACK BLACK" };
    public static Some<DummyValue> SomeValue => new(Value);
    public static None<DummyValue> NoneValue => new();
    public static Some<DummyNewValue> SomeNewValue => new(NewValue);
    public static None<DummyNewValue> NoneNewValue => new();
}
