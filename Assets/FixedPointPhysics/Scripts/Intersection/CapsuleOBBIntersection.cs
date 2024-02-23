/*
 * Create 2023/2/4
 * 応彧剛　yingyugang@gmail.com
 */
using System;
using UnityEngine;

public static class CapsuleOBBIntersection
{
    public struct IntersectionResult
    {
        public Vector3 intersectionPoint;
        public Vector3 normal;
        public float depth;
        public Vector3 contactPoint;
        public bool hit;
    }

    public static IntersectionResult TestIntersection(Vector3 capsuleStart, Vector3 capsuleEnd,float capsuleRadius, Vector3 obbCenter, Vector3 obbExtents, Quaternion obbOrientation,Action<Vector3> onTest)
    {
        IntersectionResult result = new IntersectionResult();
        Vector3 capsuleDirection = (capsuleEnd - capsuleStart).normalized;
        float distance = 0.0f;
        Vector3 closestPoint = ClosestPointOnLineSegment(capsuleStart, capsuleEnd, obbCenter, obbExtents, obbOrientation, out distance);

        onTest?.Invoke(closestPoint);


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
            result.hit = true;
        }
        else
        {
            result.depth = 0;
        }

        return result;
    }

    public static Vector3 ClosestPointOnLineSegment(Vector3 start, Vector3 end, Vector3 obbCenter, Vector3 obbExtents, Quaternion obbOrientation, out float distance)
    {
        Vector3 segmentDirection = end - start;
        Vector3 segmentToBox = obbCenter - start;
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

            Vector3 localPoint = Quaternion.Inverse(obbOrientation) * (closestPoint - obbCenter);
            localPoint.x = Mathf.Clamp(localPoint.x, -obbExtents.x, obbExtents.x);
            localPoint.y = Mathf.Clamp(localPoint.y, -obbExtents.y, obbExtents.y);
            localPoint.z = Mathf.Clamp(localPoint.z, -obbExtents.z, obbExtents.z);

            closestPoint = obbOrientation * localPoint + obbCenter;
            return closestPoint;
        }
    }
}


