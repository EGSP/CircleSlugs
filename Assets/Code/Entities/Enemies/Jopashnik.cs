
using System.Linq;
using UnityEngine;

public class Jopashnik: Enemy
{
    public float Speed = 2f;

    private FirstTickAccessor<Character> Target { get; set; }

    protected override void RegisterToTickSystem()
    {
        base.RegisterToTickSystem();
        Target = GameManager.Instance.TickRegistry.GetOrCreateCategory<Character>().AsFirstEntity<Character>().Cache(true);
    }

    public override void Tick(float deltaTime)
    {
        if (Target.Entity == null) return;
    
        Move(deltaTime);

        base.Tick(deltaTime);
    }

    private void Move(float deltaTime)
    {
        var direction = (Target.Entity.transform.position - transform.position).normalized;
        transform.position += direction * Speed * deltaTime;
    }
}