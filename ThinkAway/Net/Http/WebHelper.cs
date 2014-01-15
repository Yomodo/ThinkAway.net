using System.Text;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Net.Http
{
	/// <summary>
	/// 网络连接基础类
	/// </summary>
	public class WebHelper : HttpClient
	{
	    private Encoding _encoding = Encoding.UTF8;

	    public Encoding Encode
	    {
            get { return _encoding; }
            set { _encoding = value; }
	    }

	    public string Get(string url)
        {
            byte[] bytes = base.Get(url);
            return Encode.GetString(bytes);
        }

        public string Post(string url,byte[] data)
        {
            byte[] bytes = base.Post(url,data);
            return Encode.GetString(bytes);
        }

        public string Get(UrlBuilder urlBuilder)
        {
            return this.Get(urlBuilder.ToString());
        }
    }
}
