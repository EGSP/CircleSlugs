
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsSystem : GameSystem
{
    public bool EnableEntitySeparation = true;

    public float SeparationForceBase = 2f;
    public float SeparationForceMultiplier = 1f;

    private TickCategory EnemiesCategory { get; set; }
    private TickCategory CharacterCategory{ get; set; }

    private IEnumerable<Entity> Entities
    {
        get
        {
            return EnemiesCategory.Entities.EnumerateTogether(CharacterCategory.Entities).Cast<Entity>();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.TickRegistry.Register<PhysicsSystem>(this);

        EnemiesCategory = GameManager.Instance.TickRegistry.GetOrCreateCategory<Enemy>();
        CharacterCategory = GameManager.Instance.TickRegistry.GetOrCreateCategory<Character>();
    }

    public override void FixedTick(float deltaTime)
    {
        // Обработка расталкивания сущностей друг от друга
        if (EnableEntitySeparation)
        {
            ProcessEntitySeparation(deltaTime);
        }

        // Обработка физики для каждой сущности
        foreach (var entity in Entities)
        {
            if (entity == null || entity.Physics == null) continue;

            ProcessEntityPhysics(entity, deltaTime);
        }

        base.FixedTick(deltaTime);
    }


    /// <summary>
    /// Обработка физики одной сущности
    /// </summary>
    private void ProcessEntityPhysics(Entity entity, float deltaTime)
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
            entity.transform.position += velocity * deltaTime;
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