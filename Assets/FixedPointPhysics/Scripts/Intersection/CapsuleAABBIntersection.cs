using UnityEngine;

public static class CapsuleAABBIntersection
{
    public struct IntersectionResult
    {
        public Vector3 intersectionPoint;
        public Vector3 normal;
        public float depth;
        public Vector3 contactPoint;
    }

    public static IntersectionResult TestIntersection(CapsuleCollider capsule, BoxCollider box)
    {
        IntersectionResult result = new IntersectionResult();

        Vector3 capsuleStart = capsule.transform.TransformPoint(capsule.center + Vector3.up * capsule.height * 0.5f);
        Vector3 capsuleEnd = capsule.transform.TransformPoint(capsule.center - Vector3.up * capsule.height * 0.5f);
        Vector3 capsuleDirection = (capsuleEnd - capsuleStart).normalized;
        float capsuleRadius = capsule.radius;

        Vector3 boxMin = box.transform.TransformPoint(box.center - box.size * 0.5f);
        Vector3 boxMax = box.transform.TransformPoint(box.center + box.size * 0.5f);

        float distance = 0.0f;
        Vector3 closestPoint = ClosestPointOnLineSegment(capsuleStart, capsuleEnd, boxMin, boxMax, out distance);

        float halfHeight = Vector3.Distance(capsuleStart, capsuleEnd) * 0.5f;
        float depth = halfHeight - distance;

        if (depth > 0)
        {
            Vector3 capsuleCenter = capsuleStart + capsuleDirection * halfHeight;
            Vector3 normal = (closestPoint - capsuleCenter).normalized;
            result.intersectionPoint = closestPoint;
            result.normal = normal;
            result.depth = depth;
            result.contactPoint = capsuleCenter + normal * capsuleRadius;
        }
        else
        {
            result.depth = 0;
        }

        return result;
    }

    private static Vector3 ClosestPointOnLineSegment(Vector3 start, Vector3 end, Vector3 min, Vector3 max, out float distance)
    {
        Vector3 segmentDirection = end - start;
        Vector3 segmentToBox = min - start;
        float segmentLength = segmentDirection.magnitude;
        float segmentDot = Vector3.Dot(segmentDirection, segmentToBox);

        if (segmentDot <= 0.0f)
        {
            distance = 0.0f;
            return start;
        }
        else if (segmentDot >= segmentLength)
        {
            distance = segmentLength;
            return end;
        }
        else
        {
            distance = segmentDot / segmentLength;
            Vector3 closestPoint = start + segmentDirection * distance;
            closestPoint.x = Mathf.Clamp(closestPoint.x, min.x, max.x);
            closestPoint.y = Mathf.Clamp(closestPoint.y, min.y, max.y);
            closestPoint.z = Mathf.Clamp(closestPoint.z, min.z, max.z);
            return closestPoint;
        }
    }
}