using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ThinkAway.Net.OAuth
{
    public partial class OAuthClient
    {
        private const String UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
#if SILVERLIGHT
        private static readonly Encoding GenerateSignatureEncoding = Encoding.UTF8;
#else
        private static readonly Encoding GenerateSignatureEncoding = Encoding.GetEncoding("us-ascii");
#endif
        ///<summary>
        /// 
        ///</summary>
        public class RegexList
        {
            ///<summary>
            /// 
            ///</summary>
            public static readonly Regex OAuthToken = new Regex(@"oauth_token=([^&]*)");
            /// <summary>
            /// 
            /// </summary>
            public static readonly Regex OAuthTokenSecret = new Regex(@"oauth_token_secret=([^&]*)");
            /// <summary>
            /// 
            /// </summary>
            public static readonly Regex OAuthCallback = new Regex(@"oauth_callback=([^&]*)");
        }
        /// <summary>
        /// 
        /// </summary>
        public class Key
        {
            /// <summary>
            /// 
            /// </summary>
            public static readonly String OAuthVersionNo = "1.0";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String OAuthParameterPrefix = "oauth_";
            /// <summary>
            /// List of know and used oauth parameters' names 
            /// </summary>
            public static readonly String OAuthConsumerKey = "oauth_consumer_key";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String OAuthCallback = "oauth_callback";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String OAuthVersion = "oauth_version";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String OAuthSignatureMethod = "oauth_signature_method";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String OAuthSignature = "oauth_signature";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String OAuthTimestamp = "oauth_timestamp";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String OAuthNonce = "oauth_nonce";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String OAuthToken = "oauth_token";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String OAuthTokenSecret = "oauth_token_secret";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String HMACSHA1SignatureType = "HMAC-SHA1";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String PlainTextSignatureType = "PLAINTEXT";
            /// <summary>
            /// 
            /// </summary>
            public static readonly String RSASHA1SignatureType = "RSA-SHA1";
        }
        private static readonly Random Random = new Random();
        /// <summary>
        /// Internal function to cut out all non oauth query String parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query String part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        protected static List<QueryParameter> GetQueryParameters(String parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            List<QueryParameter> result = new List<QueryParameter>();

            if (!String.IsNullOrEmpty(parameters))
            {
                String[] p = parameters.Split('&');
                foreach (String s in p)
                {
                    if (!String.IsNullOrEmpty(s) && !s.StartsWith(Key.OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            String[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, String.Empty));
                        }
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a String representing the normalized parameters</returns>
        protected static String NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < parameters.Count; i++)
            {
                QueryParameter p = parameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty String</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty String</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce"></param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <param name="timeStamp"></param>
        /// <returns>A base64 String of the hash value</returns>
        public static SignatureInfo GenerateSignature(Uri url, String consumerKey, String consumerSecret, String token, String tokenSecret, String httpMethod, String timeStamp, String nonce, SignatureTypes signatureType)
        {
            SignatureInfo si = new SignatureInfo();
            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    si.Signature = OAuthClient.UrlEncode(String.Format("{0}&{1}", consumerSecret, tokenSecret));
                    return si;
                case SignatureTypes.HMACSHA1:
                    si = GenerateSignatureBase(url, consumerKey, token, httpMethod, timeStamp, nonce, Key.HMACSHA1SignatureType);
                    HMACSHA1 hs = new HMACSHA1();
                    hs.Key = OAuthClient.GenerateSignatureEncoding.GetBytes(String.Format("{0}&{1}", OAuthClient.UrlEncode(consumerSecret), String.IsNullOrEmpty(tokenSecret) ? "" : OAuthClient.UrlEncode(tokenSecret)));
                    si.Signature = GenerateSignatureUsingHash(si.Signature, hs);
                    return si;
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
            }
            throw new ArgumentException("Unknown signature type", "signatureType");
        }

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>        
        /// <param name="token">The token, if available. If not available pass null or an empty String</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce"></param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="OAuthClient.SignatureTypes">OAuthClient.SignatureTypes</see>.</param>
        /// <param name="timeStamp"></param>
        /// <returns>The signature base</returns>
        public static SignatureInfo GenerateSignatureBase(Uri url, String consumerKey, String token, String httpMethod, String timeStamp, String nonce, String signatureType)
        {
            SignatureInfo si = new SignatureInfo();

            if (token == null)
            {
                token = String.Empty;
            }
            if (timeStamp == null) throw new ArgumentNullException("timeStamp");

            if (String.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }

            if (String.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("httpMethod");
            }

            if (String.IsNullOrEmpty(signatureType))
            {
                throw new ArgumentNullException("signatureType");
            }

            List<QueryParameter> parameters = OAuthClient.GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(Key.OAuthVersion, Key.OAuthVersionNo));
            parameters.Add(new QueryParameter(Key.OAuthNonce, nonce));
            parameters.Add(new QueryParameter(Key.OAuthTimestamp, timeStamp));
            parameters.Add(new QueryParameter(Key.OAuthSignatureMethod, signatureType));
            parameters.Add(new QueryParameter(Key.OAuthConsumerKey, consumerKey));

            if (!String.IsNullOrEmpty(token))
            {
                parameters.Add(new QueryParameter(Key.OAuthToken, token));
            }

            parameters.Sort(new QueryParameterComparer());

            si.NormalizedUrl = String.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                si.NormalizedUrl += ":" + url.Port;
            }
            si.NormalizedUrl += url.AbsolutePath;
            si.NormalizedRequestParameters = NormalizeRequestParameters(parameters);

            StringBuilder sb = new StringBuilder(1000);
            sb.AppendFormat("{0}&", httpMethod.ToUpper());
            sb.AppendFormat("{0}&", OAuthClient.UrlEncode(si.NormalizedUrl));
            sb.AppendFormat("{0}", OAuthClient.UrlEncode(si.NormalizedRequestParameters));
            si.Signature = sb.ToString();
            return si;
        }
        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 String of the hash value</returns>
        public static String GenerateSignatureUsingHash(String signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }
        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 String of the hash value</returns>
        public static String ComputeHash(HashAlgorithm hashAlgorithm, String data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (String.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            byte[] dataBuffer = OAuthClient.GenerateSignatureEncoding.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }
        /// <summary>
        /// OAuth用のUrlEncode。OAuthのUrlEncodeは大文字でなければならない。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String UrlEncode(String value)
        {
            StringBuilder result = new StringBuilder();
            Encoding encode = Encoding.UTF8;
            Byte[] data = encode.GetBytes(value);
            int len = data.Length;

            for (int i = 0; i < len; i++)
            {
                int c = data[i];
                if (c < 0x80 && UnreservedChars.IndexOf((char)c) != -1)
                {
                    result.Append((char)c);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)data[i]));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty String</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty String</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="timeStamp"></param>
        /// <param name="nonce"></param>
        /// <returns>A base64 String of the hash value</returns>
        public static SignatureInfo GenerateSignature(Uri url, String consumerKey, String consumerSecret, String token, String tokenSecret, String httpMethod, String timeStamp, String nonce)
        {
            return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1);
        }
        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        private static String GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static String GenerateNonce()
        {
            return GenerateNonce1();
        }
        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        protected static String GenerateNonce0()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return Random.Next(123400, 9999999).ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static String GenerateNonce1()
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder result = new StringBuilder(8);
            Random random = new Random();
            for (int i = 0; i < 8; ++i)
            {
                result.Append(letters[random.Next(letters.Length)]);
            }
            return result.ToString();
        }
    }
}

