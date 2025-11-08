
using UnityEngine;
using UnityEngine.Events;

public class Autorifle : Weapon
{
    public Bullet BulletPrefab;
    public float ActivationInterval = 0.4f;

    private float _activationTimer = 0;

    public override bool CanActivate => _activationTimer <= 0;

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

    public override void Activate(Character character, Vector3 direction)
    {
        if (!BulletPrefab) return;

        var attackSpeedModifier = character.Modifiers.GetCounterOrNUll<AttackSpeedCounter>().AttackSpeedModifier;


        if (_activationTimer <= 0)
        {
            _activationTimer = ActivationInterval * attackSpeedModifier;
            var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
            bullet.Direction = direction.normalized;

            OnActivate.Invoke();
        }
    }
}