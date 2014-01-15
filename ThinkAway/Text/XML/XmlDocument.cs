using System.Text;

namespace ThinkAway.Text.Xml
{
    public class XmlDocument
    {
        private XmlElement _rootElement;

        public XmlElement RootElement
        {
            get { return _rootElement; }
            set { _rootElement = value; }
        }

        public XmlDocument(XmlElement xmlElement)
        {
            RootElement = xmlElement;
        }

        public void SetRootElement(XmlElement xmlElement)
        {
            RootElement = xmlElement;
        }

        public void SetVersion(System.Version version)
        {
            throw new System.NotImplementedException();
        }

        private Encoding _encoding;
        public System.Text.Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        public void Save(string p)
        {
            throw new System.NotImplementedException();
        }

        public void Load(string xmlFile)
        {
            throw new System.NotImplementedException();
        }

        public void LoadXml(string xml)
        {
            throw new System.NotImplementedException();
        }


        public override string ToString()
        {
            return string.Format("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n{0}",RootElement);

        }
    }
}
