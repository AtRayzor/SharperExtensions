using Microsoft.Extensions.Logging;

namespace NetFunctional.Types;

public interface ILoggableError : IErrorWithMessage
{
    LogLevel LogLevel { get; }
}
