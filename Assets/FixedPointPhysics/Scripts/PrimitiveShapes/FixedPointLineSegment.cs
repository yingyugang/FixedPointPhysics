/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixedpoint physics system.
*/
//reference: Game Physics Cookbook
using BlueNoah.Math.FixedPoint;
using UnityEngine;
namespace BlueNoah.PhysicsEngine
{
    public class FixedPointLineSegment : FixedPointShape
    {
        public FixedPointVector3 Start { get { return start; } }
        public FixedPointVector3 End { get { return end; } }
        protected FixedPointVector3 start, end;
        public FixedPointLineSegment()
        {
            shape = ShapeType.Line;
        }
        public override void DrawGizmos(bool intersected)
        {
            Gizmos.color = intersected ? Color.red : Color.white;
            Gizmos.DrawLine(start.ToVector3(), end.ToVector3());
        }
    }
}