/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixedpoint physics system.
*/
using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class FixedPointAABB : FixedPointShape
    {
        public FixedPointVector3 Min { get { return min; } }
        public FixedPointVector3 Max { get { return max; } }
        public FixedPointVector3 Center { get { return (min + max) * 0.5f; } }

        protected FixedPointVector3 min, max;
        public FixedPointAABB()
        {
            shape = ShapeType.AABB;
        }
        public FixedPointAABB(FixedPointVector3 min, FixedPointVector3 max) : base()
        {
            this.min = FixedPointVector3.Min(min, max);
            this.max = FixedPointVector3.Max(min, max);
            shape = ShapeType.AABB;
        }
        public override void DrawGizmos(bool intersected)
        {
            Gizmos.color = intersected ? Color.red : Color.white;
            var center = (Min + Max) * 0.5f;
            Gizmos.DrawWireCube(center.ToVector3(), (Max - Min).ToVector3());
        }

        public override string ToString()
        {
            return Min.ToVector3().ToString() + Max.ToVector3().ToString();
        }
    }
}