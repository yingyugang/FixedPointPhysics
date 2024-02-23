namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// 对象池
    /// 用于管理特定类型对象的创建和回收，以提高性能和减少垃圾回收。
    /// </summary>
    /// <typeparam name="T">对象池管理的对象类型，必须有一个无参数的构造函数。</typeparam>
    public class ObjectPool<T> where T : new()
    {
        // 可用对象列表，存储当前可用于重新分配的对象。
        private readonly FPFastList<T> available = new ();
        // 所有对象列表，存储对象池中创建过的所有对象。
        private readonly FPFastList<T> allObject = new ();
        
        /// <summary>
        /// 从对象池中获取一个对象。如果有可用对象，返回一个现有对象；如果没有，创建一个新对象。
        /// </summary>
        /// <returns>从对象池中获取或新创建的对象。</returns>
        public T Pull()
        {
            T spawnObj;
            if (available.Count > 0)
            {
                // 如果有可用的对象，取出并从可用列表中移除。
                spawnObj = available[0];
                available.Remove(spawnObj);
            }
            else
            {
                // 如果没有可用的对象，创建一个新对象并添加到所有对象列表中。
                spawnObj = new T();
                allObject.Add(spawnObj);
            }
            return spawnObj;
        }
        /// <summary>
        /// 把对象放回对象池中。如果该对象是对象池创建的，将其加入可用对象列表中。
        /// </summary>
        /// <param name="obj">要放回对象池的对象。</param>
        public void Push(T obj)
        {
            // 如果对象是对象池创建的，加入可用对象列表。
            if (allObject.Contains(obj))
            {
                available.Add(obj);
            }
        }
    }
}