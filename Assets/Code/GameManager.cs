using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TickRegistry TickRegistry { get; private set; } = new();

    public RecordRepository RecordRepository { get; private set; } = new();

    public CounterRegistry CounterRegistry { get; private set; } = new();
    
    private void Awake()
    {
        IniSelfSingletone();

        IniCounters();
    }

    private void IniSelfSingletone()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void IniCounters()
    {
        CounterRegistry.Register(new LevelCounter());
    }

    public void Update()
    {
        TickRegistry.ProcessAllTick(Time.deltaTime);
        TickRegistry.ApplyAllRemovals();
    }

    private void FixedUpdate()
    {
        TickRegistry.ProcessAllFixedTick(Time.fixedDeltaTime);
        TickRegistry.ApplyAllRemovals();
    }

    private void LateUpdate()
    {
        TickRegistry.ProcessAllLateTick(Time.deltaTime);
        TickRegistry.ApplyAllRemovals();
    }

}