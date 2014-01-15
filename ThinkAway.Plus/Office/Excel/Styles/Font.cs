using System;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel.Styles
{
    [Serializable]
    public class Font
    {
        /// <summary>
        /// 字体名称
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public string FontName { get; set; }
        /// <summary>
        /// 字符集
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public int CharSet { get; set; }
        /// <summary>
        /// 字体大小
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public int Size { get; set; }
        /// <summary>
        /// 字体颜色
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public string Color { get; set; }
        /// <summary>
        /// 粗体
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public int Bold { get; set; }
        /// <summary>
        /// 斜体
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public int Italic { get; set; }
        /// <summary>
        /// 下划线
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public string Underline { get; set; }

    }
}
