using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CharacterController : MonoBehaviour
{
    public float Speed = 1f;

    public Bullet BulletPrefab;
    public float AttackInterval = 1.5f;
    private float _attackTimer = 0f;

    private Vector2 _inputVector;

    private SpriteRenderer _sprite;
    private Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

        Attack(Time.deltaTime);
    }

    private void Move()
    {
        var newPosition = transform.position + Speed * Time.deltaTime * (Vector3)_inputVector;
        transform.position = newPosition;
    }

    private void Animate()
    {
        if (_inputVector.sqrMagnitude > 0)
        {
            if (_inputVector.x != 0)
            {
                _sprite.flipX = _inputVector.x > 0;
            }
        }

        if (!_animator)
            return;

        var isMoving = _inputVector.sqrMagnitude > 0;
        _animator.SetBool("Walk", isMoving);
    }

    void LateUpdate()
    {
        _inputVector = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
    }

    private void Attack(float deltaTime)
    {
        if(!BulletPrefab) return;

        var (category,enemies) = GameManager.Instance.TickRegistry.GetAll<Enemy>();

        Entity enemy = null;

        if (category.Count > 0)
        {
            var minimumDistance = float.MaxValue;
            
            foreach (var e in enemies)
            {
                var distance = (e.transform.position - transform.position).sqrMagnitude;
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    enemy = e;
                }
            }
        }

        if (enemy == null) return;

        _attackTimer += deltaTime;
        if (_attackTimer > AttackInterval)
        {
            _attackTimer = 0f;
            var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
            bullet.Direction = (enemy.transform.position - transform.position).normalized;
        }
    }
}
