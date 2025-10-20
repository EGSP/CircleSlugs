using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health), typeof(Physics))]
public abstract class Entity : MonoBehaviour, ITick
{
    public TickLifecycle Lifecycle { get; protected set; } = TickLifecycle.Alive;
    public UnityEvent OnTerminateMark { get; } = new UnityEvent();
    public UnityEvent OnTerminate { get; } = new UnityEvent();

    public Vector3 Position { get; set; }

    public Health Health { get; protected set; }
    public Physics Physics { get; protected set; }

    protected virtual void Awake()
    {
        Position = transform.position;
        Health = GetComponent<Health>();
        Physics = GetComponent<Physics>();
        RegisterToTickSystem();
    }

    protected abstract void RegisterToTickSystem();

    public virtual void FixedTick(float deltaTime) { }
    public virtual void Tick(float deltaTime)
    {
        UpdateEnginePosition();
    }
    public virtual void LateTick(float deltaTime)
    {
        UpdateEnginePosition();
    }

    public void UpdateEnginePosition()
    {
        transform.position = Position;
    }

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