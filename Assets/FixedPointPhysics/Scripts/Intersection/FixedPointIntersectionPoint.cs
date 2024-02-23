/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixedpoint physics system.
*/
using BlueNoah.Math.FixedPoint;
using System.Runtime.CompilerServices;

namespace BlueNoah.PhysicsEngine
{
    public static partial class FixedPointIntersection
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
            return ClosestPointWithPointAndPlane(point,plane.distance, plane.normal);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 ClosestPointWithPointAndPlane(FixedPointVector3 point, FixedPoint64 planeDistance,FixedPointVector3 planeNormal)
        {
            var dot = FixedPointVector3.Dot(planeNormal, point);
            var distance = dot - planeDistance;
            return point - planeNormal * distance;
        }
        //Check point whether in AABB.
        //GamePhysics
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointInAABB(FixedPointVector3 point, FixedPointVector3 min, FixedPointVector3 max)
        {
            if (point.x < min.x || point.y < min.y || point.z < min.z)
            {
                return false;
            }
            if (point.x > max.x || point.y > max.y || point.z > max.z)
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
            /*
           var t = FixedPointVector3.Dot(point - start ,lVec) / FixedPointVector3.Dot(lVec,lVec);
           t = FixedPointMath.Clamp(t, 0, 1);
           return start + lVec * t;
            */
            var t = FixedPointVector3.Dot(point - start, lVec);
           if (t <= 0.0f)
           {
               return start;
           }
           else
           {
               var denom = FixedPointVector3.Dot(lVec, lVec);
               if (t >= denom)
               {
                   return end;
               }
               else
               {
                   return start + lVec * t / denom;
               }
           }
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
        public static bool PointInTriangle(FixedPointVector3 point, FixedPointVector3 center,FixedPointVector3 min,FixedPointVector3 max ,FixedPointVector3 aVertex,FixedPointVector3 bVertex ,FixedPointVector3 cVertex)
        {
            if (!PointInAABB(point, min, max))
            {
                return false;
            }
            point = point - center;
            var a = aVertex - point;
            var b = bVertex - point;
            var c = cVertex - point;
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
        public static bool PointInTriangle(FixedPointVector3 point, FixedPointVector3 vertex, FixedPointVector3 vertex1, FixedPointVector3 vertex2)
        {
            var a = vertex - point;
            var b = vertex1 - point;
            var c = vertex2 - point;
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
        public static FixedPointVector3 ClosestPointWithPointAndTriangle(FixedPointVector3 point, 
            FixedPointVector3 vertex, 
            FixedPointVector3 vertex1, 
            FixedPointVector3 vertex2,
            FixedPointVector3 normal,
            FixedPoint64 normalDistance,
            bool pointInTriangle)
        {
            var closest = ClosestPointWithPointAndPlane(point, normalDistance, normal);
            if (pointInTriangle)
            {
                return closest;
            }
            var c1 = ClosestPointWithPointAndLine(vertex, vertex1, point);
            var c2 = ClosestPointWithPointAndLine(vertex1, vertex2, point);
            var c3 = ClosestPointWithPointAndLine(vertex2, vertex, point);
            var magSq1 = (point - c1).sqrMagnitude;
            var magSq2 = (point - c2).sqrMagnitude;
            var magSq3 = (point - c3).sqrMagnitude;
            if (magSq1 < magSq2 && magSq1 < magSq3)
            {
                return c1 ;
            }
            else if (magSq2 < magSq1 && magSq2 < magSq3)
            {
                return c2 ;
            }
            return c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPointVector3 ClosestPointWithPointAndTriangle(FixedPointVector3 point, 
            FixedPointVector3 center,
            FixedPointVector3 min,
            FixedPointVector3 max,
            FixedPointVector3 a,
            FixedPointVector3 b,
            FixedPointVector3 c)
        {
            point -= center;
            var closest = ClosestPointWithPointAndPlane(point,FromTriangle(center ,a,b,c));
            if (PointInTriangle(closest, center ,min,max,a,b,c))
            {
                return closest + center; ;
            }
            var c1 = ClosestPointWithPointAndLine(a, b, point);
            var c2 = ClosestPointWithPointAndLine(b, c, point);
            var c3 = ClosestPointWithPointAndLine(c, a, point);
            var magSq1 = (point - c1).sqrMagnitude;
            var magSq2 = (point - c2).sqrMagnitude;
            var magSq3 = (point - c3).sqrMagnitude;
            if (magSq1 < magSq2 && magSq1 < magSq3)
            {
                return c1 + center;
            }else if (magSq2 < magSq1 && magSq2 < magSq3)
            {
                return c2 + center; ;
            }
            return c3 + center; ;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FixedPointPlane FromTriangle(FixedPointVector3 center,FixedPointVector3 a,FixedPointVector3 b ,FixedPointVector3 c)
        {
            var fixedPointPlane = new FixedPointPlane
            {
                normal = FixedPointVector3.Normalize(FixedPointVector3.Cross(b - a,c - a))
            };
            fixedPointPlane.distance = FixedPointVector3.Dot(fixedPointPlane.normal,a + center);
            return fixedPointPlane;
        }
    }
}