using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BulletSystem : GameSystem
{
    private TickCategory EnemyCategory { get; set; }

    protected override void Awake()
    {
        GameManager.Instance.TickRegistry.GetOrCreateCategory<Bullet>().TickProcessor = this;
        EnemyCategory = GameManager.Instance.TickRegistry.GetOrCreateCategory<Enemy>();
    }

    public override void Tick(IReadOnlyList<ITick> entities, float deltaTime)
    {
        ProcessHits(entities, deltaTime);

        base.Tick(entities, deltaTime);
    }

    private void ProcessHits(IReadOnlyList<ITick> bulletTicks, float deltaTime)
    {
        foreach (var bullet in bulletTicks.Cast<Bullet>())
        {
            var enemies = EnemyCategory.Entities;

            foreach (var enemy in enemies.Cast<Enemy>())
            {
                var hit = Vector3.Distance(enemy.transform.position, bullet.transform.position) < bullet.Size;
                if (hit)
                {

                    Debug.Log("Hit");
                    bullet.MarkForTermination();

                    enemy.Health.Current -= bullet.Damage;
                    enemy.Physics.Force(bullet.Direction * bullet.Punch, ForceType.Continuous, 0.5f);
                    break;
                }
            }

        }
    }

    protected override void OnTerminateMarkInternal()
    {
        GameManager.Instance.TickRegistry.GetOrCreateCategory<Bullet>().TickProcessor = null;
    }
}