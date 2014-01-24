using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel.Styles
{
    /// <summary>
    /// 表示一个Excel样式相关的设置
    /// </summary>
    [Serializable,XmlRoot(ElementName = "Style")]
    public class Style
    {
        /// <summary>
        /// 样式 ID
        /// <remarks>
        /// 该属性由样式集合自动生成,用于关联单元格样式,避免修改此字段
        /// </remarks>
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public string ID { get; set; }
        /// <summary>
        /// 样式名称
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public string Name { get; set; }
        /// <summary>
        /// 对齐方式
        /// </summary>
        [XmlElement("Alignment")]
        public Alignment Alignment { get; set; }
        /// <summary>
        /// 边框
        /// </summary>
        [XmlElement("Borders")]
        public Borders Borders { get; set; }
        /// <summary>
        /// 字体
        /// </summary>
        [XmlElement("Font")]
        public Font Font { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Interior")]
        public Interior Interior { get; set; }
        /// <summary>
        /// 数字格式
        /// </summary>
        [XmlElement("NumberFormat")]
        public NumberFormat NumberFormat { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Protection")]
        public Protection Protection { get; set; }
    }

    [Serializable]
    public class StyleCollection
    {
        [XmlElementAttribute(ElementName = "Style")]
        public List<Style> Styles { get; set; }

        public StyleCollection()
        {
            Styles = new List<Style>();
        }

        public string Add(Style style)
        {
            string styleId = string.Format("s{0}", Styles.Count);
            style.ID = styleId;
            Styles.Add(style);
            return styleId;
        }
    }
}