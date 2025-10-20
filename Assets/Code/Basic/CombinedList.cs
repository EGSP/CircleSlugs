using System;
using System.Collections;
using System.Collections.Generic;

public class CombinedList<T> : IReadOnlyList<T>
{
    private readonly List<IReadOnlyList<T>> _lists;

    public CombinedList()
    {
        _lists = new List<IReadOnlyList<T>>();
    }

    public CombinedList(params IReadOnlyList<T>[] lists)
    {
        _lists = new List<IReadOnlyList<T>>(lists);
    }

    public CombinedList<T> AddList(IReadOnlyList<T> list)
    {
        if (list != null)
            _lists.Add(list);
        return this;
    }

    public CombinedList<T> RemoveList(IReadOnlyList<T> list)
    {
        _lists.Remove(list);
        return this;
    }

    public CombinedList<T> Clear()
    {
        _lists.Clear();
        return this;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            for (int i = 0; i < _lists.Count; i++)
            {
                if (index < _lists[i].Count)
                    return _lists[i][index];
                index -= _lists[i].Count;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public int Count
    {
        get
        {
            int count = 0;
            for (int i = 0; i < _lists.Count; i++)
                count += _lists[i].Count;
            return count;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _lists.Count; i++)
        {
            IReadOnlyList<T> list = _lists[i];
            for (int j = 0; j < list.Count; j++)
            {
                yield return list[j];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}