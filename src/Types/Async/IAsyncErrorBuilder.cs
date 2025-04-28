namespace DotNetCoreFunctional.Async;

public interface IAsyncErrorBuilder<out TError>
    where TError : notnull
{
    TError CreateError(Exception exception);
}
