/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixed point physics system.
* TODO AACylinder vs Triangles(OBB split it to triangles)
*/
using BlueNoah.Math.FixedPoint;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public static partial class FixedPointIntersection
    {
        /*
        public static bool Intersect(FixedPointRay r, FixedPointAABB aabb)
        {
            FixedPoint64 x0, x1, y0, y1, z0, z1;
            int intercept = 0;
            if (r.Dir.x != 0 &&  r.Dir.y != 0)
            {
                if (r.Dir.x != 0)
                {
                    y0 = (aabb.Min.x - r.Point.x) * r.Dir.y / r.Dir.x + r.Point.y;
                    y1 = (aabb.Max.x - r.Point.x) * r.Dir.y / r.Dir.x + r.Point.y;
                }
                else
                {
                    y0 = aabb.Min.y;
                    y1 = aabb.Max.y;
                }
                if ((y0 <= aabb.Max.y && y0 >= aabb.Min.y) || (y1 <= aabb.Max.y && y1 >= aabb.Min.y))
                {
                    intercept++;
                }
            }
            else
            {
                y0 = y1 = r.Point.y;
                if ((r.Point.y <= aabb.Max.y && r.Point.y >= aabb.Min.y) || (r.Point.x <= aabb.Max.x && r.Point.x >= aabb.Min.x))
                {
                    intercept++;
                }
            }

            if (r.Dir.y != 0)
            {
                z0 = (aabb.Min.y - r.Point.y) * r.Dir.z / r.Dir.y + r.Point.z;
                z1 = (aabb.Max.y - r.Point.y) * r.Dir.z / r.Dir.y + r.Point.z;
            }
            else
            {
                z0 = z1 = r.Point.z;
            }
            if ((z0 <= aabb.Max.z && z0 >= aabb.Min.z) || (z1 <= aabb.Max.z && z1 >= aabb.Min.z))
            {
                intercept++;
            }

            if (r.Dir.z != 0)
            {
                x0 = (aabb.Min.z - r.Point.z) * r.Dir.x / r.Dir.z + r.Point.x;
                x1 = (aabb.Max.z - r.Point.z) * r.Dir.x / r.Dir.z + r.Point.x;
            }
            else
            {
                x0 = x1 = r.Point.x;
            }
            if ((x0 <= aabb.Max.x && x1 >= aabb.Min.x) || (x1 <= aabb.Max.x && z1 >= aabb.Min.x))
            {
                intercept++;
            }
            
            if (intercept > 1)
            {
                FixedPoint64 t = 0;
                FixedPoint64 t0 = 0;
                FixedPoint64 t1 = 0;
                if (r.Dir.x != 0)
                {
                    t = r.Point.x / r.Dir.x;
                    t0 = x0 / r.Dir.x;
                    t1 = x1 / r.Dir.x;
                    
                }else if (r.Dir.y != 0)
                {
                    t = r.Point.y / r.Dir.y;
                    t0 = y0 / r.Dir.y;
                    t1 = y1 / r.Dir.y;
                }
                else if (r.Dir.z != 0)
                {
                    t = r.Point.z / r.Dir.z;
                    t0 = z0 / r.Dir.z;
                    t1 = z1 / r.Dir.z;
                }
                if (t0 < t && t1 < t)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        */
        /*
         * Return parametric point of intersection 0 ... 1,or a really huge 
         * number if no intersection is found.
         */
        public static float RaycastAABBFloat(
            Vector3 origin,//origin of the ray
            Vector3 delta,//length and direction of the ray
            Vector3 min,
            Vector3 max,
            out Vector3 normal//optionally , the normal is returned
            )
        {
            //return this if no intersection
            float kNoIntersection = float.MaxValue;
            //Check for point inside box,trivial reject,and determine parametric distance to each front face
            bool inside = true;
            normal = Vector3.zero;

            float xt, xn = 0;
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

            float yt, yn = 0;
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

            float zt, zn = 0;
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
                //normal = -delta;
                //normal.Normalize();
                //return 0f;
                return kNoIntersection;
            }
            //Select farthest plane - this is the plane of intersection.
            int which = 0;
            float t = xt;
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
                        float y = origin.y + delta.y * t;
                        if (y < min.y || y > max.y) return kNoIntersection;
                        float z = origin.z + delta.z * t;
                        if (z < min.z || z > max.z) return kNoIntersection;
                        normal = new Vector3(xn, 0, 0);
                    }
                    break;
                case 1://intersect with xz plane
                    {
                        float x = origin.x + delta.x * t;
                        if (x < min.x || x > max.x) return kNoIntersection;
                        float z = origin.z + delta.z * t;
                        if (z < min.z || z > max.z) return kNoIntersection;
                        normal = new Vector3(0, yn, 0);
                    }
                    break;
                case 2://intersect with xy plane
                    {
                        float x = origin.x + delta.x * t;
                        if (x < min.x || x > max.x) return kNoIntersection;
                        float y = origin.y + delta.y * t;
                        if (y < min.y || y > max.y) return kNoIntersection;
                        normal = new Vector3(0, 0, zn);
                    }
                    break;
            }
            return t;
        }

        /*
        * reference : https://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection
        * Algebraic form.
        */
        public static bool IntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint, out Vector3 intersection)
        {
            intersection = Vector3.zero;
            var denominator = Vector3.Dot(direct.normalized, planeNormal);
            if (denominator == 0)
            {
                return false;
            }
            float d = Vector3.Dot(planePoint - point, planeNormal) / denominator;
            intersection = d * direct.normalized + point;
            return true;
        }

        public static bool IntersectWithLineAndPlaneFixedPoint(FixedPointVector3 point, FixedPointVector3 direct, FixedPointVector3 planeNormal, FixedPointVector3 planePoint, out FixedPointVector3 intersection)
        {
            intersection = FixedPointVector3.zero;
            var denominator = FixedPointVector3.Dot(direct.normalized, planeNormal);
            if (denominator == 0)
            {
                return false;
            }
            var d = FixedPointVector3.Dot(planePoint - point, planeNormal) / denominator;
            intersection = d * direct.normalized + point;
            return true;
        }

        // based on https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection
        // reference https://github.com/mattatz/unity-intersections/blob/master/Assets/Intersections/Scripts/Shapes/Intersection.cs
        public static bool Intersects(FixedPointRay r, FixedPointAABB aabb)
        {
            FixedPoint64 tmin, tmax, tymin, tymax, tzmin, tzmax;
            tmin = ((r.Sign.x <= FixedPoint64.Epsilon ? aabb.Min.x : aabb.Max.x) - r.Point.x) * r.InvDir.x;
            tmax = ((r.Sign.x <= FixedPoint64.Epsilon ? aabb.Max.x : aabb.Min.x) - r.Point.x) * r.InvDir.x;
            tymin = ((r.Sign.y <= FixedPoint64.Epsilon ? aabb.Min.y : aabb.Max.y) - r.Point.y) * r.InvDir.y;
            tymax = ((r.Sign.y <= FixedPoint64.Epsilon ? aabb.Max.y : aabb.Min.y) - r.Point.y) * r.InvDir.y;
            if ((tmin > tymax) || (tymin > tmax))
            {
                return false;
            }
            if (tymin > tmin)
            {
                tmin = tymin;
            }

            if (tymax < tmax)
            {
                tmax = tymax;
            }

            tzmin = ((r.Sign.z <= FixedPoint64.Epsilon ? aabb.Min.z : aabb.Max.z) - r.Point.z) * r.InvDir.z;
            tzmax = ((r.Sign.z <= FixedPoint64.Epsilon ? aabb.Max.z : aabb.Min.z) - r.Point.z) * r.InvDir.z;

            if ((tmin > tzmax) || (tzmin > tmax))
            {
                return false;
            }

            if (tzmin > tmin)
            {
                tmin = tzmin;
            }

            if (tzmax < tmax)
            {
                tmax = tzmax;
            }
            Debug.Log(new Vector3(tmin.AsFloat(), tymin.AsFloat(), tzmin.AsFloat()));
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint64 SqrDistanceToLine(FixedPointRay ray, FixedPointVector3 point)
        {
            return FixedPointVector3.Cross(ray.Dir, point - ray.Point).sqrMagnitude;
        }
    }
}