using BlueNoah.Math.FixedPoint;
using UnityEngine;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// 表示具有固定点精度的网格碰撞器。此类允许在自定义固定点物理引擎中表示和操作网格碰撞器，
    /// 提供从Unity Meshes加载碰撞器几何体和以高精度修改属性的功能。
    /// </summary>
    public class FPMeshCollider : FPCollider
    {
        // 定义网格几何体的顶点数组。
        [SerializeField]
        internal FixedPointVector3[] vertices;

        // 定义网格三角形的索引。
        [SerializeField]
        internal int[] triangles;

        // 网格中每个顶点的法线。
        [SerializeField]
        internal FixedPointVector3[] normals;

        // 由三角形定义的每个平面到原点的距离。
        [SerializeField]
        internal FixedPoint64[] distances;

        // 网格中每个三角形的最小边界，用于优化。
        [SerializeField]
        internal FixedPointVector3[] minimals;

        // 网格中每个三角形的最大边界，用于优化。
        [SerializeField]
        internal FixedPointVector3[] maximals;
        
        public override ColliderType colliderType => ColliderType.Mesh;
        
        /// <summary>
        /// 根据网格顶点更新轴对齐包围盒(AABB)。
        /// </summary>
        internal override void UpdateAABB()
        {
            _min = FixedPointVector3.one * FixedPoint64.MaxValue;
            _max = FixedPointVector3.one * FixedPoint64.MinValue;
            // 通过寻找最小和最大顶点位置来计算AABB。
            foreach (var vertex in vertices)
            {
                var point = FixedPointVector3.Scale((fpTransform.rotation * vertex) ,fpTransform.scale);
                _min = FixedPointVector3.Min(min, point);
                _max = FixedPointVector3.Max(max, point);
            }
            _min += position;
            _max += position;
        }
        
        /// <summary>
        /// 基于MeshFilter的边界初始化碰撞器大小，相应调整胶囊的高度和半径。
        /// </summary>
        protected override void InitColliderSize()
        {
            var meshFilter = GetComponentInChildren<MeshFilter>(true); // 尝试找到MeshFilter组件。
            if (meshFilter == null) return; // 如果找不到MeshFilter则退出。
            var mesh = meshFilter.sharedMesh; // 获取共享网格。
            if (mesh == null) return; // 如果没有可用的网格则退出。

            // 将顶点转换为固定点格式。
            vertices = new FixedPointVector3[mesh.vertices.Length];
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new FixedPointVector3(mesh.vertices[i]);
            }

            // 直接复制三角形索引，因为它们不需要转换。
            triangles = new int[mesh.triangles.Length];
            for (var i = 0; i < triangles.Length; i++)
            {
                triangles[i] = mesh.triangles[i];
            }

            // 转换顶点并计算三角形属性。
            var triangleCount = triangles.Length / 3;
            normals = new FixedPointVector3[triangleCount];
            distances = new FixedPoint64[vertices.Length];
            minimals = new FixedPointVector3[triangleCount];
            maximals = new FixedPointVector3[triangleCount];

            // 为每个三角形计算平面属性。
            for (var i = 0; i < triangles.Length; i += 3)
            {
                var plane = FromTriangle(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
                var triangleIndex = i / 3;
                normals[triangleIndex] = plane.normal;
                distances[triangleIndex] = plane.distance;

                // 计算三角形的包围盒，为最小厚度调整。
                var triangleMin = FixedPointVector3.Min(vertices[triangles[i]], FixedPointVector3.Min(vertices[triangles[i + 1]], vertices[triangles[i + 2]]));
                var triangleMax = FixedPointVector3.Max(vertices[triangles[i]], FixedPointVector3.Max(vertices[triangles[i + 1]], vertices[triangles[i + 2]]));
                AdjustForMinimumThickness(ref triangleMin, ref triangleMax);
                minimals[triangleIndex] = triangleMin;
                maximals[triangleIndex] = triangleMax;
            }
        }
                        
        /// <summary>
        /// 从其所属的所有冲击节点中移除此碰撞器，确保它不再参与碰撞检查。
        /// </summary>
        protected override void RemoveFromImpactNotes()
        {
            targetNode?.FpMeshStack.Remove(this);
            targetNode = null;
        }
        
        /// <summary>
        /// 将此碰撞器添加到指定八叉树节点的圆柱碰撞器列表中，启用碰撞检查。
        /// </summary>
        protected override void AddToImpactNote(FPOctreeNode node)
        {
            node.FpMeshStack ??= new FPColliderStack<FPMeshCollider>(node);
            node.FpMeshStack.Add(this);
            targetNode = node;
        }
        
        /// <summary>
        /// 从代表三角形的三个点计算平面。
        /// </summary>
        /// <param name="point">三角形的第一个点。</param>
        /// <param name="point1">三角形的第二个点。</param>
        /// <param name="point2">三角形的第三个点。</param>
        /// <returns>表示三角形平面的FixedPointPlane。</returns>
        private static FixedPointPlane FromTriangle(FixedPointVector3 point, FixedPointVector3 point1, FixedPointVector3 point2)
        {
            var normal = FixedPointVector3.Normalize(FixedPointVector3.Cross(point1 - point, point2 - point)); // 计算法线。
            var distance = FixedPointVector3.Dot(normal, point); // 计算到原点的距离。
            return new FixedPointPlane { normal = normal, distance = distance };
        }
        
        /// <summary>
        /// 在Unity编辑器中使用Gizmos绘制网格碰撞器。此可视化有助于调试和设置碰撞器。
        /// </summary>
        protected override void OnDrawGizmosEditor()
        {
            // 绘制网格碰撞器的每个三角形。
            for (var i = 0; i < triangles.Length; i += 3)
            {
                var point = FixedPointVector3.Scale((fpTransform.rotation * vertices[triangles[i]]) ,fpTransform.scale) + position;
                var point1 = FixedPointVector3.Scale(fpTransform.rotation * vertices[triangles[i + 1]],fpTransform.scale)  + position;
                var point2 = FixedPointVector3.Scale(fpTransform.rotation * vertices[triangles[i + 2]] ,fpTransform.scale) + position;
                Gizmos.DrawLine(point.ToVector3(), point1.ToVector3());
                Gizmos.DrawLine(point1.ToVector3(), point2.ToVector3());
                Gizmos.DrawLine(point2.ToVector3(), point.ToVector3());
            }
        }

        /// <summary>
        /// 调整三角形包围盒的最小和最大向量，以确保其具有最小厚度。
        /// </summary>
        /// <param name="min">包围盒的最小点。</param>
        /// <param name="max">包围盒的最大点。</param>
        private static void AdjustForMinimumThickness(ref FixedPointVector3 min, ref FixedPointVector3 max)
        {
            // 确保包围盒具有最小厚度，以防止数值不稳定。
            if (max.x - min.x < FixedPoint64.EN2) { min.x -= 0.01; max.x += 0.01; }
            if (max.y - min.y < FixedPoint64.EN2) { min.y -= 0.01; max.y += 0.01; }
            if (max.z - min.z < FixedPoint64.EN2) { min.z -= 0.01; max.z += 0.01; }
        }
    }
}