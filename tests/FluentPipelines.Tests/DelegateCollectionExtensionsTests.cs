using FluentAssertions;
using FluentPipelines.Delegates;
using FluentPipelines.Primitives;
using FluentPipelines.Tests.DummyHandlers;

namespace FluentPipelines.Tests;

public class DelegateCollectionExtensionsTests
{
    [Fact]
    public void ThrowIfAlreadyExists_NoDuplicates_DoesNotThrow()
    {
        var collection = new DelegateCollection<IStepHandlerDelegateWrapper>();
        collection.Invoking(c =>
                c.AddStepHandlerDelegateOrThrow<string, string, string>(
                    (request, next, cancellationToken)
                        => Task.FromResult(Results.Ok("result")))
            ).Should()
            .NotThrow<DuplicateStepHandlersException>();
    }

    [Fact]
    public void ThrowIfAlreadyExists_DelegateAlreadyInCollection_ThrowsException()
    {
        var collection = new DelegateCollection<IStepHandlerDelegateWrapper>();
        collection.AddStepHandlerDelegateOrThrow<string, string, string>(
            (request, next, cancellationToken)
                => Task.FromResult(Results.Ok("result")));

        collection.Invoking(c =>
                c.AddStepHandlerDelegateOrThrow<string, string, string>(
                    (request, next, cancellationToken)
                        => Task.FromResult(Results.Ok("result")))
            ).Should()
            .Throw<DuplicateStepHandlersException>()
            .WithMessage(
                "A a handler with the parameters " +
                "System.Linq.Enumerable+SelectArrayIterator`2[System.Type,System.String] " +
                "was already added to the pipeline.");
    }

    [Fact]
    public void AddStepHandlerDelegateOrThrow_PassHandlerDelegate_RegistersHandlerDelegate()
    {
        StepHandlerDelegate<string, string, string> @delegate = (request, next, cancellationToken)
            => Task.FromResult(Results.Ok("result"));
        var expectedWrapper = new StepHandlerDelegateWrapper<string, string, string>(
            @delegate
        );
        var collection = new DelegateCollection<IStepHandlerDelegateWrapper>();
        collection.AddStepHandlerDelegateOrThrow(@delegate);

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
        collection.AddStepHandlerDelegateOrThrow(@delegate);

        collection.Should().ContainEquivalentOf(expectedWrapper);
    }


    [Fact]
    public void AddStepHandlerDelegateOrThrow_PassClassHandler_RegistersHandlerDelegate()
    {

        var collection = new DelegateCollection<IStepHandlerDelegateWrapper>();
        collection.AddStepHandlerDelegateOrThrow<string, string, string, DummyStepHandler>();

        collection.Should()
            .ContainItemsAssignableTo<StepHandlerDelegateWrapper<string, string, string, DummyStepHandler>>();
        ;
    }

    [Fact]
    public void AddStepHandlerDelegateOrThrow_PassFinalClassHandler_RegistersHandlerDelegate()
    {
        var collection = new DelegateCollection<IStepHandlerDelegateWrapper>();
        collection.AddStepHandlerDelegateOrThrow<string, string, DummyFinalStepHandler>();

        collection.Should()
            .ContainItemsAssignableTo<FinalStepHandlerDelegateWrapper<string, string, DummyFinalStepHandler>>();
    }
}