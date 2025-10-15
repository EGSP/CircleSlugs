using UnityEngine;

public interface ITickAccessor
{
    TickCategory Category { get; }
}

public static class TickCategoryAccessorExtensions
{
    /// <summary>
    /// Создать accessor для работы с первым элементом
    /// </summary>
    public static FirstTickAccessor<T> AsFirstEntity<T>(this TickCategory category)
        where T : class
    {
        return new FirstTickAccessor<T>(category);
    }

    /// <summary>
    /// Создать accessor для работы с случайным элементом
    /// </summary>
    public static RandomEntityAccessor<T> AsRandomEntity<T>(this TickCategory category)
        where T : class
    {
        return new RandomEntityAccessor<T>(category);
    }
}

// ============================================
// Accessor для работы с первым элементом
// ============================================
public class FirstTickAccessor<T> : ITickAccessor
    where T : class
{
    public TickCategory Category { get; }

    public bool ShouldCache{ get; private set; }

    /// <summary>
    /// Первый элемент или null, если коллекция пуста
    /// </summary>
    public T Entity
    {
        get
        {
            if (ShouldCache)
            {
                if (_cachedEntity == null)
                {
                    _cachedEntity = Category.Count > 0 ? Category.Entities[0] as T : null;
                }
                return _cachedEntity;
            }
            return Category.Count > 0 ? Category.Entities[0] as T : null;
        }
    }

    private T _cachedEntity = null;

    public FirstTickAccessor(TickCategory category)
    {
        Category = category;
    }

    public FirstTickAccessor<T> Cache(bool shouldCache)
    {
        ShouldCache = shouldCache;
        return this;
    }
}

public class RandomEntityAccessor<T> : ITickAccessor
    where T : class
{
    public TickCategory Category { get; }

    /// <summary>
    /// Случайный элемент или null, если коллекция пуста
    /// </summary>
    public T Entity => Category.Count > 0 ? Category.Entities[Random.Range(0, Category.Count)] as T : null;

    public RandomEntityAccessor(TickCategory category)
    {
        Category = category;
    }
}