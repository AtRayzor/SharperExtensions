using DotNetCoreFunctional.UnionTypes;

namespace DotNetCoreFunctional.Analyzers.Tests.TestSources;

public class ParameterUnionValidParams
{
    public void ValidParam([Union(typeof(int), typeof(string))] object union) { }
}
