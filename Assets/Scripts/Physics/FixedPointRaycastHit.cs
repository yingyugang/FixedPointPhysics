using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine {
    public class FixedPointRaycastHit
    {
        public FixedPointCollider collider { private set; get; }
        public FixedPointVector3 point { private set; get; }

        public FixedPointRaycastHit(FixedPointCollider collider ,FixedPointVector3 point)
        {
            this.collider = collider;
            this.point = point;
        }
    }
}