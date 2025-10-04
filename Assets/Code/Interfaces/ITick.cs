using UnityEngine.Events;

public interface ITick
{
    TickLifecycle Lifecycle { get; }

    UnityEvent OnTerminateMark { get; }
    UnityEvent OnTerminate { get; }

    public void FixedTick(float deltaTime) { return; }
    public void Tick(float deltaTime) { return; }
    public void LateTick(float deltaTime) { return; }

    public void MarkForTermination();

    public void Terminate();
}

public enum TickLifecycle
{
    Alive,
    Marked,
    Terminated
}