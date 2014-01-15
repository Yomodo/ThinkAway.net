using System;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel
{
    [Serializable]
    public class Data
    {
        [XmlText]
        public string Content { get; set; }

        [XmlAttribute("Type", Namespace = "http://schemas.lsong.org/office")]
        public string Type { get; set; }
    }
}
