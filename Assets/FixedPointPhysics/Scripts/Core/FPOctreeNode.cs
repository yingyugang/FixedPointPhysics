using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine
{
    public class FPOctreeNode
    {
        public FPOctreeNode parentNode;
        public FPOctreeNode[] nodes;
        public readonly int size;
        public readonly int halfSize;
        public FixedPointVector3 pos;
        public FPColliderStack<FPSphereCollider> FpSphereStack;
        public FPColliderStack<FPAABBCollider> FpAABBStack;
        public FPColliderStack<FPBoxCollider> FpObbStack;
        public FPColliderStack<FPCapsuleCollider> FpCapsuleStack;
        public FPColliderStack<FPCylinderCollider> FpCylinderStack;
        public FPColliderStack<FPAACapsuleCollider> FpAACapsuleStack;
        public FPColliderStack<FPMeshCollider> FpMeshStack;
        public FPColliderStack<FPCharacterController> FpCharacterStack;
        public readonly FixedPointAABB fixedPointAABB;
        //amount of colliders in sub nodes.means there is collider under this node ,but it is not the count exactly.
        public int colliderCount;
      
        public FPOctreeNode(int halfSize, FixedPointVector3 pos,int size)
        {
            this.halfSize = halfSize;
            this.pos = pos;
            var minX = pos.x - halfSize;
            var maxX = pos.x + halfSize;
            var minY = pos.y - halfSize;
            var maxY = pos.y + halfSize;
            var minZ = pos.z - halfSize;
            var maxZ = pos.z + halfSize;
            var min = new FixedPointVector3(minX, minY, minZ);
            var max = new FixedPointVector3(maxX, maxY, maxZ);
            fixedPointAABB = new FixedPointAABB(min, max);
            this.size = size;
        }

        public bool VerifyInside(FixedPointVector3 pos)
        {
            return pos.x >= fixedPointAABB.Min.x && pos.x < fixedPointAABB.Max.x && pos.y >= fixedPointAABB.Min.y && pos.y < fixedPointAABB.Max.y && pos.z >= fixedPointAABB.Min.z && pos.z < fixedPointAABB.Max.z;
        }
    }
}