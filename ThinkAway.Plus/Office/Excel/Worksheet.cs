using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel
{
    /// <summary>
    /// 表示一个 Excel 工作表
    /// </summary>
    [Serializable]
    public class Worksheet
    {
        /// <summary>
        /// Excel 工作表 名称
        /// <c>Worksheet Name</c>
        /// </summary>
        [XmlAttribute(AttributeName = "Name", Namespace = "http://schemas.lsong.org/office")]
        public string Name { get; set; }
        /// <summary>
        /// Excel 表
        /// </summary>
        [XmlElement("Table")]
        public Table Tables { get; set; }
        /// <summary>
        /// Excel 工作表选项
        /// </summary>
        [XmlElement("WorksheetOptions", Namespace = "http://schemas.lsong.org/office/excel")]
        public WorksheetOptions WorksheetOptions { get; set; }

        /// <summary>
        /// 使用默认构造函数初始化<c>Excel工作表</c>
        /// </summary>
        public Worksheet()
        {
            Tables = new Table();

            WorksheetOptions = new WorksheetOptions();
        }
        /// <summary>
        /// 使用指定的工作表名称构造函数初始化<c>Excel工作表</c>
        /// </summary>
        public Worksheet(string name):this()
        {
            this.Name = name;
        }
        /// <summary>
        /// 通过指定的行和列获取单元格对象
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <returns>单元格</returns>
        [XmlIgnore]
        public Cell this[int row, int col]
        {
            get { return Tables.Rows[row].Cells[col]; }
            set { Tables.Rows[row].Cells[col] = value; }
        }
    }
    /// <summary>
    /// 工作表的集合
    /// <c>WorksheetCollection</c>
    /// </summary>
    [Serializable]
    public class WorksheetCollection :List<Worksheet>
    {

    }
}
