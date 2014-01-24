using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel
{
    /// <summary>
    /// 表示一个 Excel 单元格对象
    /// </summary>
    [Serializable]
    public class Cell
    {
        [XmlAttribute(AttributeName = "Index", Namespace = "http://schemas.lsong.org/office")]
        public int Index { get; set; }
        /// <summary>
        /// 与该单元格关联的样式ID
        /// </summary>
        [XmlAttribute(AttributeName = "StyleID", Namespace = "http://schemas.lsong.org/office")]
        public string StyleId { get; set; }
        /// <summary>
        /// 与该单元格关联的数据内容
        /// </summary>
        [XmlElement("Data")]
        public Data Data { get; set; }

        public Cell()
        {
            Data = new Data();
        }
        public Cell(object obj):this()
        {
            switch (obj.GetType().Name)
            {
                case "String":
            Lable_String:
                    Data.Type = "String";
                    Data.Content = obj.ToString();
                    break;
                case "Int32":
                    Data.Type = "Number";
                    Data.Content = obj.ToString();
                    break;
                default:
                    goto Lable_String;
            }
        }
    }
    [Serializable]
    public class CellCollection : IList<Cell>
    {
        Dictionary<int,Cell> dictionary = new Dictionary<int, Cell>(); 

        public int IndexOf(Cell item)
        {
            foreach (KeyValuePair<int, Cell> keyValuePair in dictionary)
            {
                if (keyValuePair.Value == item)
                    return keyValuePair.Key;
            }
            return -1;
        }

        public void Insert(int index, Cell item)
        {
            dictionary.Add(index,item);
        }

        public void RemoveAt(int index)
        {
            dictionary.Remove(index);
        }
        /// <summary>
        /// 获取(如果不存在则创建)或设置单元格
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        [XmlIgnore]
        public Cell this[int col]
        {
            get
            {
                if (!dictionary.ContainsKey(col))
                {
                    dictionary.Add(col, new Cell());
                    dictionary[col].Index = col;
                }
                return dictionary[col];
            }
            set
            {
                if (!dictionary.ContainsKey(col))
                {
                    dictionary.Add(col, value);
                    dictionary[col].Index = col;
                }
                else
                {
                    dictionary[col] = value;
                    dictionary[col].Index = col;
                }
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator<Cell> GetEnumerator()
        {
            return dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<Cell>

        public void Add(Cell item)
        {
            //throw new NotImplementedException();
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(Cell item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(Cell[] array, int arrayIndex)
        {
            //throw new NotImplementedException();
        }

        public bool Remove(Cell item)
        {
            return dictionary.Remove(IndexOf(item));
        }
        [XmlIgnore]
        public int Count
        {
            get { return dictionary.Count; }
        }
        [XmlIgnore]
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion
    }
}
