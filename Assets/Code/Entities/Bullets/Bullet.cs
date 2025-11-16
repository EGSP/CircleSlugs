using UnityEditor;
using UnityEngine;

public abstract class Bullet : Entity
{
    public float Speed = 5f;
    public float LiveTime = 5f;

    public float Damage = 1f;

    public float Punch = 6f;

    public Vector3 InitalRotation;
    public Vector3 Direction { get; set; }

    private float _timer = 0f;

    protected override void RegisterToTickSystem()
    {
        GameManager.Instance.TickRegistry.Register<Bullet>(this);
    }

    public override void Tick(float deltaTime)
    {
        if (ShouldDie(deltaTime))
        {
            MarkForTermination();
            return;
        }

        base.Tick(deltaTime);

        transform.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(InitalRotation) * Direction);
        Position += deltaTime * Speed * Direction;
    }

    private bool ShouldDie(float deltaTime)
    {
        _timer += deltaTime;
        return _timer > LiveTime;
    }

}