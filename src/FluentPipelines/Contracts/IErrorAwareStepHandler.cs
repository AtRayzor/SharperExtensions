using FluentPipelines.Delegates;
using FluentPipelines.Primitives;

namespace FluentPipelines.Contracts;

public interface IErrorAwareStepHandler<TRequest, TResponse>
{
    Task<Result<TResponse>> Handle(TRequest request, ErrorAwareStepHandlerExecutionDelegate<TResponse> next,
        CancellationToken cancellationToken);
}