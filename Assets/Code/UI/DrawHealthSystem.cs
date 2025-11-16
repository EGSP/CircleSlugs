
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawHealthSystem : GameSystem
{
    public bool ShowHealthbars = false;
    public DrawHealthbar HealthbarPrefab;

    public Vector3 Offset;


    private TickCategory EnemyCategory;

    private List<DrawHealthbar> Healthbars = new();

    private TickCategory CameraSystem;

    protected override void Awake()
    {
        base.Awake();

        GameManager.Instance.TickRegistry.Register<DrawHealthSystem>(this);

        EnemyCategory = GameManager.Instance.TickRegistry.GetOrCreateCategory<Enemy>();
        EnemyCategory.Added.AddListener((enemy) => DrawHealthbar((Enemy)enemy));

        CameraSystem = GameManager.Instance.TickRegistry.GetOrCreateCategory<CameraSystem>();
    }

    protected void Start()
    {
        if(!ShowHealthbars) return;

        // Проверяем уникальность, потому что до вызова Start - категория могла обновиться и вызвать DrawHealthbar
        foreach (var enemy in EnemyCategory.Entities.Cast<Enemy>())
        {
            DrawHealthbarDuplicateCheck(enemy);
        }
    }

    public override void LateTick(float deltaTime)
    {
        if (!ShowHealthbars) return;
        // Debug.Log("DrawHealthSystem.LateTick");
        MoveHealthbars();
    }

    private void MoveHealthbars()
    {
        Camera camera = null;
        if (CameraSystem is null || CameraSystem.Entities.Count == 0) return;

        camera = CameraSystem.Entities.Cast<CameraSystem>().First().Camera;

        foreach (var healthbar in Healthbars)
        {
            var enemy = healthbar.Entity;
            var position = camera.WorldToScreenPoint(enemy.Position + Offset);

            healthbar.transform.position = position;
        }
    }


    private void DrawHealthbarDuplicateCheck(Enemy enemy)
    {
        if (Healthbars.Any(h => h.Entity == enemy)) return;
        DrawHealthbar(enemy);
    }

    private void DrawHealthbar(Enemy enemy)
    {
        var healthbar = Instantiate(HealthbarPrefab, transform);
        enemy.OnTerminateMark.AddListener(() => EnemyDied(enemy, healthbar));
        healthbar.Entity = enemy;
        Healthbars.Add(healthbar);
    }

    private void EnemyDied(Enemy enemy, DrawHealthbar healthbar)
    {
        Healthbars.Remove(healthbar);
        Destroy(healthbar.gameObject);
    }
}