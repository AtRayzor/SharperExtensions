using System.Diagnostics.CodeAnalysis;
using FluentPipelines.Builders;
using FluentPipelines.Delegates;
using FluentPipelines.Primitives;

namespace FluentPipelines;

public abstract class Pipeline<TRequest, TResponse> : IPipeline<TRequest, TResponse> where TRequest : notnull
{
    private bool _isInitialized;
    private DelegateCollection<IStepHandlerDelegateWrapper> _delegateCollection = null!;

    internal void Initialize(DelegateCollection<IStepHandlerDelegateWrapper> delegateCollection)
    {
        _delegateCollection = delegateCollection;
        _isInitialized = true;
    }
    
    protected internal abstract void RegisterPipelineSteps(IPipelineStepRegistration<TRequest, TResponse> registration);

    protected virtual Result<TResponse> HandleExceptions(Exception exception)
        => Results.Error<TResponse>("Something went wrong while trying to retrieve the requested data.");
    
    public async Task<Result<TResponse>> Execute(TRequest request, CancellationToken cancellationToken = default)
    {
        if (!_isInitialized)
        {
            return Results.Error<TResponse>("Something went wrong while trying to retrieve the requested data.");
        }
        
        try
        {
            var finalDelegate =
                ((IFinalExecutorDelegateCreator)_delegateCollection.Last()).CreateExecutor(request, cancellationToken);
            var pipelineChain = (StepHandlerExecutionsDelegate<TResponse>)_delegateCollection.Reverse()
                .Aggregate(finalDelegate,
                    (nextDelegate, currentWrapper) =>
                        ((IExecutorDelegateCreator)currentWrapper).CreateExecutor(request, nextDelegate,
                            cancellationToken));

            var response = await pipelineChain();

            return response as Result<TResponse> ?? Results.Ok(response);
        }
        catch (Exception e)
        {
            return Results.Error<TResponse>("Something went wrong while trying to retrieve the requested data.");
        }
    }
}
