using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BulletSystem : GameSystem
{
    private static TickCategory EnemyCategory { get; set; }
    private static TickCategory CharacterCategory { get; set; }

    private EnemyBulletProcessor _enemyBulletProcessor = new EnemyBulletProcessor();
    private CharacterBulletProcessor _characterBulletProcessor = new CharacterBulletProcessor();

    protected override void Awake()
    {
        // GameManager.Instance.TickRegistry.GetOrCreateCategory<Bullet>().TickProcessor = this;
        var instance = GameManager.Instance;
        instance.TickRegistry.GetOrCreateCategory<EnemyBullet>().TickProcessor = _enemyBulletProcessor;
        instance.TickRegistry.GetOrCreateCategory<CharacterBullet>().TickProcessor = _characterBulletProcessor;

        EnemyCategory = instance.TickRegistry.GetOrCreateCategory<Enemy>();
        CharacterCategory = instance.TickRegistry.GetOrCreateCategory<Character>();
    }

    private static void ProcessHitsOnEntities<T>(IReadOnlyList<ITick> bullets, IReadOnlyList<ITick> entities, float deltaTime)
        where T :  Entity
    {
        foreach (var bullet in bullets.Cast<Bullet>())
        {
            foreach (var entity in entities.Cast<T>())
            {
                var hit = Vector3.Distance(entity.Position, bullet.Position) < bullet.Size;
                if (hit)
                {
                    bullet.MarkForTermination();

                    entity.Health.Current -= bullet.Damage;
                    entity.Physics.Force(bullet.Direction * bullet.Punch, ForceType.Continuous, 0.5f);
                    break;
                }
            }

        }
    }

    protected override void OnTerminateMarkInternal()
    {
        // GameManager.Instance.TickRegistry.GetOrCreateCategory<Bullet>().TickProcessor = null;
        var instance = GameManager.Instance;
        instance.TickRegistry.GetOrCreateCategory<EnemyBullet>().TickProcessor = null;
        instance.TickRegistry.GetOrCreateCategory<CharacterBullet>().TickProcessor = null;
    }

    class EnemyBulletProcessor : ITickProcessor
    {

        public void Tick(IReadOnlyList<ITick> bullets, float deltaTime)
        {
            ProcessHitsOnEntities<Character>(bullets, CharacterCategory.Entities, deltaTime);   
        }

        public void FixedTick(IReadOnlyList<ITick> entities, float deltaTime) { return; }

        public void LateTick(IReadOnlyList<ITick> entities, float deltaTime) { return; }
    }

    class CharacterBulletProcessor : ITickProcessor
    {
        public void Tick(IReadOnlyList<ITick> bullets, float deltaTime)
        {
            ProcessHitsOnEntities<Enemy>(bullets, EnemyCategory.Entities, deltaTime);
        }

        public void FixedTick(IReadOnlyList<ITick> entities, float deltaTime) { return; }

        public void LateTick(IReadOnlyList<ITick> entities, float deltaTime) { return; }
    }
}

