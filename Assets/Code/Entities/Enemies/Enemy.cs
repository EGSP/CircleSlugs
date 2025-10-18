using UnityEngine;

public abstract class Enemy : Entity
{
    public SpriteRenderer Sprite;
    public bool InvertSprite = false;

    protected override void Awake()
    {
        base.Awake();
        Sprite = GetComponentInChildren<SpriteRenderer>();
    }

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

    protected virtual void TickOnTarget(float deltaTime)
    {
        MirrorSprite(Target);
    }
    
    protected void MirrorSprite(Character character)
    {
        Sprite.flipX = character.transform.position.x < transform.position.x ^ InvertSprite;
    }
}