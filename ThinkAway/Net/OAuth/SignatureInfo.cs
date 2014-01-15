using System;

namespace ThinkAway.Net.OAuth
{
    /// <summary>
    /// 
    /// </summary>
    public class SignatureInfo
    {
        private string _signature;

        ///<summary>
        /// 
        ///</summary>
        public string Signature
        {
            get { return _signature; }
            set { _signature = value; }
        }

        private string _normalizedUrl;

        ///<summary>
        /// 
        ///</summary>
        public string NormalizedUrl
        {
            get { return _normalizedUrl; }
            set { _normalizedUrl = value; }
        }

        private string _normalizedRequestParameters;

        ///<summary>
        /// 
        ///</summary>
        public string NormalizedRequestParameters
        {
            get { return _normalizedRequestParameters; }
            set { _normalizedRequestParameters = value; }
        }

        ///<summary>
        /// 
        ///</summary>
        public SignatureInfo()
        {
            NormalizedRequestParameters = String.Empty;
            NormalizedUrl = String.Empty;
            Signature = String.Empty;
        }
    }
}