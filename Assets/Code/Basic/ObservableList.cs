using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

// Readonly интерфейс для внешнего доступа
public interface IReadOnlyObservableList<T> : IReadOnlyList<T>
{
    void OnChanged(Action callback);
    void UnsubscribeOnChanged(Action callback);
}

public class ObservableList<T> : IReadOnlyObservableList<T>, IList<T>
{
    private readonly List<T> _items;
    private readonly List<Action> _changed;

    public ObservableList()
    {
        _items = new List<T>();
        _changed = new List<Action>();
    }

    public ObservableList(int capacity)
    {
        _items = new List<T>(capacity);
        _changed = new List<Action>();
    }

    public ObservableList(IEnumerable<T> collection)
    {
        _items = new List<T>(collection);
        _changed = new List<Action>();
    }

    // Подписки
    public void OnChanged(Action callback)
    {
        if (callback != null)
            _changed.Add(callback);
    }

    public void UnsubscribeOnChanged(Action callback)
    {
        if (callback != null)
            _changed.Remove(callback);
    }

    // Вызов события
    private void Changed()
    {
        for (int i = 0; i < _changed.Count; i++)
            _changed[i]?.Invoke();
    }

    // IList<T> реализация
    public void Add(T item)
    {
        _items.Add(item);
        Changed();
    }

    public bool Remove(T item)
    {
        bool removed = _items.Remove(item);
        if (removed)
            Changed();
        return removed;
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
        Changed();
    }

    public void Insert(int index, T item)
    {
        _items.Insert(index, item);
        Changed();
    }

    public void Clear()
    {
        if (_items.Count == 0) return;
        _items.Clear();
        Changed();
    }

    public T this[int index]
    {
        get => _items[index];
        set
        {
            _items[index] = value;
            Changed();
        }
    }

    // IReadOnlyList<T>
    public int Count => _items.Count;
    public bool IsReadOnly => false;

    public int IndexOf(T item) => _items.IndexOf(item);
    public bool Contains(T item) => _items.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class ObservableListExtensions
{
    public static IReadOnlyObservableList<T> AsReadOnly<T>(this ObservableList<T> list)
    {
        return list;
    }
}