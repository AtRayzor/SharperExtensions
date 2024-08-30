using System.Threading.Tasks;
using DotNetCoreFunctional.UnionTypes.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace DotNetCoreFunctional.Analyzers.Tests;

public class CloseUnionTypeDeclarationAnalyzerTests
{
    [Fact]
    public async Task TestNoDiagnostic()
    {
        var context = TestContextFactory.CreateContext<ClosedUnionTypeDeclarationAnalyzer>();
        context
            .TestState
            .Sources
            .Add(
                (
                    "ClosedTestType.cs",
                    SourceTextFactory.CreateSourceText("../../../TestSources/ClosedTestType.cs")
                )
            );
        context.CompilerDiagnostics = CompilerDiagnostics.Errors;

        await context.RunAsync();
    }

    [Fact]
    public async Task TestDiagnostic()
    {
        var context = TestContextFactory.CreateContext<ClosedUnionTypeDeclarationAnalyzer>();
        context
            .TestState
            .Sources
            .Add(
                (
                    "NonAbstractClosedDeclaration.cs",
                    SourceTextFactory.CreateSourceText(
                        "../../../TestSources/NonAbstractClosedDeclaration.cs"
                    )
                )
            );
        context.CompilerDiagnostics = CompilerDiagnostics.Errors;

        context
            .ExpectedDiagnostics
            .Add(
                new DiagnosticResult(ClosedUnionTypeDeclarationAnalyzer.Rule)
                    .WithArguments(
                        "record",
                        "DotNetCoreFunctional.Analyzers.Tests.TestSources.NonAbstractClosedDeclaration"
                    )
                    .WithLocation("NonAbstractClosedDeclaration.cs", 5, 1)
                    .WithSeverity(DiagnosticSeverity.Error)
            );

        await context.RunAsync();
    }
}
