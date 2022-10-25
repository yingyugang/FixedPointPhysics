using System.Collections;

namespace BlueNoah
{
    public class FastListItem
    {
        public int index;
    }
    //Use for fast remove and contain calculation.
    public class FastList<T> : IEnumerator, IEnumerable where T : FastListItem 
    {
        T[] items;
        int mIndex = -1;
        int mCount = 1000;
        public FastList()
        {
            items = new T[mCount];
        }
        public void Add(T t)
        {
            if (mIndex == mCount)
            {
                mCount *= 2;
                var newItems = new T[mCount];
                items.CopyTo(newItems, 0);
                items = newItems;
            }
            mIndex++;
            items[mIndex] = t;
            t.index = mIndex;
        }

        public void Remove(T t)
        {
            if (t.index != mIndex)
            {
                items[mIndex].index = t.index;
                items[t.index] = items[mIndex];
            }
            items[mIndex] = null;
            mIndex--;
        }

        public bool Contain(T t)
        {
            return items[t.index] == t;
        }

        public void Clear()
        {
            mIndex = -1;
            mCount = 1000;
            items = new T[mCount];
        }

        public T this[int index]
        {
            get
            {
                if (index <= mIndex)
                {
                    return items[index];
                }
                return null;
            }
        }
        public int Count
        {
            get
            {
                return mIndex + 1;
            }
        }

        int position = -1;
        //IEnumerator and IEnumerable require these methods.
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
        //IEnumerator
        public bool MoveNext()
        {
            position++;
            return (position < items.Length);
        }
        //IEnumerable
        public void Reset()
        {
            position = -1;
        }
        //IEnumerable
        public object Current
        {
            get { return items[position]; }
        }
    }
}
