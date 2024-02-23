/*
using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine
{
    public static partial class FixedPointIntersection
    {
        static FixedPointInterval GetInterval(FixedPointTriangleCollider triangle,FixedPointVector3 axis)
        {
            FixedPointInterval interval;
            interval.min = FixedPointVector3.Dot(axis, triangle.a);
            interval.max = interval.min;
            var val = FixedPointVector3.Dot(axis, triangle.b);
            interval.min = FixedPointMath.Min(interval.min,val);
            interval.max = FixedPointMath.Max(interval.max, val);
            val = FixedPointVector3.Dot(axis, triangle.c);
            interval.min = FixedPointMath.Min(interval.min, val);
            interval.max = FixedPointMath.Max(interval.max, val);
            return interval;
        }

        public static FixedPointCollision HitWithTriangleAndSphere(FixedPointVector3 center, FixedPoint64 radius, FixedPointTriangleCollider triangle)
        {
            var hit = new FixedPointCollision();
            var plane = FromTriangle(triangle);
            var closest = ClosestPointWithPointAndPlane(center, plane);
            if (PointInTriangle(closest, triangle.center,triangle.min,triangle.max,triangle.a,triangle.b,triangle.c))
            {
                var magSq = (closest - center).sqrMagnitude;
                if (magSq <= radius * radius)
                {
                    hit.hit = true;
                    hit.closestPoint = closest;
                    if (magSq == 0)
                    {
                        return hit;
                    }
                    hit.normal = (center -  closest).normalized;
                    hit.normal = plane.normal;
                }
                else
                {
                    return hit;
                }
            }
            else
            {
                var closestPoint = ClosestPointWithPointAndTriangle(center, triangle.center,triangle.min,triangle.max,triangle.a,triangle.b,triangle.c);
                var magSq = (closestPoint - center).sqrMagnitude;
                if (magSq <= radius * radius)
                {
                    hit.hit = true;
                    hit.closestPoint = closestPoint;
                    hit.normal = (center - closestPoint).normalized;
                }
                else
                {
                    return hit;
                }
            }
            var outsidePoint = center - hit.normal * radius;
            var distance = (hit.closestPoint - outsidePoint).magnitude;
            hit.contactPoint = hit.closestPoint + (outsidePoint - hit.closestPoint) * 0.5;
            hit.depth = distance * 0.5;
            return hit;
        }
    }
}*/