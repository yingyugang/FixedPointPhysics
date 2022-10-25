using BlueNoah.Math.FixedPoint;
using System;
namespace BlueNoah.PhysicsEngine
{
    public class FixedPointTransform
    {
        public FixedPointCollider fixedPointCollider { get;private set; }
        public void SetFixedPointCollider(FixedPointCollider fixedPointCollider)
        {
            this.fixedPointCollider = fixedPointCollider;
            if (fixedPointCollider != null)
            {
                fixedPointCollider.UpdateCollider();
            }
        }
        public Action onTransfered { get; set; }
        FixedPointVector3 mPosition;
        public FixedPointVector3 fixedPointPosition {
            get {
                return mPosition;
            } 
            set {
                mPosition = value;
                onTransfered?.Invoke();
                if (fixedPointCollider != null)
                {
                    fixedPointCollider.UpdateCollider();
                }
            }
        }
        public FixedPointVector3 fixedPointEulerAngles { get; set; }
        public FixedPointVector3 fixedPointForward { 
            get
            {
                return FixedPointQuaternion.Euler(fixedPointEulerAngles) * FixedPointVector3.forward;
            }
        }
        public FixedPointVector3 scale { get; set; }
        public FixedPointOctreeNode node { get; set; }
        public string name { get; set; }
        public FixedPointTransform(Action onTransfered = null , string name = "New Transform")
        {
            this.onTransfered = onTransfered;
            this.name = name;
        }
    }
}