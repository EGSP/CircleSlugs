using System;
using System.Collections.Generic;

public interface IRecord
{
    public int SequenceId { get; set; }
}

public interface IRecordCollection
{

}

public class RecordCollection<T> : IRecordCollection
    where T : IRecord
{
    private ObservableList<T> _records = new();
    public IReadOnlyObservableList<T> Records => _records;

    private int _lastSequenceId = int.MinValue;

    public T Last => _records.Count == 0
        ? throw new System.Exception($"RecordCollection{typeof(T).Name} is empty") 
        : _records[^1];

    public void Add(T record)
    {
        record.SequenceId = ++_lastSequenceId;
        _records.Add(record);
    }

    public void Clear()
    {
        _lastSequenceId = int.MinValue;
        _records.Clear();
    }

    public void OnChanged(Action action) => _records.OnChanged(action);
}
