using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public enum ProcessingMode
{
    Standard,   // Реестр сам обрабатывает сущности
    Delegated   // Используется кастомный обработчик
}

public interface ITickProcessor
{
    void Tick(IReadOnlyList<ITick> entities, float deltaTime);
    void FixedTick(IReadOnlyList<ITick> entities, float deltaTime);
    void LateTick(IReadOnlyList<ITick> entities, float deltaTime);
}

public class TickCategory
{
    private readonly DeferredRemovalList<ITick> _entities;

    public int Priority { get; set; }

    public ProcessingMode Mode => TickProcessor == null ? ProcessingMode.Standard : ProcessingMode.Delegated;

    public ITickProcessor TickProcessor { get; set; }

    public int Count => _entities.Count;

    public IReadOnlyList<ITick> Entities => _entities.List;

    public UnityEvent<ITick> Added = new();

    public TickCategory(int priority = 0)
    {
        _entities = new DeferredRemovalList<ITick>();
        Priority = priority;
    }

    public void Add(ITick entity)
    {
        if (!_entities.Contains(entity))
        {
            entity.OnTerminateMark.AddListener(() => MarkForRemoval(entity));
            _entities.Add(entity);

            Added.Invoke(entity);
        }
    }

    public void MarkForRemoval(ITick entity)
    {
        _entities.MarkForRemoval(entity);
    }

    public bool Contains(ITick entity)
    {
        return _entities.Contains(entity);
    }

    // Обработка FixedTick
    public void ProcessFixedTick(float deltaTime)
    {
        if (Mode == ProcessingMode.Standard)
        {
            foreach (var entity in _entities)
            {
                entity.FixedTick(deltaTime);
            }
        }
        else if (Mode == ProcessingMode.Delegated)
        {
            TickProcessor.FixedTick(_entities.List, deltaTime);
        }
    }

    // Обработка Tick
    public void ProcessTick(float deltaTime)
    {
        if (Mode == ProcessingMode.Standard)
        {
            foreach (var entity in _entities)
            {
                entity.Tick(deltaTime);
            }
        }
        else if (Mode == ProcessingMode.Delegated)
        {
            TickProcessor.Tick(_entities.List, deltaTime);
        }
    }

    // Обработка LateTick
    public void ProcessLateTick(float deltaTime)
    {
        if (Mode == ProcessingMode.Standard)
        {
            foreach (var entity in _entities)
            {
                entity.LateTick(deltaTime);
            }
        }
        else if (Mode == ProcessingMode.Delegated)
        {
            TickProcessor.LateTick(_entities.List, deltaTime);
        }
    }

    // Применение удалений
    public void ApplyRemovals()
    {
        // Получаем всех помеченных на удаление
        var markedEntities = _entities.Removals;

        // Терминируем их
        foreach (var entity in markedEntities)
        {
            entity.Terminate();
        }

        // Применяем удаление
        _entities.ApplyRemovals();
    }

    public void Clear()
    {
        _entities.Clear();
    }
}