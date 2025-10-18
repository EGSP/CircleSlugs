
public class CharacterBullet : Bullet
{
    protected override void RegisterToTickSystem()
    {
        base.RegisterToTickSystem();
        GameManager.Instance.TickRegistry.Register<CharacterBullet>(this);
    }
}