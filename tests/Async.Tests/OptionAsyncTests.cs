namespace SharperExtensions.Async.Tests;

public static class OptionAsyncTestData
{
    public static DummyValue DummyValue =
        new() { Name = "Test", Email = "test@example.com" };

    public static DummyNewValue DummyNewValue = new() { NameAllCaps = "TEST" };

    public static Func<DummyValue, DummyNewValue> MappingFunc =
        val => new DummyNewValue { NameAllCaps = val.Name.ToUpperInvariant() };

    public static Func<DummyValue, OptionAsync<DummyNewValue>> BindingFuncSome =
        val =>
            OptionAsync.Create(
                new DummyNewValue { NameAllCaps = val.Name.ToUpperInvariant() }
            );

    public static Func<DummyValue, OptionAsync<DummyNewValue>> BindingFuncNone =
        val => OptionAsync.Create<DummyNewValue>(null);

    public static Func<DummyValue, Task<string>> AsyncMatchSomeFunc =
        val => Task.FromResult($"Matched Some: {val.Name}");

    public static Func<Task<string>> AsyncMatchNoneFunc =
        () => Task.FromResult("Matched None");

    public static Func<DummyValue, Task> AsyncIfSomeAction =
        val =>
        {
            Console.WriteLine($"IfSome called with: {val.Name}");
            return Task.CompletedTask;
        };

    public static Func<Task> AsyncIfNoneAction =
        () =>
        {
            Console.WriteLine("IfNone called");
            return Task.CompletedTask;
        };

    public class TestException : Exception
    {
        public TestException(string message) : base(message) { }
    }
}

public class OptionAsyncTests
{
    [Fact]
    public async Task Create_FromValue_ShouldReturnSomeOptionAsync()
    {
        var optionAsync = OptionAsync.Create(OptionAsyncTestData.DummyValue);

        var option = await optionAsync;

        option.Should().BeOfType<Option<DummyValue>>().Which.IsSome.Should().BeTrue();
        option.Value.Should().BeEquivalentTo(OptionAsyncTestData.DummyValue);
    }

