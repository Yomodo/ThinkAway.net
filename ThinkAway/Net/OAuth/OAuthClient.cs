using System;
using System.Collections.Generic;
using System.Text;
using ThinkAway.Core;
using ThinkAway.Net.Http;
using System.Net;

namespace ThinkAway.Net.OAuth
{
    /// <summary>
    /// OAuth認証のクライアント機能を提供するクラスです。
    /// </summary>
    public partial class OAuthClient
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<HttpRequestUploadingEventArgs> Uploading;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<AsyncCallErrorEventArgs> Error;

        private OAuthMode _mode;

        /// <summary>
        /// 
        /// </summary>
        public OAuthMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        private int? _requestBufferSize;

        /// <summary>
        /// 
        /// </summary>
        public Int32? RequestBufferSize
        {
            get { return _requestBufferSize; }
            set { _requestBufferSize = value; }
        }

        private Encoding _requestEncoding;

        /// <summary>
        /// 
        /// </summary>
        public Encoding RequestEncoding
        {
            get { return _requestEncoding; }
            set { _requestEncoding = value; }
        }

        private Encoding _responseEncoding;

        /// <summary>
        /// 
        /// </summary>
        public Encoding ResponseEncoding
        {
            get { return _responseEncoding; }
            set { _responseEncoding = value; }
        }

        private string _consumerKey;

        /// <summary>
        /// 
        /// </summary>
        public String ConsumerKey
        {
            get { return _consumerKey; }
            private set { _consumerKey = value; }
        }

        private string _consumerSecret;

        /// <summary>
        /// 
        /// </summary>
        public String ConsumerSecret
        {
            get { return _consumerSecret; }
            private set { _consumerSecret = value; }
        }

        private string _requestTokenUrl;

        /// <summary>
        /// 
        /// </summary>
        public String RequestTokenUrl
        {
            get { return _requestTokenUrl; }
            private set { _requestTokenUrl = value; }
        }

        private string _authorizeUrl;

        /// <summary>
        /// 
        /// </summary>
        public String AuthorizeUrl
        {
            get { return _authorizeUrl; }
            private set { _authorizeUrl = value; }
        }

        private string _accessTokenUrl;

        /// <summary>
        /// 
        /// </summary>
        public String AccessTokenUrl
        {
            get { return _accessTokenUrl; }
            private set { _accessTokenUrl = value; }
        }

        private MyAction<MyAction> _beginInvoke;

