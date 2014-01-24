using System;
using System.Collections.Generic;
using System.Net;
using ThinkAway.Net.Http;

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
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <param name="callback"></param>
        public void GetResponse(HttpMethodName methodName, String url, String token, String tokenSecret, Action<HttpResponse> callback)
        {
            HttpClient cl = this.CreateHttpClient(methodName, url, token, tokenSecret, new Dictionary<String, String>());
            cl.GetResponse(callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <param name="queryString"></param>
        /// <param name="callback"></param>
        public void GetResponse(HttpMethodName methodName, String url, String token, String tokenSecret
            , IDictionary<String, String> queryString, Action<HttpResponse> callback)
        {
            HttpClient cl = this.CreateHttpClient(methodName, url, token, tokenSecret, queryString);
            cl.GetResponse(callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <param name="queryString"></param>
        /// <param name="parameters"></param>
        /// <param name="callback"></param>
        public void GetResponse(HttpMethodName methodName, String url, String token, String tokenSecret
            , IDictionary<String, String> queryString, IDictionary<String, String> parameters, Action<HttpResponse> callback)
        {
            Dictionary<String, String> d = null;
            HttpClient cl = this.CreateHttpClient(methodName, url, token, tokenSecret, queryString);
            if (parameters != null)
            {
                d = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> p in parameters)
                {
                    d[p.Key] = cl.UrlEncodeFunction(p.Value);
                }
            }
            cl.GetResponse(d, callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <param name="bodyData"></param>
        /// <param name="callback"></param>
        public void GetResponse(HttpMethodName methodName, String url, String token, String tokenSecret
            , Byte[] bodyData, Action<HttpResponse> callback)
        {
            HttpClient cl = this.CreateHttpClient(methodName, url, token, tokenSecret, new Dictionary<String, String>());
            cl.GetResponse(bodyData, callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <param name="queryString"></param>
        /// <param name="callback"></param>
        public void GetHttpWebResponse(HttpMethodName methodName, String url, String token, String tokenSecret
            , IDictionary<String, String> queryString, Action<HttpWebResponse> callback)
        {
            this.GetHttpWebResponse(methodName, url, token, tokenSecret, queryString, null, callback);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <param name="queryString"></param>
        /// <param name="parameters"></param>
        /// <param name="callback"></param>
        public void GetHttpWebResponse(HttpMethodName methodName, String url, String token, String tokenSecret
            , IDictionary<String, String> queryString, IDictionary<String, String> parameters, Action<HttpWebResponse> callback)
        {
            Dictionary<String, String> d = null;
            HttpClient cl = this.CreateHttpClient(methodName, url, token, tokenSecret, queryString);
            if (parameters != null)
            {
                d = new Dictionary<string, string>();
                cl.ContentType = HttpClient.ApplicationFormUrlEncoded;
                foreach (KeyValuePair<string, string> p in parameters)
                {
                    d[p.Key] = cl.UrlEncodeFunction(p.Value);
                }
            }
            cl.GetHttpWebResponse(d, callback);
        }
    }
}

