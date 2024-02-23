/*
 * Create 2023/1/25 
 * 応彧剛　yingyugang@gmail.com
 * Math functions about cylinder collisions.
 * It's used by fixedpoint physics system.
 */
using BlueNoah.Math.FixedPoint;
namespace BlueNoah.PhysicsEngine
{
    public static partial class FixedPointIntersection
    {
        public static FPCollision IntersectWithSphereAndCylinder(FixedPointVector3 center,
            FixedPoint64 radius,
            FixedPointVector3 start,
            FixedPointVector3 end,
            FixedPoint64 radiusCylinder)
        {
            var fixedPointCollision = new FPCollision();
            var lVec = end - start;
            var normal = lVec.normalized;
            var t = FixedPointVector3.Dot(center - start, lVec) / FixedPointVector3.Dot(lVec, lVec);
            if (t < 0)
            {
                return IntersectWithSphereAndCircle(center, radius, start,- normal, radiusCylinder);
            }
            else if (t > 1)
            {
                return IntersectWithSphereAndCircle(center, radius, end, normal, radiusCylinder);
            } 
            else
            {
                var closestPoint = start + lVec * t;
                var distance = (closestPoint - center).magnitude;
                if (distance * distance < (radius + radiusCylinder) * (radius + radiusCylinder))
                {
                    fixedPointCollision.hit = true;
                    fixedPointCollision.normal = (center - closestPoint).normalized;
                    fixedPointCollision.outsidePoint = center - fixedPointCollision.normal * FixedPointMath.Min(radius, distance);
                    fixedPointCollision.closestPoint = closestPoint + fixedPointCollision.normal * radiusCylinder;
                    fixedPointCollision.contactPoint = (fixedPointCollision.closestPoint + fixedPointCollision.outsidePoint) * 0.5;
                    fixedPointCollision.depth = (fixedPointCollision.closestPoint - fixedPointCollision.outsidePoint).magnitude * 0.5;
                }
            }
            return fixedPointCollision;
        }
    }
}