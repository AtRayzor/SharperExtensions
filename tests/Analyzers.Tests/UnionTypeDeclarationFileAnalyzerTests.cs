using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using SharperExtensions.Analyzers.Tests.TestSources;

namespace SharperExtensions.Analyzers.Tests;

public class UnionTypeDeclarationFileAnalyzerTests
{
    private static CSharpAnalyzerTest<
        UnionTypeDeclarationFileAnalyzer,
        DefaultVerifier
    > CreateContext()
    {
        return new CSharpAnalyzerTest<UnionTypeDeclarationFileAnalyzer, DefaultVerifier>
        {
            TestState =
            {
                Sources =
                {
                    (
                        "Closed.cs",
                        SourceTextFactory.CreateSourceText("../../../../../src/Core/Closed.cs")
                    ),
                    (
                        "InvalidClosedCaseTestType.cs",
                        SourceTextFactory.CreateSourceText(
                            "../../../TestSources/InvalidClosedTestType.cs"
                        )
                    ),
                    (
                        "InvalidCase.cs",
                        SourceTextFactory.CreateSourceText("../../../TestSources/InvalidCase.cs")
                    ),
                    (
                        "GlobalUsings",
                        SourceTextFactory.CreateSourceText("../../../TestSources/GlobalUsings.cs")
                    ),
                },
            },
        };
    }

    [Fact]
    public async Task Test()
    {
        var context = CreateContext();
        context.CompilerDiagnostics = CompilerDiagnostics.Errors;
        context.ExpectedDiagnostics.Add(
            new DiagnosticResult(UnionTypeDeclarationFileAnalyzer.Rule)
                .WithArguments(nameof(InvalidCase), nameof(InvalidClosedTestType))
                .WithLocation("InvalidCase.cs", 3, 1)
                .WithSeverity(DiagnosticSeverity.Error)
        );

        await context.RunAsync();
    }
}
