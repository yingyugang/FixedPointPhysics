using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine {
    public class FPRaycastHit
    {
        public FPCollider fpCollider { private set; get; }
        public FixedPointVector3 point { private set; get; }
        public FixedPointVector3 outPoint { private set; get; }
        public FixedPointVector3 normal { private set; get; }

        public FPRaycastHit(FPCollider fpCollider ,FixedPointVector3 point, FixedPointVector3 normal, FixedPointVector3 outPoint)
        {
            this.fpCollider = fpCollider;
            this.point = point;
            this.normal = normal;
            this.outPoint = outPoint;
        }
    }
}