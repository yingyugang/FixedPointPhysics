using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueNoah.PhysicsEngine
{
    /// <summary>
    /// 代表一个为了高效添加、移除和枚举元素而设计的快速访问列表。
    /// 主要用于在物理引擎上下文中管理碰撞器的集合。
    /// </summary>
    /// <typeparam name="T">列表中元素的类型。</typeparam>
    public class FPFastList<T> : IEnumerator, IEnumerable
    {
        // 用于索引访问的元素存储在列表中。
        private List<T> items { get; } = new ();

        // 将元素映射到items列表中的索引，以便快速查找。
        private Dictionary<T, int> itemDic { get; } = new ();

        /// <summary>
        /// 获取FPFastList中包含的元素数量。
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 从FPFastList中移除一个特定元素。
        /// </summary>
        /// <param name="t">要移除的元素。</param>
        public virtual bool Remove(T t)
        {
            if (itemDic.TryGetValue(t, out var index))
            {
                // 将最后一个元素移动到被移除元素的位置，以压缩列表。
                items[index] = items[Count - 1];
                // 更新字典以反映移动元素的新索引。
                itemDic[items[index]] = index;
                // 从Dic里面移除。
                itemDic.Remove(t);
                // 减少元素计数。
                Count--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 如果元素尚不存在，则向FPFastList添加一个元素。
        /// </summary>
        /// <param name="t">要添加的元素。</param>
        public virtual bool Add(T t)
        {
            if (Contains(t))
            {
                return false;
            }
            // 增加元素计数。
            Count++;
            var lastIndex = Count - 1;
            if (lastIndex >= items.Count)
            {
                // 如有必要，扩展列表并添加新元素。
                items.Add(t);
            }
            else
            {
                // 将新元素放置在下一个可用索引处。
                items[lastIndex] = t;
            }
            // 将新元素映射到其索引以便快速查找。
            itemDic[t] = lastIndex;
            return true;
        }

        /// <summary>
        /// 判断一个元素是否在类中。
        /// </summary>
        public bool Contains(T t)
        {
            return itemDic.ContainsKey(t);
        }

        /// <summary>
        /// 从FPFastList中移除所有元素。
        /// </summary>
        public void Clear()
        {
            Count = 0;
            items.Clear();
            itemDic.Clear();
        }
        
        /// <summary>
        /// 获取指定索引处的元素。
        /// 如果索引超出范围，则抛出ArgumentOutOfRangeException。
        /// </summary>
        /// <param name="index">要获取元素的从零开始的索引。</param>
        /// <returns>指定索引处的元素。</returns>
        public T this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "索引超出范围。");
                }
                return items[index];
            }
        } 
        
        // 枚举器位置，初始化为-1，表示在第一个元素之前的起始位置。
        private int position = -1;
        
        /// <summary>
        /// 将枚举器前进到FPFastList的下一个元素。
        /// </summary>
        /// <returns>如果枚举器成功前进到下一个元素，则为true；如果枚举器已经超过了集合的末尾，则为false。</returns>
        public bool MoveNext()
        {
            position++;
            return (position < Count);
        }

        /// <summary>
        /// 将枚举器设置到其初始位置，即集合中第一个元素之前。
        /// </summary>
        public void Reset()
        {
            position = -1;
        }

        /// <summary>
        /// 获取集合中的当前元素。
        /// </summary>
        public object Current => items[position];
        
        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>一个可以用来循环访问集合的IEnumerator。</returns>
        public IEnumerator GetEnumerator()
        {
            return this;
        }
    }
}