namespace SharperExtensions.Async.Tests;

public class AsyncTests
{
    [Fact]
    public async Task New_ShouldWrapValueInAsync()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };

        var asyncDummy = Async.New(dummy, TestContext.Current.CancellationToken);

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
        var dummy = new DummyValue
        { Email = "jack.black@examply.com", Name = "Jack Black" };
        var asyncDummy = new Async<DummyValue>(dummy);

        var returned = await asyncDummy.GetAwaiter();

        returned.Should().Be(dummy);
    }

    [Fact]
    public async Task Map_ShouldTransformValue()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var asyncDummy = Async.New(dummy, TestContext.Current.CancellationToken);

        var mapped = Async.Map(
            asyncDummy,
            (d, _) => new DummyNewValue { NameAllCaps = d.Name.ToUpper() }
        );

        var result = await mapped;

        result.NameAllCaps.Should().Be("JOHN");
    }

    [Fact]
    public async Task MapSourceWithDelay_ShouldTransformValue()
    {
        var mapped = Async.Map(
            CreateSource(),
            (d, _) => new DummyNewValue { NameAllCaps = d.Name.ToUpper() }
        );

        var result = await mapped;

        result.NameAllCaps.Should().Be("JOHN");

        return;

        async Async<DummyValue> CreateSource()
        {
            await Task.Delay(1000);

            return new DummyValue { Name = "John", Email = "john@example.com" };
        }
    }

    [Fact]
    public async Task Map_Extension_ShouldTransformValue()
    {
        var dummy = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var asyncDummy = Async.New(dummy, TestContext.Current.CancellationToken);

        var mapped = asyncDummy.Map((d, _) => new DummyNewValue
        { NameAllCaps = d.Name.ToUpper() }
        );

        var result = await mapped;

        result.NameAllCaps.Should().Be("JANE");
    }

    [Fact]
    public async Task Bind_ShouldChainAsyncOperations()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var asyncDummy = Async.New(dummy, TestContext.Current.CancellationToken);

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
        var asyncDummy = Async.New(dummy, TestContext.Current.CancellationToken);

        Async<DummyNewValue> Binder(DummyValue d, CancellationToken _) =>
            Async.New(new DummyNewValue { NameAllCaps = d.Name.ToUpper() });

        var bound = asyncDummy.Bind(Binder);

        var result = await bound;

        result.NameAllCaps.Should().Be("JANE");
    }

    [Fact]
    public async Task BindSourceWithDelay_ShouldChainAsyncOperations()
    {
        var bound = Async.Bind(CreateSource(), Binder);

        var result = await bound;

        result.NameAllCaps.Should().Be("JOHN");

        return;

        Async<DummyNewValue> Binder(DummyValue d, CancellationToken _) =>
            Async.New(new DummyNewValue { NameAllCaps = d.Name.ToUpper() });

        async Async<DummyValue> CreateSource()
        {
            await Task.Delay(1000);

            return new DummyValue { Name = "John", Email = "john@example.com" };
        }
    }

    [Fact]
    public async Task Apply_ShouldApplyWrappedFunction()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var asyncDummy = Async.New(dummy, TestContext.Current.CancellationToken);

        var asyncFunc =
            Async.New<Func<DummyValue, CancellationToken, DummyNewValue>>(
                (d, _) =>
                    new DummyNewValue { NameAllCaps = d.Name.ToUpper() },
                TestContext.Current.CancellationToken
            );

        var applied = Async.Apply(asyncDummy, asyncFunc);

        var result = await applied;

        result.NameAllCaps.Should().Be("JOHN");
    }

    [Fact]
    public async Task Apply_WithDelayedSource_ShouldApplyWrappedFunction()
    {
        var asyncFunc =
            Async.New<Func<DummyValue, CancellationToken, DummyNewValue>>(
                (d, _) =>
                    new DummyNewValue { NameAllCaps = d.Name.ToUpper() },
                TestContext.Current.CancellationToken
            );

        var applied = Async.Apply(CreateSourceAsync(), asyncFunc);

        var result = await applied;

        result.NameAllCaps.Should().Be("JOHN");

        return;

        static async Async<DummyValue> CreateSourceAsync()
        {
            await Task.Delay(1000);

            return new DummyValue { Name = "John", Email = "john@example.com" };
        }
    }

    [Fact]
    public async Task Apply_WithDelayedWrapperMethod_ShouldApplyWrappedFunction()
    {
        var applied = Async.Apply(CreateSourceAsync(), WrapFunctionAsync());

        var result = await applied;

        result.NameAllCaps.Should().Be("JOHN");

        return;

        static async Async<DummyValue> CreateSourceAsync()
        {
            await Task.Delay(1000);

            return new DummyValue { Name = "John", Email = "john@example.com" };
        }

        static async Async<Func<DummyValue, CancellationToken, DummyNewValue>>
            WrapFunctionAsync()
        {
            await Task.Delay(1000);

            return (d, _) => new DummyNewValue { NameAllCaps = d.Name.ToUpper() };
        }
    }

    [Fact]
    public async Task Apply_Extension_ShouldApplyWrappedFunction()
    {
        var dummy = new DummyValue { Name = "Jane", Email = "jane@example.com" };
        var asyncDummy = Async.New(dummy, TestContext.Current.CancellationToken);

        var asyncFunc =
            Async.New<Func<DummyValue, CancellationToken, DummyNewValue>>(
                (d, _) =>
                    new DummyNewValue { NameAllCaps = d.Name.ToUpper() },
                TestContext.Current.CancellationToken
            );

        var applied = asyncDummy.Apply(asyncFunc);

        var result = await applied;

        result.NameAllCaps.Should().Be("JANE");
    }

    [Fact]
    public async Task DoAsync_ShouldInvokeAction()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var asyncDummy = Async.New(dummy, TestContext.Current.CancellationToken);

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
        var asyncDummy = Async.New(dummy, TestContext.Current.CancellationToken);

        var invoked = false;

        await asyncDummy.DoAsync(d =>
            {
                invoked = true;
                d.Name.Should().Be("Jane");
                return Task.CompletedTask;
            }
        );

        invoked.Should().BeTrue();
    }

    [Fact]
    public async Task FromTask_Unit_ShouldWrapNonGenericTask()
    {
        var task = Task.Delay(10, TestContext.Current.CancellationToken);

        var asyncUnit = Async.FromTask(task, TestContext.Current.CancellationToken);

        var result = await asyncUnit;

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task FromValueTask_Unit_ShouldWrapNonGenericValueTask()
    {
        var valueTask = new ValueTask(Task.Delay(10, CancellationToken.None));

        var asyncUnit = Async.FromValueTask(
            valueTask,
            TestContext.Current.CancellationToken
        );

        var result = await asyncUnit;

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public void ToAsync_Extensions_ShouldConvertTasks()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var task = Task.FromResult(dummy);
        var valueTask = new ValueTask<DummyValue>(dummy);

        var asyncFromTask = task.ToAsync(CancellationToken.None);
        var asyncFromTaskWithToken = task.ToAsync(CancellationToken.None);
        var asyncFromValueTask = valueTask.ToAsync(CancellationToken.None);
        var asyncFromValueTaskWithToken = valueTask.ToAsync(CancellationToken.None);

        asyncFromTask.Should().NotBeNull();
        asyncFromTaskWithToken.Should().NotBeNull();
        asyncFromValueTask.Should().NotBeNull();
        asyncFromValueTaskWithToken.Should().NotBeNull();
    }

    [Fact]
    public async Task AwaitAsyncFunction_ShouldReturnResult()
    {
        var expected = new DummyValue { Name = "John", Email = "john@example.com" };
        var result = await DelayAsync();

        result.Should().BeEquivalentTo(expected);

        return;

        static async Async<DummyValue> DelayAsync()
        {
            await Task.Delay(1000);

            return new DummyValue { Name = "John", Email = "john@example.com" };
        }
    }

    [Fact]
    public async Task AwaitAsyncFunction_ShouldThrowException()
    {
        const string errorMessage = "Test error message";
        await FluentActions
            .Awaiting(async () => await DelayAsync())
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(errorMessage);

        return;

        static async Async<DummyValue> DelayAsync()
        {
            await Task.Delay(1000);

            throw new InvalidOperationException(errorMessage);
        }
    }

    [Fact]
    public async Task AsTask_ShouldBeAwaitable()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var dummyAsync = new Async<DummyValue>(dummy);

        var task = dummyAsync.AsTask();
        var awaited = await task;

        awaited.Should().Be(dummy);
    }

    [Fact]
    public void Result_CreateFromValue_ShouldReturnOk()
    {
        var dummy = new DummyValue { Name = "John", Email = "john@example.com" };
        var dummyAsync = new Async<DummyValue>(dummy);

        dummyAsync
            .Result
            .Should()
            .Satisfy<Result<DummyValue, Exception>>(o => o.IsOk.Should().BeTrue())
            .And
            .Satisfy<Result<DummyValue, Exception>>(o =>
                o.Value.Should().BeEquivalentTo(dummy)
            );
    }

    [Fact]
    public void Result_CreateFromAsyncOperation_ShouldReturnOk()
    {
        const string name = "John";
        const string email = "john@example.com";

        var expected = new DummyValue { Name = name, Email = email };

        var dummyAsync = CreateValueAsync();

        dummyAsync
            .Result
            .Should()
            .Satisfy<Result<DummyValue, Exception>>(o => o.IsOk.Should().BeTrue())
            .And
            .Satisfy<Result<DummyValue, Exception>>(o =>
                o.Value.Should().BeEquivalentTo(expected)
            );

        async Async<DummyValue> CreateValueAsync()
        {
            await Task.Delay(1000);

            return new DummyValue { Email = email, Name = name };
        }
    }

    [Fact]
    public void Result_FromCallback_ShouldReturnOk()
    {
        const string name = "John";
        const string email = "john@example.com";

        var expected = new DummyValue { Name = name, Email = email };

        var dummyAsync = new Async<DummyValue>(
            CreateValue,
            cancellationToken: TestContext.Current.CancellationToken
        );

        dummyAsync
            .Result
            .Should()
            .Satisfy<Result<DummyValue, Exception>>(o => o.IsOk.Should().BeTrue())
            .And
            .Satisfy<Result<DummyValue, Exception>>(o =>
                o.Value.Should().BeEquivalentTo(expected)
            );

        static DummyValue CreateValue()
        {
            Task.Delay(2000).Wait();

            return new DummyValue { Email = email, Name = name };
        }
    }

    [Fact]
    public async Task ConfigureAwait_AwaitReturnsSome()
    {
        const string name = "John";
        const string email = "john@example.com";
        var value = new DummyValue { Name = name, Email = email };

        var asyncValue = Async.New(value, TestContext.Current.CancellationToken);
        var returned = await asyncValue.ConfigureAwait();

        returned
            .Should()
            .BeOfType<Option<DummyValue>>()
            .Which
            .Value
            .Should()
            .BeEquivalentTo(value);
    }

    [Fact]
    public async Task ConfigureAwait_AsyncMethodThrows_AwaitReturnsNone()
    {
        const string name = "John";
        const string email = "john@example.com";
        var value = new DummyValue { Name = name, Email = email };

        var asyncValue = CreateValueAsync();
        var returned = await asyncValue.ConfigureAwait();

        returned
            .Should()
            .Satisfy<Option<DummyValue>>(o => o.IsSome.Should().BeTrue())
            .And
            .Satisfy<Option<DummyValue>>(o => o.Value.Should().BeEquivalentTo(value));

        return;

        static async Async<DummyValue> CreateValueAsync()
        {
            var value = new DummyValue { Name = name, Email = email };
            await Task.Delay(1000);

            return value;
        }
    }

    [Fact]
    public async Task ConfigureAwait_CreateFromAsyncMethod_AwaitReturnsSome()
    {
        var asyncValue = CreateValueAsync();
        var returned = await asyncValue.ConfigureAwait();

        returned.Should().BeOfType<Option<DummyValue>>().Which.IsNone.Should().BeTrue();

        return;

        static async Async<DummyValue> CreateValueAsync()
        {
            await Task.Delay(1000);

            throw new InvalidOperationException();
        }
    }

        [Fact]
    public void AsyncResult_ReturnsOk()
    {
        var async = 
            Async.New(
                OptionAsyncTestData.DummyValue, 
                TestContext.Current.CancellationToken
            );

        async
        .AsyncResult
        .WrappedResult
        .Result
        .Value
        .Should()
        .Satisfy<Result<DummyValue, Exception>>(r => r.IsOk.Should().BeTrue())
        .And
        .Satisfy<Result<DummyValue, Exception>>(r =>
            r.Value.Should().BeEquivalentTo(OptionAsyncTestData.DummyValue)
        );

    }

    [Fact]
    public void AsyncResult_ConstructedWithAsyncMethod_ReturnsOk()
    {
        var async = CreateValueAsync();

        async
        .AsyncResult
        .WrappedResult
        .Result
        .Value
        .Should()
        .Satisfy<Result<DummyValue, Exception>>(r => r.IsOk.Should().BeTrue())
        .And
        .Satisfy<Result<DummyValue, Exception>>(r =>
            r.Value.Should().BeEquivalentTo(OptionAsyncTestData.DummyValue)
        );


        async Async<DummyValue> CreateValueAsync()
        {
            await Task.Delay(1000);

            return OptionAsyncTestData.DummyValue;
        }
    }
    
    [Fact]
    public void OptionAsyncResult_AsyncMethodThrows_ReturnsExceptionError()
    {
        var async = CreateValueAsync();

        async
            .AsyncResult
            .WrappedResult
            .Result.Value
            .Should()
            .Satisfy<Result<DummyValue, Exception>>(r => r.IsError.Should().BeTrue())
            .And
            .Satisfy<Result<DummyValue, Exception>>(r =>
                r.ErrorValue.Should().BeOfType<InvalidOperationException>()
            );

        async Async<DummyValue> CreateValueAsync()
        {
            await Task.Delay(1000);

            throw new InvalidOperationException();
        }
    }

    [Fact]
    public void OptionAsyncResult_ReturnsSome()
    {
        var async = 
            Async.New(
                OptionAsyncTestData.DummyValue, 
                TestContext.Current.CancellationToken
            );

        async
            .OptionAsyncResult
            .WrappedOption
            .Result
            .Value
            .Should()
            .Satisfy<Option<DummyValue>>(o => o.IsSome.Should().BeTrue())
            .And
            .Satisfy<Option<DummyValue>>(o =>
                o.Value.Should().BeEquivalentTo(OptionAsyncTestData.DummyValue)
            );
    }

    [Fact]
    public void OptionAsyncResult_ConstructedWithAsyncMethod_ReturnsSome()
    {
        var async = CreateValueAsync();

        async
            .OptionAsyncResult
            .WrappedOption
            .Result
            .Value
            .Should()
            .Satisfy<Option<DummyValue>>(o => o.IsSome.Should().BeTrue())
            .And
            .Satisfy<Option<DummyValue>>(o =>
                o.Value.Should().BeEquivalentTo(OptionAsyncTestData.DummyValue)
            );

        async Async<DummyValue> CreateValueAsync()
        {
            await Task.Delay(1000);

            return OptionAsyncTestData.DummyValue;
        }
    }
    
    [Fact]
    public void OptionAsyncResult_AsyncMethodThrows_ReturnsNone()
    {
        var async = CreateValueAsync();

        async
            .OptionAsyncResult
            .WrappedOption
            .Result
            .Value
            .Should()
            .Satisfy<Option<DummyValue>>(o => o.IsNone.Should().BeTrue());


        async Async<DummyValue> CreateValueAsync()
        {
            await Task.Delay(1000);

            throw new InvalidOperationException();
        }
    }

}