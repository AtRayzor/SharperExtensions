using FluentPipelines.Primitives;

namespace FluentPipelines;

public interface IPipeline<TRequest, TResponse>
{
    Task<Result<TResponse>> Execute(TRequest request, CancellationToken cancellationToken = default);
}