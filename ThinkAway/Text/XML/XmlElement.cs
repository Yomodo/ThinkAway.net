using System;
using ThinkAway.Core;

namespace ThinkAway.Text.Xml
{
    public class XmlElement
    {

        private string _name;

        private readonly XmlElementCollection _xmlElementCollection;

        private readonly XmlAttributeCollection _xmlAttributeCollection;


        public string InnerXml
        {
            get { return _xmlElementCollection.ToString().Trim(); }
        }

        public string OutterXml
        {
            get
            {
                return string.Format("<{0} {1}>{2}</{0}>", _name, _xmlAttributeCollection, _xmlElementCollection).Trim();
            }
        }
        public string ElementName
        {
            get { return _name; }
            set { _name = value; }
        }

        public XmlElementCollection ChlidElements
        {
            get { return _xmlElementCollection; }
        }


        public XmlAttributeCollection Attributes
        {
            get { return _xmlAttributeCollection; }
        }


        public XmlElement(string name)
        {
            _name = name;

            _xmlElementCollection = new XmlElementCollection();
            _xmlAttributeCollection = new XmlAttributeCollection();
        }

        public XmlElement AddElement(XmlElement xmlElement)
        {
            _xmlElementCollection.Add(xmlElement);
            return xmlElement;
        }

        public void AddAttribute(XmlAttribute xmlAttribute)
        {
            _xmlAttributeCollection.Add(xmlAttribute);
        }



        public XmlElement Find(MyFunc<bool,XmlElement> func)
        {
            foreach (XmlElement chlidElement in ChlidElements)
            {
                bool b = func(chlidElement);
                if (b)
                    return chlidElement;
            }
            return null;
        }

        public XmlElement Find(string p)
        {
            throw new NotImplementedException();
        }


        public override string ToString()
        {
            return OutterXml;
        }
    }
}
