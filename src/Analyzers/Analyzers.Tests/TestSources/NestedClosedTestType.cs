using DotNetCoreFunctional.UnionTypes;

namespace DotNetCoreFunctional.Analyzers.Tests.TestSources;

[Closed]
public abstract record NestedClosedTestType
{
    public record Case1(string Message, int Value) : NestedClosedTestType;

    public record Case2(string Message, bool Flag) : NestedClosedTestType;

    public record Case3(string Message) : NestedClosedTestType;
}
