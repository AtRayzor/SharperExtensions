using System.Collections;

namespace FluentPipelines.Delegates;

public class DuplicateStepHandlersException(string message) : Exception(message);

public class StepDelegateHandlerInvocationException(string message) : Exception(message);