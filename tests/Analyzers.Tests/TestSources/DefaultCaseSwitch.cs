namespace SharperExtensions.Analyzers.Tests.TestSources;

public class DefaultCaseSwitch
{
    public object GetCaseValue(ClosedTestType testType)
    {
        return testType switch
        {
            Animal animal => animal.Name,
            _ => string.Empty,
        };
    }

    public object GetGenericCaseValue<T>(GenericClosedTestType<T> testType)
        where T : notnull
    {
        return testType switch
        {
            Case1<T> case1 => case1.Value,
            _ => string.Empty,
        };
    }

    public void Cases(ClosedTestType testType)
    {
        switch (testType)
        {
            case Animal animal:
                Console.WriteLine(animal.Name);
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
    }

    public void GenericCases<T>(GenericClosedTestType<T> testType)
        where T : notnull
    {
        switch (testType)
        {
            case Case1<T> case1:
                Console.WriteLine(case1.Value);
                break;
            default:
                Console.WriteLine("Default case");
                break;
        }
    }
}
