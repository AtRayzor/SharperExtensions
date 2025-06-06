using Microsoft.CodeAnalysis.Testing;

namespace SharperExtensions.Analyzers.Tests;

public class UnionParameterAnalyzerTests
{
    [Fact]
    public async Task ParameterUnionAnalyzer_ValidParams()
    {
        var context = TestContextFactory.CreateContext<UnionParameterAnalyzer>();
        context.TestState.Sources.Add(
            (
                "ValidParams.cs",
                SourceTextFactory.CreateSourceText(
                    "../../../TestSources/ParameterUnionValidParams.cs"
                )
            )
        );
        context.CompilerDiagnostics = CompilerDiagnostics.None;

        await context.RunAsync();
    }

    [Fact]
    public async Task ParameterUnionAnalyzer_InvalidParams()
    {
        var context = TestContextFactory.CreateContext<UnionParameterAnalyzer>();
        context.TestState.Sources.Add(
            (
                "ValidParams.cs",
                SourceTextFactory.CreateSourceText(
                    "../../../TestSources/ParameterUnionInvalidParams.cs"
                )
            )
        );
        context.ExpectedDiagnostics.Add(
            DiagnosticResult
                .CompilerError(UnionParameterAnalyzer.InvalidParamTypeDiagnosticId)
                .WithLocation("ValidParams.cs", 5, 45)
        );

        await context.RunAsync();
    }

    [Fact]
    public async Task ParameterUnionAnalyzer_ValidArguments()
    {
        var context = TestContextFactory.CreateContext<UnionParameterAnalyzer>();
        context.TestState.Sources.AddRange(
            [
                (
                    "ValidParams.cs",
                    SourceTextFactory.CreateSourceText(
                        "../../../TestSources/ParameterUnionValidParams.cs"
                    )
                ),
                (
                    "ValidArgs.cs",
                    SourceTextFactory.CreateSourceText(
                        "../../../TestSources/ParameterUnionValidArgs.cs"
                    )
                ),
            ]
        );
        context.CompilerDiagnostics = CompilerDiagnostics.None;

        await context.RunAsync();
    }

    [Fact]
    public async Task ParameterUnionAnalyzer_InvalidArguments()
    {
        var context = TestContextFactory.CreateContext<UnionParameterAnalyzer>();
        context.TestState.Sources.AddRange(
            [
                (
                    "ValidParams.cs",
                    SourceTextFactory.CreateSourceText(
                        "../../../TestSources/ParameterUnionValidParams.cs"
                    )
                ),
                (
                    "InvalidArgs.cs",
                    SourceTextFactory.CreateSourceText(
                        "../../../TestSources/ParameterUnionInvalidArgs.cs"
                    )
                ),
            ]
        );

        context.ExpectedDiagnostics.Add(
            new DiagnosticResult(UnionParameterAnalyzer.InvalidArgumentRule)
                .WithLocation("InvalidArgs.cs", 8, 32)
                .WithArguments("Boolean")
        );

        await context.RunAsync();
    }
}

file static class Setup { }
