using System.Data;
using FluentPipelines.Contracts;

namespace FluentPipelines.Delegates;

internal static class DelegateCollectionExtensions
{

    public static void AddStepHandlerDelegate<TRequest, TResponse, TNext>(
        this DelegateCollection<IStepHandlerDelegateWrapper> collection,
        StepHandlerDelegate<TRequest, TResponse, TNext> handlerDelegate)
    {
        var wrapper = new StepHandlerDelegateWrapper<TRequest, TResponse, TNext>(handlerDelegate);
        collection.Add(wrapper);
    }

    public static void AddStepHandlerDelegate<TRequest, TFinal>(
        this DelegateCollection<IStepHandlerDelegateWrapper> collection,
        StepHandlerDelegate<TRequest, TFinal> handlerDelegate)
    {
        var wrapper = new FinalStepDelegateWrapper<TRequest, TFinal>(handlerDelegate);
        collection.Add(wrapper);
    }

    public static void AddStepHandlerDelegate<TRequest, TResponse, TNext, THandler>(
        this DelegateCollection<IStepHandlerDelegateWrapper> collection)
        where THandler : IStepHandler<TRequest, TResponse, TNext>
    {
        var wrapper = new StepHandlerDelegateWrapper<TRequest, TResponse, TNext, THandler>();
        collection.Add(wrapper);
    }

    public static void AddStepHandlerDelegate<TRequest, TFinal, THandler>(
        this DelegateCollection<IStepHandlerDelegateWrapper> collection)
        where THandler : IStepHandler<TRequest, TFinal>
    {
        var wrapper = new FinalStepHandlerDelegateWrapper<TRequest, TFinal, THandler>();
        collection.Add(wrapper);
    }

    public static void InitializeStepHandlerDelegates(this DelegateCollection<IStepHandlerDelegateWrapper> collection,
        DelegateFactory factory)
    {
        foreach (var wrapper in collection)
        {
            if (wrapper is INeedsInitialization needsInitialization)
            {
                needsInitialization.Initialize(factory);
            }
        }
    }
}