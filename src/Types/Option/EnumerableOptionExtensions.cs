using System.Diagnostics.Contracts;

namespace DotNetCoreFunctional.Option;

public static class EnumerableOptionExtensions
{
    public static Option<T> FirstOrNone<T>(this IEnumerable<T> source)
        where T : notnull => source.FirstOrDefault().ToOption();

    public static Option<T> SingleOrNone<T>(this IEnumerable<T> source)
        where T : notnull => source.SingleOrDefault().ToOption();

    [Pure]
    public static IEnumerable<Option<TResult>> MapAll<TSource, TResult>(
        this IEnumerable<Option<TSource>> options,
        Func<TSource, TResult> mapping
    )
        where TSource : notnull
        where TResult : notnull => options.Select(op => op.Map(mapping));

    [Pure]
    public static IEnumerable<Option<TResult>> BindAll<TSource, TResult>(
        this IEnumerable<Option<TSource>> options,
        Func<TSource, Option<TResult>> binder
    )
        where TSource : notnull
        where TResult : notnull => options.Select(op => op.Bind(binder));

    [Pure]
    public static IEnumerable<Option<TResult>> ApplyAll<TSource, TResult>(
        this IEnumerable<Option<TSource>> options,
        Option<Func<TSource, TResult>> wrappedMapping
    )
        where TSource : notnull
        where TResult : notnull => options.Select(op => op.Apply(wrappedMapping));

    [Pure]
    public static IEnumerable<T> SelectIfSome<T>(this IEnumerable<Option<T>> options)
        where T : notnull => options.SelectValueIfSome(new None<Func<T, T>>());

    [Pure]
    public static IEnumerable<TResult> SelectIfSome<TSource, TResult>(
        this IEnumerable<Option<TSource>> options,
        Func<TSource, TResult> selector
    )
        where TSource : notnull
        where TResult : notnull =>
        options.SelectValueIfSome(new Some<Func<TSource, TResult>>(selector));

    [Pure]
    private static IEnumerable<TResult> SelectValueIfSome<TSource, TResult>(
        this IEnumerable<Option<TSource>> options,
        Option<Func<TSource, TResult>> optionalSelector
    )
        where TSource : notnull
        where TResult : notnull =>
        options
            .Select(op => op.Apply(optionalSelector))
            .Where(op => op.IsSome())
            .Select(op => ((Some<TResult>)op).Value);
}
