namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// 代表在空间划分结构中特定节点内的碰撞器栈。
    /// 这个类管理一个碰撞器对象的集合，允许高效的添加、移除和清除操作。
    /// 与FastList.cs相比，它是安全的，可以在多线程逻辑中使用。
    /// </summary>
    /// <typeparam name="T">碰撞器的类型，限制为派生自FBCollider的类型。</typeparam>
    public class FPColliderStack<T> : FPFastList<T>
    {
        // 与此碰撞器栈关联的空间划分节点的引用。
        private FPOctreeNode node { get;}

        /// <summary>
        /// 使用指定的空间划分节点初始化FPColliderStack类的新实例。
        /// </summary>
        /// <param name="node">这个碰撞器栈关联的空间划分节点。</param>
        public FPColliderStack(FPOctreeNode node)
        {
            this.node = node;
        }

        /// <summary>
        /// 从栈中移除一个特定的碰撞器，根据需要调整计数并更新父节点计数器。
        /// </summary>
        /// <param name="t">要移除的碰撞器。</param>
        public override bool Remove(T t)
        {
            if (!base.Remove(t)) return false;
            node.colliderCount--;
            var parentNode = node.parentNode; // 开始更新父节点碰撞器计数。
            while (parentNode != null)
            {
                parentNode.colliderCount--; // 减少父节点的子碰撞器计数。
                parentNode = parentNode.parentNode; // 移动到下一个父节点。
            }
            return true;
        }

        /// <summary>
        /// 向栈中添加一个新的碰撞器，根据需要更新计数和父节点计数器。
        /// </summary>
        /// <param name="t">要添加的碰撞器。</param>
        public override bool Add(T t)
        {
            if (!base.Add(t)) return false;
            node.colliderCount++;
            var parentNode = node.parentNode; // 开始更新父节点碰撞器计数。
            while (parentNode != null)
            {
                parentNode.colliderCount++; // 增加父节点的子碰撞器计数。
                parentNode = parentNode.parentNode; // 移动到下一个父节点。
            }
            return true;
        }
    }
}