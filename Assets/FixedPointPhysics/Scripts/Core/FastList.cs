using System;
using System.Collections;
using UnityEngine;

namespace BlueNoah
{
    /// <summary>
    /// Represents an item that can be stored in a FastList.
    /// </summary>
    internal interface FastListItem
    {
        public int index { get; set; }// Index of the item within the FastList.
    }
    
    /// <summary>
    /// A custom implementation of a list optimized for fast access and modifications,
    /// specifically designed for items of type FastListItem.
    /// Important: Although this method is fast and does not use a Dictionary to achieve rapid deletion,
    /// because the index is stored inside FastListItem, the index value cannot be modified externally, and a FastListItem can only correspond to one FastList.
    /// Important: It can‘t be used in multi thread logic.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list, constrained to FastListItem or its subclasses.</typeparam>
    internal class FastList<T> : IEnumerator, IEnumerable where T : FastListItem 
    {
        private T[] items; // Internal array to store the list items.
        private int mIndex = -1; // The current index for adding new items, starts at -1 indicating an empty list.
        private int mCount = 1000; // Initial size of the items array, default is 1000.

        public FastList()
        {
            items = new T[mCount]; // Initialize the items array with the default size.
        }

        /// <summary>
        /// Adds a new item to the list. If the list is full, it doubles its size before adding the new item.
        /// </summary>
        /// <param name="t">The item to be added to the list.</param>
        public void Add(T t)
        {
            if (mIndex == mCount)
            {
                mCount *= 2; // Double the size of the items array if it's full.
                var newItems = new T[mCount];
                items.CopyTo(newItems, 0); // Copy existing items to the new array.
                items = newItems;
            }
            mIndex++;
            items[mIndex] = t;
            t.index = mIndex; // Set the index of the item to its position in the list.
        }

        /// <summary>
        /// Removes an item from the list. If the item is not the last one, it swaps the last item into its place.
        /// </summary>
        /// <param name="t">The item to be removed from the list.</param>
        public void Remove(T t)
        {
            if (mIndex == -1 || items == null || items.Length <= t.index || items.Length <= mIndex)
            {
                return; // Do nothing if the list is empty or the item is out of bounds.
            }
            if (t.index != mIndex) // If the item is not the last one,
            {
                items[mIndex].index = t.index; // Swap the last item to fill the gap.
                items[t.index] = items[mIndex];
            }
            mIndex--;
        }

        /// <summary>
        /// Clears the list, resetting it to its initial state.
        /// </summary>
        public void Clear()
        {
            mIndex = -1;
            mCount = 1000;
            items = new T[mCount]; // Reinitialize the items array.
        }

        /// <summary>
        /// Indexer to access items by their position in the list.
        /// </summary>
        /// <param name="index">The index of the item to retrieve.</param>
        /// <returns>The item at the specified index, or null if the index is out of bounds.</returns>
        public T this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return items[index];
            }
        } 

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        public int Count => mIndex + 1;

        private int position = -1; // Position for the enumerator, starts at -1.

        // Methods required by the IEnumerator and IEnumerable interfaces.
        public IEnumerator GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            position++;
            return (position < Count);
        }

        public void Reset()
        {
            position = -1;
        }

        public object Current => items[position];
    }
}