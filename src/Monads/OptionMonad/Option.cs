using System.Diagnostics.CodeAnalysis;

namespace Monads.OptionMonad;

public static class Option
{
    public static Option<TValue> Some<TValue>(TValue value) where TValue : notnull => value;
    public static Option<TValue> None<TValue>() where TValue : notnull => new Option<TValue>.None();
}

public abstract record Option<TValue> where TValue : notnull
{
    public record Some(TValue Value) : Option<TValue>
    {
        public static implicit operator TValue(Some someOption) => someOption.Value;
        public static implicit operator Some(TValue value) => new(value);
    }

    public record None : Option<TValue>;

    public static implicit operator Option<TValue>(TValue? value) =>
        value is not null ? new Some(value) : new None();


    public Option<TNewValue> Map<TNewValue>(Func<TValue, Option<TNewValue>> fSome, Func<Option<TNewValue>> fNone)
        where TNewValue : notnull =>
        this switch
        {
            Some someValue => fSome(someValue),
            None => fNone()
        };
    
    public Option<TNewValue> Map<TNewValue>(Func<TValue, Option<TNewValue>> fSome) where TNewValue : notnull =>
        Map(fSome, () => new Option<TNewValue>.None());
    
    public bool IsSome() => this is Some;
    public bool IsNone() => this is None;

    public bool GetValueIfSome([NotNullWhen(returnValue: true)] out TValue? value)
    {
        value = this is Some some ? some.Value : default;
        return value is not null;
    }

    public TValue GetValueOrThrow<TException>(string errorMessage) where TException : Exception
    {
        ThrowExceptionIfIsNone<TException>([errorMessage]);

        return (Some)this;
    }

    public void ThrowIfNone<TException>() where TException : Exception => ThrowExceptionIfIsNone<TException>();

    public void ThrowIfNone<TException>(string message) where TException : Exception =>
        ThrowExceptionIfIsNone<TException>([message]);

    public void ThrowIfNone<TException>(object[] args) where TException : Exception =>
        ThrowExceptionIfIsNone<TException>(args);

    private void ThrowExceptionIfIsNone<TException>(object[]? constructorArgs = default) where TException : Exception
    {
        if (this is not None)
        {
            return;
        }

        var exception = constructorArgs is null
            ? Activator.CreateInstance<TException>()
            : Activator.CreateInstance(typeof(TException), constructorArgs) as TException
              ?? throw new InvalidOperationException(
                  "No constructor was found for the given exception type which matches given arguments."
              );

        throw exception;
    }


    public void ExecuteIfSome(Action<TValue> action)
    {
        if (GetValueIfSome(out var value))
        {
            action(value);
        }
    }

    private void ExecuteActionOrThrowException<TException>(Action<TValue> action, object[]? exceptionArgs = default)
        where TException : Exception
    {
        switch (this)
        {
            case Some some:
                action(some);
                break;
            case None:
                ThrowExceptionIfIsNone<TException>(exceptionArgs);
                break;
        }
    }

    public void ExecuteOrThrow<TException>(Action<TValue> action) where TException : Exception =>
        ExecuteActionOrThrowException<TException>(action);

    public void ExecuteOrThrow<TException>(Action<TValue> action, string message) where TException : Exception =>
        ExecuteActionOrThrowException<TException>(action, [message]);

    public void ExecuteOrThrow<TException>(Action<TValue> action, object[] exceptionArgs)
        where TException : Exception =>
        ExecuteActionOrThrowException<TException>(action, exceptionArgs);
}