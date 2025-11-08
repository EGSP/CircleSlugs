
using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour, ITick
{
    public TickLifecycle Lifecycle { get; private set; }

    public UnityEvent OnTerminateMark { get; } = new();

    public UnityEvent OnTerminate { get; } = new();


    protected virtual void Awake()
    {
        RegisterToTickSystem();
    }

    protected abstract void RegisterToTickSystem();

    public void MarkForTermination()
    {
        throw new System.NotImplementedException();
    }

    public void Terminate()
    {
        throw new System.NotImplementedException();
    }
}