namespace SharperExtensions.Analyzers.Tests.TestSources;

[Closed]
public abstract record InvalidClosedTestType;

public record ValidCase1 : InvalidClosedTestType;

public record ValidCase2 : InvalidClosedTestType;
