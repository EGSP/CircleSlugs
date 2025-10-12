using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public abstract class Entity : MonoBehaviour, ITick
{
    public TickLifecycle Lifecycle { get; protected set; } = TickLifecycle.Alive;
    public UnityEvent OnTerminateMark { get; } = new UnityEvent();
    public UnityEvent OnTerminate { get; } = new UnityEvent();

    public Health Health { get; protected set; }

    protected void Awake()
    {

        Health = GetComponent<Health>();
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
}