/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixedpoint physics system.
*/
using BlueNoah.Math.FixedPoint;
using System;
using System.Runtime.CompilerServices;

namespace BlueNoah.PhysicsEngine
{
    public static partial class FixedPointIntersection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIntersectWithSphereAndSphere(FixedPointVector3 point, FixedPoint64 radius, FixedPointVector3 point1, FixedPoint64 radius1)
        {
            return (point - point1).sqrMagnitude <= (radius + radius1) * (radius + radius1);
        }

        public static bool IsSphereInsideAABB(FixedPointVector3 center, FixedPoint64 radius, FixedPointVector3 min, FixedPointVector3 max)
        {
            if (center.x - min.x < radius)
            {
                return false;
            }
            if (max.x - center.x < radius)
            {
                return false;
            }
            if (center.y - min.y < radius)
            {
                return false;
            }
            if (max.y - center.y < radius)
            {
                return false;
            }
            if (center.z - min.z < radius)
            {
                return false;
            }
            if (max.z - center.z < radius)
            {
                return false;
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIntersectWithSphereAndAABB(FixedPointVector3 center, FixedPoint64 radius, FixedPointVector3 min, FixedPointVector3 max)
        {
            var closestPoint = ClosestPointWithPointAndAABB(center, min, max);
            if ((closestPoint - center).sqrMagnitude > radius * radius)
            {
                return false;
            }
            return true;
        }

        public static FPCollision IntersectWithSphereAndAABB(FixedPointVector3 center, FixedPoint64 radius, FixedPointVector3 min, FixedPointVector3 max)
        {
            var hit = new FPCollision();
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
                var aabbCenter = (max + min) * 0.5;
               
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
#if UNITY_EDITOR
                hit.debugInfo = aabbCenter;
                hit.debugInfo1 = closestPoint;
#endif
            }
            hit.outsidePoint = center - hit.normal * radius;
            hit.contactPoint = (hit.outsidePoint + hit.closestPoint) * 0.5;
            hit.depth = (hit.outsidePoint - hit.closestPoint).magnitude * 0.5;
            return hit;
        }

        public static FPCollision IntersectWithSphereAndOBB(FPSphereCollider fpSphere,FPBoxCollider obb)
        {
            return IntersectWithSphereAndOBB(fpSphere.position,fpSphere.radius,obb.position,obb.halfSize,obb.fpTransform.fixedPointMatrix);
        }

        public static FPCollision IntersectWithSphereAndOBB(FixedPointVector3 center, FixedPoint64 radius, FixedPointVector3 position, FixedPointVector3 halfSize,FixedPointMatrix orientation)
        {
            var hit = new FPCollision();
            var closestPoint = ClosestPointWithPointAndOBB(center, position, halfSize, orientation);
            var distance = (closestPoint - center).sqrMagnitude;
            if (distance > radius * radius)
            {
                return hit;
            }
            hit.hit = true;
            hit.closestPoint = closestPoint;
            hit.normal = (center - closestPoint).normalized;
            if(hit.normal.sqrMagnitude < 0.1)
            {
                hit.normal = FixedPointVector3.up;
            }
            var outsidePoint = center - hit.normal * radius;
            distance = (closestPoint - outsidePoint).magnitude;
            hit.outsidePoint = outsidePoint;
            hit.contactPoint = closestPoint + (outsidePoint - closestPoint) * 0.5;
            hit.depth = distance * 0.5;
            return hit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FPCollision IntersectWithSphereAndSphere(FixedPointVector3 point, FixedPoint64 radius, FixedPointVector3 target, FixedPoint64 targetRadius)
        {
            var hit = new FPCollision
            {
                hit = IsIntersectWithSphereAndSphere(point, radius, target, targetRadius)
            };
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

        //TODO 旋转，变形。
        public static FPCollision IntersectWithSphereAndMesh(FixedPointVector3 point, FixedPoint64 radius, FPMeshCollider fpMeshCollider,Action<FixedPointVector3> onAction = null)
        {
            var constraint = FixedPointVector3.zero;
            var hit = false;
            var index = 0;
            //var relativePosition = point - fpMeshCollider.position;
            var meshPosition = fpMeshCollider.position;
            var min = point - new FixedPointVector3(radius,radius,radius) ;
            var max = point + new FixedPointVector3(radius,radius,radius) ;
            for (var i = 0; i < fpMeshCollider.triangles.Length; i = i + 3)
            {
                if (!IntersectWithAABBAndAABBFixedPoint(min , max, fpMeshCollider.minimals[index] + meshPosition, fpMeshCollider.maximals[index] + meshPosition))
                {
                    index++;
                    continue;
                }
                var collision = IntersectWithSphereAndTriangle(point,radius, 
                    fpMeshCollider.vertices[fpMeshCollider.triangles[i]] + meshPosition, 
                    fpMeshCollider.vertices[fpMeshCollider.triangles[i+1]] + meshPosition, 
                    fpMeshCollider.vertices[fpMeshCollider.triangles[i+2]] + meshPosition, 
                    fpMeshCollider.normals[index],
                    fpMeshCollider.distances[index],
                    onAction);
                if (collision.hit)
                {
                    AddConstraints(ref constraint, collision.normal);
                    hit = true;
                }
                index++;
            }
            var fixedPointCollision = new FPCollision();
            if (hit)
            {
                fixedPointCollision.collider = fpMeshCollider;
                fixedPointCollision.normal = constraint.normalized;
                fixedPointCollision.depth = constraint.magnitude * 0.5;
                fixedPointCollision.closestPoint = point - fixedPointCollision.normal * (radius - fixedPointCollision.depth );
                fixedPointCollision.hit = true;
            }
            return fixedPointCollision;
        }
        
        //TODO Constraint 方向与距离需要修改。
        private static void AddConstraints(ref FixedPointVector3 constraint,FixedPointVector3 additionalConstraint)
        {
            if (constraint == FixedPointVector3.zero)
            {
                constraint = additionalConstraint;
            }
            else
            {
                var constraintNormal = constraint.normalized;
                var magnitude = constraint.magnitude;
                var dot = FixedPointVector3.Dot(additionalConstraint, constraintNormal);
                if (dot > magnitude)
                {
                    constraint = additionalConstraint;
                }
                else if (dot > 0)
                {
                    constraint = constraint + additionalConstraint - constraintNormal * dot;
                }
                else
                {
                    constraint = constraint + additionalConstraint;
                }
            }
        }

        public static FPCollision IntersectWithSphereAndTriangle(FixedPointVector3 center,
            FixedPoint64 radius,
            FixedPointVector3 point,
            FixedPointVector3 point1,
            FixedPointVector3 point2,
            FixedPointVector3 normal,
            FixedPoint64 normalDistance,
            Action<FixedPointVector3> onAction = null)
        {
            var hit = new FPCollision();
            var closest = ClosestPointWithPointAndPlane(center, normalDistance, normal);
            if (PointInTriangle(closest, point, point1, point2))
            {
                onAction?.Invoke(closest);
                var magSq = (closest - center).sqrMagnitude;
                if (magSq <= radius * radius)
                {
                    hit.hit = true;
                    hit.closestPoint = closest;
                    if (magSq == 0)
                    {
                        return hit;
                    }
                    hit.normal = normal * (radius - (closest - center).magnitude);
                }
                else
                {
                    return hit;
                }
            }
            else
            {
                //TODO 这种情况下的Normal计算
                var closestPoint = ClosestPointWithPointAndTriangle(center, point, point1, point2, normal, normalDistance, false);
                var magSq = (closestPoint - center).sqrMagnitude;
                if (magSq <= radius * radius)
                {
                    hit.hit = true;
                    hit.normal = normal * (radius - (closest - center).magnitude);
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

        public static FPCollision IntersectWithSphereAndPlane(FixedPointVector3 point, FixedPoint64 radius, FixedPointPlane plane)
        {
            return IntersectWithSphereAndPlane(point,radius,plane.distance,plane.normal);
        }

        public static FPCollision IntersectWithSphereAndPlane(FixedPointVector3 point, FixedPoint64 radius, FixedPoint64 planeDistance, FixedPointVector3 planeNormal)
        {
            var hit = new FPCollision();
            var closestPoint = ClosestPointWithPointAndPlane(point, planeDistance,planeNormal);
            var distSq = (point - closestPoint).sqrMagnitude;
            var radiusSq = radius * radius;
            if (distSq < radiusSq)
            {
                hit.hit = true;
                hit.normal = (point - closestPoint).normalized;
                hit.closestPoint = closestPoint;
                var outsidePoint = point - hit.normal * radius;
                if (point == closestPoint)
                {
                    return hit;
                }
                hit.depth = (closestPoint - outsidePoint).magnitude * 0.5;
                hit.contactPoint = point - hit.normal * (radius - hit.depth);
            }
            return hit;
        }

        private static readonly FixedPointPlane plane = new FixedPointPlane();
        public static FPCollision IntersectWithSphereAndCircle(FixedPointVector3 centerSphere,
           FixedPoint64 radiusSphere,
           FixedPointVector3 centerCircle,
           FixedPointVector3 normalCircle,
           FixedPoint64 radiusCircle)
        {
            var fixedPointCollision = new FPCollision();
            plane.distance = FixedPointVector3.Dot(centerCircle, normalCircle);
            plane.normal = normalCircle;
            var closestPointOnPlane = ClosestPointWithPointAndPlane(centerSphere, plane);
            if ((closestPointOnPlane - centerSphere).sqrMagnitude < radiusSphere * radiusSphere)
            {
                if ((closestPointOnPlane - centerCircle).sqrMagnitude < radiusCircle * radiusCircle)
                {
                    fixedPointCollision.hit = true;
                    fixedPointCollision.normal = normalCircle;
                    fixedPointCollision.closestPoint = closestPointOnPlane;
                    fixedPointCollision.outsidePoint = centerSphere - fixedPointCollision.normal * radiusSphere;
                    fixedPointCollision.contactPoint = (fixedPointCollision.closestPoint + fixedPointCollision.outsidePoint) * 0.5;
                    fixedPointCollision.depth = (fixedPointCollision.outsidePoint - closestPointOnPlane).magnitude * 0.5;
                }
                else
                {
                    var edge = (closestPointOnPlane - centerSphere).magnitude;
                    var radius1 = FixedPoint64.Sqrt(radiusSphere * radiusSphere - edge * edge);
                    var collisionPlanePointDistance = (closestPointOnPlane - centerCircle).magnitude;
                    if (radiusCircle + radius1 > collisionPlanePointDistance)
                    {
                        fixedPointCollision.hit = true;
                        var direct = (closestPointOnPlane - centerCircle).normalized;
                        fixedPointCollision.closestPoint = centerCircle + direct * radiusCircle;
                        fixedPointCollision.normal = (centerSphere - fixedPointCollision.closestPoint).normalized;
                        fixedPointCollision.outsidePoint = centerSphere - fixedPointCollision.normal * radiusSphere;
                        fixedPointCollision.contactPoint = (fixedPointCollision.closestPoint + fixedPointCollision.outsidePoint) * 0.5;
                        fixedPointCollision.depth = (fixedPointCollision.closestPoint - fixedPointCollision.outsidePoint).magnitude * 0.5;
                    }
                }
            }
            return fixedPointCollision;
        }

    }
}