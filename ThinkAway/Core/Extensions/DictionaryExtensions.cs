
using System;
using System.Collections.Generic;

namespace ThinkAway.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class DictionaryExtensions
    {
#if !NET20
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String ToString(this Dictionary<String, Object> element, String key)
        {
            if (element.ContainsKey(key) == false) { return ""; }
            return element[key].ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Boolean? ToBoolean(this Dictionary<String, Object> element, String key)
        {
            Boolean x;
            if (element.ContainsKey(key) == false) { return null; }
            if (element[key].ToString() == "0") { return false; }
            if (element[key].ToString() == "1") { return true; }
            if (Boolean.TryParse(element[key].ToString(), out x))
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
        public static Int32? ToInt32(this Dictionary<String, Object> element, String key)
        {
            Int32 x;
            if (element.ContainsKey(key) == false) { return null; }
            if (Int32.TryParse(element[key].ToString(), out x))
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
        public static Int64? ToInt64(this Dictionary<String, Object> element, String key)
        {
            Int64 x;
            if (element.ContainsKey(key) == false) { return null; }
            if (Int64.TryParse(element[key].ToString(), out x))
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
        public static Double? ToDouble(this Dictionary<String, Object> element, String key)
        {
            Double x;
            if (element.ContainsKey(key) == false) { return null; }
            if (Double.TryParse(element[key].ToString(), out x))
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
        public static DateTime? ToDateTime(this Dictionary<String, Object> element, String key)
        {
            DateTime x;
            if (element.ContainsKey(key) == false) { return null; }
            if (DateTime.TryParse(element[key].ToString(), out x))
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
        public static DateTimeOffset? ToDateTimeOffset(this Dictionary<String, Object> element, String key)
        {
            DateTimeOffset x;
            if (element.ContainsKey(key) == false) { return null; }
            if (DateTimeOffset.TryParse(element[key].ToString(), out x))
            {
                return x;
            }
            return null;
        }

#endif
        public static string GetString(Dictionary<string, object> d, string p)
        {
            throw new NotImplementedException();
        }
    }
}
