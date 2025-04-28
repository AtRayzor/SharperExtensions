namespace DotNetCoreFunctional.Async;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class AsyncErrorBuilderAttribute(Type builderType) : Attribute
{
    internal Type BuilderType { get; } = builderType;
}
