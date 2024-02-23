/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixedpoint physics system.
*/
using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class FixedPointSphere : FixedPointShape
    {
        public FixedPointVector3 Point { get { return point; } }
        public FixedPoint64 Radius { get { return radius; } }
        protected FixedPoint64 radius;
        protected FixedPointVector3 point;
        public FixedPointSphere()
        {
            shape = ShapeType.Sphere;
        }
        public override void DrawGizmos(bool intersected)
        {
            Gizmos.color = intersected ? Color.red : Color.white;
            Gizmos.DrawSphere(Point.ToVector3(), radius.AsFloat());
        }
    }
}