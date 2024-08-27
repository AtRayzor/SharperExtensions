using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace DotNetCoreFunctional.Analyzers.Tests;

internal static class TestContextFactory
{
    public static CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateContext<TAnalyzer>()
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        return new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies
                .Net
                .Net80
                .WithAssemblies([typeof(Action).Assembly.FullName!]),
            TestState =
            {
                Sources =
                {
                    (
                        "Closed.cs",
                        SourceTextFactory.CreateSourceText(
                            "../../../../../UnionTypes/UnionTypes/Closed.cs"
                        )
                    ),
                    (
                        "GlobalUsings",
                        SourceTextFactory.CreateSourceText("../../../TestSources/GlobalUsings.cs")
                    )
                }
            }
        };
    }
}
