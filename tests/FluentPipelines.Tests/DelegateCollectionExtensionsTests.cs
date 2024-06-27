using FluentAssertions;
using FluentPipelines.Delegates;
using FluentPipelines.Primitives;
using FluentPipelines.Tests.DummyHandlers;

namespace FluentPipelines.Tests;

public class DelegateCollectionExtensionsTests
{
    [Fact]
    public void AddStepHandlerDelegateOrThrow_PassHandlerDelegate_RegistersHandlerDelegate()
    {
        StepHandlerDelegate<string, string, string> @delegate = (request, next, cancellationToken)
            => Task.FromResult(Results.Ok("result"));
        var expectedWrapper = new StepHandlerDelegateWrapper<string, string, string>(
            @delegate
        );
        var collection = new DelegateCollection<IStepHandlerDelegateWrapper>();
        collection.AddStepHandlerDelegate(@delegate);

        collection.Should().ContainEquivalentOf(expectedWrapper);
    }

    [Fact]
    public void AddStepHandlerDelegateOrThrow_PassFinalHandlerDelegate_RegistersHandlerDelegate()
    {
        StepHandlerDelegate<string, string> @delegate = (request, cancellationToken)
            => Task.FromResult(Results.Ok("result"));
        var expectedWrapper = new FinalStepDelegateWrapper<string, string>(
            @delegate
        );
        var collection = new DelegateCollection<IStepHandlerDelegateWrapper>();
        collection.AddStepHandlerDelegate(@delegate);

        collection.Should().ContainEquivalentOf(expectedWrapper);
    }


    [Fact]
    public void AddStepHandlerDelegateOrThrow_PassClassHandler_RegistersHandlerDelegate()
    {

        var collection = new DelegateCollection<IStepHandlerDelegateWrapper>();
        collection.AddStepHandlerDelegate<string, string, string, DummyStepHandler>();

        collection.Should()
            .ContainItemsAssignableTo<StepHandlerDelegateWrapper<string, string, string, DummyStepHandler>>();
        ;
    }

    [Fact]
    public void AddStepHandlerDelegateOrThrow_PassFinalClassHandler_RegistersHandlerDelegate()
    {
        var collection = new DelegateCollection<IStepHandlerDelegateWrapper>();
        collection.AddStepHandlerDelegate<string, string, DummyFinalStepHandler>();

        collection.Should()
            .ContainItemsAssignableTo<FinalStepHandlerDelegateWrapper<string, string, DummyFinalStepHandler>>();
    }
}