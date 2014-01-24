using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ThinkAway.Net.Http
{
    /// <summary>
    /// HttpClient
    /// </summary>
    public class HttpClient
    {
        /// <summary>
        /// 默认HTTP头：UserAgent
        /// </summary>
        public string DefaultUserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";

        /// <summary>
        /// UserAgent
        /// </summary>
        public string UserAgent
        {
            get { return DefaultUserAgent; }
            set { DefaultUserAgent = value; }
        }

        /// <summary>
        /// 默认HTTP头：Accept
        /// </summary>
        public string DefaultAccept = "text/html, application/xhtml+xml, */*";

        public string Accept
        {
            get { return DefaultAccept; }
            set { DefaultAccept = value; }
        }

        /// <summary>
        /// 默认HTTP头：ContentType
        /// </summary>
        public static string DefaultContentType = "application/x-www-form-urlencoded";

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType
        {
            get { return DefaultContentType; }
            set { DefaultContentType = value; }
        }


        /// <summary>
        /// 空Cookie
        /// </summary>
        public static CookieContainer DefaultCookie = new CookieContainer(1000, 1000, 100000);

        /// <summary>
        /// Cookie
        /// </summary>
        public static CookieContainer Cookie
        {
            get { return DefaultCookie; }
            set { DefaultCookie = value; }
        }

        /// <summary>
        /// 读取Cookies
        /// </summary>
        /// <returns>成功与否</returns>
        internal void SetCookies(CookieContainer cookieContainer)
        {
            Cookie = cookieContainer;
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="host">主机名</param>
        /// <param name="port">端口</param>
        public void SetProxy(string host, int port)
        {
            System.Net.WebRequest.DefaultWebProxy = new WebProxy(host, port);
        }

        /// <summary>
        /// 使用默认代理
        /// </summary>
        public void ResetProxy()
        {
            System.Net.WebRequest.DefaultWebProxy = System.Net.WebRequest.GetSystemWebProxy();
        }


        public event Action<int, int> ProcessChange;

        protected virtual void OnProcessChange(int arg1, int arg2)
        {
            Action<int, int> handler = ProcessChange;
            if (handler != null) handler(arg1, arg2);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual byte[] RequestData(string url, string method,byte[] data)
        {
            List<byte> result = new List<byte>();
            HttpWebRequest request = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if (request != null)
            {
                request.Accept = DefaultAccept;
                request.Method = method;
                request.KeepAlive = true;
                request.AllowAutoRedirect = true;
                request.UserAgent = UserAgent;
                request.ContentType = DefaultContentType;
                request.CookieContainer = Cookie;
                if (!string.IsNullOrEmpty(""))
                {
                    request.Referer = "lsong.org";
                }
                if (data != null && data.Length > 0)
                {
                    request.ContentLength = data.Length;

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(data, 0, data.Length);
                    }
                }
                byte[] buffer = new byte[1024];
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response != null)
                    {
                        Stream responseStream = response.GetResponseStream();
                        int length, size = 0;
                        while (responseStream != null && (length = responseStream.Read(buffer,0,buffer.Length)) != 0)
                        {
                            for (int i = 0; i < length; i++)
                            {
                                result.Add(buffer[i]);
                            }
                            size += length;
                            OnProcessChange(length,size);
                        }
                    }
                }
            }
            return result.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public byte[] Get(string url)
        {
            return RequestData(url, HttpMethod.GET,  null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Post(string url,byte[] data)
        {
            return  RequestData(url, HttpMethod.POST,  data);
        }
    }

}
