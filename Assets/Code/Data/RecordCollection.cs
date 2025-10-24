using System.Collections.Generic;

public interface IRecordCollection
{

}

public class RecordCollection<T> : IRecordCollection
{
    private ObservableList<T> _records = new();
    public IReadOnlyObservableList<T> Records => _records;

    public T Last => _records.Count == 0
        ? throw new System.Exception($"RecordCollection{typeof(T).Name} is empty") 
        : _records[^1];

    public void Add(T record) => _records.Add(record);
    public void Clear() => _records.Clear();
}
