using BlueNoah.Math.FixedPoint;
using System;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public partial class FixedPointIntersection
    {
        public static bool IntersectWithRayAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint, out Vector3 intersection)
        {
            intersection = Vector3.zero;
            var denominator = Vector3.Dot(direct.normalized, planeNormal);
            if (denominator == 0)
            {
                return false;
            }
            float d = Vector3.Dot(planePoint - point, planeNormal) / denominator;
            if (d <= 0)
            {
                return false;
            }
            intersection = d * direct.normalized + point;
            return true;
        }
        /*
        public static bool IntersectWithRayAndPlaneFixedPoint(FixedPointVector3 point, FixedPointVector3 direct, FixedPointVector3 planeNormal, FixedPointVector3 planePoint, out FixedPointVector3 intersection)
        {
            intersection = FixedPointVector3.zero;
            var denominator = FixedPointVector3.Dot(direct.normalized, planeNormal);
            if (denominator == 0)
            {
                return false;
            }
            var d = FixedPointVector3.Dot(planePoint - point, planeNormal) / denominator;
            if (d <= 0)
            {
                return false;
            }
            intersection = d * direct.normalized + point;
            return true;
        }*/
        //Physics Cookbook Chapter10
        public static bool IntersectWithRayAndPlaneFixedPoint(FixedPointVector3 point, FixedPointVector3 direct, FixedPoint64 planeDistance, FixedPointVector3 planeNormal, out FixedPointCollision intersection)
        {
            intersection = new FixedPointCollision();
            var nd = FixedPointVector3.Dot(direct,planeNormal);
            var pn = FixedPointVector3.Dot(point,planeNormal);
            if (nd >= 0)
            {
                return false;
            }
            var t = (planeDistance - pn) / nd;
            if (t >= 0)
            {
                intersection.hit = true;
                intersection.normal = planeNormal;
                intersection.point = point + direct * t;
                return true;
            }
            return false;
        }
        //Physics Cookbook Chapter11
        //1.Create a plane from the three points of the triangle
        //2.Raycast against the plane.
        //3.Check if the raycast result inside the triangle.
        public static bool IntersectWithRayAndTriangleFixedPoint(FixedPointVector3 point, FixedPointVector3 direct,FixedPointTriangleCollider triangle, out FixedPointCollision intersection) {
            var plane = FromTriangle(triangle);
            if (IntersectWithRayAndPlaneFixedPoint(point, direct, plane.distance, plane.normal,out intersection))
            {
                var hitPoint = intersection.point;
                var barycentric = Barycentric(hitPoint,triangle);
                if (barycentric.x >= 0 && barycentric.x <= 1 && barycentric.y >= 0 && barycentric.y <= 1 && barycentric.z >= 0 && barycentric.z <= 1)
                {
                    return true;
                }
            }
            intersection = new FixedPointCollision();
            return false;
        }

        static FixedPointVector3 Barycentric(FixedPointVector3 point,FixedPointTriangleCollider triangle)
        {
            point = point - triangle.fixedPointTransform.fixedPointPosition;
            var ap = point - triangle.a;
            var bp = point - triangle.b;
            var cp = point - triangle.c;
            var ab = triangle.b - triangle.a;
            var ac = triangle.c - triangle.a;
            var bc = triangle.c - triangle.b;
            var cb = triangle.b - triangle.c;
            var ca = triangle.a - triangle.c;
            var v = ab - FixedPointVector3.Project(ab, cb.normalized);
            var a = 1 - FixedPointVector3.Dot(v, ap) / FixedPointVector3.Dot(v,ab);
            v = bc - FixedPointVector3.Project(bc,ac.normalized);
            var b = 1 - FixedPointVector3.Dot(v, bp) / FixedPointVector3.Dot(v,bc);
            v = ca - FixedPointVector3.Project(ca,ab.normalized);
            var c = 1 - FixedPointVector3.Dot(v, cp) / FixedPointVector3.Dot(v,ca);
            return new FixedPointVector3(a,b,c);
        }
        
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
            var tmin = FixedPointMath.Max(FixedPointMath.Max(FixedPointMath.Min(t[0], t[1]), FixedPointMath.Min(t[2], t[3])), FixedPointMath.Min(t[4], t[5]));
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
        public static FixedPoint64 IntersectionWithRayAndOBBFixedPoint(
              FixedPointVector3 origin,
              FixedPointVector3 direct,
              FixedPoint64 length,
              FixedPointVector3 position,
              FixedPointVector3 halfSize,
              FixedPointMatrix orientation,
              out FixedPointCollision intersection)
        {
            var t = IntersectionWithRayAndOBBFixedPoint(origin, direct, position, halfSize, orientation, out intersection);
            if (intersection.hit && intersection.t <= length)
            {
                return t;
            }
            return -1;
        }

        //GamePhysics Cookbook Chapter10
        public static FixedPoint64 IntersectionWithRayAndOBBFixedPoint(
            FixedPointVector3 origin,
            FixedPointVector3 direct,
            FixedPointVector3 position,
            FixedPointVector3 halfSize,
            FixedPointMatrix orientation,
            out FixedPointCollision intersection)
        {
            intersection = new FixedPointCollision();
            var axisX = new FixedPointVector3(orientation.M11, orientation.M12, orientation.M13);
            var axisY = new FixedPointVector3(orientation.M21, orientation.M22, orientation.M23);
            var axisZ = new FixedPointVector3(orientation.M31, orientation.M32, orientation.M33);
            var p = position - origin;
            var f = new FixedPointVector3(FixedPointVector3.Dot(axisX, direct), FixedPointVector3.Dot(axisY, direct), FixedPointVector3.Dot(axisZ, direct));
            var e = new FixedPointVector3(FixedPointVector3.Dot(axisX, p), FixedPointVector3.Dot(axisY, p), FixedPointVector3.Dot(axisZ, p));
            FixedPoint64[] t = { 0, 0, 0, 0, 0,0 };
            if (f.x == 0)
            {
                if (-e.x - halfSize.x > 0 || -e.x + halfSize.x < 0)
                {
                    return -1;
                }
                f.x = 0.00001;
            }
            t[0] = (e.x + halfSize.x) / f.x;
            t[1] = (e.x - halfSize.x) / f.x;

            if (f.y == 0)
            {
                if (-e.y - halfSize.y > 0 || -e.y + halfSize.y < 0)
                {
                    return -1;
                }
                f.y = 0.00001;
            }
            t[2] = (e.y + halfSize.y) / f.y;
            t[3] = (e.y - halfSize.y) / f.y;
            if (f.z == 0)
            {
                if (-e.z - halfSize.z > 0 || -e.z + halfSize.z < 0)
                {
                    return -1;
                }
                f.z = 0.00001;
            }
            t[4] = (e.z + halfSize.z) / f.z;
            t[5] = (e.z - halfSize.z) / f.z;
            var tmin = FixedPointMath.Max(FixedPointMath.Max(FixedPointMath.Min(t[0],t[1]), FixedPointMath.Min(t[2], t[3])), FixedPointMath.Min(t[4], t[5]));
            var tmax = FixedPointMath.Min(FixedPointMath.Min(FixedPointMath.Max(t[0], t[1]), FixedPointMath.Max(t[2], t[3])), FixedPointMath.Max(t[4], t[5]));
            if (tmax < 0)
            {
                return -1;
            }
            if (tmin > tmax)
            {
                return -1;
            }
            if (tmin < 0)
            {
                intersection.point = origin + direct * tmax;
                intersection.t = tmax;
                intersection.hit = true;
                return tmax;
            }
            intersection.point = origin + direct * tmin;
            intersection.t = tmin;
            intersection.hit = true;
            return tmin;
        }

        //GamePhysics Cookbook Capter14
        public static bool IntersetionWithRayAndSphereFixedPoint(FixedPointVector3 origin, FixedPointVector3 direct, FixedPoint64 length, FixedPointVector3 center, FixedPoint64 radius, out FixedPointCollision intersection)
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
        [Obsolete("Use another IntersectionWithRayAndSphereFixedPoint")]
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
        [Obsolete("Use another IntersectionWithRayAndSphereFixedPoint")]
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