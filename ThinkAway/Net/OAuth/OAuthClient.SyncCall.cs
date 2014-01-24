using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using ThinkAway.Net.Http;

namespace ThinkAway.Net.OAuth
{
    public partial class OAuthClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public AuthorizeInfo GetAuthorizeInfo()
        {
            String nonce = OAuthClient.GenerateNonce();
            String timestamp = OAuthClient.GenerateTimeStamp();
            String url = this.RequestTokenUrl;

            SignatureInfo si = OAuthClient.GenerateSignature(new Uri(url), this.ConsumerKey, this.ConsumerSecret
                , "", "", "GET", timestamp, nonce, OAuthClient.SignatureTypes.HMACSHA1);
            HttpClient cl = new HttpClient(String.Format("{0}?{1}&oauth_signature={2}", url, si.NormalizedRequestParameters, si.Signature));
            String result = cl.GetBodyText();
            //正規表現でoauth_token,oauth_token_secret取得
            AuthorizeInfo ai = new AuthorizeInfo();
            ai.AuthorizeUrl = String.Format("{0}?{1}", this.AuthorizeUrl, result);
            ai.RequestToken = this.GetMatchValue(RegexList.OAuthToken, result);
            ai.RequestTokenSecret = this.GetMatchValue(RegexList.OAuthTokenSecret, result);
            return ai;
        }
        private String GetMatchValue(Regex regex, String input)
        {
            Match m = regex.Match(input);
            if (m.Groups.Count > 1)
            {
                return m.Groups[1].Value;
            }
            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestToken"></param>
        /// <param name="requestTokenSecret"></param>
        /// <returns></returns>
        public AccessTokenInfo GetAccessToken(String requestToken, String requestTokenSecret)
        {
            String nonce = OAuthClient.GenerateNonce();
            String timestamp = OAuthClient.GenerateTimeStamp();
            String url = this.AccessTokenUrl;

            SignatureInfo si = OAuthClient.GenerateSignature(new Uri(url), this.ConsumerKey, this.ConsumerSecret
                , requestToken, requestTokenSecret, "POST", timestamp, nonce, OAuthClient.SignatureTypes.HMACSHA1);
            HttpClient cl = new HttpClient(String.Format("{0}?{1}&oauth_signature={2}"
                , url, si.NormalizedRequestParameters, si.Signature));
            String result = cl.GetBodyText();
            return AccessTokenInfo.Create(result, "oauth_token", "oauth_token_secret");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorizeInfo"></param>
        /// <param name="oauthVerifier"></param>
        /// <returns></returns>
        public AccessTokenInfo GetAccessToken(AuthorizeInfo authorizeInfo, String oauthVerifier)
        {
            AuthorizeInfo ai = authorizeInfo;
            SignatureInfo si = OAuthClient.GenerateSignature(new Uri(this.AccessTokenUrl), this.ConsumerKey, this.ConsumerSecret
                , ai.RequestToken, ai.RequestTokenSecret, "POST", OAuthClient.GenerateTimeStamp(), OAuthClient.GenerateNonce()
                , OAuthClient.SignatureTypes.HMACSHA1);
            HttpClient cl = new HttpClient(String.Format("{0}?{1}&oauth_verifier={2}&oauth_signature={3}", this.AccessTokenUrl
                , si.NormalizedRequestParameters, oauthVerifier, si.Signature));
            String s = cl.GetBodyText();
            return AccessTokenInfo.Create(s, "oauth_token", "oauth_token_secret");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <returns></returns>
        public HttpResponse GetResponse(HttpMethodName methodName, String url, String token, String tokenSecret)
        {
            return this.GetResponse(methodName, url, token, tokenSecret, new Dictionary<String, String>());
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
        public HttpResponse GetResponse(HttpMethodName methodName, String url, String token, String tokenSecret
            , IDictionary<String, String> queryString)
        {
            HttpClient cl = this.CreateHttpClient(methodName, url, token, tokenSecret, queryString);
            return cl.GetResponse();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <param name="bodyData"></param>
        /// <returns></returns>
        public HttpResponse GetResponse(HttpMethodName methodName, String url, String token, String tokenSecret
            , Byte[] bodyData)
        {
            HttpClient cl = this.CreateHttpClient(methodName, url, token, tokenSecret, new Dictionary<String, String>());
            return cl.GetResponse(bodyData);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        /// <returns></returns>
        public HttpWebResponse GetHttpWebResponse(HttpMethodName methodName, String url, String token, String tokenSecret)
        {
            return this.GetHttpWebResponse(methodName, url, token, tokenSecret, new Dictionary<String, String>(), new Dictionary<String, String>());
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
        public HttpWebResponse GetHttpWebResponse(HttpMethodName methodName, String url, String token, String tokenSecret
            , IDictionary<String, String> queryString)
        {
            return this.GetHttpWebResponse(methodName, url, token, tokenSecret, queryString, new Dictionary<String, String>());
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
        /// <returns></returns>
        public HttpWebResponse GetHttpWebResponse(HttpMethodName methodName, String url, String token, String tokenSecret
            , IDictionary<String, String> queryString, IDictionary<String, String> parameters)
        {
            Dictionary<String, String> d = new Dictionary<string, string>();
            HttpClient cl = this.CreateHttpClient(methodName, url, token, tokenSecret, queryString);
            foreach (KeyValuePair<string, string> p in parameters)
            {
                d[p.Key] = cl.UrlEncodeFunction(p.Value);
            }
            return cl.GetHttpWebResponse(d);
        }
    }
}
