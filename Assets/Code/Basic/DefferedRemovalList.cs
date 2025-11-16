using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class DeferredRemovalList<T>
{
    private ObservableList<T> _list;
    private ObservableList<T> _removals;

    public IReadOnlyObservableList<T> List => _list.AsReadOnly();
    public IReadOnlyObservableList<T> Removals => _removals.AsReadOnly();

    public DeferredRemovalList()
    {
        _list = new ObservableList<T>();
        _removals = new ObservableList<T>();
    }

    public DeferredRemovalList(IEnumerable<T> collection)
    {
        _list = new ObservableList<T>(collection);
        _removals = new ObservableList<T>();
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

    // Перебор элементов основного списка
    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }
}