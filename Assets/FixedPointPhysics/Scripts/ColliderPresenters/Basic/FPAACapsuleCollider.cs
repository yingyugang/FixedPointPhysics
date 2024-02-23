using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// 代表用于物理引擎的轴对齐胶囊碰撞器，使用固定点算术进行精确计算。
    /// </summary>
    public class FPAACapsuleCollider : FPCollider
    {
        [SerializeField]
        protected FixedPoint64 _radius; // 胶囊的半径。
        [SerializeField]
        protected FixedPoint64 _height; // 胶囊的高度。
        
        /// <summary>
        /// 获取或设置胶囊的半径，使用固定点算术进行精确。
        /// </summary>
        public FixedPoint64 radius
        {
            get => _radius;
            set
            {
                _radius = value;
                UpdateCollider(); // 当半径变化时，更新轴对齐包围盒。
            }
        }

        /// <summary>
        /// 获取或设置胶囊的高度，使用固定点算术进行精确。
        /// </summary>
        public FixedPoint64 height {
            get => _height;
            set
            {
                _height = value;
                UpdateCollider(); // 当高度变化时，更新轴对齐包围盒。
            }
        }

        /// <summary>
        /// 计算胶囊的缩放后半高，考虑对象的缩放。
        /// </summary>
        internal FixedPoint64 scaledHalfHeight => height * 0.5 * fpTransform.scale.y;

        /// <summary>
        /// 计算胶囊的缩放半径，选择对象的x或z缩放的最大值。
        /// </summary>
        internal FixedPoint64 scaledRadius => _radius * FixedPointMath.Max(fpTransform.scale.x, fpTransform.scale.z);

        /// <summary>
        /// 计算胶囊中心轴的起始位置，调整缩放。
        /// </summary>
        internal virtual FixedPointVector3 startPos => position - new FixedPointVector3(0, FixedPointMath.Max(0, scaledHalfHeight - scaledRadius), 0);

        /// <summary>
        /// 计算胶囊中心轴的结束位置，调整缩放。
        /// </summary>
        internal virtual FixedPointVector3 endPos => position + new FixedPointVector3(0, FixedPointMath.Max(0, scaledHalfHeight - scaledRadius), 0);

        /// <summary>
        /// 指定碰撞器类型为轴对齐胶囊碰撞器。
        /// </summary>
        public override ColliderType colliderType => ColliderType.AACapsule;

        /// <summary>
        /// 根据胶囊当前的尺寸和位置更新轴对齐包围盒。
        /// </summary>
        internal override void UpdateAABB()
        {
            var width = scaledRadius;
            var halfSize = new FixedPointVector3(width, scaledHalfHeight, width);
            _min = position - halfSize;
            _max = position + halfSize;
        }

        /// <summary>
        /// 根据附加GameObject的网格边界初始化碰撞器尺寸（如果可用）。
        /// </summary>
        protected override void InitColliderSize()
        {
            // 尝试获取MeshFilter组件，并使用其边界来设置初始碰撞器尺寸。
            var mesh = GetComponent<MeshFilter>();
            if (mesh == null) return; // 如果没有找到MeshFilter，则提前返回。
            var bounds = mesh.sharedMesh.bounds;
            // 根据网格边界计算半径和高度。
            var boundRadius = Mathf.Max(bounds.extents.x, bounds.extents.z);
            var boundHeight = Mathf.Max(2 * boundRadius, bounds.size.y);
            _radius = boundRadius;
            _height = boundHeight;
        }
        
        /// <summary>
        /// 从所有它所属的影响节点中移除此碰撞器，确保它不再参与碰撞检查。
        /// </summary>
        protected override void RemoveFromImpactNotes()
        {
            targetNode?.FpAACapsuleStack.Remove(this);
            targetNode = null;
        }
        
        /// <summary>
        /// 将此碰撞器添加到指定的八叉树节点的胶囊碰撞器列表中，启用碰撞检查。
        /// </summary>
        protected override void AddToImpactNote(FPOctreeNode node)
        {
            node.FpAACapsuleStack ??= new FPColliderStack<FPAACapsuleCollider>(node);
            node.FpAACapsuleStack.Add(this);
            targetNode = node;
        }

        /// <summary>
        /// 在Unity编辑器的场景视图中可视化胶囊碰撞器，帮助调试和设置。
        /// </summary>
        protected override void OnDrawGizmosEditor()
        {
            FPCapsuleCollider.DrawWireCapsule(startPos.ToVector3(), endPos.ToVector3(), scaledRadius.AsFloat());
        }
        
        /// <summary>
        /// 在场景中绘制调试信息，如胶囊的起始和结束位置。
        /// </summary>
        protected override void OnDrawDebugInfo()
        {
            Gizmos.DrawWireCube(startPos.ToVector3(), Vector3.one * 0.1f);
            Gizmos.DrawWireCube(endPos.ToVector3(), Vector3.one * 0.1f);
            Gizmos.DrawWireCube(min.ToVector3(), Vector3.one * 0.1f);
            Gizmos.DrawWireCube(max.ToVector3(), Vector3.one * 0.1f);
        }
    }
}