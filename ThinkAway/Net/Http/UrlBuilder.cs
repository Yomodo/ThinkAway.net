using System.Collections.Generic;
using System.Text;

namespace ThinkAway.Net.Http
{
    /// <summary>
    /// ���URL����
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
        /// ��Ӳ���
        /// </summary>
        public void Add(string key, string value)
        {
            Add(new UrlParameter(key, value));
        }
        /// <summary>
        /// �� Parameters ʵ��ת��Ϊָ���� URL ��ʽ
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