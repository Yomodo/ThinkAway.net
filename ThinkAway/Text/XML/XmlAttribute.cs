using System;

namespace ThinkAway.Text.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlAttribute
    {
        private string _key;

        /// <summary>
        /// 
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        private string _value;

        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public XmlAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}=\"{1}\"", Key, Value);
        }
    }
}