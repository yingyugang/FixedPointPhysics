using BlueNoah.Math.FixedPoint;
using System.Runtime.CompilerServices;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointPlane : FixedPointShape
    {
        public FixedPointVector3 normal;
        public FixedPoint64 distance;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointPlane(FixedPointVector3 normal, FixedPoint64 distance)
        {
            this.normal = FixedPointVector3.Normalize(normal);
            this.distance = distance;
            shape = ShapeType.Plane;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FixedPointPlane()
        {
            normal = FixedPointVector3.zero;
            shape = ShapeType.Plane;
        }

        public override void DrawGizmos(bool intersected)
        {
            
        }
    }
}