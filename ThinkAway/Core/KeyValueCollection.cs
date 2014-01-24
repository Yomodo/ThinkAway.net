using System.Collections;
using System.Collections.Generic;

namespace ThinkAway.Core
{
    /// <summary>
    /// Represents a collection that can be accessed either with the key or with the index. 
    /// </summary>
    public class KeyValueCollection<TK,TV> : IEnumerable
    {
        private readonly Dictionary<TK,TV> _dictionary = null;
        private readonly List<TV>         _list       = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public KeyValueCollection()
        {
            _dictionary = new Dictionary<TK,TV>();
            _list = new List<TV>();
        }


        #region method Add

        /// <summary>
        /// Adds the specified key and value to the collection.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void Add(TK key,TV value)
        {
            _dictionary.Add(key,value);
            _list.Add(value);
        }

        #endregion

        #region method Remove

        /// <summary>
        /// Removes the value with the specified key from the collection.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Returns if key found and removed, otherwise false.</returns>
        public bool Remove(TK key)
        {
            TV value = default(TV);
            if(_dictionary.TryGetValue(key,out value)){
                _dictionary.Remove(key);
                _list.Remove(value);

                return true;
            }
            else{
                return false;
            }
        }

        #endregion

        #region method Clear

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
            _list.Clear();
        }

        #endregion

        #region method ContainsKey

        /// <summary>
        /// Gets if the collection contains the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Returns true if the collection contains specified key.</returns>
        public bool ContainsKey(TK key)
        {
            return _dictionary.ContainsKey(key);
        }

        #endregion

        #region method TryGetValue

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found.</param>
        /// <returns>Returns true if the collection contains specified key and value stored to <b>value</b> argument.</returns>
        public bool TryGetValue(TK key,out TV value)
        {
            return _dictionary.TryGetValue(key,out value);
        }

        #endregion

        #region method TryGetValueAt

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="index">Zero based item index.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found.</param>
        /// <returns>Returns true if the collection contains specified key and value stored to <b>value</b> argument.</returns>
        public bool TryGetValueAt(int index,out TV value)
        {
            value = default(TV);

            if(_list.Count > 0 && index >= 0 && index < _list.Count){
                value = _list[index];

                return true;
            }

            return false;
        }

        #endregion

        #region method ToArray

        /// <summary>
        /// Copies all elements to new array, all elements will be in order they added. This method is thread-safe.
        /// </summary>
        /// <returns>Returns elements in a new array.</returns>
        public TV[] ToArray()
        {
            lock(_list){
                return _list.ToArray();
            }
        }

        #endregion


        #region interface IEnumerator

        /// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns>Returns IEnumerator interface.</returns>
		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		#endregion

        #region Properties implementation

        /// <summary>
        /// Gets number of items int he collection.
        /// </summary>
        public int Count
        {
            get{ return _list.Count; }
        }

        /// <summary>
        /// Gets item with the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Returns item with the specified key. If the specified key is not found, a get operation throws a KeyNotFoundException.</returns>
        public TV this[TK key]
        {
            get{ return _dictionary[key]; }
        }
   
        #endregion

    }
}
