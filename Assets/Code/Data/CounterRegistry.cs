using System;
using System.Collections.Generic;

public class CounterRegistry
{
    private Dictionary<Type, ICounter> _counters = new();

    public T GetCounterOrNUll<T>()
        where T : class,ICounter
    {
        Type type = typeof(T);
        if (!_counters.ContainsKey(type))
        {
            return null;
        }
        return (T)_counters[type];
    }
}