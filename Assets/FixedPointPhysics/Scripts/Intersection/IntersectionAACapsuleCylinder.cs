using UnityEngine;

public class IntersectionAACapsuleCylinder
{
    private Vector3 capsuleStart;
    private Vector3 capsuleEnd;
    private float capsuleRadius;

    private Vector3 cylinderStart;
    private Vector3 cylinderEnd;
    private float cylinderRadius;

    public IntersectionAACapsuleCylinder(Vector3 capsuleStart, Vector3 capsuleEnd, float capsuleRadius, Vector3 cylinderStart, Vector3 cylinderEnd, float cylinderRadius)
    {
        this.capsuleStart = capsuleStart;
        this.capsuleEnd = capsuleEnd;
        this.capsuleRadius = capsuleRadius;

        this.cylinderStart = cylinderStart;
        this.cylinderEnd = cylinderEnd;
        this.cylinderRadius = cylinderRadius;
    }

    public bool DoesIntersect()
    {
        Vector3 axis = capsuleEnd - capsuleStart;
        float axisLength = axis.magnitude;
        axis /= axisLength;

        Vector3 cylinderDirection = cylinderEnd - cylinderStart;
        float cylinderLength = cylinderDirection.magnitude;
        cylinderDirection /= cylinderLength;

        Vector3 closestPointOnCapsule = ClosestPointOnLine(cylinderStart, axis, capsuleStart, capsuleEnd);
        Vector3 closestPointOnCylinder = ClosestPointOnLine(closestPointOnCapsule, cylinderDirection, cylinderStart, cylinderEnd);
        float distance = (closestPointOnCapsule - closestPointOnCylinder).magnitude;

        if (distance > capsuleRadius + cylinderRadius)
        {
            return false;
        }

        float dot = Vector3.Dot(closestPointOnCapsule - capsuleStart, axis);

        if (dot < 0.0f)
        {
            distance = (closestPointOnCapsule - capsuleStart).magnitude;
        }
        else if (dot > axisLength)
        {
            distance = (closestPointOnCapsule - capsuleEnd).magnitude;
        }

        return distance <= capsuleRadius;
    }

    private Vector3 ClosestPointOnLine(Vector3 point, Vector3 direction, Vector3 lineStart, Vector3 lineEnd)
    {
        direction.Normalize();
        Vector3 lineDirection = lineEnd - lineStart;
        float dot = Vector3.Dot(direction, lineDirection);

        if (dot <= 0.0f)
        {
            return lineStart;
        }
        else if (dot >= lineDirection.magnitude)
        {
            return lineEnd;
        }
        else
        {
            Vector3 closestPoint = lineStart + dot * lineDirection;
            return closestPoint;
        }
    }
}
