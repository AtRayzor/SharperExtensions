namespace FluentPipelines.Delegates;

internal static class DelegateCollectionExtensions
{
    public static StepHandlerDelegate<TRequest, TResponse> FindStepHandlerDelegate<TRequest, TResponse>(
        this DelegateCollection collection)
        => (StepHandlerDelegate<TRequest, TResponse>)collection.First(d =>
            d.GetType() == typeof(StepHandlerDelegate<TRequest, TResponse>));
    public static 
}