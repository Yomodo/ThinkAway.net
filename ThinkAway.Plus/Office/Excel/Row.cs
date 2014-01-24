using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel
{
    /// <summary>
    /// 表示 Excel 工作表中的行
    /// </summary>
    [Serializable, XmlRoot(ElementName = "Row")]
    public class Row
    {
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public int Index { get; set; }

        [XmlAttribute(AttributeName = "Height", Namespace = "http://schemas.lsong.org/office")]
        public string RowHeight { get; set; }
        /// <summary>
        /// 获取或设置行高
        /// </summary>
        [XmlIgnore]
        public int Height
        {
            get { return Convert.ToInt32(RowHeight); }
            set { RowHeight = value.ToString(CultureInfo.InvariantCulture); }
        }

        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public int AutoFitHeight { get; set; }

        
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Cell")]
        public CellCollection Cells { get; set; }

        public Row()
        {
            RowHeight = null;
            Cells = new CellCollection();
        }
    }
    [Serializable]
    public class RowCollection : IList<Row>
    {
        private Dictionary<int, Row> dictionary; 

        public RowCollection()
        {
            dictionary = new Dictionary<int, Row>();
        }

        public int IndexOf(Row item)
        {
            foreach (KeyValuePair<int, Row> keyValuePair in dictionary)
            {
                if (keyValuePair.Value == item)
                    return keyValuePair.Key;
            }
            return -1;
        }

        public void Insert(int index, Row item)
        {
            dictionary.Add(index,item);
        }

        public void RemoveAt(int index)
        {
            dictionary.Remove(index);
        }

        [XmlIgnore]
        public Row this[int row]
        {
            get
            {
                if (!dictionary.ContainsKey(row))
                {
                    dictionary.Add(row, new Row());
                }
                dictionary[row].Index = row;
                return dictionary[row];
            }
            set
            {
                if (!dictionary.ContainsKey(row))
                {
                    dictionary.Add(row, value);
                    dictionary[row].Index = row;
                }
                else
                {
                    dictionary[row] = value;
                    dictionary[row].Index = row;
                }
            }
        }


        #region Implementation of IEnumerable

        public IEnumerator<Row> GetEnumerator()
        {
            return dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<Row>

        public void Add(Row item)
        {
            int key = dictionary.Count;
            dictionary.Add(key,item);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(Row item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(Row[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Row item)
        {
            return dictionary.Remove(IndexOf(item));
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}
