#region License
//=============================================================================
// ThinkAway MVC - .NET Web Application Framework 
//
// Copyright (c) 2003-2009 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using ThinkAway.Web.WebApp;

namespace ThinkAway.Web
{
    public static class TypeHelper
    {
        public static T ConvertString<T>(string stringValue)
        {
            object o = ConvertString(stringValue, typeof (T));

            if (o == null)
                return default(T);
            else
                return (T) o;
        }

        public static object ConvertString(string stringValue, Type targetType)
        {
            if (stringValue == null)
                return null;

            if (targetType == typeof(string))
                return stringValue;

            object value = stringValue;

            if (IsNullable(targetType))
            {
                if (stringValue.Trim().Length == 0)
                    return null;

                targetType = GetRealType(targetType);
            }

            if (targetType != typeof(string))
            {
                if (targetType == typeof(double) || targetType == typeof(float))
                {
                    double doubleValue;

                    if (!Double.TryParse(stringValue.Replace(',', '.'), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out doubleValue))
                        value = null;
                    else
                        value = doubleValue;
                }
                else if (targetType == typeof(decimal))
                {
                    decimal decimalValue;

                    if (!Decimal.TryParse(stringValue.Replace(',', '.'), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out decimalValue))
                        value = null;
                    else
                        value = decimalValue;
                }
                else if (targetType == typeof(Int32) || targetType == typeof(Int16) || targetType == typeof(Int64) || targetType == typeof(SByte) || targetType.IsEnum)
                {
                    long longValue;

                    if (!Int64.TryParse(stringValue, out longValue))
                        value = null;
                    else
                        value = longValue;
                }
                else if (targetType == typeof(UInt32) || targetType == typeof(UInt16) || targetType == typeof(UInt64) || targetType == typeof(Byte))
                {
                    ulong longValue;

                    if (!UInt64.TryParse(stringValue, out longValue))
                        value = null;
                    else
                        value = longValue;
                }
                else if (targetType == typeof(DateTime))
                {
                    DateTime dateTime;

                    if (!DateTime.TryParseExact(stringValue,new [] {"yyyyMMdd", "yyyy-MM-dd", "yyyy.MM.dd", "yyyy/MM/dd"}, null, DateTimeStyles.NoCurrentDateDefault, out dateTime))
                        value = null;
                    else
                        value = dateTime;
                }
                else if (targetType == typeof(bool))
                {
                    value = (stringValue == "1" || stringValue.ToUpper() == "Y" || stringValue.ToUpper() == "YES" || stringValue.ToUpper() == "T" || stringValue.ToUpper() == "TRUE");
                }
                else
                {
                    value = null;

                    foreach (IObjectBinder objectBinder in WebAppConfig.CustomObjectBinders)
                    {
                        if (objectBinder.TryConvert(stringValue, targetType, out value))
                            return value;
                    }
                }
            }

            if (value == null)
                return null;

            if (targetType.IsValueType)
            {
                if (!targetType.IsGenericType)
                {
                    if (targetType.IsEnum)
                        return Enum.ToObject(targetType, value);
                    else
                        return Convert.ChangeType(value, targetType);
                }

                if (targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type sourceType = value.GetType();

                    Type underlyingType = targetType.GetGenericArguments()[0];

                    if (sourceType == underlyingType)
                        return value;

                    if (underlyingType.IsEnum)
                    {
                        return Enum.ToObject(underlyingType, value);
                    }
                    else
                    {
                        return Convert.ChangeType(value, underlyingType);
                    }
                }
            }

            return value;
        }

        public static object ConvertType(object value, Type targetType)
        {
            if (value == null)
                return null;

            if (value.GetType() == targetType)
                return value;

            if (targetType.IsValueType)
            {
                if (!targetType.IsGenericType)
                {
                    if (targetType.IsEnum)
                        return Enum.ToObject(targetType, value);
                    else
                        return Convert.ChangeType(value, targetType);
                }

                if (targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type underlyingType = targetType.GetGenericArguments()[0];

                    return ConvertType(value, underlyingType);
                }
            }

            if (targetType.IsAssignableFrom(value.GetType()))
                return value;
            else if (targetType == typeof(string))
                return value.ToString();
            else
                return Convert.ChangeType(value, targetType);
        }

        public static bool IsNullable(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetRealType(Type type)
        {
            if (IsNullable(type))
                return type.GetGenericArguments()[0];

            return type;
        }

        /// <summary>
        /// Finds all types derived from the given type, limiting the search to the given assembly
        /// </summary>
        /// <param name="baseType">The base type or interface to use for finding types</param>
        /// <param name="assembly">The assembly to look into</param>
        /// <returns>An array of all types found in the given assembly which are either derived from the given type, or implement the given interface</returns>
        internal static Type[] FindCompatibleTypes(Assembly assembly, Type baseType)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in assembly.GetTypes())
            {
                if (type != baseType && baseType.IsAssignableFrom(type))
                    types.Add(type);
            }

            return types.ToArray();
        }
    }
}
