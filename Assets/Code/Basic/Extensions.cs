using System.Collections.Generic;
using UnityEngine;

public static class EnumerableExtensions
{
    public static IEnumerable<T> EnumerateTogether<T>(this IEnumerable<T> first, params IEnumerable<T>[] others)
    {
        foreach (var item in first)
            yield return item;
        foreach (var collection in others)
            foreach (var item in collection)
                yield return item;
    }
}

public static class QuaternionUtils
{
    /// <summary>
    /// Генерирует массив Quaternion'ов, равномерно распределенных вокруг заданной оси
    /// </summary>
    /// <param name="count">Количество точек</param>
    /// <param name="startRotation">Стартовый угол вращения</param>
    /// <param name="axis">Ось вращения (по умолчанию Vector3.forward для оси Z)</param>
    /// <returns>Массив Quaternion'ов</returns>
    public static Quaternion[] GenerateRotationArray(int count, Quaternion startRotation, Vector3? axis = null)
    {
        if (count <= 0)
        {
            Debug.LogWarning("GenerateRotationArray: count должен быть больше 0");
            return new Quaternion[0];
        }

        // Используем ось Z по умолчанию
        Vector3 rotationAxis = axis ?? Vector3.forward;
        rotationAxis.Normalize();

        Quaternion[] rotations = new Quaternion[count];
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i;
            Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);
            rotations[i] = rotation * startRotation;
        }

        return rotations;
    }

    // Перегрузка с углом в градусах вместо Quaternion
    public static Quaternion[] GenerateRotationArray(int count, float startAngle = 0f, Vector3? axis = null)
    {
        Vector3 rotationAxis = axis ?? Vector3.forward;
        Quaternion startRotation = Quaternion.AngleAxis(startAngle, rotationAxis);
        return GenerateRotationArray(count, startRotation, axis);
    }
}