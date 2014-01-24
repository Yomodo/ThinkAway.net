using System;
using System.Collections.Generic;

namespace ThinkAway.Net.OAuth
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessTokenInfo
    {
        private Dictionary<String, String> _values;
        private String _Token = "";
        private String _TokenSecret = "";
        /// <summary>
        /// 
        /// </summary>
        public String Token
        {
            get { return _Token; }
            set { _Token = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public String TokenSecret
        {
            get { return _TokenSecret; }
            set { _TokenSecret = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<String, String> Values
        {
            get { return _values; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tokenSecret"></param>
        public AccessTokenInfo(String token, String tokenSecret)
        {
            this.Token = token;
            this.TokenSecret = tokenSecret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tokenKey"></param>
        /// <param name="tokenSecretKey"></param>
        /// <returns></returns>
        public static AccessTokenInfo Create(String value, String tokenKey, String tokenSecretKey)
        {
            AccessTokenInfo t = new AccessTokenInfo("", "");
            String[] ss = value.Split('&');
            Dictionary<String, String> d = new Dictionary<string, string>();
            foreach (string t1 in ss)
            {
                if (t1.Contains("=") == false) { continue; }
                String[] sss = t1.Split('=');
                if (sss.Length == 2)
                {
                    d[sss[0].ToLower()] = sss[1];
                }
            }
            t._values = d;
            if (d.ContainsKey(tokenKey))
            {
                t.Token = d[tokenKey];
            }
            if (d.ContainsKey(tokenSecretKey))
            {
                t.TokenSecret = d[tokenSecretKey];
            }
            return t;
        }
   }
}
