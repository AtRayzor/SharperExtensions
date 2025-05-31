namespace SharperExtensions;

[AttributeUsage(AttributeTargets.Parameter)]
public class UnionAttribute(params Type[] types) : Attribute
{
    public Type[] Types { get; } = types;
}