namespace SharperExtensions.Collections;

/// <summary>
/// Provides static extension methods for performing operations on sequences of optional values.
/// </summary>
/// <remarks>
/// This class contains utility methods for working with <see cref="OptionSequence{T}"/> 
/// and extracting values from optional sequences.
/// </remarks>
public static class OptionSequenceOperations
{
    /// <summary>
    /// Retrieves the first <see cref="Option{T}"/> with a value from a sequence of optional values.
    /// </summary>
    /// <param name="options">A sequence of optional values to search through.</param>
    /// <returns>The first <see cref="Option{T}"/> with a value, or <see cref="Option{T}.None"/> if no values are found.</returns>
    /// <typeparam name="T">The type of values in the optional sequence.</typeparam>
    public static Option<T> TakeFirstSome<T>(params OptionSequence<T> options)
        where T : notnull
    {
        foreach (var opt in options.AsSpan())
        {
            if (!opt.IsSome)
            {
                continue;
            }

            return opt;
        }

        return Option<T>.None;
    }

    /// <summary>
    /// Retrieves the last <see cref="Option{T}"/> with a value from a sequence of optional values.
    /// </summary>
    /// <param name="options">A sequence of optional values to search through.</param>
    /// <returns>The last <see cref="Option{T}"/> with a value, or <see cref="Option{T}.None"/> if no values are found.</returns>
    /// <typeparam name="T">The type of values in the optional sequence.</typeparam>
    public static Option<T> TakeLastSome<T>(params OptionSequence<T> options)
        where T : notnull
    {
        var span = options.AsSpan();
        for (var i = span.Length - 1; i >= 0; i--)
        {
            if (!span[i].IsSome)
            {
                continue;
            }

            return span[i];
        }

        return Option<T>.None;
    }
}