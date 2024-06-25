using FluentPipelines.Delegates;
using FluentPipelines.Primitives;

namespace FluentPipelines.Contracts;

public interface IStepHandler<TRequest, TResponse, TNext>
{
    Task<Result<TResponse>> Handle(TRequest request, StepHandlerExecutionsDelegate<TNext> next,
        CancellationToken cancellationToken);
}

public interface IStepHandler<TRequest, TResponse>
{
    Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}