using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Analyzers.Tests;

public class UnionTypeSwitchAnalyzerTests
{

    [Theory]
    [ClassData(typeof(UnionTypeSwitchAnalyzerTestCases))]
    public async Task Test(CSharpAnalyzerTest<UnionTypeSwitchAnalyzer, DefaultVerifier> context)
    {
        await context.RunAsync();
    }
}

