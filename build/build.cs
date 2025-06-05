#!/usr/bin/dotnet run

#:property Nullable enable
#:property TreatWarningsAsErrors true
#:property LangVersion preview
#:package System.CommandLine@2.*-beta4.*

using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

var state = new State();
var handlers = new UiEventHandlers(state);

Cli.Command.Add(Cli.VersionArgument);
Cli.Command.Add(Cli.SuffixOption);
Cli.Command.Add(Cli.BuildOption);
Cli.Command.Add(Cli.KeyOption);
Cli.Command.Add(Cli.pushOption);
Cli.Command.SetHandler(handlers.InvocationHandler);

await Cli.Command.InvokeAsync(args);
await handlers.WaitForCompletion();

return state.ExitCode;

static class Cli
{
    public static RootCommand Command { get; } =
        new RootCommand("A simple test runner for dotnet.");
    public static Option<string?> SuffixOption { get; } = new Option<string?>(
        aliases: ["--suffix", "-s"],
        "The suffix to be added to the base package name for the package build."
    );
    public static Argument<string> VersionArgument { get; } =
        new Argument<string>("package version", "The package version.");
    public static Option<bool> BuildOption { get; } =
        new Option<bool>(aliases: ["--build", "-b"], "Whether to build the assemblies.");
    public static Option<string?> KeyOption { get; } =
        new Option<string?>(aliases: ["--key", "-k"], "A valid nuget.org API key.");
    public static Option<bool> pushOption = new Option<bool>(
        aliases: ["--push", "-p"],
        "Whether the generated package should be pushed to the Nuget repository."
    );

}


static partial class RegularExpressions
{
    [GeneratedRegex(@"^\w[\w\d]+(?:\.(?!nuspec)[\w\d]+)*$", RegexOptions.Singleline)]
    public static partial Regex FileRegex { get; }
}

sealed class UiEventHandlers(State state)
{
    private TaskCompletionSource Tcs =>
        field ??= new TaskCompletionSource();


    public void InvocationHandler(InvocationContext context)
    {
        var suffixOption = context.ParseResult.GetValueForOption(Cli.SuffixOption);
        var version = context.ParseResult.GetValueForArgument(Cli.VersionArgument);
        var shouldBuild = context.ParseResult.GetValueForOption(Cli.BuildOption);
        var shouldPush = context.ParseResult.GetValueForOption(Cli.pushOption);
        var key = context.ParseResult.GetValueForOption(Cli.KeyOption);

        Console.WriteLine(version);

        if (!TryGetPackageInfo(suffixOption, out var fileName, out var assemblyPaths))
        {
            OnError($"Invalid nuspec file suffix \"{suffixOption}\"");

            return;
        }

        var localDir = Environment.CurrentDirectory;
        List<string> commandsToRun =
        [
            $"cd {localDir}",
    ];

        if (shouldBuild)
        {
            commandsToRun.AddRange(assemblyPaths.Select(p => $"dotnet build {p} -c Release;"));
        }

        var commandBuilder = new StringBuilder();
        commandBuilder.Append("nuget pack ");
        commandBuilder.Append($"./{fileName} ");
        commandBuilder.Append($"-OutputDirectory ../packages/{version}");
        var nugetCommand = commandBuilder.ToString();

        if (shouldPush)
        {
            if (key is null)
            {
                OnError("An API key is required when the push option is passed.");
                return;
            }

            var suffix = suffixOption is not null ? $".{suffixOption}" : string.Empty;
            var packagePath = $"SharperExtensions{suffix}.{version}.nupkg";
            var command = $"cd ../packages/{version};nuget push {packagePath} -Source https://api.nuget.org/v3/index.json -ApiKey {key}";
            commandsToRun.Add(command);
        }

        commandsToRun.Add(nugetCommand);
        using var enumerator = commandsToRun.GetEnumerator();

        while (state.ExitCode is 0 && enumerator.MoveNext())
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Running command \"{enumerator.Current}\"");
            RunCommand(enumerator.Current);

            if (state.ExitCode is not 0)
            {
                return;
            }
        }

        Console.ResetColor();

        Tcs.SetResult();
    }

    private bool TryGetPackageInfo(
           string? nuspecArg,
           [MaybeNullWhen(returnValue: false)] out string? fileName,
           [MaybeNullWhen(returnValue: false)] out string[] assemblyPaths
       )
    {
        const string fileNameBase = "SharperExtensions";
        var srcDir = $"{Environment.CurrentDirectory}/../src";
        var analyzersAssemblyPath = $"{srcDir}/Analyzers/Analyzers.csproj";
        var coreAssemblyPath = $"{srcDir}/Core/Core.csproj";
        var collectionsAssemblyPath = $"{srcDir}/Collections/Collections.csproj";
        var asyncAssemblyPath = $"{srcDir}/Async/Async.csproj";
        string[] corePaths = [analyzersAssemblyPath, coreAssemblyPath];

        fileName = null;
        assemblyPaths = null;


        switch (nuspecArg)
        {
            case null:
                {
                    assemblyPaths = [.. corePaths, collectionsAssemblyPath, asyncAssemblyPath];
                    break;
                }
            case "Core":
                {
                    assemblyPaths = corePaths;

                    break;
                }
            case "Collections":
                {
                    assemblyPaths = [.. corePaths, collectionsAssemblyPath];

                    break;
                }
            case "Async":
                {
                    assemblyPaths = [.. corePaths, collectionsAssemblyPath, asyncAssemblyPath];
                    break;
                }
            default:
                {
                    return false;
                }
        }

        if (nuspecArg is null)
        {
            fileName = $"{fileNameBase}.nuspec";

            return true;

        }

        var match = RegularExpressions.FileRegex.Match(nuspecArg);

        if (match is not { Value: { Length: > 0 } fileSuffix })
        {
            fileName = match.Value;
            return false;
        }

        fileName = $"{fileNameBase}.{fileSuffix}.nuspec";

        return true;
    }

    private void PushPackage(string packagePath, string key)
    {
      
    }

    private void RunCommand(string command)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = startInfo, };
        process.OutputDataReceived += OnOutput;
        process.ErrorDataReceived += OnError;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();
        state.ExitCode = process.ExitCode;
        process.Close();
        process.OutputDataReceived -= OnOutput;
        process.ErrorDataReceived -= OnError;

    }

    public void OnOutput(object? _, DataReceivedEventArgs eventArgs) =>
        OnOutput(eventArgs.Data);

    public void OnOutput(string? stdOut)
    {
        if (stdOut is null)
        {
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(stdOut);
        Console.ResetColor();
    }

    public void OnError(object? _, DataReceivedEventArgs eventArgs) =>
        OnError(eventArgs.Data);

    public void OnError(string? stdErr)
    {
        if (stdErr is null)
        {
            return;
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(stdErr);
        Console.ResetColor();

        state.ExitCode = 1;
        Tcs.SetResult();
    }

    public Task WaitForCompletion()
    {
        return Tcs.Task;
    }
}

sealed class State
{
    public int ExitCode { get; set; }
}