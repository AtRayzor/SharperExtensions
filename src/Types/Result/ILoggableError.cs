using Microsoft.Extensions.Logging;

namespace DotNetCoreFunctional.Result;

public interface ILoggableError : IErrorWithMessage
{
    LogLevel LogLevel { get; }
}
