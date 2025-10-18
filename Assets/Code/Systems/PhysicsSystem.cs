
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Режим применения скорости.
/// Выбор зависит от того в каком режиме обновяется позиция камеры.
/// Когда режим применения скорости и режим камеры одинаковы, то нет джитера/лагов.
/// </summary>
public enum ApplyVelocityTick
{
    FixedTick,
    LateTick
}

public class PhysicsSystem : GameSystem
{
    public bool EnableEntitySeparation = true;

    public float SeparationForceBase = 2f;
    public float SeparationForceMultiplier = 1f;

    private TickCategory EnemiesCategory { get; set; }
    private TickCategory CharacterCategory { get; set; }

    private IEnumerable<Entity> Entities
    {
        get
        {
            return EnemiesCategory.Entities.EnumerateTogether(CharacterCategory.Entities).Cast<Entity>();
        }
    }


    public ApplyVelocityTick ApplyVelocityTick = ApplyVelocityTick.FixedTick;
    private float _fixedDeltaTime;
    private float _velocityApplyDeltaTime;

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.TickRegistry.Register<PhysicsSystem>(this);

        EnemiesCategory = GameManager.Instance.TickRegistry.GetOrCreateCategory<Enemy>();
        CharacterCategory = GameManager.Instance.TickRegistry.GetOrCreateCategory<Character>();
    }

    public override void FixedTick(float deltaTime)
    {
        _fixedDeltaTime = deltaTime;
        // Обработка расталкивания сущностей друг от друга
        if (EnableEntitySeparation)
        {
            ProcessEntitySeparation(_velocityApplyDeltaTime);
        }

        // Обработка физики для каждой сущности
        foreach (var entity in Entities)
        {
            if (entity == null || entity.Physics == null) continue;

            ClearVelocity(entity);

            CalculateVelocity(entity, _velocityApplyDeltaTime);

            if (ApplyVelocityTick == ApplyVelocityTick.FixedTick)
            {
                ApplyVelocity(entity);
            }
        }

        base.FixedTick(deltaTime);
    }

    public override void LateTick(float deltaTime)
    {
        _velocityApplyDeltaTime = deltaTime;

        if (ApplyVelocityTick == ApplyVelocityTick.LateTick)
        {
            foreach (var entity in Entities)
            {
                ApplyVelocity(entity);
            }
        }

        base.LateTick(deltaTime);
    }

    private void ApplyVelocity(Entity entity)
    {
        entity.transform.position += entity.Physics.Velocity;
        // entity.Physics.Velocity = Vector3.zero;
    }

    /// <summary>
    /// Я добавил эту функцию отдельно от ApplyVelocity,
    /// чтобы компонент Physics.OnDrawGizmos() успел отрисовать в текущем кадре вектор скорости.
    /// Если обнулять вектор скорости сразу же после использования, то компонент физики увидит лишь Vector.zero.
    /// Поэтому скорость обнуляется только при подсчете нового значения, т.е. в следующем кадре.
    /// </summary>
    /// <param name="entity"></param>
    private void ClearVelocity(Entity entity)
    {
        entity.Physics.Velocity = Vector3.zero;
    }


    /// <summary>
    /// Обработка физики одной сущности
    /// </summary>
    private void CalculateVelocity(Entity entity, float deltaTime)
    {
        Physics physics = entity.Physics;
        Vector3 velocity = Vector3.zero;

        // Обработка всех приложенных сил
        for (int i = physics.AppliedForces.Count - 1; i >= 0; i--)
        {
            AppliedForce force = physics.AppliedForces[i];

            switch (force.Type)
            {
                case ForceType.Continuous:
                    // Постоянная сила - применяется с полной мощностью
                    velocity += force.Force;

                    // Уменьшаем время действия
                    force.Duration -= deltaTime;
                    physics.AppliedForces[i] = force;

                    if (force.Duration <= 0)
                    {
                        physics.AppliedForces.RemoveAt(i);
                    }
                    break;

                case ForceType.Decay:
                    // Затухающая сила: F(t) = F₀ * e^(-kt)
                    // где k = 5 (коэффициент затухания)
                    // Сила уменьшается экспоненциально со временем
                    float decayRate = 5f;
                    float timeElapsed = 1f - force.Duration; // сколько времени прошло
                    float currentMagnitude = Mathf.Exp(-decayRate * timeElapsed);

                    velocity += force.Force * currentMagnitude;

                    // Уменьшаем оставшееся время
                    force.Duration -= deltaTime;
                    physics.AppliedForces[i] = force;

                    // Удаляем силу когда время истекло
                    if (force.Duration <= 0)
                    {
                        physics.AppliedForces.RemoveAt(i);
                    }
                    break;
            }
        }

        // Применяем сопротивление (drag)
        // Формула: V_new = V * (1 - drag)^dt
        // При drag=0: скорость не изменяется
        // При drag=1: скорость мгновенно становится 0
        // При drag=0.5 и dt=1: скорость уменьшается в 2 раза за секунду
        if (physics.Drag > 0.001f)
        {
            velocity *= Mathf.Pow(1f - physics.Drag, deltaTime);
        }

        // Применяем скорость к позиции
        if (velocity.sqrMagnitude > 0.001f)
        {
            physics.Velocity = velocity * deltaTime;
        }
    }

    /// <summary>
    /// Расталкивание сущностей друг от друга
    /// Каждая сущность отталкивается только от более значительных сущностей
    /// </summary>
    private void ProcessEntitySeparation(float deltaTime)
    {
        // for (int i = 0; i < Entities.Count(); i++)
        // {
        //     Entity entityA = entities[i];
        //     if (entityA == null || !entityA.Physics.CanBePushed) continue;

        //     for (int j = 0; j < entities.Count; j++)
        //     {
        //         if (i == j) continue;

        //         Entity entityB = entities[j];
        //         if (entityB == null || !entityB.Physics.CanPushOthers) continue;

        //         // Сущность A отталкивается только если B более значительна
        //         if (entityA.Physics.Magnitude >= entityB.Physics.Magnitude) continue;

        //         // Проверяем пересечение
        //         Vector3 direction = entityA.transform.position - entityB.transform.position;
        //         float distance = direction.magnitude;
        //         float minDistance = entityA.Physics.Size + entityB.Physics.Size;

        //         if (distance < minDistance && distance > 0.001f)
        //         {
        //             direction /= distance; // Нормализация

        //             // Сила пропорциональна глубине пересечения
        //             float overlap = minDistance - distance;

        //             // Применяем силу отталкивания
        //             // Сила = направление * глубина_пересечения
        //             entityA.Physics.AddForce(direction * overlap, ForceType.Continuous, Time.fixedDeltaTime);
        //         }
        //     }
        // }

        foreach (var entityA in Entities)
        {
            if (entityA == null || !entityA.Physics.CanBePushed)
                continue;

            foreach (var entityB in Entities)
            {
                if (ReferenceEquals(entityA, entityB))
                    continue;
                if (entityB == null || !entityB.Physics.CanPushOthers)
                    continue;

                if (entityA.Physics.Magnitude > entityB.Physics.Magnitude)
                    continue;

                Vector3 direction = entityA.transform.position - entityB.transform.position;
                float distance = direction.magnitude;
                float minDistance = entityA.Physics.Size + entityB.Physics.Size;

                if (distance < minDistance && distance > 0.001f)
                {
                    direction /= distance;
                    float overlap = minDistance - distance;
                    entityA.Physics.Force(direction * (overlap * SeparationForceMultiplier + SeparationForceBase), ForceType.Decay, 1f);
                }
            }
        }

    }

}