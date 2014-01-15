using System.Collections;
using System.IO;
using System.Net.Sockets;

namespace ThinkAway.Net.Http
{
    /// <summary>
    /// WebServerResponse
    /// </summary>
    public class WebResponse
    {
        internal Socket SocketHandler { get; set; }

        private MemoryStream _memoryStream;
        /// <summary>
        /// 响应头
        /// </summary>
        public RequestHeader Headers { get; set; }

        /// <summary>
        /// WebServerResponse
        /// </summary>
        protected internal WebResponse(Socket handle)
        {
            SocketHandler = handle;

            Headers = new RequestHeader();
        }
        
        public virtual Stream GetResponseStream()
        {
            if(_memoryStream == null)
            {
                int receive;
                ArrayList arrayList = new ArrayList();
                byte[] buffer = new byte[1024];
                do
                {
                    receive = SocketHandler.Receive(buffer);
                    for (int i = 0; i < receive; i++)
                    {
                        arrayList.Add(buffer[i]);
                    }

                } while (receive != 0);
                byte[] array = (byte[])arrayList.ToArray(typeof(byte));
                _memoryStream =  new MemoryStream(array, false);
            }

            return _memoryStream;
        }
    }
}
