using BlueNoah.Math.FixedPoint;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// 代表在物理引擎中碰撞检测操作的结果，使用定点算术来提高模拟的精度和稳定性。
    /// </summary>
    public struct FPCollision
    {
        public bool hit; // 表示是否检测到碰撞。
        public FPCollider collider; // 碰撞中被击中的碰撞器。
        
        // 在碰撞器B（被击中的碰撞器）上相对于碰撞器A（碰撞检测的源头）的最近点。
        public FixedPointVector3 closestPoint;
        
        // 在碰撞器A（碰撞检测的源头）上相对于碰撞器B（被击中的碰撞器）的最近点。
        public FixedPointVector3 outsidePoint;
        
        // 碰撞器B的最近点和碰撞器A的最近点之间的中点。这个点通常被用作解决碰撞的接触点。
        public FixedPointVector3 contactPoint;
        
        // 表示碰撞冲击方向的规范化向量。
        public FixedPointVector3 normal;
        
        // 碰撞发生的时间，表示为定点数。这在需要精确控制碰撞时间的模拟中经常使用。
        public FixedPoint64 t;
        
        // 碰撞穿透深度，表示为定点数。这可以用来计算物体解决碰撞时应该被移动的量。
        public FixedPoint64 depth;

#if UNITY_EDITOR
        // 仅在Unity编辑器中可用的可选调试信息，表示额外的点，用于碰撞的视觉调试。
        public FixedPointVector3? debugInfo;
        public FixedPointVector3? debugInfo1;
#endif
    }
}