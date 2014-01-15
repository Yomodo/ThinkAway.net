using System.Xml.Linq;

namespace ThinkAway.Text.XML
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class XmlObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlns"></param>
        /// <returns></returns>
        public abstract XElement CreateElement(XNamespace xmlns);
    }
}