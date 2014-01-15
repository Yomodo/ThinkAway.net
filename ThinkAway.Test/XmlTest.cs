using System;
using ThinkAway.Text.Xml;

namespace ThinkAway.Test
{
    class XmlTest
    {
        public void Test()
        {

            XmlElement xmlElement = new XmlElement("TestXml");
            XmlElement xmlElement1 = new XmlElement("ChlidElement1");
            xmlElement1.AddAttribute(new XmlAttribute("test", "12"));
            XmlElement addElement = xmlElement.AddElement(xmlElement1);
            string innerXml = xmlElement.InnerXml;
            string outterXml = xmlElement.OutterXml;
            XmlElement xmlElements = xmlElement.ChlidElements["ChlidElement1"][0];
            XmlElement element = xmlElement.Find(x => Equals(x.ElementName, "ChlidElement1"));
            // element = xmlElement.Find(@"\XML\@aa");
            XmlAttribute xmlAttributes = xmlElement1.Attributes["test"];
            XmlAttribute xmlAttribute = new XmlAttribute("name", "lsong");
            xmlElement.AddAttribute(xmlAttribute);

            XmlDocument xmlDocument = new XmlDocument(xmlElement);
            xmlDocument.SetRootElement(xmlElement);
            xmlDocument.SetVersion(new Version(1, 0, 0, 0));
            xmlDocument.Encoding = System.Text.Encoding.UTF8;
            xmlDocument.Save("c:\\temp.xml");

            xmlDocument.Load("c:\\temp.xml");
            //xmlDocument.LoadXml("<xml />");
            XmlElement rootElement = xmlDocument.RootElement;
        }
    }
}
