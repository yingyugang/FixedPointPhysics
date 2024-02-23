using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public static partial class FixedPointIntersection
    {
        public static (FixedPointVector3,FixedPointVector3,FixedPoint64) ClosestPointOnLineSegmentToLineSegment(FixedPointVector3 startA, FixedPointVector3 endA,FixedPointVector3 startB, FixedPointVector3 endB)
        {
            var a_A = startA;
            var a_B = endA;
            var b_A = startB;
            var b_B = endB;
            var v0 = b_A - a_A;
            var v1 = b_B - a_A;
            var v2 = b_A - a_B;
            var v3 = b_B - a_B;
            var d0 = FixedPointVector3.Dot(v0, v0);
            var d1 = FixedPointVector3.Dot(v1, v1);
            var d2 = FixedPointVector3.Dot(v2, v2);
            var d3 = FixedPointVector3.Dot(v3, v3);
            FixedPointVector3 bestA;
            if (d2 < d0 || d2 < d1 || d3 < d0 || d3 < d1)
            {
                bestA = a_B;
            }
            else
            {
                bestA = a_A;
            }
            var bestB = ClosestPointWithPointAndLine(b_A, b_B, bestA);
            bestA = ClosestPointWithPointAndLine(a_A, a_B, bestB);
            var d4 = FixedPointVector3.Distance(bestB, bestA);
            return (bestB,bestA, d4);
        }

        public static Vector3 ClosestPointOnLineSegmentToOBB(Vector3 p1 ,Vector3 p2,Vector3 c,Vector3 u,Vector3 v,Vector3 w)
        {
            var p1Local = c + u * Vector3.Dot(p1 - c, u) + v * Vector3.Dot(p1 - c, v) + w * Vector3.Dot(p1 - c, w);
            var p2Local = c + u * Vector3.Dot(p2 - c, u) + v * Vector3.Dot(p2 - c, v) + w * Vector3.Dot(p2 - c, w);
            var lineDirection = p2Local - p1Local;
            var oc = c - p1Local;
            var projection = Vector3.Dot(oc, lineDirection) / lineDirection.sqrMagnitude;
            var projectionT = Mathf.Clamp(projection, 0, 1);
            var closestPointOnLineSegment = p1Local + projectionT * lineDirection;
            var closestPointOnOBB = c + u * closestPointOnLineSegment.x + v * closestPointOnLineSegment.y + w * closestPointOnLineSegment.z;
            return closestPointOnOBB;
        }

        public static FixedPoint64 ClosestWithLineAndOBB(
            FixedPointVector3 origin,
            FixedPointVector3 direct,
            FixedPointVector3 position,
            FixedPointVector3 halfSize,
            FixedPointMatrix orientation,
            out FPCollision intersection)
        {
            intersection = new FPCollision();
            var axisX = new FixedPointVector3(orientation.M11, orientation.M12, orientation.M13);
            var axisY = new FixedPointVector3(orientation.M21, orientation.M22, orientation.M23);
            var axisZ = new FixedPointVector3(orientation.M31, orientation.M32, orientation.M33);
            var p = position - origin;
            var f = new FixedPointVector3(FixedPointVector3.Dot(axisX, direct), FixedPointVector3.Dot(axisY, direct), FixedPointVector3.Dot(axisZ, direct));
            var e = new FixedPointVector3(FixedPointVector3.Dot(axisX, p), FixedPointVector3.Dot(axisY, p), FixedPointVector3.Dot(axisZ, p));
            FixedPoint64[] proportion = { 0, 0, 0, 0, 0, 0 };
            FixedPointVector3[] boxNormals = { axisX, axisX * -1, axisY, axisY * -1, axisZ, axisZ * -1 };
            if (f.x == 0)
            {
                if (-e.x - halfSize.x > 0 || -e.x + halfSize.x < 0)
                {
                    return -1;
                }
                f.x = 0.00001;
            }
            proportion[0] = (e.x + halfSize.x) / f.x;
            proportion[1] = (e.x - halfSize.x) / f.x;

            if (f.y == 0)
            {
                if (-e.y - halfSize.y > 0 || -e.y + halfSize.y < 0)
                {
                    return -1;
                }
                f.y = 0.00001;
            }
            proportion[2] = (e.y + halfSize.y) / f.y;
            proportion[3] = (e.y - halfSize.y) / f.y;
            if (f.z == 0)
            {
                if (-e.z - halfSize.z > 0 || -e.z + halfSize.z < 0)
                {
                    return -1;
                }
                f.z = 0.00001;
            }
            proportion[4] = (e.z + halfSize.z) / f.z;
            proportion[5] = (e.z - halfSize.z) / f.z;
            var tMin = FixedPointMath.Max(FixedPointMath.Max(FixedPointMath.Min(proportion[0], proportion[1]), FixedPointMath.Min(proportion[2], proportion[3])), FixedPointMath.Min(proportion[4], proportion[5]));
            var tMax = FixedPointMath.Min(FixedPointMath.Min(FixedPointMath.Max(proportion[0], proportion[1]), FixedPointMath.Max(proportion[2], proportion[3])), FixedPointMath.Max(proportion[4], proportion[5]));

            intersection.closestPoint = origin + direct * tMax;
            intersection.t = tMax;
            intersection.outsidePoint = origin + direct * tMin;
            intersection.contactPoint = (intersection.closestPoint + intersection.outsidePoint) * 0.5;
            intersection.hit = true;
            for (var i = 0; i < proportion.Length; i++)
            {
                if (tMax == proportion[i])
                {
                    intersection.normal = boxNormals[i];
                    break;
                }
            }
            return tMax;
/*
            if (tMax < 0)
            {
                return -1;
            }
            if (tMin > tMax)
            {
                return -1;
            }
            if (tMin < 0)
            {
                intersection.closestPoint = origin + direct * tMax;
                intersection.t = tMax;
                intersection.outsidePoint = origin + direct * tMin; ;
                intersection.hit = true;
                for (var i = 0; i < t.Length; i++)
                {
                    if (tMax == t[i])
                    {
                        intersection.normal = normals[i];
                        break;
                    }
                }
                return tMax;
            }
            intersection.closestPoint = origin + direct * tMin;
            intersection.t = tMin;
            intersection.outsidePoint = origin + direct * tMax; ;
            for (var i = 0; i < t.Length; i++)
            {
                if (tMin == t[i])
                {
                    intersection.normal = normals[i];
                    break;
                }
            }
            intersection.hit = true;
            return tMin;
*/
        }
    }
}