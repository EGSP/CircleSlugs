using UnityEngine;
using UnityEngine.Events;

public interface ICounter
{

}

public abstract class Counter : ICounter
{
    public UnityEvent Changed { get; } = new();

    protected RecordCollection<T> GetRecordCollection<T>() where T : IRecord
    {
        return GameManager.Instance.RecordRepository.GetOrCreateCollection<T>();
    }

    protected abstract void Calculate();
}

