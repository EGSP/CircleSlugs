

using UnityEngine;

public class Bullet : Entity
{
    public float Speed = 5f;
    public float LiveTime = 5f;

    public float Damage = 1f;
    public float Size = 0.3f;

    public float Punch = 6f;

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
        transform.position += deltaTime * Speed * Direction;
    }

    private bool ShouldDie(float deltaTime)
    {
        _timer += deltaTime;
        return _timer > LiveTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.greenYellow;
        Gizmos.DrawWireSphere(transform.position, Size);
    }


}