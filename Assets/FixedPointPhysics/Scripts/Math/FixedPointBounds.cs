using System.Collections;
using System.Collections.Generic;

namespace BlueNoah.Math.FixedPoint
{

    public class Bounds
    {
        public Bounds GetInstance(List<FixedPointVector3> points)
        {
            FixedPoint64 minX = 0;
            FixedPoint64 maxX = 0;
            FixedPoint64 minY = 0;
            FixedPoint64 maxY = 0;
            FixedPoint64 minZ = 0;
            FixedPoint64 maxZ = 0;
            for (int i = 0; i < points.Count; i++)
            {
                minX = FixedPointMath.Min(minX, points[i].x);
                maxX = FixedPointMath.Max(maxX, points[i].x);
                minY = FixedPointMath.Min(minY, points[i].y);
                maxY = FixedPointMath.Max(maxY, points[i].y);
                minZ = FixedPointMath.Min(minZ, points[i].z);
                maxZ = FixedPointMath.Max(maxZ, points[i].z);
            }
            return new Bounds(new FixedPointVector3(minX, minY, minZ), new FixedPointVector3(maxX, maxY, maxZ));
        }

        public Bounds GetInstance(FixedPointVector3 min, FixedPointVector3 max)
        {
            Bounds bounds = new Bounds(min, max);
            return bounds;
        }

        private Bounds(FixedPointVector3 min, FixedPointVector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public FixedPointVector3 GetClosest(FixedPointVector3 point)
        {
            if (Countain(point))
            {
                return point;
            }
            else
            {
                return Clamp(point);
            }
        }

        public FixedPointVector3 min
        {
            get; private set;
        }

        public FixedPointVector3 max
        {
            get; private set;
        }

        public bool Countain(FixedPointVector3 point)
        {
            return point.x >= min.x && point.y >= min.y && point.z >= min.z && point.x <= max.x && point.y <= max.y && point.z <= max.z;
        }

        FixedPointVector3 Clamp(FixedPointVector3 point)
        {
            FixedPoint64 x = FixedPointMath.Clamp(point.x, min.x, max.x);
            FixedPoint64 y = FixedPointMath.Clamp(point.y, min.y, max.y);
            FixedPoint64 z = FixedPointMath.Clamp(point.z, min.z, max.z);
            return new FixedPointVector3(x, y, z);
        }
    }
}