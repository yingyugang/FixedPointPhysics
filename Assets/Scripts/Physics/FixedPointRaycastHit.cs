using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine {
    public class FixedPointRaycastHit
    {
        public FixedPointCollider collider { private set; get; }
        public FixedPointVector3 point { private set; get; }

        public FixedPointVector3 normal { private set; get; }

        public FixedPointRaycastHit(FixedPointCollider collider ,FixedPointVector3 point, FixedPointVector3 normal)
        {
            this.collider = collider;
            this.point = point;
            this.normal = normal;
        }
    }
}