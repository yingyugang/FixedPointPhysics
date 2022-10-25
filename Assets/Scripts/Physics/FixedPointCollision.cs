using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine
{
    public class FixedPointCollision
    {
        public bool hit;
        public FixedPointCollider collider;
        public FixedPointVector3 point;
        public FixedPointVector3 normal;
        public FixedPoint64 t;
    }
}