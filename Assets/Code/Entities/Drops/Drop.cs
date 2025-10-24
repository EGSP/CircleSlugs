using UnityEngine;
using UnityEngine.Events;

public abstract class Drop : MonoBehaviour, ITick
{
    public TickLifecycle Lifecycle { get; private set; }

    public UnityEvent OnTerminateMark { get; } = new();

    public UnityEvent OnTerminate { get; } = new();

    public Vector3 Position { get; set; }


    protected virtual void Awake()
    {
        Position = transform.position;
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