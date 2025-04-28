using DotNetCoreFunctional;
using DotNetCoreFunctional.Async;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;

namespace NetFunction.Types.Tests;

public class AsyncTests
{
    [Fact]
    public async Task New_ShouldWrapValueInAsync()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };

        var asyncDummy = Async.New(dummy);

        var result = await asyncDummy;

        result.Should().BeEquivalentTo(dummy);
    }

    [Fact]
    public async Task FromTask_ShouldWrapTaskInAsync()
    {
        var dummy = new DummyNewValue { NameAllCaps = "ALICE" };
        var task = Task.FromResult(dummy);

        var asyncDummy = Async.FromTask(task);

        var result = await asyncDummy;

        result.Should().BeEquivalentTo(dummy);
    }

    [Fact]
    public async Task FromValueTask_ShouldWrapValueTaskInAsync()
    {
        var dummy = new DummyNewerValue { NameLowercase = "bob" };
        var valueTask = new ValueTask<DummyNewerValue>(dummy);

        var asyncDummy = Async.FromValueTask(valueTask);

        var result = await asyncDummy;

        result.Should().BeEquivalentTo(dummy);
    }

    [Fact]
    public async Task GetAwaiter_ReturnsValue()
    {
        var dummy = new DummyValue { Email = "jack.black@examply.com", Name = "Jack Black" };
        var asyncDummy = new Async<DummyValue>(dummy);

        var returned = await asyncDummy.GetAwaiter();

        returned.Should().Be(dummy);
    }

    [Fact]
    public async Task Map_ShouldTransformValue()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var asyncDummy = Async.New(dummy);

        var mapped = Async.Map(
            asyncDummy,
            (d, _) => new DummyNewValue { NameAllCaps = d.Name.ToUpper() }
        );

        var result = await mapped;

        result.NameAllCaps.Should().Be("JOHN");
    }

    [Fact]
    public async Task Map_Extension_ShouldTransformValue()
    {
        var dummy = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var asyncDummy = Async.New(dummy);

        var mapped = asyncDummy.Map((d, _) => new DummyNewValue { NameAllCaps = d.Name.ToUpper() });

        var result = await mapped;

        result.NameAllCaps.Should().Be("JANE");
    }

    [Fact]
    public async Task Bind_ShouldChainAsyncOperations()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var asyncDummy = Async.New(dummy);

        Async<DummyNewValue> Binder(DummyValue d, CancellationToken _) =>
            Async.New(new DummyNewValue { NameAllCaps = d.Name.ToUpper() });

        var bound = Async.Bind(asyncDummy, Binder);

        var result = await bound;

        result.NameAllCaps.Should().Be("JOHN");
    }

    [Fact]
    public async Task Bind_Extension_ShouldChainAsyncOperations()
    {
        var dummy = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var asyncDummy = Async.New(dummy);

        Async<DummyNewValue> Binder(DummyValue d, CancellationToken _) =>
            Async.New(new DummyNewValue { NameAllCaps = d.Name.ToUpper() });

        var bound = asyncDummy.Bind(Binder);

        var result = await bound;

        result.NameAllCaps.Should().Be("JANE");
    }

    [Fact]
    public async Task Apply_ShouldApplyWrappedFunction()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var asyncDummy = Async.New(dummy);

        var asyncFunc = Async.New<Func<DummyValue, CancellationToken, DummyNewValue>>(
            (d, _) => new DummyNewValue { NameAllCaps = d.Name.ToUpper() }
        );

        var applied = Async.Apply(asyncDummy, asyncFunc);

        var result = await applied;

        result.NameAllCaps.Should().Be("JOHN");
    }

    [Fact]
    public async Task Apply_Extension_ShouldApplyWrappedFunction()
    {
        var dummy = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var asyncDummy = Async.New(dummy);

        var asyncFunc = Async.New<Func<DummyValue, CancellationToken, DummyNewValue>>(
            (d, _) => new DummyNewValue { NameAllCaps = d.Name.ToUpper() }
        );

        var applied = asyncDummy.Apply(asyncFunc);

        var result = await applied;

        result.NameAllCaps.Should().Be("JANE");
    }

    [Fact]
    public async Task DoAsync_ShouldInvokeAction()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var asyncDummy = Async.New(dummy);

        var invoked = false;

        await Async.DoAsync(
            asyncDummy,
            (d, _) =>
            {
                invoked = true;
                d.Name.Should().Be("John");
                return Task.CompletedTask;
            },
            CancellationToken.None
        );

        invoked.Should().BeTrue();
    }

    [Fact]
    public async Task DoAsync_Extension_ShouldInvokeAction()
    {
        var dummy = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var asyncDummy = Async.New(dummy);

        var invoked = false;

        await asyncDummy.DoAsync(d =>
        {
            invoked = true;
            d.Name.Should().Be("Jane");
            return Task.CompletedTask;
        });

        invoked.Should().BeTrue();
    }

    [Fact]
    public async Task FromTask_Unit_ShouldWrapNonGenericTask()
    {
        var task = Task.Delay(10);

        var asyncUnit = Async.FromTask(task);

        var result = await asyncUnit;

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task FromValueTask_Unit_ShouldWrapNonGenericValueTask()
    {
        var valueTask = new ValueTask(Task.Delay(10));

        var asyncUnit = Async.FromValueTask(valueTask);

        var result = await asyncUnit;

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public void ToAsync_Extensions_ShouldConvertTasks()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var task = Task.FromResult(dummy);
        var valueTask = new ValueTask<DummyValue>(dummy);

        var asyncFromTask = task.ToAsync();
        var asyncFromTaskWithToken = task.ToAsync(CancellationToken.None);
        var asyncFromValueTask = valueTask.ToAsync();
        var asyncFromValueTaskWithToken = valueTask.ToAsync(CancellationToken.None);

        asyncFromTask.Should().NotBeNull();
        asyncFromTaskWithToken.Should().NotBeNull();
        asyncFromValueTask.Should().NotBeNull();
        asyncFromValueTaskWithToken.Should().NotBeNull();
    }

    [Fact]
    public async Task Async_Task_ShouldBeAwaitable()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var dummyAsync = new Async<DummyValue>(dummy);

        var task = dummyAsync.Task;
        var awaited = await task;

        awaited.Should().Be(dummy);
    }
}
