using System;
using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    public enum ColliderType
    {
        Sphere,
        AABB,
        OBB,
        Capsule,
        Cylinder,
        AACapsule,
        Mesh,
        CharacterController
    }
    
    /// <summary>
    /// 为Unity编辑器中的固定点碰撞器提供抽象基类。
    /// 提供将固定点物理碰撞器与Unity的游戏对象集成的基础设施，使得可以进行精确的物理模拟。
    /// </summary>
    [RequireComponent(typeof(FPTransform))]
    [DefaultExecutionOrder(-899)]
    [ExecuteInEditMode]
    public abstract class FPCollider : MonoBehaviour
    {
        public FPTransform fpTransform; // 固定点变换的引用，用于物理计算。
        
        protected FixedPointVector3 _min; // 碰撞器的最小边界。
        protected FixedPointVector3 _max; // 碰撞器的最大边界。

        /// <summary>
        /// 获取碰撞器的最小边界向量。
        /// </summary>
        public FixedPointVector3 min => _min;
        
        /// <summary>
        /// 获取碰撞器的最大边界向量。
        /// </summary>
        public FixedPointVector3 max => _max;

        [SerializeField] 
        public FixedPointVector3 center; // 碰撞器的中心，用于定位。
        
        /// <summary>
        /// 使用其中心和当前位置计算碰撞器的位置。
        /// </summary>
        public FixedPointVector3 position => fpTransform.Position(center);

        [SerializeField] 
        public bool isDynamic; // 指示碰撞器是动态的还是静态的。
        
        [SerializeField] 
        public int layer; // 碰撞器所属的物理层。
        
        [SerializeField]
        public bool isTrigger; // 确定碰撞器是否充当触发器。

        public Action<FPCollision> onCharacterCollide; // 与角色碰撞器发生碰撞的事件。

        [SerializeField]
        public FixedPoint64 rebound; // 碰撞器的反弹性。

        internal int castIndex { get; set; } // 在碰撞检测算法中的优化索引。

        /// <summary>
        /// 指定碰撞器的类型（例如，Sphere, AABB, OBB等）。
        /// </summary>
        public abstract ColliderType colliderType { get; }

        protected FPOctreeNode targetNode;
        
        [SerializeField]
        private bool isInit; // 指示碰撞器是否已初始化。

        /// <summary>
        /// 初始化碰撞器，将其添加到物理系统中并设置必要的属性。
        /// </summary>
        protected virtual void Awake()
        {
            fpTransform = GetComponent<FPTransform>();
            if (!isInit)
            {
                InitColliderSize();
                isInit = true;
            }
            if (Application.isPlaying)
            {
                FPPhysicsPresenter.Instance.fpOctree.AddCollider(this);
                UpdateCollider();
            }
        }
        
        /// <summary>
        /// 更新碰撞器在空间划分结构中的位置和大小的方法。
        /// 这是一个占位符，供派生类实现具体的更新逻辑。
        /// 只在需要更新碰撞器时调用。
        /// </summary>
        internal void UpdateCollider()
        {
            if (!Application.isPlaying || FPPhysicsPresenter.Instance.fpOctree == null) return;
    
            UpdateAABB();
            var octree = FPPhysicsPresenter.Instance.fpOctree;
            var node = octree.root;
            if (targetNode != null)
            {
                // 如果碰撞器仍然包含在当前节点内，只需要以当前节点为根进行搜索。
                if (FixedPointIntersection.IsAABBInsideAABB(min, max, targetNode.fixedPointAABB.Min,
                        targetNode.fixedPointAABB.Max))
                {
                    node = targetNode;
                }
                RemoveFromImpactNotes();
            }

            while (true)
            {
                if (node.nodes == null)
                {
                    AddToImpactNote(node);
                    break;
                }
                var inside = false;
                foreach (var item in node.nodes)
                {
                    if (FixedPointIntersection.IsAABBInsideAABB(min, max,item.fixedPointAABB.Min, item.fixedPointAABB.Max))
                    {
                        node = item;
                        inside = true;
                        break;
                    }
                }
                // 如果没有被任何子节点完全包含，但被父节点包围，则与这个父节点关联。
                if (inside) continue;
                AddToImpactNote(node);
                break;
            }
        }

        /// <summary>
        /// 更新碰撞器的轴对齐包围盒（AABB）的虚拟方法。
        /// </summary>
        internal abstract void UpdateAABB();

        protected abstract void RemoveFromImpactNotes();

        protected abstract void AddToImpactNote(FPOctreeNode node);

        [SerializeField]
        private bool drawAABB = true; // 在编辑器中绘制轴对齐包围盒的开关。

        [SerializeField]
        private Color gizmosColor = Color.blue; // 在Unity编辑器中绘制的gizmos的颜色。

        [SerializeField]
        private bool drawDebugInfo; // 绘制额外调试信息的开关。

        protected static readonly Vector3 DebugCubeSize = Vector3.one * 0.1f; // 调试立方体的标准大小。
        
        /// <summary>
        /// 初始化碰撞器大小的占位方法。由子类实现。
        /// </summary>
        protected abstract void InitColliderSize();

        /// <summary>
        /// 在Unity编辑器中绘制gizmos以进行视觉调试。此方法处理绘制AABB和任何子类特定的gizmos。
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!enabled) return;

            var color = Gizmos.color;
            Gizmos.color = gizmosColor;
            OnDrawGizmosEditor();
            if (drawAABB)
            {
                DrawAABBEditor();
            }
            if (drawDebugInfo)
            {
                OnDrawDebugInfo();
            }
            Gizmos.color = color;
        }

        /// <summary>
        /// 为调试目的在编辑器中绘制碰撞器的轴对齐包围盒（AABB）。
        /// </summary>
        private void DrawAABBEditor()
        {
            UpdateAABB();
            var color = Gizmos.color;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(((max + min) * 0.5).ToVector3(), (max - min).ToVector3());
            Gizmos.color = color;
        }

        /// <summary>
        /// 绘制特定于碰撞器的gizmos的抽象方法。由子类实现。
        /// </summary>
        protected abstract void OnDrawGizmosEditor();

        /// <summary>
        /// 绘制额外调试信息的可选方法。可以被子类重写。
        /// </summary>
        protected virtual void OnDrawDebugInfo() { }
    }
}