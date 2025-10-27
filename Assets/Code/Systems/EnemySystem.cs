

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySystem : GameSystem
{
    private RecordCollection<EnemyDieRecord> _enemyDieRecords = null;

    public EnemyConfig[] EnemyConfigs;

    protected override void Awake()
    {
        base.Awake();

        LoadEnemyConfigs();

        GameManager.Instance.TickRegistry.GetOrCreateCategory<Enemy>().TickProcessor = this;
        _enemyDieRecords = GameManager.Instance.RecordRepository.GetOrCreateCollection<EnemyDieRecord>();
    }

    public override void Tick(IReadOnlyList<ITick> entities, float deltaTime)
    {
        foreach (var enemy in entities.Cast<Enemy>())
        {
            if (enemy.Health.Current <= 0)
            {
                _enemyDieRecords.Add(new EnemyDieRecord() { Position = enemy.Position });
                Debug.Log($"Enemy died at {enemy.Position}");
                enemy.MarkForTermination();
            }
        }

        base.Tick(entities, deltaTime);
    }

    protected override void OnTerminateMarkInternal()
    {
        GameManager.Instance.TickRegistry.GetOrCreateCategory<Enemy>().TickProcessor = null;
    }


    private void LoadEnemyConfigs()
    {
        EnemyConfigs = Resources.LoadAll<EnemyConfig>("Enemies");
        Debug.Log($"Loaded enemy configs: {EnemyConfigs.Length}");
    }
}