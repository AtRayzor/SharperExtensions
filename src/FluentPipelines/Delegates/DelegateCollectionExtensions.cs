using System.Data;
using FluentPipelines.Contracts;

namespace FluentPipelines.Delegates;

internal static class DelegateCollectionExtensions
{
    public static void ThrowIfAllReadyExists<TWrapper>(this DelegateCollection<TWrapper> collection,
        IDelegateWrapper wrapper) where TWrapper : IDelegateWrapper
    {
        var equivalentTypes = collection.Where(dw => dw.IsEquivalent(wrapper)).ToArray();

        if (equivalentTypes.Length == 0) return;

        var wrapperGenericListLines = equivalentTypes.Select(w => w.ToString()!).ToArray();
        var message = wrapperGenericListLines.Length == 1
            ? $"A a handler with the parameters {wrapperGenericListLines[0]} was already added to the pipeline."
            : $"Multiple handlers with parameters matching {wrapperGenericListLines[0]} were already added to the pipeline";

        throw new DuplicateNameException(message);
    }

    public static void AddStepHandlerDelegateOrThrow<TRequest, TResponse, TNext>(
        this DelegateCollection<IStepHandlerDelegateWrapper> collection,
        StepHandlerDelegate<TRequest, TResponse, TNext> handlerDelegate)
    {
        var wrapper = new StepHandlerDelegateWrapper<TRequest, TResponse, TNext>(handlerDelegate);
        collection.ThrowIfAllReadyExists(wrapper);
        collection.Add(new StepHandlerDelegateWrapper<TRequest, TResponse, TNext>(handlerDelegate));
    }

    public static void AddStepHandlerDelegateOrThrow<TRequest, TFinal>(
        this DelegateCollection<IStepHandlerDelegateWrapper> collection,
        StepHandlerDelegate<TRequest, TFinal> handlerDelegate)
    {
        var wrapper = new FinalStepDelegateWrapper<TRequest, TFinal>(handlerDelegate);
        collection.ThrowIfAllReadyExists(wrapper);
        collection.Add(wrapper);
    }
}