namespace SharperExtensions.Core.Tests;

internal static class OptionTestData
{
    public static readonly DummyValue Value = new()
    {
        Name = "Jack Black",
        Email = "jack.black@example.com",
    };

    public static readonly DummyNewValue NewValue = new() { NameAllCaps = "JACK BLACK" };
    public static Option<DummyValue> SomeValue => Option<DummyValue>.Some(Value);
    public static Option<DummyValue> NoneValue => Option<DummyValue>.None;

    public static Option<DummyNewValue> SomeNewValue =>
        Option<DummyNewValue>.Some(NewValue);

    public static Option<DummyNewValue> NoneNewValue => Option<DummyNewValue>.None;
}