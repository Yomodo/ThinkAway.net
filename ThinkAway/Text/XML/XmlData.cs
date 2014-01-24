using System;
using System.Collections.Generic;
using System.Text;

namespace ThinkAway.Text.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlData : Dictionary<String, String>
    {
        private const int DefaultIndent = 2;

        private string _xmlDeclaration;

        /// <summary>
        /// 
        /// </summary>
        public String XmlDeclaration
        {
            get { return _xmlDeclaration; }
            set { _xmlDeclaration = value; }
        }

        private string _rootElementName;

        /// <summary>
        /// 
        /// </summary>
        public String RootElementName
        {
            get { return _rootElementName; }
            set { _rootElementName = value; }
        }

        private XmlData _child;

        /// <summary>
        /// 
        /// </summary>
        public XmlData Child
        {
            get { return _child; }
            set { _child = value; }
        }

        private List<XmlAttribute> _attributes;

        /// <summary>
        /// 
        /// </summary>
        public List<XmlAttribute> Attributes
        {
            get { return _attributes; }
            private set { _attributes = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public XmlData()
            : this(String.Empty)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootElementName"></param>
        public XmlData(String rootElementName)
        {
            XmlDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
            RootElementName = rootElementName;
            Attributes = new List<XmlAttribute>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return CreateXmlText(this, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string CreateAttributeText(XmlData data)
        {
            if (data.Attributes.Count > 0)
            {
                //NOTE:NET2.0
                StringBuilder stringBuilder = new StringBuilder(" ");
                foreach (XmlAttribute xmlAttribute in data.Attributes)
                {
                    stringBuilder.Append(xmlAttribute);
                    stringBuilder.Append(" ");
                }
                return stringBuilder.ToString();
            }
            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        private String CreateXmlText(XmlData data, int indent)
        {
            StringBuilder sb = new StringBuilder(1024);

            if (indent == 0)
            {
                sb.AppendLine(data.XmlDeclaration);
            }

            sb.AppendFormat("{0}<{1}{2}>", new String(' ', indent), data.RootElementName, CreateAttributeText(data));
            sb.AppendLine();
            foreach (string key in data.Keys)
            {
                sb.AppendFormat("{0}<{1}>{2}</{1}>", new String(' ', indent + DefaultIndent), key, data[key]);
                sb.AppendLine();
            }
            if (data.Child != null)
            {
                sb.AppendLine(CreateXmlText(data.Child, indent + DefaultIndent));
            }
            sb.AppendFormat("{0}</{1}>", new String(' ', indent), data.RootElementName);

            return sb.ToString();
        }
    }
}