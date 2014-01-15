using System.Collections.Generic;
using System.Text;

namespace ThinkAway.Text.Xml
{
    public class XmlElementCollection : List<XmlElement>
    {
        public XmlElement[] this[string name]
        {
            get 
            {
                List<XmlElement> list = new List<XmlElement>();
                foreach (XmlElement xmlElement in this)
                {
                    if (Equals(xmlElement.ElementName, name))
                        list.Add(xmlElement);
                }
                return list.ToArray();
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (XmlElement xmlElement in this)
            {
                stringBuilder.AppendLine(xmlElement.ToString());
            }
            return stringBuilder.ToString();
        }
    }
}
