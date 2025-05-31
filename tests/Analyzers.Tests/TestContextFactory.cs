using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace SharperExtensions.Analyzers.Tests;

internal static class TestContextFactory
{
    public static CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateContext<TAnalyzer>()
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        return new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80.WithAssemblies(
                [typeof(Action).Assembly.FullName!]
            ),
            TestState =
            {
                Sources =
                {
                    (
                        "Closed.cs",
                        SourceTextFactory.CreateSourceText("../../../../../UnionTypes/Closed.cs")
                    ),
                    (
                        "UnionAttribute.cs",
                        SourceTextFactory.CreateSourceText(
                            "../../../../../UnionTypes/UnionAttribute.cs"
                        )
                    ),
                    (
                        "GlobalUsings",
                        SourceTextFactory.CreateSourceText("../../../TestSources/GlobalUsings.cs")
                    ),
                },
            },
        };
    }
}
