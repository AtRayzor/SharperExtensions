namespace SharperExtensions.Analyzers.Tests.TestSources;

public class AllCasesSwitch
{
    public object GetCaseValue(ClosedTestType testType)
    {
        return testType switch
        {
            Animal animal => animal.Name,
            Number number => number,
            Letter => "letter",
        };
    }

    public object GetGenericCaseValue<T>(GenericClosedTestType<T> testType)
        where T : notnull
    {
        return testType switch
        {
            Case1<T> case1 => case1.Value,
            Case2<T> case2 => case2.Values,
            Case3<T> => default,
        };
    }

    public void Cases(ClosedTestType testType)
    {
        switch (testType)
        {
            case Animal animal:
                Console.WriteLine(animal.Name);
                break;
            case Number number:
                Console.WriteLine(number.Value);
                break;
            case Letter:
                Console.WriteLine("Letter");
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
            case Case2<T> case2:
                Console.WriteLine(case2.Values);
                break;
            case Case3<T>:
                Console.WriteLine("Case3");
                break;
        }
    }
}
