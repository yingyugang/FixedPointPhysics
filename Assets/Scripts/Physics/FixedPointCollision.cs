using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointCollision
    {
        public bool hit;
        public FixedPointCollider collider;
        public FixedPointVector3 closestPoint;
        public FixedPointVector3 contactPoint;
        public FixedPointVector3 normal;
        public FixedPoint64 t;
        public FixedPoint64 depth;
    }
}