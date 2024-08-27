using System.Threading.Tasks;
using DotNetCoreFunctional.UnionTypes.Analyzers;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace DotNetCoreFunctional.Analyzers.Tests;

public class UnionTypeSwitchAnalyzerTests
{
    [Theory]
    [ClassData(typeof(UnionTypeSwitchAnalyzerTestCases))]
    public async Task Test(CSharpAnalyzerTest<UnionTypeSwitchAnalyzer, DefaultVerifier> context)
    {
        await context.RunAsync();
    }
}
