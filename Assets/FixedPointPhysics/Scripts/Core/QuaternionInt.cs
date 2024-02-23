using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine
{
    [System.Serializable]
    public struct QuaternionInt
    {
        public int x;
        public int y;
        public int z;
        public int w;
        public QuaternionInt(int x,int y,int z,int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public FixedPointQuaternion ToFixedPointQuaternion()
        {
            return new FixedPointQuaternion(x, y, z, w);
        }
    }
}