/*
 * Create 2023/1/15 
 * 応彧剛　yingyugang@gmail.com
 * Math functions about capsule collisions.
 * It's used by fixedpoint physics system.
 */
using BlueNoah.Math.FixedPoint;
namespace BlueNoah.PhysicsEngine
{
    public static partial class FixedPointIntersection
    {
        public static FPCollision IntersectWithRayAndCapsule(
           FixedPointVector3 origin,//origin of the ray
           FixedPointVector3 delta,
           FixedPoint64 length,
           FixedPointVector3 min,
           FixedPointVector3 max,
            FixedPoint64 radius
           )
        {
            var fixedPointCollision = new FPCollision();


            return fixedPointCollision;
        }

        public static FPCollision IntersectWithSphereAndCapsule(FixedPointVector3 center,
           FixedPoint64 radius,
            FixedPointVector3 start,
            FixedPointVector3 end,
           FixedPoint64 radiusCapsule)
        {
            var fixedPointCollision = new FPCollision();
            var closestPointOnLine = ClosestPointWithPointAndLine(start, end, center);
            var distance = (closestPointOnLine - center).magnitude;
            if (distance * distance < (radius + radiusCapsule) * (radius + radiusCapsule))
            {
                fixedPointCollision.hit = true;
                var direct = (center - closestPointOnLine).normalized;
                fixedPointCollision.normal = direct;
                fixedPointCollision.outsidePoint = center - direct * FixedPointMath.Min(radius, distance);
                fixedPointCollision.closestPoint = closestPointOnLine + direct * radiusCapsule;
                fixedPointCollision.contactPoint = (fixedPointCollision.outsidePoint + fixedPointCollision.closestPoint) * 0.5;
                fixedPointCollision.depth = (fixedPointCollision.closestPoint - fixedPointCollision.outsidePoint).magnitude * 0.5;
            }
            return fixedPointCollision;
        }

        public static FPCollision IntersectWithCapsuleAndCapsule(FixedPointVector3 startA,
            FixedPointVector3 endA,
            FixedPoint64 capsuleRadiusA,
            FixedPointVector3 startB,
            FixedPointVector3 endB,
            FixedPoint64 capsuleRadiusB)
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
            var penetration_normal = bestA - bestB;
            var len = penetration_normal.magnitude;
            penetration_normal /= len;  // normalize
            var penetration_depth = capsuleRadiusA + capsuleRadiusB - len;
            bool intersects = penetration_depth > 0;
            var fixedPointCollision = new FPCollision();
            if (intersects)
            {
                fixedPointCollision.hit = true;
                fixedPointCollision.normal = penetration_normal;
                fixedPointCollision.closestPoint = bestB + penetration_normal * capsuleRadiusB;
                fixedPointCollision.outsidePoint = bestA - penetration_normal * capsuleRadiusA;
                fixedPointCollision.contactPoint = (fixedPointCollision.closestPoint + fixedPointCollision.outsidePoint) * 0.5;
                fixedPointCollision.depth = penetration_depth * 0.5;
            }
            return fixedPointCollision;
        }

        public static FPCollision IntersectWithCapsuleAndCapsule(FPCapsuleCollider fpCapsuleA,FPCapsuleCollider fpCapsuleB)
        {
            return IntersectWithCapsuleAndCapsule(fpCapsuleA.startPos, fpCapsuleA.endPos, fpCapsuleA.scaledRadius, fpCapsuleB.startPos, fpCapsuleB.endPos, fpCapsuleB.scaledRadius);
        }

