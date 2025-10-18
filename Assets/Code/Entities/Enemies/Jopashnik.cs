
using System.Linq;
using UnityEngine;

public class Jopashnik : CharacterBasedEnemy
{
    public float Speed = 2f;

    protected override void TickOnTarget(float deltaTime)
    {
        base.TickOnTarget(deltaTime);
        Move(deltaTime);
    }

    private void Move(float deltaTime)
    {
        var direction = (Target.transform.position - transform.position).normalized;
        transform.position += direction * Speed * deltaTime;
    }
}