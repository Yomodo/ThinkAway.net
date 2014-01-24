using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    public class ContentValues
    {
        /// <summary>
        /// Dictionary
        /// </summary>
        private readonly Dictionary<string, string> _dictionary;

        /// <summary>
        /// ContentValues 是一种以键值形式存储的数据结构
        /// </summary>
        public ContentValues()
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
            get { return _dictionary.Keys.ToArray(); }
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

        public virtual void Add(string key, bool value)
        {
            _dictionary.Add(key, (value ? 1 : 0).ToString(CultureInfo.InvariantCulture));
        }

        public virtual void Add(string key, double value)
        {
            _dictionary.Add(key, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
