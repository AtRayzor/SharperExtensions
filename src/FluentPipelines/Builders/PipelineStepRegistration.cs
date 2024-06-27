using FluentPipelines.Contracts;
using FluentPipelines.Delegates;

namespace FluentPipelines.Builders;

internal class PipelineStepRegistration<TRequest, TFinal> : IPipelineStepRegistration<TRequest, TFinal>
    where TRequest : notnull
{
    private readonly DelegateCollection<IStepHandlerDelegateWrapper> _delegateCollection = [];

    public IPipelineStepRegistration<TRequest, TFinal> AddPipelineStep<TResponse, TNext>(
        StepHandlerDelegate<TRequest, TResponse, TNext> handler) where TResponse : notnull
    {
        _delegateCollection.AddStepHandlerDelegateOrThrow(handler);
        return this;
    }

    public void AddPipelineStep(StepHandlerDelegate<TRequest, TFinal> handler)
    {
        _delegateCollection.AddStepHandlerDelegateOrThrow(handler);
    }

    public IPipelineStepRegistration<TRequest, TFinal> AddPipelineStep<TResponse, TNext, THandler>()
        where TResponse : notnull where THandler : IStepHandler<TRequest, TResponse, TNext>
    {
        _delegateCollection.AddStepHandlerDelegateOrThrow<TRequest, TResponse, TNext, THandler>();

        return this;
    }

    public void AddPipelineStep<THandler>() where THandler : IStepHandler<TRequest, TFinal>
    {
        _delegateCollection.AddStepHandlerDelegateOrThrow<TRequest, TFinal, THandler>();
    }

    internal DelegateCollection<IStepHandlerDelegateWrapper> Build(DelegateFactory delegateFactory)
    {
        _delegateCollection.InitializeStepHandlerDelegates(delegateFactory);

        return _delegateCollection;
    }
}