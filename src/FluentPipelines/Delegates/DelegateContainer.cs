namespace FluentPipelines.Delegates;

internal record DelegateContainer(
    Delegate StepDelegate,
    Type UnboundDelegateType,
    Type RequestType,
    Type ResponseType,
    Type? NextType = default)
{
    public bool IsFinalDelegate => NextType is null;
}