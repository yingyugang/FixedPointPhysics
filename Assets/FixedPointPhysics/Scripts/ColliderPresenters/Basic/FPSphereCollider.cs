using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// 用于处理固定点物理系统中球形碰撞器的展示器。
    /// 此类提供在Unity编辑器中创建、修改和可视化表示球形碰撞器的功能，
    /// 确保与固定点算术的高精度和兼容性，用于物理仿真。
    /// </summary>
    public class FPSphereCollider : FPCollider
    {
        // 用于存储胶囊体半径的字段。
        [SerializeField]
        private FixedPoint64 _radius;
        
        /// <summary>
        /// 获取或设置胶囊体的半径。设置半径会标记AABB为脏并更新边缘。
        /// </summary>
        public FixedPoint64 radius
        {
            get => _radius;
            set
            {
                _radius = value;
                invRadius = 1 / _radius;
                UpdateCollider();
            }
        }
        
        public FixedPoint64 invRadius { get; set; }
        
        /// <summary>
        /// 表示对象的缩放半径，考虑到其原始半径和x及z轴之间的最大缩放因子。
        /// 这确保对象在两个方向上均匀缩放。
        /// </summary>
        internal FixedPoint64 scaledRadius => _radius * FixedPointMath.Max(FixedPointMath.Max(fpTransform.scale.x, fpTransform.scale.z),fpTransform.scale.y);

        public override ColliderType colliderType => ColliderType.Sphere;
        
        internal override void UpdateAABB()
        {
            // 通过调整胶囊体的起始和结束位置加上胶囊体的边缘（半径）来计算AABB的最小和最大点。
            var width = scaledRadius;
            var halfSize = new FixedPointVector3(width, width, width);
            _min = position - halfSize;
            _max = position + halfSize;
        }

        protected override void InitColliderSize()
        {
            var mesh = GetComponent<MeshFilter>();
            if (mesh == null)
                return;
            var bounds = mesh.sharedMesh.bounds;
            _radius = FixedPointMath.Max(bounds.size.z * 0.5f,FixedPointMath.Max(bounds.size.x * 0.5f, bounds.size.y * 0.5f));
        }
                        
        /// <summary>
        /// 将此碰撞器从所有它所属的影响节点中移除，确保它不再参与碰撞检查。
        /// </summary>
        protected override void RemoveFromImpactNotes()
        {
            targetNode?.FpSphereStack.Remove(this);
            targetNode = null;
        }
        
        /// <summary>
        /// 将此碰撞器添加到指定的八叉树节点的球形碰撞器列表中，启用碰撞检查。
        /// </summary>
        protected override void AddToImpactNote(FPOctreeNode node)
        {
            node.FpSphereStack ??= new FPColliderStack<FPSphereCollider>(node);
            node.FpSphereStack.Add(this);
            targetNode = node;
        }

        /// <summary>
        /// 在Unity编辑器中绘制球形碰撞器的线框表示，用于可视化。
        /// 此方法用于调试和设置目的，允许在场景视图中直观检查碰撞器的大小和位置。
        /// </summary>
        protected override void OnDrawGizmosEditor()
        {
            // 在碰撞器中心绘制一个线框球体，以及它的半径。
            // 中心和半径从固定点转换为浮点数，以兼容Unity的Gizmos。
            Gizmos.DrawWireSphere(position.ToVector3(), scaledRadius.AsFloat());
        }
    }
}