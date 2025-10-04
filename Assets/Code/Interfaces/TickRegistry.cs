using System;
using System.Collections.Generic;
using System.Linq;

public class TickRegistry
{
    private readonly Dictionary<Type, TickCategory> _categories;
    private List<TickCategory> _sortedCategories;
    private bool _needsSort;

    public TickRegistry()
    {
        _categories = new Dictionary<Type, TickCategory>();
        _sortedCategories = new List<TickCategory>();
        _needsSort = false;
    }

    // Регистрация сущности
    public void Register<T>(T entity) where T : ITick
    {
        var category = GetOrCreateCategory<T>();
        category.Add(entity);
    }

    // Отмена регистрации сущности
    public void Unregister<T>(T entity) where T : ITick
    {
        var type = typeof(T);
        if (_categories.TryGetValue(type, out var category))
        {
            category.MarkForRemoval(entity);
        }
    }

    // Получение категории (с автоматическим созданием)
    public TickCategory GetOrCreateCategory<T>(int priority = 0) where T : ITick
    {
        var type = typeof(T);
        if (!_categories.TryGetValue(type, out var category))
        {
            category = new TickCategory(priority);
            _categories[type] = category;
            _sortedCategories.Add(category);
            _needsSort = true;
        }
        return category;
    }

    // Получение существующей категории
    public TickCategory GetCategory<T>() where T : ITick
    {
        var type = typeof(T);
        return _categories.TryGetValue(type, out var category) ? category : null;
    }

    // Получение всех сущностей типа T
    public (TickCategory, IEnumerable<T>) GetAll<T>() where T : ITick
    {
        var category = GetCategory<T>();
        return (category, category?.Entities.Cast<T>() ?? Enumerable.Empty<T>());
    }

    // Обработка всех категорий - FixedTick
    public void ProcessAllFixedTick(float deltaTime)
    {
        SortCategoriesIfNeeded();

        foreach (var category in _sortedCategories)
        {
            category.ProcessFixedTick(deltaTime);
        }
    }

    // Обработка всех категорий - Tick
    public void ProcessAllTick(float deltaTime)
    {
        SortCategoriesIfNeeded();

        foreach (var category in _sortedCategories)
        {
            category.ProcessTick(deltaTime);
        }
    }

    // Обработка всех категорий - LateTick
    public void ProcessAllLateTick(float deltaTime)
    {
        SortCategoriesIfNeeded();

        foreach (var category in _sortedCategories)
        {
            category.ProcessLateTick(deltaTime);
        }
    }

    // Применение удалений для всех категорий
    public void ApplyAllRemovals()
    {
        foreach (var category in _categories.Values)
        {
            category.ApplyRemovals();
        }
    }

    // Сортировка категорий по приоритету
    private void SortCategoriesIfNeeded()
    {
        if (_needsSort)
        {
            _sortedCategories = _sortedCategories.OrderBy(c => c.Priority).ToList();
            _needsSort = false;
        }
    }

    // Установка приоритета категории
    public void SetCategoryPriority<T>(int priority) where T : ITick
    {
        var category = GetCategory<T>();
        if (category != null)
        {
            category.Priority = priority;
            _needsSort = true;
        }
    }

    // Очистка всех категорий
    public void Clear()
    {
        foreach (var category in _categories.Values)
        {
            category.Clear();
        }
        _categories.Clear();
        _sortedCategories.Clear();
    }

    // Получение количества категорий
    public int CategoryCount => _categories.Count;

    // Получение общего количества сущностей
    public int TotalEntityCount
    {
        get
        {
            int total = 0;
            foreach (var category in _categories.Values)
            {
                total += category.Count;
            }
            return total;
        }
    }
}