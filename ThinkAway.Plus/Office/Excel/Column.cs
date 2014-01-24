using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel
{
    /// <summary>
    /// 表示 Excel 工作表中的列
    /// </summary>
    [Serializable]
    public class Column
    {
        [XmlAttribute("Index", Namespace = "http://schemas.lsong.org/office")]
        public int Index { get; set; }

        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public int AutoFitWidth { get; set; }
        /// <summary>
        /// 获取或设置列宽
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public double Width { get; set; }
    }
    /// <summary>
    /// 列集合
    /// </summary>
    [Serializable]
    public class ColumnCollection : IList<Column>
    {
        private readonly Dictionary<int, Column> _dictionary;

        public ColumnCollection()
        {
            _dictionary = new Dictionary<int, Column>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Column item)
        {
            foreach (KeyValuePair<int, Column> keyValuePair in _dictionary)
            {
                if (keyValuePair.Value == item)
                    return keyValuePair.Key;
            }
            return -1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, Column item)
        {
            _dictionary.Add(index, item);
        }

        public void RemoveAt(int index)
        {
            _dictionary.Remove(index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        [XmlIgnore]
        public Column this[int row]
        {
            get
            {
                if (!_dictionary.ContainsKey(row))
                {
                    _dictionary.Add(row, new Column());
                }
                _dictionary[row].Index = row;
                return _dictionary[row];
            }
            set
            {
                if (!_dictionary.ContainsKey(row))
                {
                    _dictionary.Add(row, value);
                    _dictionary[row].Index = row;
                }
                else
                {
                    _dictionary[row] = value;
                    _dictionary[row].Index = row;
                }
            }
        }


        #region Implementation of IEnumerable
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Column> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<Row>

        public void Add(Column item)
        {
            int key = _dictionary.Count;
            _dictionary.Add(key, item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(Column item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(Column[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Column item)
        {
            return _dictionary.Remove(IndexOf(item));
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}
