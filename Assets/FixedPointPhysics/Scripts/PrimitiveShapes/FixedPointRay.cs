/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixedpoint physics system.
*/
//reference: https://github.com/mattatz/unity-intersections/tree/master/Assets/Intersections/Scripts
//reference: Game Physics Cookbook
using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointRay : FixedPointShape
    {
        public FixedPointVector3 Point { get { return point; } }
        public FixedPointVector3 Dir { get { return dir; } }
        public FixedPointVector3 InvDir { get { return invDir; } }
        public FixedPointVector3 Sign { get { return sign; } }
        protected FixedPointVector3 point, dir, invDir;
        protected FixedPointVector3 sign;
        public FixedPointRay()
        {
            shape = ShapeType.Ray;
        }
        public FixedPointRay(FixedPointVector3 point, FixedPointVector3 dir) : base()
        {
            this.point = point;
            this.dir = dir.normalized;
            invDir = new FixedPointVector3(
                1 / this.dir.x,
                1 / this.dir.y,
                1 / this.dir.z
            );
            sign = new FixedPointVector3(
                invDir.x < 0 ? 1 : 0,
                invDir.y < 0 ? 1 : 0,
                invDir.z < 0 ? 1 : 0
            );
            shape = ShapeType.Ray;
        }
        public override void DrawGizmos(bool intersected)
        {
            Gizmos.color = intersected ? Color.red : Color.white;
            Gizmos.DrawSphere(Point.ToVector3(), 0.1f);
            Gizmos.DrawRay(Point.ToVector3(), Dir.ToVector3() * float.MaxValue);
        }
    }
}