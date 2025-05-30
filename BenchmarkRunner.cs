#!/usr/bin/dotnet run

#:property TreateWarningsAsErrors true
#:package ./packages/1.0.0-alpha-0.0.1/Release/net10.0/Benchmarks.1.0.0-alpha.0.0.1.nupkg
#:package System.CommandLine@2.*-beta*

using System.CommandLine;
using System.CommandLine.Invocation;


var command = new RootCommand("A collection of benchmarks for DotNetCoreFunctional");
var benchmarkOption = new System.CommandLine.Option<string?>(
    aliases: ["--benchmark", "-b"],
    description: "The benchmark to run."
);
var benchmarkGroupOption = new System.CommandLine.Option<string?>(
    aliases: ["--group", "-g"],
    description: "The benchmark group to run."
);

command.AddOption(benchmarkOption);
command.SetHandler(BenchmarkOptionHandler);

return await command.InvokeAsync(args);

void BenchmarkOptionHandler(InvocationContext context)
{
    var values = new OptionValues
    {
        Benchmark = context.ParseResult.GetValueForOption(benchmarkOption),
        BenchmarkGroup = context.ParseResult.GetValueForOption(benchmarkGroupOption),
    };


    if (context.ParseResult.GetValueForOption(benchmarkOption) is not { } value)
    {
        throw new CommandLineConfigurationException(
            "No value was passed for benchmark option"
            );
    }

    Console.WriteLine($"The value passed for \"--benchmark\" is {value}.");
}


ref struct OptionValues
{
    public string? Benchmark { get; set; }
    public string? BenchmarkGroup { get; set; }
}