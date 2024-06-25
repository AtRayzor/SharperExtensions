using System.Collections;

namespace FluentPipelines.Delegates;

internal class DelegateCollection : IList<Delegate>
{
    private readonly List<Delegate> _delegates = [];

    public IEnumerator<Delegate> GetEnumerator() => _delegates.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public void Add(Delegate item) => _delegates.Add(item);


    public void Clear() => _delegates.Clear();


    public bool Contains(Delegate item) => _delegates.Contains(item);


    public void CopyTo(Delegate[] array, int arrayIndex) => _delegates.CopyTo(array, arrayIndex);


    public bool Remove(Delegate item) => _delegates.Remove(item);


    public int Count => _delegates.Count;
    public bool IsReadOnly { get; private set; }

    public int IndexOf(Delegate item) => _delegates.IndexOf(item);

    public void Insert(int index, Delegate item) => _delegates.Insert(index, item);

    public void RemoveAt(int index) => _delegates.RemoveAt(index);

    public Delegate this[int index]
    {
        get => _delegates[index];
        set => _delegates[index] = value;
    }

    public void SetAsReadOnly() => IsReadOnly = true;

    public void AddStepHandlerDelegate<TRequest, TResponse, TNext>(
        StepHandlerDelegate<TRequest, TResponse> handlerDelegate)
    {
    }
}