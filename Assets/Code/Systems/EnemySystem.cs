using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySystem : GameSystem
{
    public float OuterRingOffeset = 5f;

    public float SpawnInterval = 2f;
    private float _timer;

    private FirstTickAccessor<Character> Character;
    private FirstTickAccessor<CameraSystem> CameraSystem;

    private RecordCollection<EnemyDieRecord> _enemyDieRecords = null;

    public bool SpawnAtLeastWeakeast = true;
    [ReadOnly] public EnemyConfig[] EnemyConfigs;

    private TerrorCounter TerrorCounter;

    protected override void Awake()
    {
        base.Awake();

        LoadEnemyConfigs();

        var gm = GameManager.Instance;
        gm.TickRegistry.Register<EnemySystem>(this);

        CameraSystem = gm.TickRegistry.GetOrCreateCategory<CameraSystem>().AsFirstEntity<CameraSystem>();
        Character = gm.TickRegistry.GetOrCreateCategory<Character>().AsFirstEntity<Character>();

        gm.TickRegistry.GetOrCreateCategory<Enemy>().TickProcessor = this;

        _enemyDieRecords = GameManager.Instance.RecordRepository.GetOrCreateCollection<EnemyDieRecord>();

        TerrorCounter = gm.CounterRegistry.GetCounterOrNUll<TerrorCounter>();
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
        _timer += deltaTime;

        if (_timer > SpawnInterval)
        {
            _timer = 0f;
            SpawnEnemies();
        }
    }

    private struct EnemySpawnData
    {
        public EnemyConfig Config;

        public string Id;

        public float Power;

        public float StatsMultiplier;
    }

    private void SpawnEnemies()
    {
        float availableTerror = TerrorCounter.AvailableTerror;

        var spawnList = new List<EnemySpawnData>();
        var spawnTerrorSum = 0f;

        var iterations = 0;
        const int maxIterations = 10000;

        var availableEnemies = EnemyConfigs.Where(e => e.Power <= availableTerror).ToList();
        if (availableEnemies.Count == 0)
        {
            // Если система никого не может заспавнить по основным правилам - спавним слабого.
            if (TerrorCounter.Terror == 0 && SpawnAtLeastWeakeast)
            {
                availableEnemies.Add(GetWeakiestEnemyConfig(EnemyConfigs.ToList()));
                // Увеличиваем допустимый попрог террора чтобы функция заспавнила слабого врага.
                availableTerror = availableEnemies[0].Power;
            }
            else
            {
                return;
            }
        }

        while (spawnTerrorSum < availableTerror || iterations < maxIterations)
        {
            iterations++;

            var randomConfig = GetRandomEnemyConfig(availableEnemies);

            var spawnData = new EnemySpawnData()
            {
                Config = randomConfig,
                Id = randomConfig.Prefab.Id,
                Power = randomConfig.Power,
                StatsMultiplier = 1f
            };

            spawnList.Add(spawnData);
            spawnTerrorSum += spawnData.Power;

            // Если превысили максимальный террор - прекращаем набор врагов.
            if (spawnTerrorSum >= availableTerror)
            {
                if (spawnList.Count == 1)
                    break;

                // Превысили доступное превышение порога - удаляем последнего врага.
                if (spawnTerrorSum > TerrorCounter.TerrorCapWithThreshold)
                    spawnList.Remove(spawnList[^1]);

                break;
            }
        }

        if (spawnList.Count == 0)
            return;

        // Создание врагов на сцене.
        for (var i = 0; i < spawnList.Count; i++)
        {
            var spawnData = spawnList[i];

            var enemy = Instantiate(spawnData.Config.Prefab, GetRandomPointAreaAroundCharacter(), Quaternion.identity);
            enemy.Power = spawnData.Power;
        }

        EnemyConfig GetWeakiestEnemyConfig(List<EnemyConfig> availableConfigs)
        {
            return availableConfigs.OrderBy(e => e.Power).First();
        }

        EnemyConfig GetRandomEnemyConfig(List<EnemyConfig> availableConfigs)
        {
            return availableConfigs[Random.Range(0, availableConfigs.Count)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shrinkFactor">Сколько оставить от общего количества (0.1 == 10%)</param>
        /// <param name="statsDampingFactor">Насколько замедлить прирост характеристик при сжатии
        /// 1.0 == не замедлять; 0.5 == квадратный корень; 0.33 == кубический корень.
        /// Чем меньше statsDampingFactor, тем меньше прирост характеристик.
        /// </param>
        /// <returns></returns>
        float GetShrinkedStatsMultiplier(float shrinkFactor, float statsDampingFactor)
        {
            // M = pow(1/K, statsDampingFactor)
            // ratio = 1.0 / shrinkFactor
            // stat_multiplier = pow(ratio, statsDampingFactor)
            // Чем меньше K → сильнее сжатие → больше Ratio → больше M
            // Чем меньше statsDampingFactor → меньше M → больше снижение total характеристик
            var ratio = 1 / shrinkFactor;
            return Mathf.Pow(ratio, statsDampingFactor);
        }
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

    // public EnemyConfig GetEnemyConfig(Enemy enemy)
    // {
    //     return EnemyConfigs.FirstOrDefault(e => e.Prefab.Id == enemy.Id);
    // }

    // public float GetEnemyPower(Enemy enemy)
    // {
    //     var config= GetEnemyConfig(enemy);
    //     return config.Power;
    // }


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