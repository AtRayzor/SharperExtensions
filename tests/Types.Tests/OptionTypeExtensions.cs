using DotNetCoreFunctional.Option;
using FluentAssertions;
using Xunit;

namespace NetFunction.Types.Tests;

public sealed record TestRecord(string Value)
{ 
    public static implicit operator string?(TestRecord? testRecord) => testRecord?.Value;
    public static implicit operator int(TestRecord? testRecord) => testRecord?.Value.Length ?? 0;
}

public class OptionTypeExtensions
{
    [Fact]
    public void AsOptionOfTest()
    {
        Option<string> expected = new Some<string>("just testing");
        ((TestRecord?)new TestRecord("just testing"))
            .CastToOption<TestRecord?, string>()
            .Should()
            .BeEquivalentTo(expected);
    }
}