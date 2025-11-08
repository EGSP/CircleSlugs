
using System.Collections.Generic;

public class WeaponSystem : GameSystem
{
    private FirstTickAccessor<Character> Character;

    protected override void Awake()
    {
        base.Awake();

        Character = Game.TickRegistry.GetOrCreateCategory<Character>().AsFirstEntity<Character>();
    }

    public override void Tick(IReadOnlyList<ITick> entities, float deltaTime)
    {
        
    }
}