using System.IO;
using System.Net.Sockets;

namespace ThinkAway.Net.Http
{
    /// <summary>
    /// Web ·þÎñÇëÇó
    /// </summary>
    public class WebRequest
    {
        private MemoryStream _memoryStream;

        internal Socket SocketHandler { get; set; }

        public RequestHeader Headers { get; set; }

        public string RequestUrl
        {
            get { return string.Format("{0}{1}", Headers.Host, Headers.RequestUrl); }
        }

        protected internal WebRequest(Socket handle)
        {
            SocketHandler = handle;

            Headers = new RequestHeader();
        }

        public virtual Stream GetRequestStream()
        {
            MemoryStream memoryStream = new MemoryStream();
            if(_memoryStream == null)
            {
                _memoryStream = new MemoryStream();
            }
            else
            {
                byte[] bytes = System.Text.Encoding.GetEncoding(Headers.Encoding).GetBytes(Headers.ToString());
                memoryStream.Write(bytes,0,bytes.Length);
                byte[] array = _memoryStream.ToArray();
                memoryStream.Write(array, 0, array.Length);
            }
            return memoryStream;
        }
    }
}