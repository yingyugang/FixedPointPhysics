using BlueNoah.Math.FixedPoint;
using System.Collections.Generic;
namespace BlueNoah.PhysicsEngine
{
    [System.Serializable]
    public class FixedPointOctreeNode
    {
        public FixedPointOctreeNode parentNode;
        public FixedPointOctreeNode[] nodes;
        public int exp;
        public int halfSize;
        public FixedPointVector3 pos;

        public FixedPoint64 minX;
        public FixedPoint64 minY;
        public FixedPoint64 minZ;
        public FixedPoint64 maxX;
        public FixedPoint64 maxY;
        public FixedPoint64 maxZ;
        public FixedPointVector3 min;
        public FixedPointVector3 max;
        //public HashSet<FixedPointSphereCollider> intersectedSphereColliders = new HashSet<FixedPointSphereCollider>();
        public HashSet<FixedPointAABBCollider> intersectedAABBColliders = new HashSet<FixedPointAABBCollider>();
        //public FastList<FixedPointAABBCollider> intersectedAABBColliders = new FastList<FixedPointAABBCollider>();
        public HashSet<FixedPointSphereCollider> intersectedSphereColliders = new HashSet<FixedPointSphereCollider>();
        public HashSet<FixedPointOBBCollider> intersectedOBBColliders = new HashSet<FixedPointOBBCollider>();

        public FixedPointAABB fixedPointAABB;

        public FixedPointOctreeNode(int exp, int halfSize, FixedPointVector3 pos)
        {
            this.exp = exp;
            this.halfSize = halfSize;
            this.pos = pos;
            minX = pos.x - halfSize;
            maxX = pos.x + halfSize;
            minY = pos.y - halfSize;
            maxY = pos.y + halfSize;
            minZ = pos.z - halfSize;
            maxZ = pos.z + halfSize;
            min = new FixedPointVector3(minX, minY, minZ);
            max = new FixedPointVector3(maxX, maxY, maxZ);
            fixedPointAABB = new FixedPointAABB(min, max);
        }

        public bool VerifyInside(FixedPointVector3 pos)
        {
            return pos.x >= minX && pos.x < maxX && pos.y >= minY && pos.y < maxY && pos.z >= minZ && pos.z < maxZ;
        }
    }
}