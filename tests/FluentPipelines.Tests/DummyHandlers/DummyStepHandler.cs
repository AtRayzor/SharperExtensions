using FluentPipelines.Contracts;
using FluentPipelines.Delegates;
using FluentPipelines.Primitives;

namespace FluentPipelines.Tests.DummyHandlers;

public class DummyStepHandler : IStepHandler<string, string, string>
{
    public Task<Result<string>> Handle(string request, StepHandlerExecutionsDelegate<string> next,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.Ok("result"));
    }
}