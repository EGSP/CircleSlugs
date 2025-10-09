

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySystem : GameSystem
{
    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.TickRegistry.GetOrCreateCategory<Enemy>().TickProcessor = this;
    }

    public override void Tick(IReadOnlyList<ITick> entities, float deltaTime)
    {
        foreach (var enemy in entities.Cast<Enemy>())
        {
            if (enemy.Health.Current <= 0)
            {   
                Debug.Log("Enemy died");
                enemy.MarkForTermination();
            }
        }

        base.Tick(entities, deltaTime);
    }

    protected override void OnTerminateMarkInternal()
    {
        GameManager.Instance.TickRegistry.GetOrCreateCategory<Enemy>().TickProcessor = null;
    }
}