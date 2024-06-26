namespace FluentPipelines.Delegates;

internal interface IExecutorDelegateCreator
{
    Delegate CreateExecutor(object request, Delegate next, CancellationToken cancellationToken);

}

internal interface IFinalExecutorDelegateCreator
{ 
    Delegate CreateExecutor(object request, CancellationToken cancellationToken);
}