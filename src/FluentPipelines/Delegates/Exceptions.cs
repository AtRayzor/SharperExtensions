using System.Collections;

namespace FluentPipelines.Delegates;

public class StepDelegateHandlerInvocationException(string message) : Exception(message);

public class NoFinalHandlerException(string message) : Exception(message);

public class NoHandlersRegisteredException(string message) : Exception(message);