#if !NET20
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ThinkAway.Text.Xml
{

    /// <summary>
    /// 
    /// </summary>
    public static class XmlExtensions
    {
        public static string CastAttributeToString(this XElement element,string str1, string str2)
        {
            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String CastElementToString(this XElement element, String key)
        {
            if (element.Element(key) == null) { return ""; }
            return element.Element(key).Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Boolean? CastElementToBoolean(this XElement element, String key)
        {
            Boolean x = false;
            if (element.Element(key) == null) { return null; }
            if (Boolean.TryParse(element.Element(key).Value, out x) == true)
            {
                return x;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Int32? CastElementToInt32(this XElement element, String key)
        {
            Int32 x = 0;
            if (element.Element(key) == null) { return null; }
            if (Int32.TryParse(element.Element(key).Value, out x) == true)
            {
                return x;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Int64? CastElementToInt64(this XElement element, String key)
        {
            Int64 x = 0;
            if (element.Element(key) == null) { return null; }
            if (Int64.TryParse(element.Element(key).Value, out x) == true)
            {
                return x;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String CastAttributeToString(this XElement element, String key)
        {
            if (element.Attribute(key) == null) { return ""; }
            return element.Attribute(key).Value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Boolean? CastAttributeToBoolean(this XElement element, String key)
        {
            Boolean x = false;
            if (element.Attribute(key) == null) { return null; }
            if (Boolean.TryParse(element.Attribute(key).Value, out x) == true)
            {
                return x;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Int32? CastAttributeToInt32(this XElement element, String key)
        {
            Int32 x = 0;
            if (element.Attribute(key) == null) { return null; }
            if (Int32.TryParse(element.Attribute(key).Value, out x) == true)
            {
                return x;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Int64? CastAttributeToInt64(this XElement element, String key)
        {
            Int64 x = 0;
            if (element.Attribute(key) == null) { return null; }
            if (Int64.TryParse(element.Attribute(key).Value, out x) == true)
            {
                return x;
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement ElementByNamespace(this XElement element, string name)
        {
            return element.ElementByNamespace(null, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement ElementByNamespace(this XElement element, string nameSpace, string name)
        {
            var ns = String.IsNullOrEmpty(nameSpace) ? element.GetDefaultNamespace() : element.GetNamespaceOfPrefix(nameSpace);
            return element.Element(ns + name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> ElementsByNamespace(this XElement element, string name)
        {
            return element.ElementsByNamespace(null, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> ElementsByNamespace(this XElement element, string nameSpace, string name)
        {
            var ns = String.IsNullOrEmpty(nameSpace) ? element.GetDefaultNamespace() : element.GetNamespaceOfPrefix(nameSpace);
            return element.Elements(ns + name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XAttribute AttributeByNamespace(this XElement element, string name)
        {
            return element.AttributeByNamespace(null, name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XAttribute AttributeByNamespace(this XElement element, string nameSpace, string name)
        {
            var ns = String.IsNullOrEmpty(nameSpace) ? element.GetDefaultNamespace() : element.GetNamespaceOfPrefix(nameSpace);
            return element.Attribute(ns + name) ?? element.Attribute(name);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool? CastElementToBoolean(this XElement element, string nameSpace, string name)
        {
            var e = element.ElementByNamespace(nameSpace, name);
            if (e == null)
            {
                return null;
            }

            bool x;
            if (Boolean.TryParse(e.Value, out x))
            {
                return x;
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int? CastElementToInt32(this XElement element, string nameSpace, string name)
        {
            var e = element.ElementByNamespace(nameSpace, name);
            if (e == null)
            {
                return null;
            }

            int x;
            if (Int32.TryParse(e.Value, out x))
            {
                return x;
            }
            return null;
        }


   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool? CastAttributeToBoolean(this XElement element, string nameSpace, string name)
        {
            var a = element.AttributeByNamespace(nameSpace, name);
            if (a == null)
            {
                return null;
            }

            bool x;
            if (Boolean.TryParse(a.Value, out x))
            {
                return x;
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nameSpace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int? CastAttributeToInt32(this XElement element, string nameSpace, string name)
        {
            var a = element.AttributeByNamespace(nameSpace, name);
            if (a == null)
            {
                return null;
            }

            int x;
            if (Int32.TryParse(a.Value, out x))
            {
                return x;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="addElement"></param>
        /// <returns></returns>
        public static XDocument AddElement(this XDocument document, XElement addElement)
        {
            document.Add(addElement);
            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XElement AddElement(this XElement element, XName name)
        {
            return element.AddElement(new XElement(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XElement AddElement(this XElement element, XName name, object value)
        {
            return element.AddElement(new XElement(name, value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="addElement"></param>
        /// <returns></returns>
        public static XElement AddElement(this XElement element, XElement addElement)
        {
            element.Add(addElement);
            return element;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static XElement AddAttribute(this XElement element, XName name, object value)
        {
            element.Add(new XAttribute(name, value));
            return element;
        }
    }
}
#endif