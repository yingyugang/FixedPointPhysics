using BlueNoah.Math.FixedPoint;
using System;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public partial class FixedPointIntersection
    {
        public static bool IntersectionWithRayAndAABBFixedPointA(FixedPointVector3 origin,//origin of the ray
            FixedPointVector3 direct,//length and direction of the ray
            FixedPointVector3 min,
            FixedPointVector3 max,
            out FixedPointCollision intersection)
        {
            intersection = new FixedPointCollision();
            var t = new FixedPoint64[] { 0, 0, 0, 0, 0, 0 };
            t[0] = (min.x - origin.x) / direct.x;
            t[1] = (max.x - origin.x) / direct.x;
            t[2] = (min.y - origin.y) / direct.y;
            t[3] = (max.y - origin.y) / direct.y;
            t[4] = (min.z - origin.z) / direct.z;
            t[5] = (max.z - origin.z) / direct.z;
            var tmin = FixedPointMath.Max( FixedPointMath.Max(FixedPointMath.Min(t[0], t[1]), FixedPointMath.Min(t[2], t[3])), FixedPointMath.Min(t[4], t[5]));
            var tmax = FixedPointMath.Min(FixedPointMath.Min(FixedPointMath.Max(t[0], t[1]), FixedPointMath.Max(t[2], t[3])), FixedPointMath.Max(t[4], t[5]));

            if (tmax < 0)
            {
                return false;
            }
            if (tmin > tmax)
            {
                return false;
            }
            var t_result = tmin;
            if (tmin < 0)
            {
                t_result = tmax;
            }
            intersection.hit = true;
            intersection.t = t_result;
            intersection.point = origin + direct * t_result;
            var normals = new FixedPointVector3[] { FixedPointVector3.left, FixedPointVector3.right, FixedPointVector3.down, FixedPointVector3.up, FixedPointVector3.back, FixedPointVector3.forward };
            for (int i = 0; i < 6; i++)
            {
                if (t_result == t[i])
                {
                    intersection.normal = normals[i];
                }
            }
            return true;
        }


        public static FixedPoint64 IntersectionWithRayAndAABBFixedPoint(
            FixedPointVector3 origin,//origin of the ray
            FixedPointVector3 delta,//length and direction of the ray
            FixedPointVector3 min,
            FixedPointVector3 max,
            out FixedPointVector3 intersection//optionally , the intersection is returned
            )
        {
            //return this if no intersection
            FixedPoint64 kNoIntersection = FixedPoint64.MaxValue;
            //Check for point inside box,trivial reject,and determine parametric distance to each front face
            bool inside = true;
            intersection = FixedPointVector3.zero;

            FixedPoint64 xt, xn = 0;
            //check x coordination
            if (origin.x < min.x)
            {
                //distance origin x and min x.
                xt = min.x - origin.x;
                if (xt > delta.x) return kNoIntersection;
                xt /= delta.x;
                inside = false;
                xn = -1.0f;
            }
            else if (origin.x > max.x)
            {
                xt = max.x - origin.x;
                if (xt < delta.x) return kNoIntersection;
                xt /= delta.x;
                inside = false;
                xn = 1.0f;
            }
            else
            {
                xt = -1.0f;
            }

            FixedPoint64 yt, yn = 0;
            if (origin.y < min.y)
            {
                yt = min.y - origin.y;
                if (yt > delta.y) return kNoIntersection;
                yt /= delta.y;
                inside = false;
                yn = -1.0f;
            }
            else if (origin.y > max.y)
            {
                yt = max.y - origin.y;
                if (yt < delta.y) return kNoIntersection;
                yt /= delta.y;
                inside = false;
                yn = 1.0f;
            }
            else
            {
                yt = -1.0f;
            }

            FixedPoint64 zt, zn = 0;
            if (origin.z < min.z)
            {
                zt = min.z - origin.z;
                if (zt > delta.z) return kNoIntersection;
                zt /= delta.z;
                inside = false;
                zn = -1.0f;
            }
            else if (origin.z > max.z)
            {
                zt = max.z - origin.z;
                if (zt < delta.z) return kNoIntersection;
                zt /= delta.z;
                inside = false;
                zn = 1.0f;
            }
            else
            {
                zt = -1.0f;
            }
            //Ray origin inside box
            if (inside)
            {
                //intersection = new FixedPointVector3(-delta.x, -delta.y, -delta.z);
                //intersection.Normalize();
                return kNoIntersection;
            }
            //Select farthest plane - this is the plane of intersection.
            int which = 0;
            FixedPoint64 t = xt;
            if (yt > t)
            {
                which = 1;
                t = yt;
            }
            if (zt > t)
            {
                which = 2;
                t = zt;
            }

            switch (which)
            {
                case 0://intersect with yz plane
                    {
                        FixedPoint64 y = origin.y + delta.y * t;
                        if (y < min.y || y > max.y) return kNoIntersection;
                        FixedPoint64 z = origin.z + delta.z * t;
                        if (z < min.z || z > max.z) return kNoIntersection;
                    }
                    break;
                case 1://intersect with xz plane
                    {
                        FixedPoint64 x = origin.x + delta.x * t;
                        if (x < min.x || x > max.x) return kNoIntersection;
                        FixedPoint64 z = origin.z + delta.z * t;
                        if (z < min.z || z > max.z) return kNoIntersection;
                    }
                    break;
                case 2://intersect with xy plane
                    {
                        FixedPoint64 x = origin.x + delta.x * t;
                        if (x < min.x || x > max.x) return kNoIntersection;
                        FixedPoint64 y = origin.y + delta.y * t;
                        if (y < min.y || y > max.y) return kNoIntersection;
                    }
                    break;
            }
            intersection = origin + delta * t;
            return t;
        }

        //GamePhysics Cookbook Capter14
        public static bool IntersetionWithRayAndSphereFixedPointA(FixedPointVector3 origin, FixedPointVector3 direct, FixedPoint64 length, FixedPointVector3 center, FixedPoint64 radius, out FixedPointCollision intersection)
        {
            intersection = new FixedPointCollision();
            var e = center - origin;
            var rSq = radius * radius;
            var eSq = e.sqrMagnitude;
            //inside
            if (eSq < rSq)
            {
                return false;
            }
            var a = FixedPointVector3.Dot(e, direct);
            var bSq = eSq - (a * a);
            //if rSq - bSq is native value will be exception.
            if (rSq < bSq)
            {
                return false;
            }
            var f = FixedPointMath.Sqrt(rSq - bSq);
            var t = a - f;
            if (rSq - (eSq - a * a) < 0.0f)
            {
                return false;
            }else if (eSq < rSq)
            {
                t = a + f;
            }
            if (t > length || t < 0)
            {
                return false;
            }
            intersection.t = t;
            intersection.hit = true;
            intersection.point = origin + direct * t;
            intersection.normal = FixedPointVector3.Normalize(intersection.point - center);
            return true;
        }

        //Algebraic function.
        //reference 3D Math Primer for Graphics and Game Development A.12
        //reference www.realtimerendering.com/intersection.com
        //direct must be normalized.
        [Obsolete("User IntersectionWithRayAndSphereFixedPointA")]
        public static bool IntersectionWithRayAndSphereFixedPoint(FixedPointVector3 origin, FixedPointVector3 direct, FixedPoint64 length, FixedPointVector3 center, FixedPoint64 radius, out FixedPointVector3 intersection)
        {
            intersection = FixedPointVector3.zero;
            var sqrRadius = radius * radius;
            var distanceA = FixedPointVector3.Dot(center - origin, direct);
            var sqrDistanceE = (center - origin).sqrMagnitude;
            if (sqrDistanceE < sqrRadius)
            {
                return false;
            }
            var sqrDistanceB = sqrDistanceE - distanceA * distanceA;
            if (sqrDistanceB > sqrRadius)
            {
                return false;
            }
            var distanceF = FixedPointMath.Sqrt(radius * radius - sqrDistanceB);
            var distanceT = distanceA - distanceF;
            if (distanceT < 0 || distanceT < length)
            {
                return false;
            }
            intersection = origin + direct * distanceT;
            return true;
        }
        [Obsolete("User IntersectionWithRayAndSphereFixedPointA")]
        public static bool IntersectionWithRayAndSphereFixedPoint(FixedPointVector3 origin, FixedPointVector3 direct, FixedPointVector3 center, FixedPoint64 radius)
        {
            var sqrRadius = radius * radius;
            var distanceA = FixedPointVector3.Dot(center - origin, direct);
            var sqrDistanceE = (center - origin).sqrMagnitude;
            if (sqrDistanceE < sqrRadius)
            {
                return false;
            }
            var sqrDistanceB = sqrDistanceE - distanceA * distanceA;
            if (sqrDistanceB > sqrRadius)
            {
                return false;
            }
            var distanceF = FixedPointMath.Sqrt(radius * radius - sqrDistanceB);
            var distanceT = distanceA - distanceF;
            if (distanceT < 0)
            {
                return false;
            }
            return true;
        }
    }
}