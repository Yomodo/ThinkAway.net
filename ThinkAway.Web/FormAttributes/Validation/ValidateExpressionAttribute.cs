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
using ThinkAway.Core.Parser;
using ThinkAway.Core.Parser.ExpressionParser.Parser;
using ThinkAway.Core.Parser.Parsers.CSharp;

namespace ThinkAway.Web
{
    public class ValidateExpressionAttribute : ValidationAttribute
    {
        private readonly string _expression;

        private class ValidationContext : CSharpContext
        {
            public ValidationContext(object currentValue, Dictionary<string, object> validationContext)
            {
                foreach (KeyValuePair<string, object> pair in validationContext)
                {
                    Set(pair.Key, pair.Value, pair.Value == null ? typeof(object) : pair.Value.GetType());
                }

                Set("this", currentValue, currentValue == null ? typeof(object) : currentValue.GetType());
            }
        }

        public ValidateExpressionAttribute(string expression)
        {
            _expression = expression;
        }

        protected override bool Validate(object value, Type targetType)
        {
            ExpressionParser parser = new CSharpParser();

            return parser.Evaluate<bool>("!!(" + _expression + ")", new ValidationContext(value, Context));
        }
    }
}
