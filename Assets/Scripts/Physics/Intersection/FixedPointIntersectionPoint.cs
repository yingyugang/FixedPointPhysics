using BlueNoah.Math.FixedPoint;
using System.Runtime.CompilerServices;

namespace BlueNoah.PhysicsEngine
{
    public partial class FixedPointIntersection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointInSphere(FixedPointVector3 point,FixedPointVector3 position, FixedPoint64 radius)
        {
            return (point - position).sqrMagnitude <= radius * radius;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 ClosestPointWithPointAndSphere(FixedPointVector3 point, FixedPointVector3 position, FixedPoint64 radius)
        {
            if ((point - position).sqrMagnitude <= radius * radius)
            {
                return point;
            }
            return  (point - position).normalized * radius + position;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointOnPlane(FixedPointVector3 point, FixedPointPlane plane)
        {
            var dot = FixedPointVector3.Dot(point,plane.normal);
            return dot - plane.distance == 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 ClosestPointWithPointAndPlane(FixedPointVector3 point, FixedPointPlane plane)
        {
            var dot = FixedPointVector3.Dot(plane.normal,point);
            var distance = dot - plane.distance;
            return point - plane.normal * distance;
        }
        //Check point whether in AABB.
        //GamePhysics
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointInAABB(FixedPointVector3 point, FixedPointVector3 min, FixedPointVector3 max)
        {
            if (point.x < min.x || point.y < min.y || point.y < min.y)
            {
                return false;
            }
            if (point.x > max.x || point.y > max.y || point.y > max.y)
            {
                return false;
            }
            return true;
        }

        //ClosestPoint between Point and AABB.
        //GamePhysics
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 ClosestPointWithPointAndAABB(FixedPointVector3 point, FixedPointVector3 min, FixedPointVector3 max)
        {
            var result = point;
            result.x = FixedPointMath.Clamp(result.x, min.x, max.x);
            result.y = FixedPointMath.Clamp(result.y, min.y, max.y);
            result.z = FixedPointMath.Clamp(result.z, min.z, max.z);
            return result;
        }
        //Check point whether in OBB.
        //GamePhysics
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointInOBB(FixedPointVector3 point , FixedPointVector3 position, FixedPointVector3 halfSize, FixedPointMatrix fixedPointMatrix)
        {
            var dir = point - position;
            if (PointInOBBAxis(dir, new FixedPointVector3(fixedPointMatrix.M11, fixedPointMatrix.M12, fixedPointMatrix.M13), halfSize.x))
            {
                return false;
            }
            if (PointInOBBAxis(dir, new FixedPointVector3(fixedPointMatrix.M21, fixedPointMatrix.M22, fixedPointMatrix.M23), halfSize.y))
            {
                return false;
            }
            if (PointInOBBAxis(dir, new FixedPointVector3(fixedPointMatrix.M31, fixedPointMatrix.M32, fixedPointMatrix.M33), halfSize.z))
            {
                return false;
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool PointInOBBAxis(FixedPointVector3 dir,FixedPointVector3 axis,FixedPoint64 axisDis)
        {
            var distance = FixedPointVector3.Dot(dir, axis);
            return (distance < -axisDis || distance > axisDis);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 ClosestPointWithPointAndOBB(FixedPointVector3 point, FixedPointVector3 position, FixedPointVector3 halfSize, FixedPointMatrix fixedPointMatrix)
        {
            var dir = point - position;
            return position + PointDistanceOBBAxis(dir, new FixedPointVector3(fixedPointMatrix.M11, fixedPointMatrix.M12, fixedPointMatrix.M13), halfSize.x)
                + PointDistanceOBBAxis(dir, new FixedPointVector3(fixedPointMatrix.M21, fixedPointMatrix.M22, fixedPointMatrix.M23), halfSize.y)
                + PointDistanceOBBAxis(dir, new FixedPointVector3(fixedPointMatrix.M31, fixedPointMatrix.M32, fixedPointMatrix.M33), halfSize.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static FixedPointVector3 PointDistanceOBBAxis(FixedPointVector3 dir, FixedPointVector3 axis, FixedPoint64 axisDis)
        {
            var distance = FixedPointVector3.Dot(dir, axis);
            return FixedPointMath.Clamp(distance,-axisDis,axisDis) * axis;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 ClosestPointWithPointAndLine(FixedPointVector3 start , FixedPointVector3 end, FixedPointVector3 point)
        {
            var lVec = end - start;
            var t = FixedPointVector3.Dot(point - start ,lVec) / FixedPointVector3.Dot(lVec,lVec);
            t = FixedPointMath.Clamp(t, 0, 1);
            return start + lVec * t;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointOnLine(FixedPointVector3 start, FixedPointVector3 end, FixedPointVector3 point)
        {
            var closest = ClosestPointWithPointAndLine(start, end, point);
            var distanceSq = (closest - point).sqrMagnitude;
            return distanceSq == 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointOnRay(FixedPointVector3 origin, FixedPointVector3 direct, FixedPointVector3 point)
        {
            if (point == origin)
            {
                return true;
            }
            var directP = (point - origin).normalized;
            var dot = FixedPointVector3.Dot(direct,directP);
            return dot == FixedPoint64.One;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 ClostPointWithPointAndRay(FixedPointVector3 origin, FixedPointVector3 direct, FixedPointVector3 point)
        {
            var t = FixedPointVector3.Dot((point - origin), direct);
            t = FixedPointMath.Max(t,0);
            return origin + direct * t;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointInTriangle(FixedPointVector3 point, FixedPointTriangleCollider triangle)
        {
            if (!PointInAABB(point, triangle.min, triangle.max))
            {
                return false;
            }
            point = point - triangle.position;
            var a = triangle.a - point;
            var b = triangle.b - point;
            var c = triangle.c - point;
            var normPBC = FixedPointVector3.Cross(b, c);
            var normPCA = FixedPointVector3.Cross(c, a);
            var normPAB = FixedPointVector3.Cross(a, b);
            if (FixedPointVector3.Dot(normPBC, normPCA) < 0)
            {
                return false;
            }
            else if (FixedPointVector3.Dot(normPBC, normPAB) < 0)
            {
                return false;
            }
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 ClosestPointWithPointAndTriangle(FixedPointVector3 point, FixedPointTriangleCollider triangle)
        {
            point = point - triangle.position;
            var plane = FromTriangle(triangle);
            var closest = ClosestPointWithPointAndPlane(point,plane);
            if (PointInTriangle(closest, triangle))
            {
                return closest + triangle.position; ;
            }
            var c1 = ClosestPointWithPointAndLine(triangle.a, triangle.b,point);
            var c2 = ClosestPointWithPointAndLine(triangle.b, triangle.c, point);
            var c3 = ClosestPointWithPointAndLine(triangle.c, triangle.a, point);
            var magSq1 = (point - c1).sqrMagnitude;
            var magSq2 = (point - c2).sqrMagnitude;
            var magSq3 = (point - c3).sqrMagnitude;
            if (magSq1 < magSq2 && magSq1 < magSq3)
            {
                return c1 + triangle.position;
            }else if (magSq2 < magSq1 && magSq2 < magSq3)
            {
                return c2 + triangle.position; ;
            }
            return c3 + triangle.position; ;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static FixedPointPlane FromTriangle(FixedPointTriangleCollider triangle)
        {
            var plane = new FixedPointPlane();
            plane.normal = FixedPointVector3.Normalize(FixedPointVector3.Cross(triangle.b - triangle.a,triangle.c- triangle.a));
            plane.distance = FixedPointVector3.Dot(plane.normal,triangle.a + triangle.position);
            return plane;
        }
    }
}