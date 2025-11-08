using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Rendering;

public class CharacterController : MonoBehaviour
{
    public Character Character;

    public float Speed = 1f;

    private Vector2 _moveDirection;

    private SpriteRenderer _sprite;
    private Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Character = GetComponent<Character>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();

        var cameraComponent = FindFirstObjectByType<CinemachineCamera>();
        cameraComponent.Follow = transform;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Animate();

        // Attack(Time.deltaTime);
    }

    private void Move()
    {
        var newPosition = Character.Position + Speed * Time.deltaTime * (Vector3)_moveDirection;
        Character.Position = newPosition;
    }

    private void Animate()
    {
        if (_moveDirection.sqrMagnitude > 0)
        {
            if (_moveDirection.x != 0)
            {
                _sprite.flipX = _moveDirection.x > 0;
            }
        }

        if (!_animator)
            return;

        var isMoving = _moveDirection.sqrMagnitude > 0;
        _animator.SetBool("Walk", isMoving);
    }

    void LateUpdate()
    {
        _moveDirection = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
    }

    // private void Attack(float deltaTime)
    // {
    //     if (!BulletPrefab) return;

    //     var attackSpeedModifier = Character.Modifiers.GetCounterOrNUll<AttackSpeedCounter>().AttackSpeedModifier;
    //     _attackTimer += deltaTime;

    //     var direction = Vector3.right;

    //     if (_isManualAim)
    //     {
    //         if (ManualAttackWithManualAim && !ShouldAttack) return;
    //         UseAttackInput();

    //         var pointerPosition = InputSystem.actions.FindAction("PointerPosition").ReadValue<Vector2>();

    //         // Конвертируем из Screen Space в Viewport Space (0-1)
    //         Vector2 viewportPos = Camera.main.ScreenToViewportPoint(pointerPosition);
    //         var viewportDirection = viewportPos - Vector2.one * 0.5f;
    //         viewportDirection.x *= Camera.main.aspect;
    //         direction = viewportDirection.normalized;
    //     }
    //     else
    //     {
    //         var (category, enemies) = GameManager.Instance.TickRegistry.GetAll<Enemy>();

    //         Entity enemy = null;

    //         if (category.Count > 0)
    //         {
    //             var minimumDistance = float.MaxValue;

    //             foreach (var e in enemies)
    //             {
    //                 var distance = (e.Position - Character.Position).sqrMagnitude;
    //                 if (distance < minimumDistance)
    //                 {
    //                     minimumDistance = distance;
    //                     enemy = e;
    //                 }
    //             }
    //         }

    //         if (enemy != null)
    //             direction = (enemy.Position - Character.Position).normalized;
    //     }

    //     if (_attackTimer > AttackInterval * attackSpeedModifier)
    //     {
    //         _attackTimer = 0f;
    //         var bullet = Instantiate(BulletPrefab, Character.Position, Quaternion.identity);
    //         bullet.Direction = direction.normalized;

    //         OnShoot.Invoke();
    //     }
    // }
}
