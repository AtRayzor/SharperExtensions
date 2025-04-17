using DotNetCoreFunctional.Async;
using DotNetCoreFunctional.Result;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;

namespace NetFunction.Types.Tests
{
    public class AsyncResultTests
    {
        private static readonly CancellationToken NoneToken = CancellationToken.None;

        [Fact]
        public async Task CreateOk_ShouldReturnOkResult()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            var result = await asyncResult.ConfigureAwait();

            result.Should().BeOfType<Ok<DummyValue, DummyError>>();
            result.As<Ok<DummyValue, DummyError>>().Value.Should().BeEquivalentTo(dummyValue);
        }

        [Fact]
        public async Task CreateError_ShouldReturnErrorResult()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncResult = AsyncResult.CreateError<DummyValue, DummyError>(dummyError);

            var result = await asyncResult.ConfigureAwait();

            result.Should().BeOfType<Error<DummyValue, DummyError>>();
            result.As<Error<DummyValue, DummyError>>().Err.Should().BeEquivalentTo(dummyError);
        }

        [Fact]
        public async Task Create_ShouldWrapGivenResult()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var okResult = Result<DummyValue, DummyError>.Ok(dummyValue);
            var asyncResult = AsyncResult.Create(okResult);

            var result = await asyncResult.ConfigureAwait();

            result.Should().BeOfType<Ok<DummyValue, DummyError>>();
            result.As<Ok<DummyValue, DummyError>>().Value.Should().BeEquivalentTo(dummyValue);
        }

        [Fact]
        public async Task LiftToOk_ShouldWrapValueIntoOk()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncValue = Async.New(dummyValue);
            var asyncResult = AsyncResult.LiftToOk<DummyValue, DummyError>(asyncValue);

            var result = await asyncResult.ConfigureAwait();

            result.Should().BeOfType<Ok<DummyValue, DummyError>>();
            result.As<Ok<DummyValue, DummyError>>().Value.Should().BeEquivalentTo(dummyValue);
        }

        [Fact]
        public async Task LiftToError_ShouldWrapValueIntoError()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncError = Async.New(dummyError);
            var asyncResult = AsyncResult.LiftToError<DummyValue, DummyError>(asyncError);

            var result = await asyncResult.ConfigureAwait();

            result.Should().BeOfType<Error<DummyValue, DummyError>>();
            result.As<Error<DummyValue, DummyError>>().Err.Should().BeEquivalentTo(dummyError);
        }

        [Fact]
        public async Task Map_ShouldMapOkValue()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            var mapped = AsyncResult.Map(asyncResult, (value, ct) =>
                new DummyNewValue { NameAllCaps = value.Name.ToUpperInvariant() });

            var result = await mapped.ConfigureAwait();

            result.Should().BeOfType<Ok<DummyNewValue, DummyError>>();
            result.As<Ok<DummyNewValue, DummyError>>().Value.NameAllCaps.Should().Be("TEST");
        }

        [Fact]
        public async Task Map_ShouldNotMapError()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncResult = AsyncResult.CreateError<DummyValue, DummyError>(dummyError);

            var mapped = AsyncResult.Map(asyncResult, (value, ct) =>
                new DummyNewValue { NameAllCaps = value.Name.ToUpperInvariant() });

            var result = await mapped.ConfigureAwait();

            result.Should().BeOfType<Error<DummyNewValue, DummyError>>();
            result.As<Error<DummyNewValue, DummyError>>().Err.Should().BeEquivalentTo(dummyError);
        }

        [Fact]
        public async Task Bind_ShouldBindOkValue()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            AsyncResult<DummyNewValue, DummyError> Binder(DummyValue val, CancellationToken ct) =>
                AsyncResult.CreateOk<DummyNewValue, DummyError>(
                    new DummyNewValue { NameAllCaps = val.Name.ToUpperInvariant() });

            var bound = AsyncResult.Bind(asyncResult, Binder);

            var result = await bound.ConfigureAwait();

            result.Should().BeOfType<Ok<DummyNewValue, DummyError>>();
            result.As<Ok<DummyNewValue, DummyError>>().Value.NameAllCaps.Should().Be("TEST");
        }

        [Fact]
        public async Task Bind_ShouldReturnErrorIfOriginalIsError()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncResult = AsyncResult.CreateError<DummyValue, DummyError>(dummyError);

            AsyncResult<DummyNewValue, DummyError> Binder(DummyValue val, CancellationToken ct) =>
                AsyncResult.CreateOk<DummyNewValue, DummyError>(
                    new DummyNewValue { NameAllCaps = val.Name.ToUpperInvariant() });

            var bound = AsyncResult.Bind(asyncResult, Binder);

            var result = await bound.ConfigureAwait();

            result.Should().BeOfType<Error<DummyNewValue, DummyError>>();
            result.As<Error<DummyNewValue, DummyError>>().Err.Should().BeEquivalentTo(dummyError);
        }

        [Fact]
        public async Task Apply_ShouldApplyFunctionToOkValue()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            Func<DummyValue, CancellationToken, DummyNewValue> mapper = (val, ct) =>
                new DummyNewValue { NameAllCaps = val.Name.ToUpperInvariant() };

            var asyncMapper = AsyncResult.CreateOk<Func<DummyValue, CancellationToken, DummyNewValue>, DummyError>(mapper);

            var applied = AsyncResult.Apply(asyncResult, asyncMapper);

            var result = await applied.ConfigureAwait();

            result.Should().BeOfType<Ok<DummyNewValue, DummyError>>();
            result.As<Ok<DummyNewValue, DummyError>>().Value.NameAllCaps.Should().Be("TEST");
        }

        [Fact]
        public async Task Apply_ShouldReturnErrorIfMapperIsError()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            var dummyError = new DummyError { Message = "Mapper error" };
            var asyncMapper = AsyncResult.CreateError<Func<DummyValue, CancellationToken, DummyNewValue>, DummyError>(dummyError);

            var applied = AsyncResult.Apply(asyncResult, asyncMapper);

            var result = await applied.ConfigureAwait();

            result.Should().BeOfType<Error<DummyNewValue, DummyError>>();
            result.As<Error<DummyNewValue, DummyError>>().Err.Should().BeEquivalentTo(dummyError);
        }

        [Fact]
        public async Task MatchAsync_ShouldInvokeOkArmForOk()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            var matched = await AsyncResult.MatchAsync(
                asyncResult,
                (val, ct) => val.Name,
                (err, ct) => err.Message);

            matched.Should().Be("Test");
        }

        [Fact]
        public async Task MatchAsync_ShouldInvokeErrorArmForError()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncResult = AsyncResult.CreateError<DummyValue, DummyError>(dummyError);

            var matched = await AsyncResult.MatchAsync(
                asyncResult,
                (val, ct) => val.Name,
                (err, ct) => err.Message);

            matched.Should().Be("Error occurred");
        }

        [Fact]
        public async Task Unsafe_DoIfOkAsync_ShouldInvokeActionForOk()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            DummyValue? captured = null;
            await AsyncResult.Unsafe.DoIfOkAsync(asyncResult, val => captured = val);

            captured.Should().NotBeNull();
            captured.Should().BeEquivalentTo(dummyValue);
        }

        [Fact]
        public async Task Unsafe_DoIfOkAsync_ShouldNotInvokeActionForError()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncResult = AsyncResult.CreateError<DummyValue, DummyError>(dummyError);

            var invoked = false;
            await AsyncResult.Unsafe.DoIfOkAsync(asyncResult, val => invoked = true);

            invoked.Should().BeFalse();
        }

        [Fact]
        public async Task Unsafe_DoIfErrorAsync_ShouldInvokeActionForError()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncResult = AsyncResult.CreateError<DummyValue, DummyError>(dummyError);

            DummyError? captured = null;
            await AsyncResult.Unsafe.DoIfErrorAsync(asyncResult, err => captured = err);

            captured.Should().NotBeNull();
            captured.Should().BeEquivalentTo(dummyError);
        }

        [Fact]
        public async Task Unsafe_DoIfErrorAsync_ShouldNotInvokeActionForOk()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            var invoked = false;
            await AsyncResult.Unsafe.DoIfErrorAsync(asyncResult, err => invoked = true);

            invoked.Should().BeFalse();
        }

        [Fact]
        public async Task Unsafe_GetValueOrDefaultAsync_ShouldReturnValueForOk()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            var value = await AsyncResult.Unsafe.GetValueOrDefaultAsync(asyncResult);

            value.Should().BeEquivalentTo(dummyValue);
        }

        [Fact]
        public async Task Unsafe_GetValueOrDefaultAsync_ShouldReturnNullForError()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncResult = AsyncResult.CreateError<DummyValue, DummyError>(dummyError);

            var value = await AsyncResult.Unsafe.GetValueOrDefaultAsync(asyncResult);

            value.Should().BeNull();
        }

        [Fact]
        public async Task Unsafe_GetValueOrDefaultAsync_WithDefault_ShouldReturnValueForOk()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            var value = await AsyncResult.Unsafe.GetValueOrDefaultAsync(asyncResult, new DummyValue { Name = "Default", Email = "default@example.com" });

            value.Should().BeEquivalentTo(dummyValue);
        }

        [Fact]
        public async Task Unsafe_GetValueOrDefaultAsync_WithDefault_ShouldReturnDefaultForError()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncResult = AsyncResult.CreateError<DummyValue, DummyError>(dummyError);

            var defaultValue = new DummyValue { Name = "Default", Email = "default@example.com" };
            var value = await AsyncResult.Unsafe.GetValueOrDefaultAsync(asyncResult, defaultValue);

            value.Should().BeEquivalentTo(defaultValue);
        }

        [Fact]
        public async Task Unsafe_GetErrorOrDefaultAsync_ShouldReturnErrorForError()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncResult = AsyncResult.CreateError<DummyValue, DummyError>(dummyError);

            var error = await AsyncResult.Unsafe.GetErrorOrDefaultAsync(asyncResult);

            error.Should().BeEquivalentTo(dummyError);
        }

        [Fact]
        public async Task Unsafe_GetErrorOrDefaultAsync_ShouldReturnNullForOk()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            var error = await AsyncResult.Unsafe.GetErrorOrDefaultAsync(asyncResult);

            error.Should().BeNull();
        }

        [Fact]
        public async Task Unsafe_GetErrorOrDefaultAsync_WithDefault_ShouldReturnErrorForError()
        {
            var dummyError = new DummyError { Message = "Error occurred" };
            var asyncResult = AsyncResult.CreateError<DummyValue, DummyError>(dummyError);

            var defaultError = new DummyError { Message = "Default error" };
            var error = await AsyncResult.Unsafe.GetErrorOrDefaultAsync(asyncResult, defaultError);

            error.Should().BeEquivalentTo(dummyError);
        }

        [Fact]
        public async Task Unsafe_GetErrorOrDefaultAsync_WithDefault_ShouldReturnDefaultForOk()
        {
            var dummyValue = new DummyValue { Name = "Test", Email = "test@example.com" };
            var asyncResult = AsyncResult.CreateOk<DummyValue, DummyError>(dummyValue);

            var defaultError = new DummyError { Message = "Default error" };
            var error = await AsyncResult.Unsafe.GetErrorOrDefaultAsync(asyncResult, defaultError);

            error.Should().BeEquivalentTo(defaultError);
        }
    }
}
