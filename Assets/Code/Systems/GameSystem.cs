

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class GameSystem : MonoBehaviour, ITickProcessor, ITick
{
    public TickLifecycle Lifecycle { get; protected set; } = TickLifecycle.Alive;

    public UnityEvent OnTerminateMark { get; } = new UnityEvent();

    public UnityEvent OnTerminate { get; } = new UnityEvent();

    protected virtual void Awake() { }

    public virtual void FixedTick(float deltaTime) { }
    public virtual void Tick(float deltaTime) { }
    public virtual void LateTick(float deltaTime) { }

    public virtual void FixedTick(IReadOnlyList<ITick> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            entity.FixedTick(deltaTime);
        }
    }

    public virtual void Tick(IReadOnlyList<ITick> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            entity.Tick(deltaTime);
        }
    }

    public virtual void LateTick(IReadOnlyList<ITick> entities, float deltaTime)
    {
        foreach (var entity in entities)
        {
            entity.LateTick(deltaTime);
        }
    }

    public virtual void MarkForTermination()
    {
        if (Lifecycle == TickLifecycle.Alive)
        {
            OnTerminateMarkInternal();

            Lifecycle = TickLifecycle.Marked;
            OnTerminateMark?.Invoke();
        }
    }

    protected virtual void OnTerminateMarkInternal() { }

    public virtual void Terminate()
    {
        if (Lifecycle != TickLifecycle.Terminated)
        {
            OnTerminateInternal(); 

            Lifecycle = TickLifecycle.Terminated;
            OnTerminate?.Invoke();
            Destroy(gameObject);
        }
    }

    protected virtual void OnTerminateInternal() { }


}