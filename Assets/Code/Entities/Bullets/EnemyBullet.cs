public class EnemyBullet : Bullet
{
    protected override void RegisterToTickSystem()
    {
        base.RegisterToTickSystem();
        GameManager.Instance.TickRegistry.Register<EnemyBullet>(this);
    }
}