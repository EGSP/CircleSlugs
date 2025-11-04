using System;
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

    protected T GetCounter<T>() where T : class, ICounter
    {
        var counter = GameManager.Instance.CounterRegistry.GetCounterOrNUll<T>();

        if (counter == null)
            throw new ArgumentNullException($"Counter dependecy not found of {typeof(T)}");


        return counter;
    }

    protected abstract void Calculate();
}

