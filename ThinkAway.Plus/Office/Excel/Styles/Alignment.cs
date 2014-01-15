using System;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel.Styles
{
    [Serializable]
    public class Alignment
    {
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public string Vertical { get; set; }

        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public string Horizontal { get; set; }

        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public int WrapText { get; set; }
    }
}
