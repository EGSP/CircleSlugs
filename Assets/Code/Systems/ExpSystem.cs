using System.Collections.Generic;
using UnityEngine;

public class ExpSystem : GameSystem
{
    public Exp ExpPrefab;
    public float ExpTakeRadius = 3f;

    private RecordCollection<EnemyDieRecord> _enemyDieRecords;
    private int _lastSequenceId = int.MinValue;

    private TickCategory Exps;

    private FirstTickAccessor<Character> _Character { get; set; }

    private RecordCollection<AddExperienceRecord> _addedExperience;

    protected override void Awake()
    {
        base.Awake();
        GameManager.Instance.TickRegistry.Register<ExpSystem>(this);
        _enemyDieRecords = GameManager.Instance.RecordRepository.GetOrCreateCollection<EnemyDieRecord>();

        Exps = GameManager.Instance.TickRegistry.GetOrCreateCategory<Exp>();
        Exps.TickProcessor = this;

        _Character = GameManager.Instance.TickRegistry.GetOrCreateCategory<Character>().AsFirstEntity<Character>();

        _addedExperience = GameManager.Instance.RecordRepository.GetOrCreateCollection<AddExperienceRecord>();
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

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

    public override void Tick(IReadOnlyList<ITick> entities, float deltaTime)
    {
        base.Tick(entities, deltaTime);
        if(_Character.Entity == null) return;
        CheckCharacterCanTakeExp(_Character.Entity, entities);
    }

    private void SpawnExp(Vector3 position, int value)
    {
        var exp = Instantiate(ExpPrefab, position, Quaternion.identity);
        exp.Value = value;
    }

    private void CheckCharacterCanTakeExp(Character character, IReadOnlyList<ITick> exps)
    {
        foreach (var tick in exps)
        {
            if (tick is not Exp exp) continue;

            if (Vector3.Distance(character.Position, exp.Position) < ExpTakeRadius)
            {
                _addedExperience.Add(new() { Value = exp.Value });
                exp.MarkForTermination();
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        GizmosMore.DrawCircle(transform.position, ExpTakeRadius);
    }
}