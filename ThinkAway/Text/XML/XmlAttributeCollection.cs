using System.Collections.Generic;
using System.Text;

namespace ThinkAway.Text.Xml
{
    public class XmlAttributeCollection : List<XmlAttribute>
    {
        public XmlAttribute this[string name]
        {
            get { return this.Find(delegate(XmlAttribute x) { return x.Key.Equals(name); }); }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (XmlAttribute xmlAttribute in this)
            {
                stringBuilder.AppendFormat("{0} ", xmlAttribute);
            }
            return stringBuilder.ToString();
        }
    }
}
