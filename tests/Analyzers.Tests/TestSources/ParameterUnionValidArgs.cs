namespace SharperExtensions.Analyzers.Tests.TestSources;

public class UnionParametersValidArgs
{
    public void CallValidArgs()
    {
        var validParams = new ParameterUnionValidParams();
        validParams.ValidParam(7);
    }
}
