namespace DotNetCoreFunctional.Analyzers.Tests.TestSources;

public class AllCasesPatternMatchSwitch
{
    public object GetCaseValue(ClosedTestType testType)
    {
        return testType switch
        {
            Animal { Name: { } name } => name,
            Number { Value: var value } => value,
            Letter { Value: var character } => character,
        };
    }

    public object GetGenericCaseValue<T>(GenericClosedTestType<T> testType)
        where T : notnull
    {
        return testType switch
        {
            Case1<T> { Value: { } value } => value,
            Case2<T> { Values: var values } => values,
            Case3<T> { Value: null } => default,
        };
    }

    public void Cases(ClosedTestType testType)
    {
        switch (testType)
        {
            case Animal { Name: { } name }:
                Console.WriteLine(name);
                break;
            case Number { Value: > 0 }:
                Console.WriteLine("The value is positive.");
                break;
            case Letter { Value: 'A' }:
                Console.WriteLine("The letter is 'A'.");
                break;
        }
    }

    public void GenericCases<T>(GenericClosedTestType<T> testType)
        where T : notnull
    {
        switch (testType)
        {
            case Case1<T> { Value: var value }:
                Console.WriteLine(value);
                break;
            case Case2<T> { Values: var values }:
                Console.WriteLine();
                break;
            case Case3<T> { Value: not null, Count: 7 }:
                Console.WriteLine("The value exists and the count is 7.");
                break;
        }
    }
}
