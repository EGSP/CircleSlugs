
using UnityEngine;

public class Zombik : CharacterBasedEnemy
{
    public float Speed = 3f;

    public override string Id => "zombik";

    protected override void TickOnTarget(float deltaTime)
    {
        base.TickOnTarget(deltaTime);
        Move(deltaTime);
    }

    private void Move(float deltaTime)
    {
        var pos = Vector3.MoveTowards(Position, Target.Position, Speed * deltaTime);
        Position = pos;
    }
}