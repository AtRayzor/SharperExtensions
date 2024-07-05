namespace Monads.Tests.DummyTypes;

public class DummyNewerError
{
    public required string Message { get; init; }
    public bool IsCritical { get; init; }
}