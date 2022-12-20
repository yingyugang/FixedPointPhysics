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
        public bool isColliderDirty;
        public FixedPointVector3 fixedPointPosition {
            get {
                return mPosition;
            } 
            set {
                mPosition = value;
                onTransfered?.Invoke();
                isColliderDirty = true;
            }
        }
        bool isMatrixDirty;
        FixedPointMatrix _fixedPointMatrix;
        public FixedPointMatrix fixedPointMatrix { 
            get {
                if (isMatrixDirty)
                {
                    _fixedPointMatrix = FixedPointMatrix.CreateFromYawPitchRoll(_fixedPointEulerAngles.y * FixedPoint64.Deg2Rad, _fixedPointEulerAngles.x * FixedPoint64.Deg2Rad, _fixedPointEulerAngles.z * FixedPoint64.Deg2Rad);
                    isMatrixDirty = false;
                }
                return _fixedPointMatrix;
            }
            set {
                _fixedPointMatrix = value;
                isEulerAnglesDirty = true;
                _fixedPointEulerAngles = _fixedPointMatrix.eulerAngles;
                onTransfered?.Invoke();
                isColliderDirty = true;
            } 
        }
        bool isEulerAnglesDirty;
        FixedPointVector3 _fixedPointEulerAngles;
        public FixedPointVector3 fixedPointEulerAngles { 
            get {
                if (isEulerAnglesDirty)
                {
                    _fixedPointEulerAngles = _fixedPointMatrix.eulerAngles;
                    isEulerAnglesDirty = false;
                }
                return _fixedPointEulerAngles;
            }
            set { 
                _fixedPointEulerAngles = value;
                isMatrixDirty = true;
                onTransfered?.Invoke();
                isColliderDirty = true;
            }
        }
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