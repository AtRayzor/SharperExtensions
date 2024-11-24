namespace DotNetCoreFunctional.Analyzers.Tests.TestSources;

public class ParameterUnionInvalidArgs
{
    public void CallWithInvalidArgs()
    {
        var validParams = new ParameterUnionValidParams();
        validParams.ValidParam(true);
    }
}
