using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace ThinkAway.Net.Http
{
    /// <summary>
    /// 请求的数据头
    /// </summary>
    public class RequestHeader : NameValueCollection
    {
        public string Method { get; set; }

        public string RequestUrl { get; set; }
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName
        {
            get { return base["Server"]; }
            set { base["Server"] = value; }
        }
        /// <summary>
        /// HTTP 协议版本
        /// </summary>
        public string HttpVersion { get; set; }
        /// <summary>
        /// 响应状态码
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// MIME 类型
        /// </summary>
        public string ContentType
        {
            get { return base["Content-Type"]; }
            set { base["Content-Type"] = value; }
        }
        /// <summary>
        /// 内容长度
        /// </summary>
        public int ContentLength
        {
            get { return Convert.ToInt32(base["ContentLength"]); }
            set { base["ContentLength"] = value.ToString(CultureInfo.InvariantCulture); }
        }

        public string Encoding
        {
            get { return base["Encoding"]; }
            set { base["Encoding"] = value; }
        }

        public string Location
        {
            get { return base["Location"]; }
            set { base["Location"] = value; }
        }

        public string Host
        {
            get { return base["Host"]; }
            set { base["Host"] = value; }
        }


        internal RequestHeader()
        {
            Encoding = "UTF-8";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("{0} {1} {2}", Method, RequestUrl,HttpVersion, StatusCode));
            foreach (string key in base.Keys)
            {
                stringBuilder.AppendLine(string.Format("{0}:{1}", key, base[key]));
            }
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }
    }
}
