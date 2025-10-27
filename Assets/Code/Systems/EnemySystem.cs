

using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class EnemySystem : GameSystem
{
    public float OuterRingOffeset = 5f;

    public float SpawnInterval = 2f;
    private float _timer;

    private FirstTickAccessor<Character> Character;
    private FirstTickAccessor<CameraSystem> CameraSystem;

    private RecordCollection<EnemyDieRecord> _enemyDieRecords = null;

    [ReadOnly] public EnemyConfig[] EnemyConfigs;

    protected override void Awake()
    {
        base.Awake();

        LoadEnemyConfigs();

        var gm = GameManager.Instance;
        CameraSystem = gm.TickRegistry.GetOrCreateCategory<CameraSystem>().AsFirstEntity<CameraSystem>();
        Character = gm.TickRegistry.GetOrCreateCategory<Character>().AsFirstEntity<Character>();
        gm.TickRegistry.GetOrCreateCategory<Enemy>().TickProcessor = this;
        _enemyDieRecords = GameManager.Instance.RecordRepository.GetOrCreateCollection<EnemyDieRecord>();
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        SpawnEnemies(deltaTime);
    }

    public override void Tick(IReadOnlyList<ITick> entities, float deltaTime)
    {
        foreach (var enemy in entities.Cast<Enemy>())
        {
            if (enemy.Health.Current <= 0)
            {
                _enemyDieRecords.Add(new EnemyDieRecord() { Position = enemy.Position });
                Debug.Log($"Enemy died at {enemy.Position}");
                enemy.MarkForTermination();
            }
        }

        base.Tick(entities, deltaTime);
    }

    protected override void OnTerminateMarkInternal()
    {
        GameManager.Instance.TickRegistry.GetOrCreateCategory<Enemy>().TickProcessor = null;
    }

    private void SpawnEnemies(float deltaTime)
    {

        _timer += Time.deltaTime;

        if (_timer > SpawnInterval)
        {
            _timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        var spawnPoint = GetRandomPointAreaAroundCharacter();
        Instantiate(EnemyConfigs[Random.Range(0, EnemyConfigs.Length)].Prefab, spawnPoint, Quaternion.identity);
    }

    private float GetCameraVisionRadius()
    {
        var camera = CameraSystem.Entity.Camera;
        float halfHeight = camera.orthographicSize;
        float halfWidth = halfHeight * camera.aspect;
        return Mathf.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
    }

    private float GetCameraVisionRadius(Camera camera)
    {
        float halfHeight = camera.orthographicSize;
        float halfWidth = halfHeight * camera.aspect;
        return Mathf.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight);
    }

    private void GetRingBounds(out float innerRingRadius, out float outerRingRadius)
    {
        innerRingRadius = GetCameraVisionRadius();
        outerRingRadius = innerRingRadius + OuterRingOffeset;
    }

    private void GetRingBounds(Camera camera, out float innerRingRadius, out float outerRingRadius)
    {
        innerRingRadius = GetCameraVisionRadius(camera);
        outerRingRadius = innerRingRadius + OuterRingOffeset;
    }

    public Vector3 GetRandomPointAreaAroundCharacter()
    {
        GetRingBounds(out float innerRingRadius, out float outerRingRadius);

        float randomAngle = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.AngleAxis(randomAngle, Vector3.forward); // вращение вокруг Z

        // Получения радиуса (дистанция от игрока) равномерно по площади кольца.
        float randomRadius = Mathf.Sqrt(Random.Range(innerRingRadius * innerRingRadius, outerRingRadius * outerRingRadius));

        return Character.Entity.Position + (randomRotation * (Vector3.right * randomRadius));
    }


    private void LoadEnemyConfigs()
    {
        EnemyConfigs = Resources.LoadAll<EnemyConfig>("Enemies");
        Debug.Log($"Loaded enemy configs: {EnemyConfigs.Length}");
    }


    private void OnDrawGizmos()
    {
        Camera camera = null;
        if (CameraSystem is null)
        {
            camera = GameObject.FindAnyObjectByType<Camera>();
        }
        else
        {
            camera = CameraSystem.Entity.Camera;
        }

        if (camera is null) return;

        var characterPosition = Character?.Entity?.Position ?? camera.transform.position;

        GetRingBounds(camera, out float innerRingRadius, out float outerRingRadius);

        Gizmos.color = Color.red;
        GizmosMore.DrawCircle(characterPosition, innerRingRadius);

        Gizmos.color = Color.orangeRed;
        GizmosMore.DrawCircle(characterPosition, outerRingRadius);
    }
}