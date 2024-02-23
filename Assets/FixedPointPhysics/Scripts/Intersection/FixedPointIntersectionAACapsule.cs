/*
 * Create 2023/2/4
 * 応彧剛　yingyugang@gmail.com
 * Math functions about y axis aligned capsule collisions.
 */
using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine
{
    public static partial class FixedPointIntersection
    {
        public static FPCollision IntersectWithAACapsuleAndSphere(
             FixedPointVector3 start,
             FixedPointVector3 end,
            FixedPoint64 radiusCapsule, 
            FixedPointVector3 center,
            FixedPoint64 radius)
        {
            var fixedPointCollision = IntersectWithSphereAndAACapsule(center, radius, start, end, radiusCapsule);
            var closestPoint = fixedPointCollision.outsidePoint;
            var outsidePoint = fixedPointCollision.closestPoint;
            fixedPointCollision.outsidePoint = outsidePoint;
            fixedPointCollision.closestPoint = closestPoint;
            fixedPointCollision.normal = -fixedPointCollision.normal;
            return fixedPointCollision;
        }
        /*
        Function name: IntersectWithSphereAndAACapsule
        Purpose: To determine if a sphere and an axis-aligned capsule intersect, and if so, to calculate the collision information.
        */
        public static FPCollision IntersectWithSphereAndAACapsule(FixedPointVector3 center,
             FixedPoint64 radius,
              FixedPointVector3 start,
              FixedPointVector3 end,
             FixedPoint64 radiusCapsule)
        {
            var fixedPointCollision = new FPCollision();
            var closestPointOnLine = new FixedPointVector3(start.x,FixedPointMath.Clamp(center.y,start.y,end.y), start.z);
            var distance = (closestPointOnLine - center).magnitude;
            if (distance * distance < (radius + radiusCapsule) * (radius + radiusCapsule))
            {
                fixedPointCollision.hit = true;
                //when the center of sphere on the axis of the capsule.
                if (center == closestPointOnLine)
                {
                    var position = (start + end) * 0.5;
                    //when the center of the sphere coincide with the center of the aacapsule.
                    if (center == position )
                    {
                        fixedPointCollision.normal = FixedPointVector3.up;
                    }
                    else
                    {
                        fixedPointCollision.normal = (center - position).normalized;
                    }
                }
                else
                {
                    fixedPointCollision.normal = (center - closestPointOnLine).normalized;
                }
                fixedPointCollision.outsidePoint = center - fixedPointCollision.normal * radius;
                fixedPointCollision.closestPoint = closestPointOnLine + fixedPointCollision.normal * FixedPointMath.Min(radiusCapsule, distance);
                fixedPointCollision.contactPoint = (fixedPointCollision.outsidePoint + fixedPointCollision.closestPoint) * 0.5;
                distance = (fixedPointCollision.closestPoint - fixedPointCollision.outsidePoint).magnitude;
                fixedPointCollision.depth = distance * 0.5;
            }
            return fixedPointCollision;
        }
        /*
        Function name: IntersectWithAACapsuleAndAACapsule
        Purpose: To determine if an axis-aligned capsule and an axis-aligned capsule intersect, and if so, to calculate the collision information.
        */
        public static FPCollision IntersectWithAACapsuleAndAACapsule(FixedPointVector3 startA,
        FixedPointVector3 endA,
        FixedPoint64 radiusCapsuleA,
        FixedPointVector3 startB,
        FixedPointVector3 endB,
        FixedPoint64 radiusCapsuleB)
        {
            FixedPointVector3 center;
            if (endB.y < startA.y)
            {
                center = startA;
            }
            else if (startB.y > endA.y)
            {
                center = endA;
            }
            else
            {
                var positionA = (startA + endA) * 0.5;
                FixedPointVector3 closestPoint = new FixedPointVector3(startB.x, FixedPointMath.Clamp(positionA.y, startB.y, endB.y), startB.z);
                center = new FixedPointVector3(startA.x, closestPoint.y, startA.z);
            }
            return IntersectWithSphereAndAACapsule(center, radiusCapsuleA, startB, endB, radiusCapsuleB);
        }

        /*
        Function name: IntersectWithAACapsuleAndAABB
        Purpose: To determine if an axis-aligned capsule and an axis-aligned bound box, and if so, to calculate the collision information.
        */
        public static FPCollision IntersectWithAACapsuleAndAABB(FixedPointVector3 startA,
        FixedPointVector3 endA,
        FixedPoint64 radiusCapsuleA,
        FixedPointVector3 min,
        FixedPointVector3 max)
        {
            FixedPointVector3 center;
            if (startA.y > max.y)
            {
                center = startA;
            }
            else if (endA.y < min.y)
            {
                center = endA;
            }
            else
            {
                var closestPoint1 = ClosestPointWithPointAndAABB(startA, min, max);
                var closestPoint2 = ClosestPointWithPointAndAABB(endA, min, max);
                var closestPoint = (closestPoint1 + closestPoint2) * 0.5;
                center = new FixedPointVector3(startA.x, closestPoint.y, startA.z);
            }
            return IntersectWithSphereAndAABB(center, radiusCapsuleA, min, max);
        }

        public static FPCollision IntersectWithAACapsuleAndOBB(FixedPointVector3 startA,
            FixedPointVector3 endA,
            FixedPoint64 radiusCapsuleA,
            FixedPointVector3 position,
            FixedPointVector3 halfSize,
            FixedPointMatrix fixedPointMatrix,
            FixedPointVector3 min,
            FixedPointVector3 max
            )
        {
            if (startA.y  > max.y)
            {
                return IntersectWithSphereAndOBB(startA, radiusCapsuleA, position, halfSize, fixedPointMatrix);
            }else if (endA.y < min.y)
            {
                return IntersectWithSphereAndOBB(endA, radiusCapsuleA, position, halfSize, fixedPointMatrix);
            }
            FPCollision fpCollision;
            var halfRadiusCapsuleA = radiusCapsuleA;
            IntersectWithRayAndOBBFixedPoint(startA - new FixedPointVector3(0, radiusCapsuleA, 0), (endA - startA).normalized, position, halfSize + new FixedPointVector3(halfRadiusCapsuleA, halfRadiusCapsuleA, halfRadiusCapsuleA), fixedPointMatrix, out fpCollision);
            if (fpCollision.hit)
            {
                var yMin = startA.y;
                var yMax = endA.y;

                if (fpCollision.outsidePoint.y < yMin)
                {
                    return IntersectWithSphereAndOBB(startA, radiusCapsuleA, position, halfSize, fixedPointMatrix);
                }
                else if (fpCollision.closestPoint.y > yMax)
                {
                    return IntersectWithSphereAndOBB(endA, radiusCapsuleA, position, halfSize, fixedPointMatrix);
                }
                else
                {
                    var y = FixedPointMath.Clamp(fpCollision.closestPoint.y, yMin, yMax);
                    var y1 = FixedPointMath.Clamp(fpCollision.outsidePoint.y, yMin, yMax);
                    var closestPoint = new FixedPointVector3(fpCollision.closestPoint.x, (y + y1) * 0.5, fpCollision.closestPoint.z);
                    return IntersectWithSphereAndOBB(closestPoint, radiusCapsuleA, position, halfSize, fixedPointMatrix);
                }
            }
            return new FPCollision();
        }
    }
}