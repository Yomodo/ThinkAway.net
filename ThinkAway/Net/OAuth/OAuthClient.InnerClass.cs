using System;
using System.Collections.Generic;

namespace ThinkAway.Net.OAuth
{
    public partial class OAuthClient
    {
        /// <summary>
        /// Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            /// <summary>
            /// 
            /// </summary>
            HMACSHA1,
            /// <summary>
            /// 
            /// </summary>
            PLAINTEXT,
            /// <summary>
            /// 
            /// </summary>
            RSASHA1,
        }
        /// <summary>
        /// Provides an internal structure to sort the query parameter
        /// </summary>
        protected class QueryParameter
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="value"></param>
            public QueryParameter(String name, String value)
            {
                this.Name = name;
                this.Value = value;
            }

            private string _name;

            /// <summary>
            /// 
            /// </summary>
            public string Name
            {
                get { return _name; }
                private set { _name = value; }
            }

            private string _value;

            /// <summary>
            /// 
            /// </summary>
            public string Value
            {
                get { return _value; }
                private set { _value = value; }
            }
        }
        /// <summary>
        /// Comparer class used to perform the sorting of the query parameters
        /// </summary>
        protected class QueryParameterComparer : IComparer<QueryParameter>
        {
            #region IComparer<QueryParameter> Members

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Name == y.Name)
                {
                    return System.String.CompareOrdinal(x.Value, y.Value);
                }
                return System.String.CompareOrdinal(x.Name, y.Name);
            }

            #endregion
        }
    }
}