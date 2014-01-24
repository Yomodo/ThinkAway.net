#if !NET20
using System;

namespace ThinkAway.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringValueExtension
    {
        // Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int GetStringCount<T>(this T value)
        {
            return GetStringValueAttributes<T>(value).Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetStringValue<T>(this T value)
        {
            return value.GetStringValue<T>(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetStringValue<T>(this T value, int index)
        {
            StringValueAttribute[] stringValueAttributes = GetStringValueAttributes<T>(value);
            if (stringValueAttributes.Length <= index)
            {
                return null;
            }
            return stringValueAttributes[index].StringValue;
        }

        private static StringValueAttribute[] GetStringValueAttributes<T>(T value)
        {
            Type type = value.GetType();
            if (value is Enum)
            {
                return (type.GetField(value.ToString()).GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[]);
            }
            return (type.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[]);
        }
    }


}
#endif