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
using ThinkAway.Web.Controls;

namespace ThinkAway.Web
{
    public class FormRadioButton : FormFieldAttribute
    {
        private string _group;
        private string _onClick;


        public FormRadioButton()
        {
        }

        public FormRadioButton(string group)
        {
            _group = group;
        }

        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public string OnClick
        {
            get { return _onClick; }
            set { _onClick = value; }
        }

        protected internal override bool IsRightType()
        {
            return FieldType == typeof(bool);
        }

        protected internal override object GetControlValue(Control control)
        {
            return ((RadioButtonControl)control).Checked;
        }

        protected internal override void SetControlValue(Control control, object value)
        {
            ((RadioButtonControl) control).Checked = (bool) value;
        }

        protected internal override Control CreateControl(string name)
        {
            RadioButtonControl control = new RadioButtonControl(name,Group);

            control.OnClick = OnClick;

            return control;
        }

        protected internal override bool Validate(Control control)
        {
            return true;
        }
    }
}