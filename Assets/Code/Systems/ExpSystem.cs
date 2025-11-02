using System.Collections.Generic;
using UnityEngine;

public class ExpSystem : GameSystem
{
    public Exp ExpPrefab;
    public float ExpTriggerRadius = 4f;
    public float ExpFlySpeed = 5f;
    public float ExpTakeRadius = 1f;

    public float ExpAcceleration = 0.1f;

    private RecordCollection<EnemyDieRecord> _enemyDieRecords;
    private int _lastSequenceId = int.MinValue;

    private TickCategory Exps;

    private FirstTickAccessor<Character> _Character { get; set; }

    private RecordCollection<AddExperienceRecord> _addedExperience;

    private RecordCollection<IncreaseLevelRecord> _increaseLevel;

    private LevelCounter _levelCounter;

    protected override void Awake()
    {
        base.Awake();
        GameManager gm = GameManager.Instance;

        gm.TickRegistry.Register<ExpSystem>(this);

        _enemyDieRecords = gm.RecordRepository.GetOrCreateCollection<EnemyDieRecord>();

        Exps = gm.TickRegistry.GetOrCreateCategory<Exp>();
        Exps.TickProcessor = this;

        _Character = gm.TickRegistry.GetOrCreateCategory<Character>().AsFirstEntity<Character>();

        _addedExperience = gm.RecordRepository.GetOrCreateCollection<AddExperienceRecord>();
        _increaseLevel = gm.RecordRepository.GetOrCreateCollection<IncreaseLevelRecord>();

        _increaseLevel.Records.OnChanged(() => Debug.Log($"Level increased by {_increaseLevel.Records[^1].Increase}"));


        // _levelCounter = gm.CounterRegistry.GetCounterOrNUll<LevelCounter>();
        _increaseLevel.OnChanged(UpgradeCharacter);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        CheckEnemyDeaths();
    }

    public override void Tick(IReadOnlyList<ITick> exps, float deltaTime)
    {
        base.Tick(exps, deltaTime);
        if (_Character.Entity == null) return;

        ProcessExps(_Character.Entity, exps, deltaTime);
    }

    public void CheckEnemyDeaths()
    {
        if (_enemyDieRecords.Records.Count == 0) return;

        for (int i = _enemyDieRecords.Records.Count - 1; i >= 0; i--)
        {
            if (_enemyDieRecords.Records[i].SequenceId > _lastSequenceId)
            {
                var record = _enemyDieRecords.Records[i];
                SpawnExp(record.Position, 1);
            }
            else
            {
                break;
            }
        }

        _lastSequenceId = _enemyDieRecords.Records[^1].SequenceId;
    }

    private void SpawnExp(Vector3 position, int value)
    {
        var exp = Instantiate(ExpPrefab, position, Quaternion.identity);
        exp.Value = value;
    }

    private void ProcessExps(Character character, IReadOnlyList<ITick> exps, float deltaTime)
    {
        var expTriggerModifier = character.Modifiers.GetCounterOrNUll<PickupRangeCounter>().PickupRangeModifier;
        foreach (var tick in exps)
        {
            if (tick is not Exp exp) continue;

            if (exp.Triggered)
            {
                exp.Acceleration += ExpAcceleration * deltaTime;
                exp.Position = Vector3.MoveTowards(
                    exp.Position, character.Position,
                    (ExpFlySpeed + exp.Acceleration) * deltaTime);

                if (Vector3.Distance(character.Position, exp.Position) < ExpTakeRadius)
                {
                    _addedExperience.Add(new() { Value = exp.Value });
                    exp.MarkForTermination();
                }
            }
            else
            {
                if (Vector3.Distance(character.Position, exp.Position) < ExpTriggerRadius * expTriggerModifier)
                    exp.Triggered = true;
            }
        }
    }

    private void UpgradeCharacter()
    {
        var character = _Character.Entity;

        character.Records.AddRecord(new PickupRangeRecord
        { Change = 0.1f * _increaseLevel.Records[^1].Increase });

        character.Records.AddRecord(new AttackSpeedRecord
        { Change = 0.04f * _increaseLevel.Records[^1].Increase });
    }

    private void OnDrawGizmosSelected()
    {

        Vector3 point = _Character?.Entity?.Position ?? transform.position;

        if (Application.isPlaying)
        {
            Gizmos.color = Color.coral;
            GizmosMore.DrawCircle(point, ExpTriggerRadius);
            Gizmos.color = Color.blue;
            GizmosMore.DrawCircle(point, ExpTriggerRadius * _Character.Entity.Modifiers.GetCounterOrNUll<PickupRangeCounter>().PickupRangeModifier);

            Gizmos.color = Color.seaGreen;
            GizmosMore.DrawCircle(point, ExpTakeRadius);
        }
        else
        {
            Gizmos.color = Color.blue;
            GizmosMore.DrawCircle(point, ExpTriggerRadius);

            Gizmos.color = Color.seaGreen;
            GizmosMore.DrawCircle(point, ExpTakeRadius);
        }
    }
}