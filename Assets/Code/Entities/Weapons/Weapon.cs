
using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour, ITick
{
    public TickLifecycle Lifecycle { get; private set; }

    public UnityEvent OnTerminateMark { get; } = new();

    public UnityEvent OnTerminate { get; } = new();

    public SpriteRenderer SpriteRenderer;

    public abstract bool CanActivate { get; }


    protected virtual void Awake()
    {
        SpriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
        RegisterToTickSystem();
    }

    protected abstract void RegisterToTickSystem();

    public virtual void FixedTick(float deltaTime) { }
    public virtual void Tick(float deltaTime) { }
    public virtual void LateTick(float deltaTime) { }


    public void MarkForTermination()
    {
        if (Lifecycle == TickLifecycle.Alive)
        {
            Lifecycle = TickLifecycle.Marked;
            OnTerminateMark?.Invoke();
        }
    }

    public void Terminate()
    {
        if (Lifecycle != TickLifecycle.Terminated)
        {
            Lifecycle = TickLifecycle.Terminated;
            OnTerminate?.Invoke();
            Destroy(gameObject);
        }
    }

    private void Reset()
    {
        SpriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
    }
}