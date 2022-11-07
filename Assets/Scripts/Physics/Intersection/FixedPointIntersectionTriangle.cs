using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine
{
    public partial class FixedPointIntersection
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

        public static FixedPointCollision HitWithTriangleAndSphere(FixedPointVector3 point, FixedPoint64 radius, FixedPointTriangleCollider triangle)
        {
            var hit = new FixedPointCollision();
            var plane = FromTriangle(triangle);
            var closest = ClosestPointWithPointAndPlane(point, plane);
            if (PointInTriangle(closest, triangle))
            {
                var magSq = (closest - point).sqrMagnitude;
                if (magSq <= radius * radius)
                {
                    hit.hit = true;
                    hit.point = closest;
                    hit.normal = (point - closest).normalized;
                }
            }
            else
            {
                var closestPoint = ClosestPointWithPointAndTriangle(point, triangle);
                var magSq = (closestPoint - point).sqrMagnitude;
                if (magSq <= radius * radius)
                {
                    hit.hit = true;
                    hit.point = closestPoint;
                    hit.normal = (point - closestPoint).normalized;
                }
            }
            return hit;
        }
    }
}