using DotNetCoreFunctional.UnionTypes;

namespace DotNetCoreFunctional.Analyzers.Tests.TestSources;

[Closed]
public record GenericClosedTestType<T>
    where T : notnull;

public record Case1<T>(T Value) : GenericClosedTestType<T>
    where T : notnull;

public record Case2<T>(IEnumerable<T> Values) : GenericClosedTestType<T>
    where T : notnull;

public record Case3<T>(T Value, int Count) : GenericClosedTestType<T>
    where T : notnull;
