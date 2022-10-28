using BlueNoah.Math.FixedPoint;
using System.Runtime.CompilerServices;

namespace BlueNoah.PhysicsEngine
{
    public partial class FixedPointIntersection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectWithSphereAndSphere(FixedPointVector3 point, FixedPoint64 radius, FixedPointVector3 point1, FixedPoint64 radius1)
        {
            return (point - point1).sqrMagnitude <= (radius + radius1) * (radius + radius1);
        }
        public static FixedPointCollision HitWithSphereAndAABB(FixedPointVector3 point, FixedPoint64 radius, FixedPointVector3 min, FixedPointVector3 max)
        {
            var hit = new FixedPointCollision();
            var closestPoint = ClosestPointWithPointAndAABB(point, min, max);
            if ((closestPoint - point).sqrMagnitude > radius * radius)
            {
                hit.hit = false;
                return hit;
            }
            else
            {
                hit.hit = true;
                hit.point = closestPoint;
                if (closestPoint == point)
                {
                    if (hit.normal == FixedPointVector3.zero)
                    {
                        hit.normal = FixedPointVector3.up;
                    }
                    else
                    {
                        hit.normal = (point - (min + (max - min) / 2)).normalized;
                    }
                }
                else
                {
                    hit.normal = (point - closestPoint).normalized;
                }
            }
            return hit;
        }

        public static FixedPointCollision HitWithSphereAndOBB(FixedPointVector3 point, FixedPoint64 radius, FixedPointVector3 position, FixedPointVector3 halfSize,FixedPointMatrix orientation)
        {
            var hit = new FixedPointCollision();
            var closestPoint = ClosestPointWithPointAndOBB(point, position, halfSize, orientation);
            if ((closestPoint - point).sqrMagnitude > radius * radius)
            {
                hit.hit = false;
                return hit;
            }
            else
            {
                hit.hit = true;
                hit.point = closestPoint;
                if (closestPoint == point)
                {
                    if (hit.normal == FixedPointVector3.zero)
                    {
                        hit.normal = FixedPointVector3.up;
                    }
                    else
                    {
                        hit.normal = (point - position).normalized;
                    }
                }
                else
                {
                    hit.normal = (point - closestPoint).normalized;
                }
            }
            return hit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointCollision HitWithSphereAndSphere(FixedPointVector3 point, FixedPoint64 radius, FixedPointVector3 target, FixedPoint64 targetRadius)
        {
            var hit = new FixedPointCollision();
            hit.hit = IntersectWithSphereAndSphere(point, radius, target, targetRadius);
            if (hit.hit)
            {
                if (point == target)
                {
                    hit.normal = FixedPointVector3.up;
                    hit.point = point;
                }
                else
                {
                    hit.normal = (point - target).normalized;
                    hit.point = point - hit.normal * radius;
                }
            }
            return hit;
        }
    }
}