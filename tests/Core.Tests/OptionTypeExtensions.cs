namespace SharperExtensions.Core.Tests;

public sealed record TestRecord(string Value)
{
    public static implicit operator string(TestRecord testRecord) => testRecord.Value;

    public static implicit operator int(TestRecord? testRecord) =>
        testRecord?.Value.Length ?? 0;
}

public class OptionTypeExtensions
{
    [Fact]
    public void AsOptionOfTest()
    {
        var expected = Option.Some<string>("just testing");
        var testRecordOption = Option.Return(new TestRecord("just testing"));

        Option
            .Cast<TestRecord, string>(testRecordOption)
            .Should()
            .BeEquivalentTo(expected);
    }
}