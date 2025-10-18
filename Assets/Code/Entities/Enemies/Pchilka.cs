

using UnityEngine;

public class Pchilka : CharacterBasedEnemy
{
    public float Speed = 2f;

    public float AttackRadius = 7f;

    public EnemyBullet BulletPrefab;

    public float AttackInterval = 1f;
    private float _attackTimer = 0f;

    protected override void TickOnTarget(float deltaTime)
    {
        UpdateAttackTimer(deltaTime);

        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance < AttackRadius)
        {
            TryAttack();
        }
        else
        {
            Move(deltaTime);
        }


    }

    private void UpdateAttackTimer(float deltaTime)
    {
       _attackTimer -= deltaTime; 
    } 

    private void TryAttack()
    {
        if (_attackTimer <= 0)
        {
            _attackTimer = AttackInterval;
            var bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);

            bullet.Direction = (Target.transform.position - transform.position).normalized;
        }
    }

    private void Move(float deltaTime)
    {
        var direction = (Target.transform.position - transform.position).normalized;
        transform.position += direction * Speed * deltaTime;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        GizmosMore.DrawCircle(transform.position, AttackRadius);
    }
        
}