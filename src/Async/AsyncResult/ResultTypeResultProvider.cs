using SharperExtensions.Collections;

namespace SharperExtensions.Async;

internal class ResultTypeResultProvider<T, TError>(
    AsyncMutableState<T> state,
    Func<TError> defaultErrorFactory,
    Option<Func<Exception, TError>> exceptionHandler = default
)
    : ResultProvider<T, Result<T, TError>>(state)
    where T : notnull
    where TError : notnull
{
    public override Result<T, TError> GetResult()
    {
        return State
            .Result
            .Map(box => box.Value)
            .Bind(result =>
                exceptionHandler.Map(handler => result.MapError(handler))
            )
            .Match(result => result, ErrorResultFactory);

        Result<T, TError> ErrorResultFactory() =>
            Result<T, TError>.Error(defaultErrorFactory());
    }
}

internal sealed class ResultAsyncResultProvider<T, TError>(
    AsyncMutableState<Result<T, TError>> state,
    Option<Func<TError>> defaultErrorFactory = default,
    Option<Func<Exception, TError>> exceptionHandler = default
) : ResultProvider<Result<T, TError>, Result<T, TError>>(state)
    where T : notnull where TError : notnull
{
    public override Result<T, TError> GetResult()
    {
        var resultOption = State.Result.Map(box => box.Value);

        OptionSequence<Result<T, TError>> sequence =
        [
            exceptionHandler.Bind(handler =>
                resultOption.Map(result => result.MapError(handler).Flatten())
            ),
            resultOption.Bind(result =>
                result.Match(
                    inner =>
                        inner.ToOption(),
                    _ => defaultErrorFactory.Map(factory =>
                        Result<T, TError>.Error(factory())
                    )
                )
            ),
        ];

        return sequence
            .TakeFirstSome()
            .GetValueOrThrow<Result<T, TError>, InvalidOperationException>();
    }
}