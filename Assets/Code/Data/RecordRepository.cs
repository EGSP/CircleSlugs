using System;
using System.Collections.Generic;

public class RecordRepository
{
    private Dictionary<Type, IRecordCollection> _collections = new();

    /// <summary>
    /// Добавляет запись в хранилище
    /// </summary>
    public void AddRecord<T>(T record)
    {
        GetOrCreateCollection<T>().Add(record);
    }

    public RecordCollection<T> GetOrCreateCollection<T>()
    {
        Type type = typeof(T);
        if (!_collections.ContainsKey(type))
        {
            _collections[type] = new RecordCollection<T>();
        }
        return (RecordCollection<T>)_collections[type];
    }
}