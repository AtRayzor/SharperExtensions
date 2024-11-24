using DotNetCoreFunctional.UnionTypes;

namespace DotNetCoreFunctional.Analyzers.Tests.TestSources;

public class ParameterUnionInvalidParams
{
    public void InvalidParam([Union] string invalid)
    {
        
    }
}