using System.Collections;

namespace FluentPipelines.Delegates;

internal class DelegateCollection : IList<DelegateContainer>
{
    private readonly List<DelegateContainer> _delegates = [];

    public IEnumerator<DelegateContainer> GetEnumerator() => _delegates.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public void Add(DelegateContainer item) => _delegates.Add(item);


    public void Clear() => _delegates.Clear();


    public bool Contains(DelegateContainer item) => _delegates.Contains(item);


    public void CopyTo(DelegateContainer[] array, int arrayIndex) => _delegates.CopyTo(array, arrayIndex);


    public bool Remove(DelegateContainer item) => _delegates.Remove(item);


    public int Count => _delegates.Count;
    public bool IsReadOnly { get; private set; }

    public int IndexOf(DelegateContainer item) => _delegates.IndexOf(item);

    public void Insert(int index, DelegateContainer item) => _delegates.Insert(index, item);

    public void RemoveAt(int index) => _delegates.RemoveAt(index);

    public DelegateContainer this[int index]
    {
        get => _delegates[index];
        set => _delegates[index] = value;
    }

    public void SetAsReadOnly() => IsReadOnly = true;

    public void AddStepHandlerDelegate<TRequest, TResponse, TNext>(
        StepHandlerDelegate<TRequest, TResponse, TNext> handlerDelegate)
    {
        Add(new DelegateContainer(handlerDelegate, handlerDelegate.GetType().GetGenericTypeDefinition(),
            typeof(TRequest), typeof(TResponse), typeof(TNext)));
    }

    public void AddStepHandlerDelegate<TRequest, TResponse>(StepHandlerDelegate<TRequest, TResponse> handlerDelegate)
    {
        Add(new DelegateContainer(handlerDelegate, handlerDelegate.GetType().GetGenericTypeDefinition(),
            typeof(TRequest), typeof(TResponse)));
    }
}