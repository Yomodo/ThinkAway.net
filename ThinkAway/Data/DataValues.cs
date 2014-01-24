using System.Collections;
using System.Collections.Generic;
using System.Globalization;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Data
{
    /// <summary>
    /// 数据容器
    /// </summary>
    public class DataValues : IDictionary<string,string>
    {
        /// <summary>
        /// Dictionary
        /// </summary>
        private readonly Dictionary<string, string> _dictionary;

        /// <summary>
        /// ContentValues 是一种以键值形式存储的数据结构
        /// </summary>
        public DataValues()
        {
            _dictionary = new Dictionary<string, string>();
        }

        /// <summary>
        /// get
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal string Get(string key)
        {
            return _dictionary[key];
        }

        public bool TryGetValue(string key, out string value)
        {
            throw new System.NotImplementedException();
        }

        string IDictionary<string, string>.this[string key]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        ICollection<string> IDictionary<string, string>.Keys
        {
            get { return Keys; }
        }

        public ICollection<string> Values
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal string this[string key]
        {
            get { return _dictionary[key]; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal string[] Keys
        {
            //NOTE:NET2.0
            get
            {
                List<string> list = new List<string>();
                foreach (string key in _dictionary.Keys)
                    list.Add(key);
                return list.ToArray();
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new System.NotImplementedException();
        }

        int ICollection<KeyValuePair<string, string>>.Count
        {
            get { return Count; }
        }

        public bool IsReadOnly
        {
            get { throw new System.NotImplementedException(); }
        }

        /// <summary>
        /// 获取该集合中的对象数量
        /// </summary>
        internal int Count
        {
            get { return _dictionary.Count; }
        }

        /// <summary>
        /// put
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Add(string key, int value)
        {
            _dictionary.Add(key, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Add(string key, System.DateTime value)
        {
            _dictionary.Add(key, string.Format("'{0:yyyy-MM-dd HH:mm:dd}'", value));
        }

        public bool ContainsKey(string key)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Add(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                _dictionary.Add(key, value == null ? "NULL" : "''");
            }
            else
            {
                _dictionary.Add(key, string.Format("'{0}'", value));
            }
        }

        public bool Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Add(string key, bool value)
        {
            _dictionary.Add(key, (value ? 1 : 0).ToString(CultureInfo.InvariantCulture));
        }

        public virtual void Add(string key, double value)
        {
            _dictionary.Add(key, value.ToString(CultureInfo.InvariantCulture));
        }

        #region Implementation of IEnumerable

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
