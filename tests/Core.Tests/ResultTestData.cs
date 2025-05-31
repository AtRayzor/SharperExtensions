namespace SharperExtensions.Core.Tests;

internal static class ResultTestData
{
    public static readonly DummyValue Value = new()
    {
        Name = "Jack Black",
        Email = "jack.black@example.com",
    };

    
    public static readonly DummyValue DefaultValue = new()
    {
        Name = "Jim Slim",
        Email = "jim.slim@example.com",
    };
    
    public static readonly DummyError Error = new() { Message = "error message" };
    public static readonly DummyError DefaultError = new() { Message = "default error message" };
    
    public static readonly DummyNewValue NewValue = new() { NameAllCaps = "JACK BLACK" };
    public static readonly DummyNewError NewError = new()
    {
        Message = "new error message",
        Count = 2,
    };
}
