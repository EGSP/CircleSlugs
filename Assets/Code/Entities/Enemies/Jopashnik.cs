
using System.Linq;
using UnityEngine;

public class Jopashnik : CharacterBasedEnemy
{
    public float Speed = 2f;

    public override string Id => "jopashnik";

    protected override void TickOnTarget(float deltaTime)
    {
        base.TickOnTarget(deltaTime);
        Move(deltaTime);
    }

    private void Move(float deltaTime)
    {
        var direction = (Target.Position - Position).normalized;
        Position += direction * Speed * deltaTime;
    }
}