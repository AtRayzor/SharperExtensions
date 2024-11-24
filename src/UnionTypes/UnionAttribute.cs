namespace DotNetCoreFunctional.UnionTypes;

[AttributeUsage(AttributeTargets.Parameter)]
public class UnionAttribute(params Type[] types) : Attribute;
