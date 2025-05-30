using System.Diagnostics;
using DotNetCoreFunctional.Option;
using DotNetCoreFunctional.Result;

namespace DotNetCoreFunctional.Sequence;

public static class EnumerableToSequenceExtensions
{
      extension<T>(IEnumerable<T> enumerable)
        where T : notnull
    {
        public Sequence<T> ToSequence() => SequenceInstance.Create(enumerable);
    }

    extension<T>(IEnumerable<T> enumerable)
        where T : IEquatable<T>
    {
        public EquatableSequence<T> ToEquatableSequence() =>
            EquatableSequence.Create(enumerable);
    }

    extension<T>(IEnumerable<T?> enumerable)
        where T : notnull
    {
        public OptionSequence<T> ToOptionSequence() =>
            OptionSequence.Create(enumerable);
    }

    extension<T>(IEnumerable<Option<T>> options)
       where T : notnull
    {
        public OptionSequence<T> ToOptionSequence() => [.. options];
    }

    extension<T, TError>(IEnumerable<Result<T, TError>> results)
        where T : notnull
        where TError : notnull

    {
        public ResultSequence<T, TError> ToResultSequence() => [.. results];
    }
}

public static class SequenceExtensions
{


    extension<T>(ISequence<T> generalSequence)
        where T : IEquatable<T>
    {
        public EquatableSequence<T> ToEquatbleSequence() => new(generalSequence.AsSequence());
    }

    extension<T>(ISequence<Option<T>> options)
        where T : notnull
    {
        public OptionSequence<T> ToOptionSequence() => new(options.AsSequence());
    }

        extension<T, TError>(ISequence<Result<T, TError>> results)
        where T : notnull
        where TError : notnull
    {
        public ResultSequence<T, TError> ToResultSequence() => new(results.AsSequence());
    }


    extension<T>(Sequence<T> sequence)
        where T : notnull
    {
        public Sequence<TResult> Select<TResult>(Func<T, TResult> selector)
            where TResult : notnull =>
            SequenceInstance
                .Create(SpanIteration.ApplySelector(sequence.AsSpan(), selector));

        public Sequence<TResult> SelectMany<TResult>(Func<T, Sequence<TResult>> selector)
            where TResult : notnull
        {
            return SequenceInstance
                .Create(
                    SpanIteration.ApplySelectorToMany(sequence.AsSpan(), SpanSelector)
                );

            ReadOnlySpan<TResult> SpanSelector(T item) => selector(item).AsSpan();
        }


        public Sequence<T> Where(Predicate<T> predicate)
        {
            var sequenceSpan = sequence.AsSpan();
            var index = 0;
            Span<T> destination = new T[sequenceSpan.Length];

            foreach (var item in sequenceSpan)
            {
                if (!predicate(item))
                {
                    continue;
                }

                destination[index++] = item;
            }

            return [.. destination[..index] ];
        }

    }

    extension<T>(EquatableSequence<T> equatableSequence)
        where T : IEquatable<T>
    {
        public Sequence<TResult> Select<TResult>(Func<T, TResult> selector)
            where TResult : notnull => SequenceInstance
            .Create(
                SpanIteration.ApplySelector(equatableSequence.AsSpan(), selector)
            );

        public Sequence<TResult> SelectMany<TResult>(Func<T, Sequence<TResult>> selector)
            where TResult : notnull
        {

            return SequenceInstance
                .Create(
                    SpanIteration
                        .ApplySelectorToMany(equatableSequence.AsSpan(), SpanSelector)
                );

            ReadOnlySpan<TResult> SpanSelector(T item) => selector(item).AsSpan();

        }

        public EquatableSequence<T> Where(Predicate<T> predicate) => new(equatableSequence.AsSequence().Where(predicate));
    }

    extension<T>(OptionSequence<T> optionSequence)
        where T : notnull
    {
        public OptionSequence<TResult> Select<TResult>(Func<T, TResult> selector)
            where TResult : notnull
        {
            return [.. optionSequence.AsSequence().Select(OptionSelector)];

            Option<TResult> OptionSelector(Option<T> option) => option.Map(selector);
        }

        public OptionSequence<TResult> SelectMany<TResult>(
            Func<T, OptionSequence<TResult>> selector
        )
            where TResult : notnull
        {
            return [.. optionSequence.AsSequence().SelectMany(SequenceSelector)];

            Sequence<Option<TResult>> SequenceSelector(Option<T> option) =>
                option.Match(value =>
                        selector(value).AsSequence(),
                    () => []
                );

        }

        public OptionSequence<T> Where(Predicate<Option<T>> predicate) => new(optionSequence.AsSequence().Where(predicate));

        public OptionSequence<T> Where(Predicate<T> predicate) =>
            Where(optionSequence, option => option.TryGetValue(out var value) && predicate(value));

        public OptionSequence<T> WhereNotNone() => Where(optionSequence, option => option.IsNone);
    }

    extension<T, TError>(ResultSequence<T, TError> results)
        where T : notnull
        where TError : notnull
    {
        public ResultSequence<TResult, TError> Select<TResult>(Func<T, TResult> selector)
            where TResult : notnull
        {

            return new ResultSequence<TResult, TError>(results.AsSequence().Select(ResultSelector));

            Result<TResult, TError> ResultSelector(Result<T, TError> result) => result.Map(selector);

        }

        public ResultSequence<TResult, TError> SelectMany<TResult>(Func<T, ResultSequence<TResult, TError>> selector)
            where TResult : notnull
        {
            return [.. results.AsSequence().SelectMany(SequenceSelector)];

            Sequence<Result<TResult, TError>> SequenceSelector(Result<T, TError> result) =>
            result.Match(
                value => selector(value).AsSequence(),
                error => [Result<TResult, TError>.Error(error)]
            );
        }

        public ResultSequence<T, TError> Where(Predicate<Result<T, TError>> predicate) =>
            new(results.AsSequence().Where(predicate));

        public ResultSequence<T, TError> Where(Predicate<T> predicate) =>
            Where(results, result => result.TryGetValue(out var value) && predicate(value));

        public ResultSequence<T, TError> WhereError(Predicate<TError> predicate) =>
            Where(results, result => result.TryGetError(out var error) && predicate(error));
    }
}

internal static class SpanIteration
{

    public static ReadOnlySpan<TResult> ApplySelector<TSource, TResult>(
        ReadOnlySpan<TSource> itemSpan,
        Func<TSource, TResult> selector
    )
    {
        Span<TResult> destionation = new TResult[itemSpan.Length];

        for (var i = 0; i < itemSpan.Length; i++)
        {
            var item = itemSpan[i];
            destionation[i] = selector(item);
        }

        return destionation;
    }

    public static ReadOnlySpan<TResult> ApplySelectorToMany<TSource, TResult>(
        ReadOnlySpan<TSource> sourceSpan,
        Func<TSource, ReadOnlySpan<TResult>> spanSelector
    )
    {
        var bufferLength = 200;
        Span<TResult> buffer = new TResult[bufferLength];
        var bufferIndex = -1;

        foreach (var sourceItem in sourceSpan)
        {
            var resultSpan = spanSelector(sourceItem);

            foreach (var resultItem in resultSpan)
            {
                if (++bufferIndex == buffer.Length)
                {
                    bufferLength *= 2;
                    var tempBuffer = buffer;
                    buffer = new TResult[bufferLength];
                    tempBuffer.CopyTo(buffer);
                }

                buffer[bufferIndex] = resultItem;
            }

        }
        return buffer[..bufferIndex];
    }

}
