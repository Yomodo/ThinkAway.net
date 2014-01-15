using System;

namespace ThinkAway.Text
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class StringValueAttribute : Attribute
    {
        // Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public StringValueAttribute(string value)
        {
            this.StringValue = value;
        }

        // Properties
        private string _stringValue;

        /// <summary>
        /// 
        /// </summary>
        public string StringValue
        {
            get { return _stringValue; }
            protected set { _stringValue = value; }
        }
    }


}
