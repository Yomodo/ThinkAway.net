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
using System.Text.RegularExpressions;
using ThinkAway.Web.Controls;

namespace ThinkAway.Web
{
    public class FormHiddenAttribute : FormFieldAttribute
    {
        protected internal override bool IsRightType()
        {
            Type type = Nullable.GetUnderlyingType(FieldType) ?? FieldType;

            return (
                   type == typeof(string)
                || type == typeof(Int16)
                || type == typeof(Int32)
                || type == typeof(Int64)
                || type == typeof(UInt16)
                || type == typeof(UInt32)
                || type == typeof(UInt64)
                || type == typeof(Double)
                || type == typeof(Single)
                || type == typeof(Decimal)
                );
        }

        protected internal override bool Validate(Control control)
        {
            string value = ((HiddenControl) control).Value ?? "";

            if (value.Length > 0 && FieldType != typeof(string))
            {
                return Regex.Match(value, @"^\-?\d+([\.,]\d+)?$").Success;
            }

            return true;
        }

        protected internal override Control CreateControl(string name)
        {
            return new HiddenControl(name);
        }

        protected internal override object GetControlValue(Control control)
        {
            string stringValue = ((HiddenControl)control).Value;

            return TypeHelper.ConvertString(stringValue, FieldType);
        }

        protected internal override void SetControlValue(Control control, object value)
        {
            ((HiddenControl)control).Value = (value != null) ? value.ToString() : "";
        }
    }
}