    [Fact]
    public async Task Create_FromNull_ShouldReturnNoneOptionAsync()
    {
        var optionAsync = OptionAsync.Create<DummyValue>(null);

        var option = await optionAsync;

        option.Should().BeOfType<Option<DummyValue>>().Which.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task Create_FromSomeOption_ShouldReturnSomeOptionAsync()
    {
        var someOption = Option<DummyValue>.Some(OptionAsyncTestData.DummyValue);
        var optionAsync = OptionAsync.Create(someOption);

        var option = await optionAsync;

        option.Should().BeOfType<Option<DummyValue>>().Which.IsSome.Should().BeTrue();
        option.Value.Should().BeEquivalentTo(OptionAsyncTestData.DummyValue);
    }

    [Fact]
    public async Task Create_FromNoneOption_ShouldReturnNoneOptionAsync()
    {
        var noneOption = Option<DummyValue>.None;
        var optionAsync = OptionAsync.Create(noneOption);

        var option = await optionAsync;

        option.IsNone.Should().BeTrue();
    }

    [Fact]
    public async Task Lift_FromAsyncValue_ShouldReturnSomeOptionAsync()
    {
        var asyncValue = Async.New(
            OptionAsyncTestData.DummyValue,
            TestContext.Current.CancellationToken
        );
        var optionAsync = OptionAsync.Lift(asyncValue);

        var option = await optionAsync;

        option.Should().BeOfType<Option<DummyValue>>().Which.IsSome.Should().BeTrue();
        option.Value.Should().BeEquivalentTo(OptionAsyncTestData.DummyValue);
    }

    [Fact]
    public async Task Map_OnSome_ShouldApplyMappingFunction()
    {
        var optionAsync = OptionAsync.Create(OptionAsyncTestData.DummyValue);

        var mappedOptionAsync = OptionAsync.Map(
            optionAsync,
            OptionAsyncTestData.MappingFunc
        );

        var mappedOption = await mappedOptionAsync;

        mappedOption
            .Should()
            .BeOfType<Option<DummyNewValue>>()
            .Which.IsSome
            .Should()
            .BeTrue();
        mappedOption.Value.Should().BeEquivalentTo(OptionAsyncTestData.DummyNewValue);
    }

    [Fact]
    public async Task Map_OnNone_ShouldReturnNone()
    {
        var optionAsync = OptionAsync.Create<DummyValue>(null);

        var mappedOptionAsync = OptionAsync.Map(
            optionAsync,
            OptionAsyncTestData.MappingFunc
        );

        var mappedOption = await mappedOptionAsync;

        mappedOption
            .Should()
            .BeOfType<Option<DummyNewValue>>()
            .Which
            .IsNone
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task Bind_OnSome_WhenBinderReturnsSome_ShouldReturnSome()
    {
        var optionAsync = OptionAsync.Create(OptionAsyncTestData.DummyValue);

        var boundOptionAsync = OptionAsync.Bind(
            optionAsync,
            OptionAsyncTestData.BindingFuncSome
        );

        var boundOption = await boundOptionAsync;


        boundOption
            .Should()
            .BeOfType<Option<DummyNewValue>>()
            .Which
            .IsSome
            .Should()
            .BeTrue();
        boundOption.Value.Should().BeEquivalentTo(OptionAsyncTestData.DummyNewValue);
    }

    [Fact]
    public async Task Bind_OnSome_WhenBinderReturnsNone_ShouldReturnNone()
    {
        var optionAsync = OptionAsync.Create(OptionAsyncTestData.DummyValue);

        var boundOptionAsync = OptionAsync.Bind(
            optionAsync,
            OptionAsyncTestData.BindingFuncNone
        );

        var boundOption = await boundOptionAsync;

        boundOption
            .Should()
            .BeOfType<Option<DummyNewValue>>()
            .Which
            .IsNone
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task Bind_OnNone_ShouldReturnNoneAndNotCallBinder()
    {
        var optionAsync = OptionAsync.Create<DummyValue>(null);
        bool binderCalled = false;
        Func<DummyValue, OptionAsync<DummyNewValue>> binder =
            val =>
            {
                binderCalled = true;
                return OptionAsync.Create(OptionAsyncTestData.DummyNewValue);
            };

        var boundOptionAsync = OptionAsync.Bind(optionAsync, binder);

        var boundOption = await boundOptionAsync;

        boundOption
            .Should()
            .BeOfType<Option<DummyNewValue>>()
            .Which
            .IsNone
            .Should()
            .BeTrue();
        binderCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Apply_WithSomeValueAndSomeMapper_ShouldReturnSome()
    {
        var optionAsyncValue = OptionAsync.Create(OptionAsyncTestData.DummyValue);
        var optionAsyncMapper = OptionAsync.Create(OptionAsyncTestData.MappingFunc);

        var appliedOptionAsync = OptionAsync.Apply(optionAsyncValue, optionAsyncMapper);

        var appliedOption = await appliedOptionAsync;

        appliedOption
            .Should()
            .BeOfType<Option<DummyNewValue>>()
            .Which
            .IsSome
            .Should()
            .BeTrue();
        appliedOption.Value.Should().BeEquivalentTo(OptionAsyncTestData.DummyNewValue);
    }

    [Fact]
    public async Task Apply_WithSomeValueAndNoneMapper_ShouldReturnNone()
    {
        var optionAsyncValue = OptionAsync.Create(OptionAsyncTestData.DummyValue);
        var optionAsyncMapper = OptionAsync.Create<Func<DummyValue, DummyNewValue>>(null);

        var appliedOptionAsync = OptionAsync.Apply(optionAsyncValue, optionAsyncMapper);

        var appliedOption = await appliedOptionAsync;

        appliedOption
            .Should()
            .BeOfType<Option<DummyNewValue>>()
            .Which
            .IsNone
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task Apply_WithNoneValueAndSomeMapper_ShouldReturnNone()
    {
        var optionAsyncValue = OptionAsync.Create<DummyValue>(null);
        var optionAsyncMapper = OptionAsync.Create(OptionAsyncTestData.MappingFunc);

        var appliedOptionAsync = OptionAsync.Apply(optionAsyncValue, optionAsyncMapper);

        var appliedOption = await appliedOptionAsync;

        appliedOption
            .Should()
            .BeOfType<Option<DummyNewValue>>()
            .Which
            .IsNone
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task Apply_WithNoneValueAndNoneMapper_ShouldReturnNone()
    {
        var optionAsyncValue = OptionAsync.Create<DummyValue>(null);
        var optionAsyncMapper =
            OptionAsync.Create<Func<DummyValue, DummyNewValue>>(null);

        var appliedOptionAsync =
            OptionAsync.Apply(optionAsyncValue, optionAsyncMapper);

        var appliedOption = await appliedOptionAsync;

        appliedOption
            .Should()
            .BeOfType<Option<DummyNewValue>>()
            .Which
            .IsNone
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task Unsafe_MatchAsync_OnSome_ShouldInvokeSomeFunc()
    {
        var optionAsync = OptionAsync.Create(OptionAsyncTestData.DummyValue);
        bool someFuncCalled = false;
        bool noneFuncCalled = false;

        var result = await OptionAsync.Unsafe.MatchAsync(
            optionAsync,
            async val =>
            {
                someFuncCalled = true;
                return await OptionAsyncTestData.AsyncMatchSomeFunc(val);
            },
            async () =>
            {
                noneFuncCalled = true;
                return await OptionAsyncTestData.AsyncMatchNoneFunc();
            }
        );

        someFuncCalled.Should().BeTrue();
        noneFuncCalled.Should().BeFalse();
        result.Should().Be($"Matched Some: {OptionAsyncTestData.DummyValue.Name}");
    }

    [Fact]
    public async Task Unsafe_MatchAsync_OnNone_ShouldInvokeNoneFunc()
    {
        var optionAsync = OptionAsync.Create<DummyValue>(null);
        bool someFuncCalled = false;
        bool noneFuncCalled = false;

        var result = await OptionAsync.Unsafe.MatchAsync(
            optionAsync,
            async val =>
            {
                someFuncCalled = true;
                return await OptionAsyncTestData.AsyncMatchSomeFunc(val);
            },
            async () =>
            {
                noneFuncCalled = true;
                return await OptionAsyncTestData.AsyncMatchNoneFunc();
            }
        );

        someFuncCalled.Should().BeFalse();
        noneFuncCalled.Should().BeTrue();
        result.Should().Be("Matched None");
    }

    [Fact]
    public async Task Unsafe_GetValueOrDefaultAsync_OnSome_ShouldReturnValue()
    {
        var optionAsync = OptionAsync.Create(OptionAsyncTestData.DummyValue);

        var value = await OptionAsync.Unsafe.GetValueOrDefaultAsync(optionAsync);

        value.Should().BeEquivalentTo(OptionAsyncTestData.DummyValue);
    }

    [Fact]
    public async Task Unsafe_GetValueOrDefaultAsync_OnNone_ShouldReturnDefault()
    {
        var optionAsync = OptionAsync.Create<DummyValue>(null);

        var value = await OptionAsync.Unsafe.GetValueOrDefaultAsync(optionAsync);

        value.Should().BeNull();
    }

    [Fact]
    public async Task Unsafe_GetValueOrThrowAsync_OnSome_ShouldNotThrow()
    {
        var optionAsync = OptionAsync.Create(OptionAsyncTestData.DummyValue);

        await optionAsync
            .Awaiting(async ValueTask (oa) =>
                {
                    (await OptionAsync.Unsafe.GetValueOrDefaultAsync(oa))
                        .Should()
                        .BeEquivalentTo(OptionAsyncTestData.DummyValue);
                }
            )
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task Unsafe_GetValueOrThrowAsync_OnNone_ShouldThrowSpecifiedException()
    {
        var optionAsync = OptionAsync.Create<DummyValue>(null);

        await optionAsync
            .Awaiting(async ValueTask (oa) =>
                {
                    await OptionAsync.Unsafe.GetValueOrDefaultAsync(oa);
                }
            )
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task Unsafe_IfSomeAsync_OnSome_ShouldExecuteAction()
    {
        var optionAsync = OptionAsync.Create(OptionAsyncTestData.DummyValue);
        bool actionCalled = false;
        Func<DummyValue, Task> action =
            val =>
            {
                actionCalled = true;
                return Task.CompletedTask;
            };

        await OptionAsync.Unsafe.IfSomeAsync(optionAsync, action);

        actionCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Unsafe_IfSomeAsync_OnNone_ShouldNotExecuteAction()
    {
        var optionAsync = OptionAsync.Create<DummyValue>(null);
        bool actionCalled = false;
        Func<DummyValue, Task> action =
            val =>
            {
                actionCalled = true;
                return Task.CompletedTask;
            };

        await OptionAsync.Unsafe.IfSomeAsync(optionAsync, action);

        actionCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Unsafe_IfNoneAsync_OnSome_ShouldNotExecuteAction()
    {
        var optionAsync = OptionAsync.Create(OptionAsyncTestData.DummyValue);
        bool actionCalled = false;
        Func<Task> action =
            () =>
            {
                actionCalled = true;
                return Task.CompletedTask;
            };

        await OptionAsync.Unsafe.IfNoneAsync(optionAsync, action);

        actionCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Unsafe_IfNoneAsync_OnNone_ShouldExecuteAction()
    {
        var optionAsync = OptionAsync.Create<DummyValue>(null);
        bool actionCalled = false;
        Func<Task> action =
            () =>
            {
                actionCalled = true;
                return Task.CompletedTask;
            };

        await OptionAsync.Unsafe.IfNoneAsync(optionAsync, action);

        actionCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Unsafe_DoAsync_OnSome_ShouldExecuteSomeFunc()
    {
        var optionAsync = OptionAsync.Create(OptionAsyncTestData.DummyValue);
        bool someFuncCalled = false;
        bool noneFuncCalled = false;
        Func<DummyValue, Task> someFunc =
            val =>
            {
                someFuncCalled = true;
                return Task.CompletedTask;
            };
        Func<Task> noneFunc =
            () =>
            {
                noneFuncCalled = true;
                return Task.CompletedTask;
            };

        await OptionAsync.Unsafe.DoAsync(optionAsync, someFunc, noneFunc);

        someFuncCalled.Should().BeTrue();
        noneFuncCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Unsafe_DoAsync_OnNone_ShouldExecuteNoneFunc()
    {
        var optionAsync = OptionAsync.Create<DummyValue>(null);
        bool someFuncCalled = false;
        bool noneFuncCalled = false;
        Func<DummyValue, Task> someFunc =
            val =>
            {
                someFuncCalled = true;
                return Task.CompletedTask;
            };
        Func<Task> noneFunc =
            () =>
            {
                noneFuncCalled = true;
                return Task.CompletedTask;
            };

        await OptionAsync.Unsafe.DoAsync(optionAsync, someFunc, noneFunc);

        someFuncCalled.Should().BeFalse();
        noneFuncCalled.Should().BeTrue();
    }
}