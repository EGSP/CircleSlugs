

using UnityEngine;

public static class GizmosMore
{
    public static void DrawCircle(
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

