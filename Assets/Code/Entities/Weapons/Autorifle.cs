
public class Autorifle : Weapon
{
    public Bullet BulletPrefab;
    public float ActivationInterval = 0.4f;

    private float _activationTimer = 0;

    protected override void RegisterToTickSystem()
    {
        GameManager.Instance.TickRegistry.Register<Weapon>(this);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        _activationTimer -= deltaTime;
        if (_activationTimer <= 0)
            _activationTimer = 0;
    }

    private void Activate()
    {
        
    }
}