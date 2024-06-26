using System.Runtime.CompilerServices;
using FluentPipelines.Primitives;

[assembly: InternalsVisibleTo("FluentPipelines.Tests")]

namespace FluentPipelines;

public interface IPipeline<TRequest, TResponse>
{
    Task<Result<TResponse>> Execute(TRequest request, CancellationToken cancellationToken = default);
}