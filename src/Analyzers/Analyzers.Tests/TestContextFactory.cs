using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Analyzers.Tests;

internal static class TestContextFactory
{
    public static CSharpAnalyzerTest<TAnalyzer, DefaultVerifier> CreateContext<TAnalyzer>()
        where TAnalyzer : DiagnosticAnalyzer, new()
        => new()
        {
            ReferenceAssemblies = ReferenceAssemblies
                .Net
                .Net80
                .WithAssemblies([typeof(Action).Assembly.FullName!]),
            TestState =
            {
                Sources =
                {
                    ("Closed.cs", SourceTextFactory.CreateSourceText("../../../../../Core/UnionTypes/Closed.cs")),
                    ("GlobalUsings", SourceTextFactory.CreateSourceText("../../../TestSources/GlobalUsings.cs")),
                },
            },
        };
}