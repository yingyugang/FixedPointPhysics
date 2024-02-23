using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// Represents a transform component that uses fixed point arithmetic for position, rotation, and scale,
    /// designed to improve determinism and precision in physics simulations within the Unity engine.
    /// </summary>
    [DefaultExecutionOrder(-900)]
    [ExecuteInEditMode]
    public class FPTransform : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private FPTransform parent;
        internal int indexInList;
        /// <summary>
        /// Serializes the current state of the Unity Transform into fixed point integer values.
        /// Returns true if any of the transform's serialized values have changed.
        /// </summary>
        /// <returns>True if any value has changed; otherwise, false.</returns>
        internal bool Serialize()
        {
            var prePos = localPosition;
            var preEuler = localEulerAngles;
            var preScale = localScale;
            var trans = transform;
            localPosition = new FixedPointVector3(trans.localPosition);
            localEulerAngles = new FixedPointVector3(trans.localEulerAngles);
            localScale = new FixedPointVector3(trans.localScale);
            parent = trans.parent != null ? trans.parent.GetComponent<FPTransform>() : null;
            return prePos != localPosition || preEuler != localEulerAngles || preScale != localScale ;
        }
        
        [SerializeField]
        private FixedPointVector3 _position;
        [SerializeField]
        private FixedPointVector3 _euler;
        [SerializeField]
        private FixedPointVector3 _scale;
        [SerializeField]
        private FixedPointQuaternion mLocalRotation = new (0,0,0,1);
        
        internal bool colliderUpdateFlag;
        public FixedPointVector3 localScale
        {
            get => _scale;
            set
            {
                _scale = value;
                colliderUpdateFlag = true;
            }
        }

        public FixedPointVector3 localPosition
        {
            get => _position;
            set
            {
                _position = value;
                colliderUpdateFlag = true;
            }
        }

        /// <summary>
        /// Used to calculate position with offset, like Collider center.
        /// it is faster than calculate position and offset separate.
        /// </summary>
        internal FixedPointVector3 Position(FixedPointVector3 offset)
        {
            if (parent == null)
            {
                return _position + offset;
            }
            {
                return parent.position + parent.rotation * (_position + offset);
            }
        }
        
        public FixedPointVector3 position
        {
            get
            {
                if (parent == null)
                {
                    return _position;
                }
                {
                    return parent.position + parent.rotation * _position;
                }
            }
            set
            {
                if (parent == null)
                {
                    _position = value;
                }
                else
                {
                    var p = value - parent.position;
                    _position = FixedPointQuaternion.Inverse(parent.rotation) * p;
                }
                colliderUpdateFlag = true;
            }
        }
        public FixedPointQuaternion localRotation
        {
            get => mLocalRotation;
            set
            {
                mLocalRotation = value;
                colliderUpdateFlag = true;
            }
        }

        public FixedPointQuaternion rotation
        {
            get
            {
                if (parent == null)
                {
                    return mLocalRotation;
                }
                {
                    return parent.rotation * mLocalRotation;
                }
            }
            set
            {
                if (parent == null)
                {
                    mLocalRotation = value ;
                }
                else
                {
                    mLocalRotation = FixedPointQuaternion.Inverse(parent.rotation) * value;
                }
                colliderUpdateFlag = true;
            }
        }
        public FixedPointVector3 localEulerAngles
        {
            get
            {
                _euler = mLocalRotation.eulerAngles;
                return _euler;
            }
            set
            {
                _euler = value;
                mLocalRotation = FixedPointQuaternion.Euler(_euler);
                colliderUpdateFlag = true;
            }
        }
        public FixedPointVector3 eulerAngles
        {
            get
            {
                _euler = mLocalRotation.eulerAngles;
                return parent == null ? _euler : rotation.eulerAngles;
            }
            set
            {
                rotation = FixedPointQuaternion.Euler(value);
                _euler = mLocalRotation.eulerAngles;
                colliderUpdateFlag = true;
            }
        }
        public void Rotate(FixedPointVector3 euler)
        {
            localRotation = mLocalRotation * FixedPointQuaternion.Euler(euler);
        }
        public FixedPointVector3 scale => parent == null ? localScale : FixedPointVector3.Scale(localScale, parent.scale);
        public FixedPointMatrix fixedPointMatrix => FixedPointMatrix.CreateFromQuaternion(rotation);
        public FixedPointVector3 forward => rotation * FixedPointVector3.forward;

        public FixedPointVector3 back => rotation * FixedPointVector3.back;

        public FixedPointVector3 up => rotation * FixedPointVector3.up;

        public FixedPointVector3 down => rotation * FixedPointVector3.down;

        public FixedPointVector3 right => rotation * FixedPointVector3.right;

        public FixedPointVector3 left => rotation * FixedPointVector3.left;
        
    }
}