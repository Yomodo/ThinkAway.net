using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ThinkAway.Net.Http
{
    public class WebServerResponse : WebResponse
    {
        private MemoryStream _memoryStream;
        private MemoryStream _memoryStreamData;


        public WebServerResponse(Socket handle) : base(handle)
        {
        }

        public override Stream GetResponseStream()
        {
            if(_memoryStreamData == null)
            {
                _memoryStream = new MemoryStream();

                _memoryStreamData = new MemoryStream();
                //
                byte[] data = _memoryStreamData.ToArray();
                //
                byte[] bytes = Encoding.GetEncoding(Headers.Encoding).GetBytes(Headers.ToString());
                //
                _memoryStream.Write(bytes, 0, bytes.Length);
                _memoryStream.Write(data, 0, data.Length);
                //
            }
            return _memoryStream;
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="str"></param>
        public void Write(string str)
        {
            Encoding encoding = Encoding.GetEncoding(Headers.Encoding);
            byte[] bytes = encoding.GetBytes(str);
            Write(bytes);
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data)
        {
            _memoryStreamData.Write(data, 0, data.Length);
        }


        public void Redirect(string url)
        {
            Headers.Location = url;
            Headers.StatusCode = 302;
        }
    }
}
