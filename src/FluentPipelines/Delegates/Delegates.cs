using FluentPipelines.Primitives;

namespace FluentPipelines.Delegates;

public delegate Task<Result<TResponse>> StepHandlerDelegate<TRequest, TResponse, TNext>(TRequest request,
    StepHandlerExecutionsDelegate<TNext> next, CancellationToken cancellationToken);

public delegate Task<Result<TResponse>> StepHandlerDelegate<TRequest, TResponse>(TRequest request,
    CancellationToken cancellationToken);
public delegate Task<TResponse> StepHandlerExecutionsDelegate<TResponse>();


public static class Check
{
}