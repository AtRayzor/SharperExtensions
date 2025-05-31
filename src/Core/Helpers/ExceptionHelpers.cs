namespace SharperExtensions;

public static class ExceptionHelpers
{
    public static TException TryToConstructException<TException>(
        object[]? constructorArgs = default
    )
        where TException : Exception
    {
        if (
            Activator.CreateInstance(typeof(TException), constructorArgs ?? [])
            is not TException activatedException
        )
            throw new InvalidOperationException(
                $"No matching constructor found for the given exception type '{typeof(Exception).FullName}'."
            );

        return activatedException;
    }
}
