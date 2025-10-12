using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TickRegistry TickRegistry { get; private set; } = new();

    public Transform Player;

    public float OuterRingOffeset = 5f;

    public Enemy[] EnemiesPrefabs;
    public float SpawnInterval = 2f;
    private float _timer;

    private TickCategory CameraSystem;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        CameraSystem = TickRegistry.GetOrCreateCategory<CameraSystem>();
    }

    public void Update()
    {

        TickRegistry.ProcessAllTick(Time.deltaTime);
        TickRegistry.ApplyAllRemovals();

        _timer += Time.deltaTime;

        if (_timer > SpawnInterval)
        {
            _timer = 0f;
            SpawnEnemy();
        }
    }

    private void FixedUpdate()
    {
        TickRegistry.ProcessAllFixedTick(Time.fixedDeltaTime);
        TickRegistry.ApplyAllRemovals();
    }

    private void LateUpdate()
    {
        TickRegistry.ProcessAllLateTick(Time.deltaTime);
        TickRegistry.ApplyAllRemovals();
    }

    private void SpawnEnemy()
    {
        var spawnPoint = GetRandomPointArea();
        var enemy = Instantiate(EnemiesPrefabs[Random.Range(0, EnemiesPrefabs.Length)], spawnPoint, Quaternion.identity);
    }

    private float GetCameraVisionRadius()
    {
        var camera = CameraSystem.Entities.Cast<CameraSystem>().First().Camera;
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

    public Vector3 GetRandomPointArea()
    {
        GetRingBounds(out float innerRingRadius, out float outerRingRadius);

        float randomAngle = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.AngleAxis(randomAngle, Vector3.forward); // вращение вокруг Z

        // Получения радиуса (дистанция от игрока) равномерно по площади кольца.
        float randomRadius = Mathf.Sqrt(Random.Range(innerRingRadius * innerRingRadius, outerRingRadius * outerRingRadius));

        return Player.position + (randomRotation * (Vector3.right * randomRadius));
    }

    private void OnDrawGizmos()
    {
        Camera camera = null;
        if(CameraSystem is null)
        {
            camera = GameObject.FindAnyObjectByType<Camera>();
        }
        else
        {
            camera = CameraSystem.Entities.Cast<CameraSystem>().First().Camera;
        }

        if (camera is null || Player is null) return;

        GetRingBounds(camera, out float innerRingRadius, out float outerRingRadius);

        Gizmos.color = Color.red;
        DrawCircle(Player.position, innerRingRadius);

        Gizmos.color = Color.orangeRed;
        DrawCircle(Player.position, outerRingRadius);
    }


    private void DrawCircle(
        Vector3 circleCenter,
        float circleRadius,
        int segmentCount = 64,
        Vector3 rotationAxis = default)
    {
        if (rotationAxis == default)
            rotationAxis = Vector3.forward; // по умолчанию для 2D

        Quaternion initialRotation = Quaternion.AngleAxis(0f, rotationAxis);
        Vector3 previousPoint = circleCenter + initialRotation * (Vector3.right * circleRadius);

        for (int segmentIndex = 1; segmentIndex <= segmentCount; segmentIndex++)
        {
            float segmentAngleDegrees = (segmentIndex * 360f) / segmentCount;
            Quaternion rotationForSegment = Quaternion.AngleAxis(segmentAngleDegrees, rotationAxis);
            Vector3 currentPoint = circleCenter + rotationForSegment * (Vector3.right * circleRadius);
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

}