using System;

namespace ThinkAway.Net.OAuth
{
    ///<summary>
    /// 
    ///</summary>
    public class AuthorizeInfo
    {
        private string _authorizeUrl;

        ///<summary>
        /// 
        ///</summary>
        public string AuthorizeUrl
        {
            get { return _authorizeUrl; }
            set { _authorizeUrl = value; }
        }

        private string _requestToken;

        ///<summary>
        /// 
        ///</summary>
        public string RequestToken
        {
            get { return _requestToken; }
            set { _requestToken = value; }
        }

        private string _requestTokenSecret;

        ///<summary>
        /// 
        ///</summary>
        public string RequestTokenSecret
        {
            get { return _requestTokenSecret; }
            set { _requestTokenSecret = value; }
        }

        ///<summary>
        /// 
        ///</summary>
        public AuthorizeInfo()
        {
            this.AuthorizeUrl = String.Empty;
            this.RequestToken = String.Empty;
            this.RequestTokenSecret = String.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizeUrl"></param>
        /// <param name="requestToken"></param>
        /// <param name="requestTokenSecret"></param>
        public AuthorizeInfo(String authorizeUrl, String requestToken, String requestTokenSecret)
        {
            this.AuthorizeUrl = authorizeUrl;
            this.RequestToken = requestToken;
            this.RequestTokenSecret = requestTokenSecret;
        }
    }
}