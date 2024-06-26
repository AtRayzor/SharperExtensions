using FluentPipelines.Contracts;
using FluentPipelines.Delegates;
using FluentPipelines.Primitives;

namespace FluentPipelines.Builders;

public interface IPipelineStepRegistration<TRequest, TFinal> where TRequest : notnull
{
    IPipelineStepRegistration<TRequest, TFinal> AddPipelineStep<TResponse, TNext>(
        StepHandlerDelegate<TRequest, TResponse, TNext> handler)
        where TResponse : notnull;

    void AddPipelineStep(
        StepHandlerDelegate<TRequest, TFinal> handler);

    IPipelineStepRegistration<TRequest, TFinal> AddPipelineStep<TResponse, TNext, THandler>()
        where TResponse : notnull
        where THandler : IStepHandler<TRequest, TResponse, TNext>;

    void AddPipelineStep<THandler>() where THandler : IStepHandler<TRequest, TFinal>;
}