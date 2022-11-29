using BlueNoah.Math.FixedPoint;
using System.Runtime.CompilerServices;

namespace BlueNoah.PhysicsEngine
{
    public partial class FixedPointIntersection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIntersectWithSphereAndSphere(FixedPointVector3 point, FixedPoint64 radius, FixedPointVector3 point1, FixedPoint64 radius1)
        {
            return (point - point1).sqrMagnitude <= (radius + radius1) * (radius + radius1);
        }
        public static FixedPointCollision IntersectWithSphereAndAABB(FixedPointVector3 center, FixedPoint64 radius, FixedPointVector3 min, FixedPointVector3 max)
        {
            var hit = new FixedPointCollision();
            var closestPoint = ClosestPointWithPointAndAABB(center, min, max);
            if ((closestPoint - center).sqrMagnitude > radius * radius)
            {
                hit.hit = false;
                return hit;
            }
            else
            {
                hit.hit = true;
                hit.closestPoint = closestPoint;
                var aabbCenter = (min + (max - min)) / 2;
                if (closestPoint == center)
                {
                    if (closestPoint == aabbCenter)
                    {
                        return hit;
                    }
                    else
                    {
                        hit.normal = (closestPoint - aabbCenter).normalized;
                    }
                }
                else
                {
                    hit.normal = (center - closestPoint).normalized;
                }
            }
            var outsidePoint = center - hit.normal * radius;
            var distance = (closestPoint - outsidePoint).magnitude;
            hit.contactPoint = closestPoint + (outsidePoint - closestPoint) * 0.5;
            hit.depth = distance * 0.5;
            return hit;
        }

        public static FixedPointCollision IntersectWithSphereAndOBB(FixedPointSphereCollider sphere,FixedPointOBBCollider obb)
        {
            return IntersectWithSphereAndOBB(sphere.position,sphere.radius,obb.position,obb.halfSize,obb.fixedPointTransform.fixedPointMatrix);
        }

        public static FixedPointCollision IntersectWithSphereAndOBB(FixedPointVector3 center, FixedPoint64 radius, FixedPointVector3 position, FixedPointVector3 halfSize,FixedPointMatrix orientation)
        {
            var hit = new FixedPointCollision();
            var closestPoint = ClosestPointWithPointAndOBB(center, position, halfSize, orientation);
            var distance = (closestPoint - center).sqrMagnitude;
            if (distance > radius * radius)
            {
                return hit;
            }
            hit.hit = true;
            hit.closestPoint = closestPoint;
            if (closestPoint == center)
            {
                if (closestPoint == position)
                {
                    hit.normal = FixedPointVector3.zero;
                    return hit;
                }
                else
                {
                    hit.normal = (closestPoint - position).normalized;
                }
            }
            else
            {
                hit.normal = (center - closestPoint).normalized;
            }
            var outsidePoint = center - hit.normal * radius;
            distance = (closestPoint - outsidePoint).magnitude;
            hit.contactPoint = closestPoint + (outsidePoint - closestPoint) * 0.5;
            hit.depth = distance * 0.5;
            return hit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointCollision IntersectWithSphereAndSphere(FixedPointVector3 point, FixedPoint64 radius, FixedPointVector3 target, FixedPoint64 targetRadius)
        {
            var hit = new FixedPointCollision();
            hit.hit = IsIntersectWithSphereAndSphere(point, radius, target, targetRadius);
            if (hit.hit)
            {
                if (point == target)
                {
                    hit.normal = FixedPointVector3.up;
                    hit.closestPoint = point;
                }
                else
                {
                    hit.normal = (point - target).normalized;
                    hit.closestPoint = point - hit.normal * radius;
                    hit.t = radius + targetRadius - (point - target).magnitude;
                    hit.depth = hit.t / 2;
                }
            }
            return hit;
        }

        public static FixedPointCollision IntersectWithSphereAndTriangle(FixedPointVector3 point, FixedPoint64 radius, FixedPointTriangleCollider triangle)
        {
            return HitWithTriangleAndSphere(point, radius, triangle);
        }

        public static FixedPointCollision IntersectWithSphereAndPlane(FixedPointVector3 point, FixedPoint64 radius, FixedPointPlane plane)
        {
            var hit = new FixedPointCollision();
            var closestPoint = ClosestPointWithPointAndPlane(point, plane);
            var distSq = (point - closestPoint).sqrMagnitude;
            var radiusSq = radius * radius;
            if(distSq < radiusSq)
            {
                hit.hit = true;
                hit.closestPoint = closestPoint;
                if (point == closestPoint)
                {
                    return hit;
                }
                hit.normal = (point - closestPoint).normalized;
                hit.t = radius - (point - closestPoint).magnitude;
                hit.depth = hit.t / 2;
            }
            return hit;
        }
    }
}