using System.Dynamic;
using FluentPipelines.Contracts;
using FluentPipelines.Primitives;

namespace FluentPipelines.Delegates;

internal interface IDelegateWrapper
{
    Delegate Delegate { get; }
}

interface IStepHandlerDelegateWrapper : IDelegateWrapper
{
}

internal interface IStepHandlerDelegateWrapper<out TDelegate> : IStepHandlerDelegateWrapper
    where TDelegate : Delegate
{
    new TDelegate Delegate { get; }
}

internal interface
    IStepHandlerDelegateWrapper<TRequest, TResponse, TNext> : IStepHandlerDelegateWrapper<
    StepHandlerDelegate<TRequest, TResponse, TNext>>
{
}

internal sealed class
    StepHandlerDelegateWrapper<TRequest, TResponse, TNext>(StepHandlerDelegate<TRequest, TResponse, TNext> @delegate)
    : IStepHandlerDelegateWrapper<TRequest, TResponse, TNext>, IExecutorDelegateCreator
{
    Delegate IDelegateWrapper.Delegate => Delegate;
    public StepHandlerDelegate<TRequest, TResponse, TNext> Delegate { get; } = @delegate;

    public override string ToString() =>
        $"{GetType().GetGenericArguments().Select(t => t.Name)}";

    public Delegate CreateExecutor(object request, Delegate next, CancellationToken cancellationToken)
        => CreateExecutor((TRequest)request, (StepHandlerExecutionsDelegate<TNext>)next, cancellationToken);

    private StepHandlerExecutionsDelegate<TResponse> CreateExecutor(TRequest request,
        StepHandlerExecutionsDelegate<TNext> next, CancellationToken cancellationToken)
        => () => Delegate.Method.Invoke(Delegate, [request, next, cancellationToken]) as Task<TResponse>
                 ?? throw new StepDelegateHandlerInvocationException(
                     $"The handler for request type '{typeof(TRequest).FullName}' could not be invoke.");
}

internal sealed class FinalStepDelegateWrapper<TRequest, TFinal>(StepHandlerDelegate<TRequest, TFinal> @delegate)
    : IStepHandlerDelegateWrapper<StepHandlerDelegate<TRequest, TFinal>>, IFinalExecutorDelegateCreator
{
    Delegate IDelegateWrapper.Delegate => Delegate;

    public StepHandlerDelegate<TRequest, TFinal> Delegate { get; } = @delegate;

    public Delegate CreateExecutor(object request, CancellationToken cancellationToken)
        => CreateExecutor((TRequest)request, cancellationToken);

    private StepHandlerExecutionsDelegate<TFinal> CreateExecutor(TRequest request,
        CancellationToken cancellationToken)
        => () => Delegate.Method.Invoke(Delegate, [request, cancellationToken]) as Task<TFinal>
                 ?? throw new StepDelegateHandlerInvocationException(
                     $"The handler for request type '{typeof(TRequest).FullName}' could not be invoke.");

    public override string ToString() =>
        $"{GetType().FullName}<{string.Join(", ", GetType().GetGenericArguments().Select(t => t.Name))}>";
}

internal sealed class StepHandlerDelegateWrapper<TRequest, TResponse, TNext, THandler>(DelegateFactory factory)
    : IStepHandlerDelegateWrapper<StepHandlerDelegate<TRequest, TResponse, TNext>>, IExecutorDelegateCreator
    where THandler : IStepHandler<TRequest, TResponse, TNext>
{
    Delegate IDelegateWrapper.Delegate => Delegate;

    private StepHandlerDelegate<TRequest, TResponse, TNext>? _delegate;

    public StepHandlerDelegate<TRequest, TResponse, TNext> Delegate
    {
        get
        {
            _delegate ??= factory.CreateDelegate<TRequest, TResponse, TNext, THandler>();
            return _delegate;
        }
    }

    public Delegate CreateExecutor(object request, Delegate next, CancellationToken cancellationToken)
        => CreateExecutor((TRequest)request, (StepHandlerExecutionsDelegate<TNext>)next, cancellationToken);

    private StepHandlerExecutionsDelegate<TResponse> CreateExecutor(TRequest request,
        StepHandlerExecutionsDelegate<TNext> next, CancellationToken cancellationToken)
        => () => Delegate.Method.Invoke(Delegate, [request, next, cancellationToken]) as Task<TResponse>
                 ?? throw new StepDelegateHandlerInvocationException(
                     $"The handler for request type '{typeof(TRequest).FullName}' could not be invoke.");

    public override string ToString() =>
        $"{GetType().FullName}<{string.Join(", ", GetType().GetGenericArguments().Select(t => t.Name))}>";
}

internal sealed class FinalStepHandlerDelegateWrapper<TRequest, TFinal, THandler>(DelegateFactory factory)
    : IStepHandlerDelegateWrapper<StepHandlerDelegate<TRequest, TFinal>>
    where THandler : IStepHandler<TRequest, TFinal>, IFinalExecutorDelegateCreator
{
    Delegate IDelegateWrapper.Delegate => Delegate;

    private StepHandlerDelegate<TRequest, TFinal>? _delegate;

    public StepHandlerDelegate<TRequest, TFinal> Delegate
    {
        get
        {
            _delegate ??= factory.CreateDelegate<TRequest, TFinal, THandler>();
            return _delegate;
        }
    }

    public Delegate CreateExecutor(object request, CancellationToken cancellationToken)
        => CreateExecutor((TRequest)request, cancellationToken);

    private StepHandlerExecutionsDelegate<TFinal> CreateExecutor(TRequest request,
        CancellationToken cancellationToken)
        => () => Delegate.Method.Invoke(Delegate, [request, cancellationToken]) as Task<TFinal>
                 ?? throw new StepDelegateHandlerInvocationException(
                     $"The handler for request type '{typeof(TRequest).FullName}' could not be invoke.");

    public override string ToString() =>
        $"{GetType().FullName}<{string.Join(", ", GetType().GetGenericArguments().Select(t => t.Name))}>";
}

internal interface IStepHandlerExecutionDelegateWrapper : IDelegateWrapper
{
}

internal sealed class StepHandlerExecutionDelegateWrapper<TResponse>(StepHandlerExecutionsDelegate<TResponse> @delegate)
    : IStepHandlerExecutionDelegateWrapper
{
    Delegate IDelegateWrapper.Delegate => Delegate;
    StepHandlerExecutionsDelegate<TResponse> Delegate { get; } = @delegate;
}