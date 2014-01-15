using System;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel
{
    [Serializable]
    public class Table
    {
        //[XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        //public int ExpandedColumnCount { get; set; }

        //[XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        //public int ExpandedRowCount { get; set; }

       [XmlAttribute(Namespace = "urn:schemas-microsoft-com:office:excel")]
        public int FullColumns { get; set; }

       [XmlAttribute(Namespace = "urn:schemas-microsoft-com:office:excel")]
        public int FullRows { get; set; }

       [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public double DefaultColumnWidth { get; set; }

       [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public double DefaultRowHeight { get; set; }

        /// <summary>
        /// 获取该表列的集合
        /// </summary>
        [XmlElement("Column")]
        public ColumnCollection Columns { get;  set; }
        /// <summary>
        /// 获取该表行的集合
        /// </summary>
        [XmlElement("Row")]
        public RowCollection Rows { get; set; }

        public Table()
        {
            //ExpandedColumnCount = 1;

            //ExpandedRowCount = 1;

            FullColumns = 1;

            FullRows = 1;

            DefaultColumnWidth = 54.0;

            DefaultRowHeight = 13.5;

            Rows = new RowCollection();
            Columns = new ColumnCollection();
        }
    }
}
