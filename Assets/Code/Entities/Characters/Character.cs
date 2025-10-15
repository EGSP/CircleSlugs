public abstract class Character : Entity
{
    protected override void RegisterToTickSystem()
    {
        GameManager.Instance.TickRegistry.Register<Character>(this);
    }
}