
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class WeaponSystem : GameSystem
{
    public bool StartWithManualAim = false;
    public bool ManualAttackWithManualAim = true;

    public float WeaponsOffset = 0.5f;

    public Weapon[] WeaponsPrefabs;

    private FirstTickAccessor<Character> Character;
    private TickCategory Weapons;

    private bool _isManualAim = false;
    private bool _doHoldAttack = false;
    private bool _doPressAttack = false;

    public UnityEvent<bool> ToggleManualAim = new();
    public UnityEvent OnShoot = new();

    public bool DoAttack => _doHoldAttack || _doPressAttack;

    protected override void Awake()
    {
        base.Awake();
        Game.TickRegistry.Register<WeaponSystem>(this);

        Weapons = Game.TickRegistry.GetOrCreateCategory<Weapon>();
        Weapons.TickProcessor = this;

        Character = Game.TickRegistry.GetOrCreateCategory<Character>().AsFirstEntity<Character>();

        _isManualAim = StartWithManualAim;
        InputSystem.actions.FindAction("Option").performed += ToggleAim;


        InputSystem.actions.FindAction("Attack").performed += OnAttackInput;
        InputSystem.actions.FindAction("Attack").canceled += OnAttackCancelInput;

        InstWeapons();
    }

    private void InstWeapons()
    {
        if (WeaponsPrefabs is null)
            return;
        if (WeaponsPrefabs.Length == 0)
            return;
            
        foreach (var prefab in WeaponsPrefabs)
        {
            var weapon = Instantiate(prefab, transform.position, Quaternion.identity, null);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ToggleManualAim.Invoke(_isManualAim);
    }

    private void ToggleAim(InputAction.CallbackContext context)
    {
        _isManualAim = !_isManualAim;
        ToggleManualAim.Invoke(_isManualAim);
    }

    private void OnAttackInput(InputAction.CallbackContext context)
    {
        switch (context.interaction)
        {
            case HoldInteraction hold:
                _doHoldAttack = true;
                Debug.Log("Hold performed");
                break;
            case PressInteraction press:
                _doPressAttack = true;
                Debug.Log("Press performed");
                break;

            case TapInteraction tap:
                Debug.Log("Tap performed");
                _doPressAttack = true;
                break;
            case SlowTapInteraction slowTap:
                Debug.Log("SlowTap performed");
                _doPressAttack = true;
                break;
            default:
                break;
        }
    }

    private void OnAttackCancelInput(InputAction.CallbackContext context)
    {
        switch (context.interaction)
        {
            case HoldInteraction hold:
                Debug.Log("Hold canceled");
                _doHoldAttack = false;
                break;

            case TapInteraction tap:
                Debug.Log("Tap canceled");
                _doPressAttack = false;
                break;
            case SlowTapInteraction slowTap:
                Debug.Log("SlowTap canceled");
                _doPressAttack = false;
                break;
            default:
                break;
        }
    }

    private void UseAttackInput()
    {
        _doPressAttack = false;
    }

    public override void Tick(IReadOnlyList<ITick> entities, float deltaTime)
    {
        base.Tick(entities, deltaTime);

        PositionWeapon();
    }

    private void PositionWeapon()
    {
        var character = Character.Entity;
        if (character == null) return;

        var rotations = QuaternionUtils.GenerateRotationArray(Weapons.Count, Quaternion.Euler(0, 0, 0), Vector3.forward);

        for (int i = 0; i < Weapons.Count; i++)
        {
            var weapon = Weapons.Entities[i] as Weapon;
            weapon.transform.position = character.Position + rotations[i] * Vector3.right * WeaponsOffset;
        }
    }

    private void OnDrawGizmosSelected()
    {
        var point = Character?.Entity?.transform.position ?? transform.position;

        Gizmos.color = Color.magenta;
        GizmosMore.DrawCircle(point + Vector3.right * WeaponsOffset, 1f);

        Gizmos.DrawLine(point, point + Vector3.right * WeaponsOffset);
    }
}