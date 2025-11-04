
using System.Linq;
using UnityEngine;

public class TerrorCounter : Counter
{
    private const float LevelToTerrorMultiplier = 10f;
    public const float TerrorCapThreshold = 1.2f; // percent

    public float TerrorCap { get; private set; }
    public float Terror { get; private set; }

    private LevelCounter _levelCounter;

    private TickCategory _enemies;

    public float TerrorCapWithThreshold => TerrorCap * TerrorCapThreshold;
    public float AvailableTerror => TerrorCap - Terror;


    public TerrorCounter()
    {
        _levelCounter = GetCounter<LevelCounter>();
        _levelCounter.Changed.AddListener(Calculate);

        _enemies = GameManager.Instance.TickRegistry.GetOrCreateCategory<Enemy>();
        _enemies.Added.AddListener((tick) => { Calculate(); tick.OnTerminateMark.AddListener(Calculate); });
    }

    protected override void Calculate()
    {
        var level = _levelCounter.Level;
        TerrorCap = level * LevelToTerrorMultiplier;

        Terror = _enemies.Alive.Sum(e => { return e is Enemy enemy ? enemy.Power : 0; });
        Debug.Log($"Terror: {Terror}, TerrorCap: {TerrorCap}, TerrorCapWithThreshold: {TerrorCapWithThreshold}");

        Changed.Invoke();
    }
}