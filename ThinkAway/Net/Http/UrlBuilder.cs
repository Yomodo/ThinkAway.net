using System.Collections.Generic;
using System.Text;

namespace ThinkAway.Net.Http
{
    /// <summary>
    /// 多个URL参数
    /// </summary>
    public class UrlBuilder : List<UrlParameter>
    {
        private readonly string _url;
        /// <summary>
        /// Parameters
        /// </summary>
        public UrlBuilder(){}
        /// <summary>
        /// Parameters
        /// </summary>
        /// <param name="url"></param>
        public UrlBuilder(string url)
        {
            _url = url;
        }
        /// <summary>
        /// 添加参数
        /// </summary>
        public void Add(string key, string value)
        {
            Add(new UrlParameter(key, value));
        }
        /// <summary>
        /// 将 Parameters 实例转换为指定的 URL 格式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if(!string.IsNullOrEmpty(_url))
            {
                sb.AppendFormat("{0}?", _url);
            }
            for (int index = 0; index < this.Count; index++)
            {
                sb.Append(this[index].ToString());
                if ((index + 1) != this.Count)
                {
                    sb.Append("&");
                }
            }
            return sb.ToString();
        }
    }
}