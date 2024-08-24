using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Analyzers.Tests.TestSources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace Analyzers.Tests;

internal class UnionTypeSwitchAnalyzerTestCases : IEnumerable<object[]>
{
    private (string, SourceText)[]? _sourceTexts;

    private readonly string[] _testFiles =
    [
        "../../../TestSources/AllCasesSwitch.cs",
        "../../../TestSources/MissingCasesExpression.cs",
        "../../../TestSources/DefaultCaseSwitch.cs",
    ];

    private readonly string[] _sources =
    [
        "../../../TestSources/ClosedTestType.cs",
        "../../../TestSources/GenericClosedTestType.cs",
    ];

    private (string, SourceText)[] SourceTexts =>
        _sourceTexts ??= _sources.Select(s => (s.Split('/').Last(), SourceTextFactory.CreateSourceText(s))).ToArray();


    private CSharpAnalyzerTest<UnionTypeSwitchAnalyzer, DefaultVerifier> CreateAnalyzer(
        string source,
        CompilerDiagnostics compilerDiagnostics,
        params DiagnosticResult[] expected)
    {
        var context = TestContextFactory.CreateContext<UnionTypeSwitchAnalyzer>();


        context.CompilerDiagnostics = compilerDiagnostics;
        context.ExpectedDiagnostics.AddRange(expected);
        context.TestState.Sources.Add((source.Split('/').Last(), SourceTextFactory.CreateSourceText(source)));
        context.TestState.Sources.AddRange(SourceTexts);

        return context;
    }


    public IEnumerator<object[]> GetEnumerator()
    {
        var baseDiagnostic = new DiagnosticResult(UnionTypeSwitchAnalyzer.Rule)
            .WithArguments(typeof(ClosedTestType).FullName!)
            .WithSeverity(DiagnosticSeverity.Error);

        var genericBaseDiagnostic = new DiagnosticResult(UnionTypeSwitchAnalyzer.Rule)
            .WithArguments($"{typeof(GenericClosedTestType<>).FullName!.Split('`').First()}<T>")
            .WithSeverity(DiagnosticSeverity.Error);


        yield return [CreateAnalyzer("../../../TestSources/AllCasesSwitch.cs", CompilerDiagnostics.Errors)];
        yield return [CreateAnalyzer("../../../TestSources/DefaultCaseSwitch.cs", CompilerDiagnostics.Errors)];
        yield return
        [
            CreateAnalyzer(
                "../../../TestSources/MissingCaseSwitch.cs",
                CompilerDiagnostics.Errors,
                baseDiagnostic.WithLocation("MissingCaseSwitch.cs", 9, 16),
                genericBaseDiagnostic.WithLocation("MissingCaseSwitch.cs", 18, 16),
                baseDiagnostic.WithLocation("MissingCaseSwitch.cs", 27, 9),
                genericBaseDiagnostic.WithLocation("MissingCaseSwitch.cs", 40, 9)
            )
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}