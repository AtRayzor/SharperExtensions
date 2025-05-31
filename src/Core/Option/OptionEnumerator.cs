using System.Collections;

namespace SharperExtensions;

public class OptionEnumerator<T> : IEnumerator<T>
    where T : notnull
{
    private readonly T[] _values;
    private int _index = -1;

    public OptionEnumerator(IEnumerable<T> values)
    {
        _values = values.ToArray();
    }

    public OptionEnumerator(Option<T> option)
    {
        _values = option switch
        {
            Some<T> s => [s.Value],
            _ => [],
        };
    }

    public void Dispose() { }

    public bool MoveNext()
    {
        return ++_index < _values.Length;
    }

    public void Reset()
    {
        _index = -1;
    }

    public T Current => _values[_index];

    object? IEnumerator.Current => Current;
}
