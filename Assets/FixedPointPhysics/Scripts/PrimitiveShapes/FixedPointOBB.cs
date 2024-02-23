/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixedpoint physics system.
*/
using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// Oriented Bounding Box
    /// </summary>
    public class FixedPointOBB : FixedPointShape
    {
        public FixedPointVector3 position;
        public FixedPointVector3 size;
        bool isMatrixDirty;
        bool isQuatanionDirty;
        FixedPointMatrix _orientation;
        public FixedPointMatrix orientation {
            get {
                if (isMatrixDirty)
                {
                    _orientation = FixedPointMatrix.CreateFromQuaternion(_quaternion);
                    isMatrixDirty = false;
                }
               return _orientation;
            }
            set {
                _orientation = value;
                isQuatanionDirty = true;
            }
        }
        FixedPointQuaternion _quaternion;
        public FixedPointQuaternion quaternion
        {
            get
            {
                if (isQuatanionDirty)
                {
                    _quaternion = FixedPointQuaternion.CreateFromMatrix(_orientation);
                    isQuatanionDirty = false;
                }
                return _quaternion;
            }
            set
            {
                _quaternion = value;
                isMatrixDirty = true;
            }
        }

        public FixedPointAABB aabb;
        public FixedPointOBB()
        {
            shape = ShapeType.OBB;
        }
        public FixedPointOBB(FixedPointVector3 position,FixedPointVector3 size, FixedPointMatrix orientation)
        {
            this.position = position;
            this.size = size;
            _orientation = orientation;
            _quaternion = FixedPointQuaternion.CreateFromMatrix(orientation) ;
            shape = ShapeType.OBB;
        }

        public FixedPointOBB(FixedPointVector3 position, FixedPointVector3 size, FixedPointQuaternion orientation)
        {
            this.position = position;
            this.size = size;
            _quaternion = orientation;
            _orientation = FixedPointMatrix.CreateFromQuaternion(orientation) ;
            shape = ShapeType.OBB;
        }

        public FixedPointOBB(FixedPointVector3 position, FixedPointVector3 size, FixedPointVector3 orientation)
        {
            this.position = position;
            this.size = size;
            _quaternion = FixedPointQuaternion.Euler(orientation);
            _orientation = FixedPointMatrix.CreateFromQuaternion(_quaternion);
            shape = ShapeType.OBB;
        }

        public override void DrawGizmos(bool intersected)
        {
            var matrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position.ToVector3(), quaternion.ToQuaternion(), size.ToVector3());
            Gizmos.DrawCube(position.ToVector3(), size.ToVector3());
            Gizmos.matrix = matrix;
        }
        //Point and oriented bounding box.
        public bool PointInOBB(FixedPointVector3 point)
        {
            var distance3d = point - position;
            var distance = FixedPointVector3.Dot(distance3d, new FixedPointVector3(_orientation.M11, _orientation.M12, _orientation.M13));
            if (distance > size.x || distance < -size.x)
            {
                return false;
            }
            distance = FixedPointVector3.Dot(distance3d, new FixedPointVector3(_orientation.M21, _orientation.M22, _orientation.M23));
            if (distance > size.y || distance < -size.y)
            {
                return false;
            }
            distance = FixedPointVector3.Dot(distance3d, new FixedPointVector3(_orientation.M31, _orientation.M32, _orientation.M33));
            if (distance > size.z || distance < -size.z)
            {
                return false;
            }
            return true;
        }
    }
}