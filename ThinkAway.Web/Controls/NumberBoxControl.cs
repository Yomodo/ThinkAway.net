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

namespace ThinkAway.Web
{
    public class NumberBoxControl<T> : TextBoxControl where T:IFormattable
    {
        public new T Value;

        private string _formatString;

        public NumberBoxControl(string name) : base(name)
        {
        }

        public NumberBoxControl(string name, string formatString) : base(name)
        {
            _formatString = formatString;
        }

        public string FormatString
        {
            get { return _formatString; }
            set { _formatString = value; }
        }

        protected override string GenerateHtml(View view, string className, string clasError)
        {
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>) && Value == null)
                base.Value = null;
            else
                base.Value = Value.ToString(_formatString, null);

            return base.GenerateHtml(view, className, clasError);
        }

        protected override void HandlePostback(ClientDataCollection postData)
        {
            base.HandlePostback(postData);

            Value = (T) TypeHelper.ConvertString(base.Value, typeof (T));
        }
    }
}
