namespace FluentPipelines.Delegates;

internal static class DelegateWrapperExtensions
{
    public static bool IsEquivalent(this IDelegateWrapper wrapper, IDelegateWrapper otherWrapper)
        => wrapper.GetType() == otherWrapper.GetType() || wrapper.Delegate.GetType() == otherWrapper.Delegate.GetType();
}