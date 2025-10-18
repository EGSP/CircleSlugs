
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонент, хранящий физические данные сущности
/// </summary>
public class Physics : MonoBehaviour
{
    [Header("Physical Properties")]
    public float Magnitude = 1f;  // Значительность сущности (для сравнения кто кого толкает)
    public float Size = 0.5f;     // Радиус тела
    public float Drag = 0f;       // Сопротивление движению

    [Header("Collision Properties")]
    public bool CanBePushed = false;
    public bool CanPushOthers = false;

    // Список всех приложенных сил

    public List<AppliedForce> AppliedForces { get; } = new List<AppliedForce>();

    public Vector3 Velocity { get; set; }

    /// <summary>
    /// Применить силу к сущности
    /// </summary>
    public void Force(Vector3 force, ForceType type, float duration = 1f)
    {
        AppliedForces.Add(new AppliedForce(force, type, duration));
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.greenYellow;
        GizmosMore.DrawCircle(transform.position, Size);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (Velocity.sqrMagnitude > 0.01f)
        {
            var nextPosition = transform.position + Velocity;
            Gizmos.DrawLine(transform.position, nextPosition);
            GizmosMore.DrawCircle(nextPosition, Size);
        }
    }
}

/// <summary>
/// Представляет одну приложенную силу
/// </summary>
public struct AppliedForce
{
    public Vector3 Force;
    public ForceType Type;
    public float Duration; // Оставшееся время действия силы

    public AppliedForce(Vector3 force, ForceType type, float duration = 1f)
    {
        Force = force;
        Type = type;
        Duration = duration;
    }
}

/// <summary>
/// Типы сил для различной обработки
/// </summary>
public enum ForceType
{
    Continuous,   // Постоянная сила на протяжении Duration
    Decay         // Затухающая сила экспоненциально
}