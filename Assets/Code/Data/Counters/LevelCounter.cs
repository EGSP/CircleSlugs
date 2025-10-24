
using System.Linq;
using UnityEngine;

public class LevelCounter : ICounter
{
    private RecordCollection<AddExperienceRecord> _addedExperience = null;

    public float Experience { get; private set; }

    public LevelCounter()
    {
        _addedExperience = GameManager.Instance.RecordRepository.GetOrCreateCollection<AddExperienceRecord>();

        _addedExperience.Records.OnChanged(Calculate);
    }

    private void Calculate()
    {
        if (_addedExperience.Records.Count == 0) return;

        Experience += _addedExperience.Records[^1].Value;
        Debug.Log($"Experience calculated: {Experience}");
    }

}