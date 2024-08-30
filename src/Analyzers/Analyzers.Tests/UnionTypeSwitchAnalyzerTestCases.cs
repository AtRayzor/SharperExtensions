using System.Collections;
using System.Linq;
using DotNetCoreFunctional.Analyzers.Tests.TestSources;
using DotNetCoreFunctional.UnionTypes.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace DotNetCoreFunctional.Analyzers.Tests;

internal class UnionTypeSwitchAnalyzerTestCases : IEnumerable<object[]>
{
    private readonly string[] _sources =
    [
        "../../../TestSources/ClosedTestType.cs",
        "../../../TestSources/GenericClosedTestType.cs",
        "../../../TestSources/NestedClosedTestType.cs"
    ];

    private readonly string[] _testFiles =
    [
        "../../../TestSources/AllCasesSwitch.cs",
        "../../../TestSources/MissingCasesExpression.cs",
        "../../../TestSources/DefaultCaseSwitch.cs"
    ];

    private (string, SourceText)[]? _sourceTexts;

    private (string, SourceText)[] SourceTexts =>
        _sourceTexts ??= _sources
            .Select(s => (s.Split('/').Last(), SourceTextFactory.CreateSourceText(s)))
            .ToArray();

    public IEnumerator<object[]> GetEnumerator()
    {
        var baseDiagnostic = new DiagnosticResult(UnionTypeSwitchAnalyzer.Rule)
            .WithArguments(typeof(ClosedTestType).FullName!)
            .WithSeverity(DiagnosticSeverity.Error);

        var genericBaseDiagnostic = new DiagnosticResult(UnionTypeSwitchAnalyzer.Rule)
            .WithArguments(
                $"{typeof(GenericClosedTestType<>).FullName!.Split('`').First()}<string>"
            )
            .WithSeverity(DiagnosticSeverity.Error);

        var nestedBaseDiagnostic = new DiagnosticResult(UnionTypeSwitchAnalyzer.Rule)
            .WithArguments($"{typeof(NestedClosedTestType).FullName}")
            .WithSeverity(DiagnosticSeverity.Error);

        yield return
        [
            CreateAnalyzer("../../../TestSources/AllCasesSwitch.cs", CompilerDiagnostics.Errors)
        ];
        yield return
        [
            CreateAnalyzer("../../../TestSources/DefaultCaseSwitch.cs", CompilerDiagnostics.Errors)
        ];
        yield return
        [
            CreateAnalyzer(
                "../../../TestSources/MissingCaseSwitch.cs",
                CompilerDiagnostics.Errors,
                baseDiagnostic.WithLocation("MissingCaseSwitch.cs", 19, 16),
                genericBaseDiagnostic.WithLocation("MissingCaseSwitch.cs", 28, 16),
                baseDiagnostic.WithLocation("MissingCaseSwitch.cs", 37, 9),
                genericBaseDiagnostic.WithLocation("MissingCaseSwitch.cs", 50, 9),
                nestedBaseDiagnostic.WithLocation("MissingCaseSwitch.cs", 62, 9)
            )
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private CSharpAnalyzerTest<UnionTypeSwitchAnalyzer, DefaultVerifier> CreateAnalyzer(
        string source,
        CompilerDiagnostics compilerDiagnostics,
        params DiagnosticResult[] expected
    )
    {
        var context = TestContextFactory.CreateContext<UnionTypeSwitchAnalyzer>();

        context.CompilerDiagnostics = compilerDiagnostics;
        context.ExpectedDiagnostics.AddRange(expected);
        context
            .TestState
            .Sources
            .Add((source.Split('/').Last(), SourceTextFactory.CreateSourceText(source)));
        context.TestState.Sources.AddRange(SourceTexts);

        return context;
    }
}
