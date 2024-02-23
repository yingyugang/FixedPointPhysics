using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// 代表一个轴对齐的边界框（AABB）碰撞器，使用固定点精度，在自定义物理引擎内。
    /// 这个类处理创建、初始化和管理一个固定点AABB碰撞器，以实现精确的碰撞检测和物理模拟。
    /// </summary>
    public class FPAABBCollider : FPCollider
    {
        [SerializeField]
        protected FixedPointVector3 _size;

        /// <summary>
        /// 使用固定点精度获取或设置AABB碰撞器的大小。调整大小会自动更新碰撞器的轴对齐边界框。
        /// </summary>
        public FixedPointVector3 size
        {
            get => FixedPointVector3.Scale(_size, fpTransform.scale);
            set
            {
                _size = value;
                halfSize = _size * 0.5;
                UpdateCollider();
            }
        }

        /// <summary>
        /// 获取或设置AABB碰撞器的半尺寸，代表碰撞器在每个方向上的尺寸的一半。这个属性便于轻松操纵和计算碰撞器的边界框。
        /// </summary>
        public FixedPointVector3 halfSize
        {
            get => size * 0.5;
            set => _size = value * 2;
        }

        /// <summary>
        /// 返回碰撞器的类型，特别指出这个碰撞器是一个轴对齐边界框（AABB）。
        /// </summary>
        public override ColliderType colliderType => ColliderType.AABB;

        /// <summary>
        /// 基于附加GameObject的网格边界初始化碰撞器大小，将这些边界转换为固定点精度以用于物理引擎。
        /// </summary>
        protected override void InitColliderSize()
        {
            var mesh = GetComponent<MeshFilter>();
            if (mesh == null)
                return;
            var bounds = mesh.sharedMesh.bounds;
            size = new FixedPointVector3(bounds.size);
        }
        
        /// <summary>
        /// 根据其当前位置和大小计算并更新碰撞器的轴对齐边界框（AABB），确保物理引擎内的精确碰撞检测。
        /// </summary>
        internal override void UpdateAABB()
        {
            var pos = position;
            _min = pos - halfSize;
            _max = pos + halfSize;
        }
                
        /// <summary>
        /// 从所有它是其一部分的影响节点中移除这个碰撞器，确保它不再参与碰撞检查。
        /// </summary>
        protected override void RemoveFromImpactNotes()
        {
            targetNode?.FpAABBStack.Remove(this);
            targetNode = null;
        }
        
        /// <summary>
        /// 将这个碰撞器添加到指定八叉树节点的AABB碰撞器列表中，启用碰撞检查。
        /// </summary>
        protected override void AddToImpactNote(FPOctreeNode node)
        {
            node.FpAABBStack ??= new FPColliderStack<FPAABBCollider>(node);
            node.FpAABBStack.Add(this);
            targetNode = node;
        }
        
        /// <summary>
        /// 在Unity编辑器中使用Gizmos可视化AABB碰撞器，通过绘制一个围绕碰撞器边界的线框立方体，帮助调试和设置碰撞器。
        /// </summary>
        protected override void OnDrawGizmosEditor()
        {
            Gizmos.DrawWireCube(position.ToVector3(), size.ToVector3());
        }

        /// <summary>
        /// 在场景视图中绘制额外的调试信息，包括碰撞器最小点和最大点的视觉表示，用于详细检查和调试目的。
        /// </summary>
        protected override void OnDrawDebugInfo()
        {
            // 保存当前Gizmos颜色
            var color = Gizmos.color;
    
            // 设置Gizmos颜色为青色，用于绘制最小点立方体
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(min.ToVector3(), DebugCubeSize);
    
            // 设置Gizmos颜色为品红色，用于绘制最大点立方体
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(max.ToVector3(), DebugCubeSize);
    
            // 恢复原始Gizmos颜色
            Gizmos.color = color;
        }
    }
}