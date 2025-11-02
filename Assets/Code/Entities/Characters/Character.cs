public abstract class Character : Entity
{
    public RecordRepository Records = new();
    public CounterRegistry Modifiers = new();

    protected override void Awake()
    {
        Modifiers.Register(new PickupRangeCounter(Records.GetOrCreateCollection<PickupRangeRecord>()));
        base.Awake();
    }

    protected override void RegisterToTickSystem()
    {
        GameManager.Instance.TickRegistry.Register<Character>(this);
    }
}