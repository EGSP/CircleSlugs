
using System.Linq;

public class LevelCounter {

    private RecordCollection<AddedExperience> _addedExperience = null;

    public float Experience { get; private set; }

    public LevelCounter()
    {
        _addedExperience = GameManager.Instance.RecordRepository.GetOrCreateCollection<AddedExperience>();

        _addedExperience.Records.OnChanged(Calculate);
    }

    private void Calculate()
    {
        if (_addedExperience.Records.Count == 0) return;

        Experience += _addedExperience.Records[^1].Value;
    }

}