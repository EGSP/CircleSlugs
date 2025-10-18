public abstract class Enemy : Entity
{
    protected override void RegisterToTickSystem()
    {
        GameManager.Instance.TickRegistry.Register<Enemy>(this);
    }
}

public abstract class CharacterBasedEnemy : Enemy
{
    private FirstTickAccessor<Character> _Target { get; set; }

    protected Character Target => _Target.Entity;

    protected override void RegisterToTickSystem()
    {
        base.RegisterToTickSystem();
        _Target = GameManager.Instance.TickRegistry.GetOrCreateCategory<Character>().AsFirstEntity<Character>().Cache(true);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        if (Target == null) return;

        TickOnTarget(deltaTime);
    }

    protected abstract void TickOnTarget(float deltaTime);
}