        /// <summary>
        /// 
        /// </summary>
        public MyAction<MyAction> BeginInvoke
        {
            get { return _beginInvoke; }
            set { _beginInvoke = value; }
        }

//NOTE:NET2.0
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="consumerSecret"></param>
        /// <param name="requestTokenUrl"></param>
        /// <param name="authorizeUrl"></param>
        /// <param name="accessTokenUrl"></param>
        public OAuthClient(String consumerKey, String consumerSecret,
            String requestTokenUrl, String authorizeUrl, String accessTokenUrl)
        {
            this.Mode = OAuthMode.Header;
            this.RequestEncoding = Encoding.UTF8;
            this.ResponseEncoding = Encoding.UTF8;
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            RequestTokenUrl = requestTokenUrl;
            AuthorizeUrl = authorizeUrl;
            AccessTokenUrl = accessTokenUrl;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <returns></returns>
        public HttpClient CreateHttpClient(HttpMethodName methodName, String url, String token, String tokenSecret)
        {
            return this.CreateHttpClient(methodName, url, token, tokenSecret, new Dictionary<String, String>());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public HttpClient CreateHttpClient(HttpMethodName methodName, String url, String token, String tokenSecret, IDictionary<String, String> queryString)
        {
            HttpClient cl;
            switch (this.Mode)
            {
                case OAuthMode.QueryString: cl = this.CreateHttpClientQueryStringMode(methodName, url, token, tokenSecret, queryString); break;
                case OAuthMode.Header: cl = this.CreateHttpClientRequestHeaderMode(methodName, url, token, tokenSecret, queryString); break;
                default: throw new InvalidOperationException();
            }
            this.SetHttpClientProperty(cl);
            return cl;
        }
        private void SetHttpClientProperty(HttpClient client)
        {
            HttpClient cl = client;
            //NOTE:NET2.0
            cl.RequestBufferSize = this.RequestBufferSize;
            cl.RequestEncoding = this.RequestEncoding;
            cl.ResponseEncoding = this.ResponseEncoding;
            cl.Uploading += this.Uploading;
            cl.Error += this.Error;
            cl.BeginInvoke = this.BeginInvoke;
        }
        private HttpClient CreateHttpClientQueryStringMode(HttpMethodName methodName, String url, String token, String tokenSecret, IEnumerable<KeyValuePair<string, string>> queryString)
        {
            String timeStamp = OAuthClient.GenerateTimeStamp();
            String nonce = OAuthClient.GenerateNonce();
            Dictionary<String, String> pp = OAuthClient.GenerateParameters(ConsumerKey, token, timeStamp, nonce);
            foreach (KeyValuePair<string, string> p in queryString)
            {
                pp.Add(p.Key, p.Value);
            }
            //NOTE:NET2.0
            Uri u = new Uri(HttpClient.CreateQueryString(url, pp, OAuthClient.UrlEncode));
            SignatureInfo si = GenerateSignature(u, this.ConsumerKey, this.ConsumerSecret, token, tokenSecret
                , methodName.ToString().ToUpper(), timeStamp, nonce);
            pp.Add("oauth_signature", OAuthClient.UrlEncode(si.Signature));
            //NOTE:NET2.0
            HttpClient cl = new HttpClient(HttpClient.CreateQueryString(url, pp, HttpClient.UrlEncode));
            cl.MethodName = methodName;
            return cl;
        }
        private HttpClient CreateHttpClientRequestHeaderMode(HttpMethodName methodName, String url, String token, String tokenSecret, IDictionary<String, String> queryString)
        {
            String timeStamp = OAuthClient.GenerateTimeStamp();
            String nonce = OAuthClient.GenerateNonce();
            Dictionary<String, String> pp = OAuthClient.GenerateParameters(ConsumerKey, token, timeStamp, nonce);
            Uri u = new Uri(HttpClient.CreateQueryString(url, queryString, OAuthClient.UrlEncode));
            SignatureInfo si = GenerateSignature(u, this.ConsumerKey, this.ConsumerSecret, token, tokenSecret
                , methodName.ToString().ToUpper(), timeStamp, nonce);
            pp.Add("oauth_signature", OAuthClient.UrlEncode(si.Signature));
            //NOTE:NET2.0
            HttpClient cl = new HttpClient(HttpClient.CreateQueryString(url, queryString, HttpClient.UrlEncode));
            cl.MethodName = methodName;
            cl.Headers[HttpRequestHeader.Authorization] = this.CreateOAuthHeader(pp);
            return cl;
        }
        private String CreateOAuthHeader(IDictionary<String, String> parameters)
        {
            StringBuilder sb = new StringBuilder(512);

            sb.Append("OAuth ");
            foreach (string key in parameters.Keys)
            {
                sb.AppendFormat("{0}=\"{1}\",", key, parameters[key]);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerKey"></param>
        /// <param name="token"></param>
        /// <param name="timeStamp"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        public static Dictionary<String, String> GenerateParameters(String consumerKey, String token, String timeStamp, String nonce)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();
            result.Add("oauth_consumer_key", consumerKey);
            result.Add("oauth_signature_method", "HMAC-SHA1");
            result.Add("oauth_timestamp", timeStamp);
            result.Add("oauth_nonce", nonce);
            result.Add("oauth_version", "1.0");
            if (String.IsNullOrEmpty(token) == false)
            { result.Add("oauth_token", token); }
            return result;
        }
#if SILVERLIGHT
        public void SetDispatcher(System.Windows.Threading.Dispatcher dispatcher)
        {
            this.BeginInvoke = action => dispatcher.BeginInvoke(action);
        }
#endif
    }
}

