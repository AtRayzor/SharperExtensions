namespace DotNetCoreFunctional.Analyzers.Tests.TestSources;

public class MissingCaseSwitch
{
    private readonly ClosedTestType _testType;
    private readonly GenericClosedTestType<string> _genericClosedTestType;

    public MissingCaseSwitch(ClosedTestType testType, GenericClosedTestType<string> genericClosedTestType)
    {
        _testType = testType;
        _genericClosedTestType = genericClosedTestType;
    }

    public object GetCaseValue()
    {
        return _testType switch
        {
            Animal animal => animal.Name,
            Number number => number
        };
    }

    public object GetGenericCaseValue()
    {
        return _genericClosedTestType switch
        {
            Case1<string> case1 => case1.Value,
            Case2<string> case2 => case2.Values
        };
    }

    public void Cases()
    {
        switch (_testType)
        {
            case Animal animal:
                Console.WriteLine(animal.Name);
                break;
            case Number number:
                Console.WriteLine(number.Value);
                break;
        }
    }

    public void GenericCases()
    {
        switch (_genericClosedTestType)
        {
            case Case1<string> case1:
                Console.WriteLine(case1.Value);
                break;
            case Case2<string> case2:
                Console.WriteLine(case2.Values);
                break;
        }
    }
}
