using FluentPipelines.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FluentPipelines.Delegates;

internal sealed class DelegateFactory
{
    private readonly IServiceProvider _serviceProvider;

    public DelegateFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public StepHandlerDelegate<TRequest, TResponse, TNext> CreateDelegate<TRequest, TResponse, TNext, THandler>()
        where THandler : IStepHandler<TRequest, TResponse, TNext>
    {
        var handlerInstance =
            (IStepHandler<TRequest, TResponse, TNext>)ActivatorUtilities.CreateInstance<THandler>(_serviceProvider);

        return handlerInstance.Handle;
    }
    
    public StepHandlerDelegate<TRequest, TFinal> CreateDelegate<TRequest, TFinal, THandler>()
        where THandler : IStepHandler<TRequest, TFinal>
    {
        var handlerInstance =
            (IStepHandler<TRequest, TFinal>)ActivatorUtilities.CreateInstance<THandler>(_serviceProvider);

        return handlerInstance.Handle;
    }
    
}