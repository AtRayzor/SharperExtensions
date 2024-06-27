using FluentPipelines.Builders;
using FluentPipelines.Delegates;
using Microsoft.Extensions.DependencyInjection;

namespace FluentPipelines;

internal sealed class PipelineFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PipelineFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPipeline<TRequest, TResponse> Create<TRequest, TResponse, TPipeline>()
        where TPipeline : Pipeline<TRequest, TResponse>
        where TRequest : notnull
    {
        var handlerDelegateFactoryFactory = new DelegateFactory(_serviceProvider);
        var registration = new PipelineStepRegistration<TRequest, TResponse>();


        var pipelineInstance = Activator.CreateInstance<TPipeline>();
        pipelineInstance.RegisterPipelineSteps(registration);
        pipelineInstance.Initialize(registration.Build(handlerDelegateFactoryFactory));

        return pipelineInstance;
    }
}