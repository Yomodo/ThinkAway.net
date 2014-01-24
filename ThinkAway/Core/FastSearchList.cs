using System;
using System.Collections;
using System.Collections.Generic;

namespace ThinkAway.Core
{
    public class FastSearchList<T> : IList<T>
    {
        private readonly IList<T> _internalList;
        private readonly IDictionary<T, int> _internalLookup;

        public FastSearchList()
        {
            this._internalList = new List<T>();
            this._internalLookup = new System.Collections.Generic.Dictionary<T, int>();
        }

        public FastSearchList(int capacity)
        {
            this._internalList = new List<T>(capacity);
            this._internalLookup = new System.Collections.Generic.Dictionary<T, int>(capacity);
        }

        public int IndexOf(T item)
        {
            if (this._internalLookup.ContainsKey(item))
                return this._internalLookup[item];
            return -1;
        }

        public void Insert(int index, T item)
        {
            if (_internalLookup.ContainsKey(item))
                throw new ArgumentException("Duplicate item already exist in the list");

            this._internalList.Insert(index, item);
            this._internalLookup.Add(item, index);

            // require re-indexing
            for (int i = index; i < this._internalList.Count; i++)
            {
                T itemKey = this._internalList[i];
                this._internalLookup[itemKey] = i;
            }
        }

        public void RemoveAt(int index)
        {
            T item = this._internalList[index];
            _internalList.RemoveAt(index);
            this._internalLookup.Remove(item);

            // require re-indexing
            for (int i = index; i < this._internalList.Count; i++)
            {
                T itemKey = _internalList[i];
                _internalLookup[itemKey] = i;
            }
        }

        public T this[int index]
        {
            get { return _internalList[index]; }
            set { _internalList[index] = value; }
        }

        public void Add(T item)
        {
            this._internalList.Add(item);
            if (!_internalLookup.ContainsKey(item))
            {
                this._internalLookup.Add(item, _internalList.Count - 1);
            }
        }

        public void Clear()
        {
            this._internalList.Clear();
            this._internalLookup.Clear();
        }

        public bool Contains(T item)
        {
            return this._internalLookup.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._internalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (this._internalLookup.ContainsKey(item))
            {
                int index = this._internalLookup[item];
                this.RemoveAt(index); // re-indexing implictly
                return true;
            }
            return false;
        }

        public int Count
        {
            get { return _internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return _internalList.IsReadOnly; }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }
    }
}
