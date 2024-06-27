using FluentPipelines.Contracts;
using FluentPipelines.Primitives;

namespace FluentPipelines.Tests.DummyHandlers;

public class DummyFinalStepHandler : IStepHandler<string, string>
{
    public Task<Result<string>> Handle(string request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.Ok("result"));
    }
}