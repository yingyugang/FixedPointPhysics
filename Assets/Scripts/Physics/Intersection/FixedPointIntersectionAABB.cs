using BlueNoah.Math.FixedPoint;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public partial class FixedPointIntersection
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static FixedPoint64 squared(FixedPoint64 v) { return v * v; }
        public static bool IntersectWithAABBAndSphere(FixedPointVector3 min, FixedPointVector3 max, FixedPointVector3 center, FixedPoint64 radius)
        {
            FixedPoint64 dist_squared = radius * radius;
            /* assume min and max are element-wise sorted, if not, do that now */
            if (center.x < min.x) dist_squared -= squared(center.x - min.x);
            else if (center.x > max.x) dist_squared -= squared(center.x - max.x);
            if (center.y < min.y) dist_squared -= squared(center.y - min.y);
            else if (center.y > max.y) dist_squared -= squared(center.y - max.y);
            if (center.z < min.z) dist_squared -= squared(center.z - min.z);
            else if (center.z > max.z) dist_squared -= squared(center.z - max.z);
            return dist_squared > 0;
        }
        //https://subscription.packtpub.com/book/game-development/9781787123663/9/ch09lvl1sec82/sphere-to-aabb
        //ClosestPoint between Point and AABB.
        //GamePhysics
        public static FixedPointVector3 ClosestPointWithAABBAndSphere(FixedPointVector3 point, FixedPointVector3 min, FixedPointVector3 max)
        {
            return ClosestPointWithPointAndAABB(point, min, max);
        }
        public static bool IntersectWithAABBAndAABBFixedPoint(FixedPointVector3 minA, FixedPointVector3 maxA, FixedPointVector3 minB, FixedPointVector3 maxB)
        {
            //Check for a separating axis.
            /*
            if (minA.x >= maxB.x) return false;
            if (minA.y >= maxB.y) return false;
            if (minA.z >= maxB.z) return false;
            if (maxA.x <= minB.x) return false;
            if (maxA.y <= minB.y) return false;
            if (maxA.z <= minB.z) return false;
            */
            if (minA.x > maxB.x) return false;
            if (minA.y > maxB.y) return false;
            if (minA.z > maxB.z) return false;
            if (maxA.x < minB.x) return false;
            if (maxA.y < minB.y) return false;
            if (maxA.z < minB.z) return false;
            //Overlap on all three axis,so their 
            // intersection must be non-empty
            return true;
        }
        //GamePhysics Cookbook
        public static bool IntersectWithAABBAndOBBFixedPoint(FixedPointVector3 min,FixedPointVector3 max, FixedPointOBBCollider obb)
        {
            var test = new FixedPointVector3[15];
            test[0] = new FixedPointVector3(1, 0, 0);
            test[1] = new FixedPointVector3(0, 1, 0);
            test[2] = new FixedPointVector3(0, 0, 1);
            test[3] = new FixedPointVector3(obb.fixedPointTransform.fixedPointMatrix.M11, obb.fixedPointTransform.fixedPointMatrix.M12, obb.fixedPointTransform.fixedPointMatrix.M13);
            test[4] = new FixedPointVector3(obb.fixedPointTransform.fixedPointMatrix.M21, obb.fixedPointTransform.fixedPointMatrix.M22, obb.fixedPointTransform.fixedPointMatrix.M23);
            test[5] = new FixedPointVector3(obb.fixedPointTransform.fixedPointMatrix.M31, obb.fixedPointTransform.fixedPointMatrix.M32, obb.fixedPointTransform.fixedPointMatrix.M33);
       
            for (int i = 0; i < 3; ++i)
            {
                test[6 + i * 3 + 0] = FixedPointVector3.Cross(test[i], test[0]);
                test[6 + i * 3 + 1] = FixedPointVector3.Cross(test[i], test[1]);
                test[6 + i * 3 + 2] = FixedPointVector3.Cross(test[i], test[2]);
            }
            for (int i = 0; i < 15; ++i)
            {
                if (!OverlapOnAxis(min,max,obb,test[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static FixedPointInterval GetInterval(FixedPointVector3 min, FixedPointVector3 max, FixedPointVector3 axis)
        {
            var i = min;
            var a = max;
            FixedPointVector3[] vertex = new FixedPointVector3[] {
                new FixedPointVector3(i.x, a.y, a.z),
                new FixedPointVector3(i.x, a.y, i.z),
                new FixedPointVector3(i.x, i.y, a.z),
                new FixedPointVector3(i.x, i.y, i.z),
                new FixedPointVector3(a.x, a.y, a.z),
                new FixedPointVector3(a.x, a.y, i.z),
                new FixedPointVector3(a.x, i.y, a.z),
                new FixedPointVector3(a.x, i.y, i.z) };
            var result = new FixedPointInterval();
            result.min = result.max = FixedPointVector3.Dot(axis,vertex[0]);
            for (int j = 1; j < 8; ++j)
            {
                var projection = FixedPointVector3.Dot(axis,vertex[j]);
                result.min = (projection < result.min) ? projection : result.min;
                result.max = (projection > result.max) ? projection : result.max;
            }
            return result;
        }

        public static FixedPointInterval GetInterval(FixedPointOBBCollider obb,FixedPointVector3 axis)
        {
            var vertex = new FixedPointVector3[8];
            var c = obb.position;
            var e = obb.halfSize;
            var a = new FixedPointVector3[] { 
                new FixedPointVector3(obb.fixedPointTransform.fixedPointMatrix.M11, obb.fixedPointTransform.fixedPointMatrix.M12, obb.fixedPointTransform.fixedPointMatrix.M13), 
                new FixedPointVector3(obb.fixedPointTransform.fixedPointMatrix.M21, obb.fixedPointTransform.fixedPointMatrix.M22, obb.fixedPointTransform.fixedPointMatrix.M23), 
                new FixedPointVector3(obb.fixedPointTransform.fixedPointMatrix.M31, obb.fixedPointTransform.fixedPointMatrix.M32, obb.fixedPointTransform.fixedPointMatrix.M33) 
            };
            vertex[0] = c + a[0] * e.x + a[1] * e.y + a[2] * e.z;
            vertex[1] = c - a[0] * e.x + a[1] * e.y + a[2] * e.z;
            vertex[2] = c + a[0] * e.x - a[1] * e.y + a[2] * e.z;
            vertex[3] = c + a[0] * e.x + a[1] * e.y - a[2] * e.z;
            vertex[4] = c - a[0] * e.x - a[1] * e.y - a[2] * e.z;
            vertex[5] = c + a[0] * e.x - a[1] * e.y - a[2] * e.z;
            vertex[6] = c - a[0] * e.x + a[1] * e.y - a[2] * e.z;
            vertex[7] = c - a[0] * e.x - a[1] * e.y + a[2] * e.z;
            FixedPointInterval result;
            result.min = result.max = FixedPointVector3.Dot(axis,vertex[0]);
            for (int i = 1; i < 8; i++)
            {
                var projection = FixedPointVector3.Dot(axis,vertex[i]);
                result.min = (projection < result.min) ? projection : result.min;
                result.max = (projection > result.max) ? projection : result.max;
            }
            return result;
        }

        static bool OverlapOnAxis(FixedPointVector3 min, FixedPointVector3 max, FixedPointOBBCollider obb,FixedPointVector3 axis)
        {
            var a = GetInterval(min,max,axis);
            var b = GetInterval(obb, axis);
            return ((b.min <= a.max) && (a.min <= b.max));
        }
    }
}