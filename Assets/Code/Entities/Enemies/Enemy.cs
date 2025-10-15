public class Enemy : Entity
{
    protected override void RegisterToTickSystem()
    {
        GameManager.Instance.TickRegistry.Register<Enemy>(this);
    }
}