        public static FPCollision IntersectWithCapsuleAndPlane(FixedPointVector3 startPos, FixedPointVector3 endPos, FixedPointVector3 center, FixedPoint64 radius, FixedPoint64 planeDistance, FixedPointVector3 planeNormal)
        {
            var fixedPointCollision = new FPCollision();
            var normal = (endPos - startPos).normalized;
            var denominator = FixedPointVector3.Dot(normal, planeNormal);
            //Line parallel with plane.
            if (denominator == 0 )
            {
                var closestPoint = ClosestPointWithPointAndPlane(center, planeDistance, planeNormal);
                var distance = (closestPoint - startPos).magnitude;
                if (distance < radius)
                {
                    fixedPointCollision.hit = true;
                    fixedPointCollision.normal = ( center - closestPoint).normalized;
                    fixedPointCollision.closestPoint = center - fixedPointCollision.normal * radius;
                    fixedPointCollision.depth = radius - distance;
                    fixedPointCollision.contactPoint = fixedPointCollision.closestPoint + fixedPointCollision.normal * fixedPointCollision.depth * 0.5;
                }
            }
            else
            {
                var d = FixedPointVector3.Dot(planeDistance * planeNormal - startPos, planeNormal) / denominator;
                var sqrDistance = (endPos - startPos).sqrMagnitude;
                if (d < 0)
                {
                    return IntersectWithSphereAndPlane(startPos, radius, planeDistance, planeNormal);
                }
                else if (sqrDistance < d * d)
                {
                    return IntersectWithSphereAndPlane(endPos, radius, planeDistance, planeNormal);
                }
                else
                {
                    var intersection = d * normal + startPos;
                    fixedPointCollision.hit = true;
                    fixedPointCollision.normal = planeNormal;
                    center = (intersection - startPos).sqrMagnitude < (intersection - endPos).sqrMagnitude ? startPos : endPos;
                    var closestPoint = ClosestPointWithPointAndPlane(center, planeDistance, planeNormal);
                    fixedPointCollision.normal = (closestPoint - center).normalized;
                    fixedPointCollision.depth = (closestPoint - center).magnitude + radius;
                    fixedPointCollision.closestPoint = center - fixedPointCollision.normal * radius;
                    fixedPointCollision.contactPoint = fixedPointCollision.closestPoint - fixedPointCollision.normal * fixedPointCollision.depth * 0.5;
                }
            }
            return fixedPointCollision;
        }

        public static FPCollision IntersectWithCapsuleAndCylinder(FixedPointVector3 startA,
        FixedPointVector3 endA,
        FixedPoint64 capsuleRadiusA,
        FixedPointVector3 startB,
        FixedPointVector3 endB,
        FixedPoint64 cylinderRadiusB)
        {
            var fixedPointCollision = new FPCollision();
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

            if (bestB == startB )
            {
                fixedPointCollision = IntersectWithSphereAndCircle(bestA, capsuleRadiusA, bestB, (startB - endB).normalized, cylinderRadiusB);
            }
            else if (bestB == endB)
            {
                fixedPointCollision = IntersectWithSphereAndCircle(bestA, capsuleRadiusA, bestB, (endB - startB).normalized, cylinderRadiusB);
            }
            else
            {
                var penetration_normal = bestA - bestB;
                var len = penetration_normal.magnitude;
                penetration_normal /= len;  // normalize
                var penetration_depth = capsuleRadiusA + cylinderRadiusB - len;
                bool intersects = penetration_depth > 0;
                if (intersects)
                {
                    fixedPointCollision.hit = true;
                    fixedPointCollision.normal = penetration_normal;
                    fixedPointCollision.closestPoint = bestB + penetration_normal * cylinderRadiusB;
                    fixedPointCollision.outsidePoint = bestA - penetration_normal * capsuleRadiusA;
                    fixedPointCollision.contactPoint = (fixedPointCollision.closestPoint + fixedPointCollision.outsidePoint) * 0.5;
                    fixedPointCollision.depth = penetration_depth * 0.5;
                }
            }
#if UNITY_EDITOR
            fixedPointCollision.debugInfo = bestA;
            fixedPointCollision.debugInfo1 = bestB;
#endif
            return fixedPointCollision;
        }
    }
}