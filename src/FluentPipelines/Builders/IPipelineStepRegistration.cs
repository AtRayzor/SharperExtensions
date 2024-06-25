using FluentPipelines.Contracts;
using FluentPipelines.Delegates;
using FluentPipelines.Primitives;

namespace FluentPipelines.Builders;

public interface IPipelineStepRegistration<TRequest> where TRequest : notnull
{
    IPipelineStepRegistration<TRequest> AddPipelineStep<TResponse, TNext>(
        StepHandlerDelegate<TRequest, TResponse, TNext> handler)
        where TResponse : notnull;

    IPipelineStepRegistration<TRequest> AddPipelineStep<TResponse>(
        StepHandlerDelegate<TRequest, TResponse, TResponse> handler)
        where TResponse : notnull;
    
    void AddPipelineStep<TResponse>(
        StepHandlerDelegate<TRequest, TResponse> handler)
        where TResponse : notnull;
    
    void AddPipelineStep<TResponse, THandler>()
        where TResponse : notnull
        where THandler : IStepHandler<TRequest, TResponse>;
}