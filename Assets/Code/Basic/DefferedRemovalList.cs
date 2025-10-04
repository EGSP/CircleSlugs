using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class DeferredRemovalList<T>
{
    private List<T> _list;
    private List<T> _removals;

    public IReadOnlyList<T> List => _list.AsReadOnly();
    public IReadOnlyList<T> Removals => _removals.AsReadOnly();

    public DeferredRemovalList()
    {
        _list = new List<T>();
        _removals = new List<T>();
    }

    public DeferredRemovalList(IEnumerable<T> collection)
    {
        _list = new List<T>(collection);
        _removals = new List<T>();
    }

    // Добавление элемента в основной список
    public void Add(T item)
    {
        _list.Add(item);
    }

    // Пометка элемента на удаление
    public void MarkForRemoval(T item)
    {
        if (_list.Contains(item) && !_removals.Contains(item))
        {
            _removals.Add(item);
        }
    }

    // Применение отложенного удаления
    public void ApplyRemovals()
    {
        foreach (var item in _removals)
        {
            _list.Remove(item);
        }
        _removals.Clear();
    }

    // Получение количества элементов в основном списке
    public int Count => _list.Count;

    // Получение количества элементов на удаление
    public int RemovalCount => _removals.Count;

    // Доступ по индексу
    public T this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    // Проверка содержания элемента в основном списке
    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    // Очистка основного списка
    public void Clear()
    {
        _list.Clear();
        _removals.Clear();
    }

    // Получение всех элементов основного списка
    public IReadOnlyList<T> GetAll()
    {
        return _list.AsReadOnly();
    }

    // Получение элементов, помеченных на удаление
    public IReadOnlyList<T> GetMarkedForRemoval()
    {
        return _removals.AsReadOnly();
    }

    // Перебор элементов основного списка
    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }
}