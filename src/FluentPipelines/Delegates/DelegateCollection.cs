using System.Collections;

namespace FluentPipelines.Delegates;

internal class DelegateCollection<TWrapper> : IList<TWrapper> where TWrapper : IDelegateWrapper
{
    private readonly List<TWrapper> _delegates = [];

    public IEnumerator<TWrapper> GetEnumerator() => _delegates.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public void Add(TWrapper item) => _delegates.Add(item);


    public void Clear() => _delegates.Clear();


    public bool Contains(TWrapper item) => _delegates.Contains(item);


    public void CopyTo(TWrapper[] array, int arrayIndex) => _delegates.CopyTo(array, arrayIndex);


    public bool Remove(TWrapper item) => _delegates.Remove(item);


    public int Count => _delegates.Count;
    public bool IsReadOnly { get; private set; }

    public int IndexOf(TWrapper item) => _delegates.IndexOf(item);

    public void Insert(int index, TWrapper item) => _delegates.Insert(index, item);

    public void RemoveAt(int index) => _delegates.RemoveAt(index);

    public TWrapper this[int index]
    {
        get => _delegates[index];
        set => _delegates[index] = value;
    }

    public void SetAsReadOnly() => IsReadOnly = true;
}