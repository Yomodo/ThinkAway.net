using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Office.Excel.Styles
{
    /// <summary>
    /// ��ʾһ��Excel��ʽ��ص�����
    /// </summary>
    [Serializable,XmlRoot(ElementName = "Style")]
    public class Style
    {
        /// <summary>
        /// ��ʽ ID
        /// <remarks>
        /// ����������ʽ�����Զ�����,���ڹ�����Ԫ����ʽ,�����޸Ĵ��ֶ�
        /// </remarks>
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public string ID { get; set; }
        /// <summary>
        /// ��ʽ����
        /// </summary>
        [XmlAttribute(Namespace = "http://schemas.lsong.org/office")]
        public string Name { get; set; }
        /// <summary>
        /// ���뷽ʽ
        /// </summary>
        [XmlElement("Alignment")]
        public Alignment Alignment { get; set; }
        /// <summary>
        /// �߿�
        /// </summary>
        [XmlElement("Borders")]
        public Borders Borders { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [XmlElement("Font")]
        public Font Font { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Interior")]
        public Interior Interior { get; set; }
        /// <summary>
        /// ���ָ�ʽ
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