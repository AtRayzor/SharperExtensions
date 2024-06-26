using FluentPipelines.Primitives;

namespace FluentPipelines;

internal sealed class PipelineExecutor<TRequest, TResponse> :  IPipeline<TRequest, TResponse>
{
    public Task<Result<TResponse>> Execute(TRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}