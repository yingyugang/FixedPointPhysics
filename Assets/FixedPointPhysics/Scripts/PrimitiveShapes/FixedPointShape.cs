/*
* Create 2022/11/1
* 応彧剛　yingyugang@gmail.com
* It's used by fixedpoint physics system.
*/
using System;
using System.Runtime.CompilerServices;
namespace BlueNoah.PhysicsEngine
{
    public enum ShapeType { 
        Line,
        Ray,
        Plane,
        Sphere,
        AABB,
        OBB
    }
    public abstract class FixedPointShape 
    {
        public ShapeType shape { get; protected set; }

        public abstract void DrawGizmos(bool intersected);
    }
}