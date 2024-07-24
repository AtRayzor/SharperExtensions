using System;
using NetFunctional.Core;

namespace Analyzers.Sample;

//public record Plant(string Species) : IThing;

//public class D : IThing;
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

public class SwitchExpressionEx
{
    public object ValueOfThing(IThing thing)
    {
        return thing switch
        {
            Number number => number.Value,
            Letter letter => letter.Char,
            Animal animal => animal.Name
        };
    }
}

public class SwitchExpressionGeneric
{
    public string GenericValue(IGenericThing<string> thing) =>
        thing switch
        {
            Case1<string> case1 => case1.Value,
            Case2<string> case2 => case2.Value
        };
}

public class SwitchStatementEx
{
    public void Statement(IThing thing)
    {
        switch (thing)
        {
            case Animal animal:
                Console.WriteLine(animal.Name);
                break;
            case Number:
                break;
            default:
                break;
        }
    }
}

public class SwitchStatementGenericEx
{
    public void Statement(IGenericThing<int> thing)
    {
        switch (thing)
        {
            case Case1<int>:
                break;
            case Case2<int>:
                break;
        }
    }
}