namespace SharperExtensions.Analyzers.Tests.TestSources;

[Closed]
public abstract record ClosedTestType;

public record Animal(string Name) : ClosedTestType;

public record Number(int Value) : ClosedTestType;

public record Letter(char Value) : ClosedTestType;
