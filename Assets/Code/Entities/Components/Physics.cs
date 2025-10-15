
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

    //TODO: Добавить Velocity, но которая будет обновляться самой PhysicsSystem и PhysicsSystem будет устанавливать это значени в конце своих процедур. Сама она не пользуется. Это значение понадобится системе ходьбы. (или сделать так, чтобы система ходьбы на время ходьбы могла добавить вирутальную силу, просто чтобы при подсчете все сил - учитывалась и ходьба)

    // Список всех приложенных сил

    public List<AppliedForce> AppliedForces { get; } = new List<AppliedForce>();

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