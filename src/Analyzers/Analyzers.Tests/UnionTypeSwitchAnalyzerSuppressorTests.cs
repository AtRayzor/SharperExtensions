using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Analyzers.Tests;

public class UnionTypeSwitchAnalyzerSuppressorTests
{
    [Fact]
    public async Task SuppressesWhenCasesExhaustive()
    {
        var context = TestContextFactory.CreateContext<UnionTypeSwitchExpressionSuppressor>();
        context
            .TestState
            .Sources
            .AddRange(
                [
                    (
                        "AllCasesSwitch.cs",
                        SourceTextFactory.CreateSourceText("../../../TestSources/AllCasesSwitch.cs")
                    ),
                    (
                        "ClosedTestType.cs",
                        SourceTextFactory.CreateSourceText("../../../TestSources/ClosedTestType.cs")
                    ),
                    (
                        "GenericClosedTestType.cs",
                        SourceTextFactory.CreateSourceText(
                            "../../../TestSources/GenericClosedTestType.cs"
                        )
                    )
                ]
            );

        context.CompilerDiagnostics = CompilerDiagnostics.Warnings;
        context.DisabledDiagnostics.AddRange(["CS1591"]);
        context
            .ExpectedDiagnostics
            .AddRange(
                [
                    DiagnosticResult
                        .CompilerWarning(UnionTypeSwitchExpressionSuppressor.SuppressedDiagnosticId)
                        .WithSpan("AllCasesSwitch.cs", 9, 25, 9, 31)
                        .WithArguments("_")
                        .WithIsSuppressed(true),
                    DiagnosticResult
                        .CompilerWarning(UnionTypeSwitchExpressionSuppressor.SuppressedDiagnosticId)
                        .WithSpan("AllCasesSwitch.cs", 19, 25, 19, 31)
                        .WithArguments("_")
                        .WithIsSuppressed(true)
                ]
            );

        await context.RunAsync();
    }
}
