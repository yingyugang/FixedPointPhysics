using System;
using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// 代表一个具有固定点精度的圆柱形碰撞器的展示器。
    /// 该类负责处理物理引擎中圆柱形碰撞器的具体细节，
    /// 包括其创建、修改和在Unity编辑器中的视觉表示。
    /// </summary>
    public class FPCylinderCollider : FPCollider
    {
        // 用于存储胶囊的半径的字段。
        [SerializeField]
        protected FixedPoint64 _radius;
        // 用于存储胶囊的高度的字段。
        [SerializeField]
        protected FixedPoint64 _height;
        
        /// <summary>
        /// 获取或设置胶囊的半径。设置半径会标记AABB为脏并更新边界。
        /// </summary>
        public FixedPoint64 radius
        {
            get => _radius;
            set
            {
                _radius = value;
                UpdateCollider();
            }
        }
        /// <summary>
        /// 获取或设置胶囊的高度。设置高度会标记AABB为脏并更新半高。
        /// </summary>
        public FixedPoint64 height {
            get => _height;
            set
            {
                _height = value;
                UpdateCollider();
            }
        }
        /// <summary>
        /// 表示对象的缩放后半高，考虑到其高度和y轴上的缩放。
        /// </summary>
        internal FixedPoint64 scaledHalfHeight => height * 0.5 * fpTransform.scale.y;

        /// <summary>
        /// 表示对象的缩放半径，考虑到其原始半径和x与z轴之间的最大缩放因子。这确保对象在两个方向上均匀缩放。
        /// </summary>
        internal FixedPoint64 scaledRadius => _radius * FixedPointMath.Max(fpTransform.scale.x, fpTransform.scale.z);
        /// <summary>
        /// 通过从其位置减去半高来获取胶囊的位置。
        /// 这不是底部位置。
        /// </summary>
        internal FixedPointVector3 startPos => position - fpTransform.rotation * new FixedPointVector3(0,scaledHalfHeight ,0);

        /// <summary>
        /// 通过向其位置加上半高来获取胶囊的位置。
        /// 这不是顶部位置。
        /// </summary>
        internal FixedPointVector3 endPos => position + fpTransform.rotation * new FixedPointVector3(0,scaledHalfHeight,0);

        public override ColliderType colliderType => ColliderType.Cylinder;
        
        /// <summary>
        /// 定义包围盒的八个角点。
        /// </summary>
        private FixedPointVector3[] points { get; } = new FixedPointVector3[8];
        
        internal override void UpdateAABB()
        {
            var halfSize = new FixedPointVector3(scaledRadius,scaledHalfHeight,scaledRadius);
            var orientation = fpTransform.rotation;
            // 计算包围圆柱的概念包围盒的八个角落的位置。
            var pos = position;
            points[0] = pos + orientation * new FixedPointVector3(halfSize.x, halfSize.y, halfSize.z);
            points[1] = pos + orientation * new FixedPointVector3(halfSize.x, halfSize.y, -halfSize.z);
            points[2] = pos + orientation * new FixedPointVector3(halfSize.x, -halfSize.y, -halfSize.z);
            points[3] = pos + orientation * new FixedPointVector3(halfSize.x, -halfSize.y, halfSize.z);
            points[4] = pos + orientation * new FixedPointVector3(-halfSize.x, halfSize.y, halfSize.z);
            points[5] = pos + orientation * new FixedPointVector3(-halfSize.x, halfSize.y, -halfSize.z);
            points[6] = pos + orientation * new FixedPointVector3(-halfSize.x, -halfSize.y, -halfSize.z);
            points[7] = pos + orientation * new FixedPointVector3(-halfSize.x, -halfSize.y, halfSize.z);
            // 初始化_min和_max为第一个点。
            _min = points[0];
            _max = points[0];

            // 查找形成AABB的最小和最大点。
            for (var i = 1; i < 8; i++)
            {
                _min = FixedPointVector3.Min(_min, points[i]);
                _max = FixedPointVector3.Max(_max, points[i]);
            }
        }

        /// <summary>
        /// 根据MeshFilter的边界初始化碰撞器大小，相应地调整胶囊的高度和半径。
        /// </summary>
        protected override void InitColliderSize()
        {
            // 尝试获取MeshFilter组件，并使用其边界设置初始碰撞器尺寸。
            var mesh = GetComponent<MeshFilter>();
            if (mesh == null) return; // 如果没有找到MeshFilter，则提前返回。
            var bounds = mesh.sharedMesh.bounds;
            // 根据网格边界计算半径和高度。
            var boundRadius = Mathf.Max(bounds.extents.x, bounds.extents.z);
            var boundHeight = Mathf.Max(0, bounds.size.y - 2 * boundRadius);
            _radius = boundRadius;
            _height = boundHeight;
        }
                        
        /// <summary>
        /// 从其所属的所有影响节点中移除此碰撞器，确保它不再参与碰撞检查。
        /// </summary>
        protected override void RemoveFromImpactNotes()
        {
            targetNode?.FpCylinderStack.Remove(this);
            targetNode = null;
        }
        
        /// <summary>
        /// 将此碰撞器添加到指定的八叉树节点的圆柱碰撞器列表中，启用碰撞检查。
        /// </summary>
        protected override void AddToImpactNote(FPOctreeNode node)
        {
            node.FpCylinderStack ??= new FPColliderStack<FPCylinderCollider>(node);
            node.FpCylinderStack.Add(this);
            targetNode = node;
        }

        /// <summary>
        /// 在Unity编辑器中绘制圆柱碰撞器的线框表示。
        /// 此方法覆盖基类中的抽象方法OnDrawGizmosEditor。
        /// </summary>
        protected override void OnDrawGizmosEditor()
        {
            // 使用线框可视化圆柱碰撞器。
            DrawWireCylinder(startPos.ToVector3(), endPos.ToVector3(), scaledRadius.AsFloat());
        }

        /// <summary>
        /// 在两点之间绘制一个指定半径的线框圆柱。
        /// 此方法是静态的，因为它不依赖于实例特定的数据。
        /// </summary>
        /// <param name="p1">圆柱的起始点。</param>
        /// <param name="p2">圆柱的结束点。</param>
        /// <param name="radius">圆柱的半径。</param>
        private static void DrawWireCylinder(Vector3 p1, Vector3 p2, float radius)
        {
#if UNITY_EDITOR
            // 处理起点和终点相同的特殊情况。
            if (p1 == p2)
            {
                Gizmos.DrawWireSphere(p1, radius);
                return;
            }

            using (new UnityEditor.Handles.DrawingScope(Gizmos.color, Gizmos.matrix))
            {
                // 确定绘制圆柱顶部和底部的旋转。
                var p1Rotation = Quaternion.LookRotation(p1 - p2);
                var p2Rotation = Quaternion.LookRotation(p2 - p1);
                
                // 检查圆柱的方向是否与Vector3.up共线。
                var c = Vector3.Dot((p1 - p2).normalized, Vector3.up);
                // 如有必要，调整旋转以防止图形错误。
                if (System.Math.Abs(c - 1f) < 0.000001 || System.Math.Abs(c - (-1f)) < 0.000001)
                {
                    p2Rotation = Quaternion.Euler(p2Rotation.eulerAngles.x, p2Rotation.eulerAngles.y + 180f, p2Rotation.eulerAngles.z);
                }
                
                // 绘制圆柱的顶部和底部盖子。
                UnityEditor.Handles.DrawWireDisc(p1, (p2 - p1).normalized, radius);
                UnityEditor.Handles.DrawWireDisc(p2, (p1 - p2).normalized, radius);

                // 绘制线条连接顶部和底部盖子，形成圆柱的侧面。
                UnityEditor.Handles.DrawLine(p1 + p1Rotation * Vector3.down * radius, p2 + p2Rotation * Vector3.down * radius);
                UnityEditor.Handles.DrawLine(p1 + p1Rotation * Vector3.left * radius, p2 + p2Rotation * Vector3.right * radius);
                UnityEditor.Handles.DrawLine(p1 + p1Rotation * Vector3.up * radius, p2 + p2Rotation * Vector3.up * radius);
                UnityEditor.Handles.DrawLine(p1 + p1Rotation * Vector3.right * radius, p2 + p2Rotation * Vector3.left * radius);
            }
#endif
        }
    }
}