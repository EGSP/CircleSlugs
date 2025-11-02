
public struct PickupRangeRecord : IRecord
{
    public int SequenceId { get; set; }

    /// <summary>
    /// The percentage change. It will be used for sum (value should be like 10% is 0.1f)
    /// </summary>
    public float Change { get; set; }
}

public class PickupRangeCounter : Counter
{
    public float PickupRangeModifier => 1 + PickupRange;

    private float PickupRange { get; set; }

    private RecordCollection<PickupRangeRecord> _records;

    public PickupRangeCounter(RecordCollection<PickupRangeRecord> records)
    {
        _records = records;
        _records.Records.OnChanged(Calculate);
    }
    
    protected override void Calculate() => PickupRange += _records.Records[^1].Change;
}