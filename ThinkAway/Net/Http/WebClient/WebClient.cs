using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
    public class WebClient : WebRequest
    {
        private static Socket _socket;
        
        public WebClient() :base(_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp))
        {
            
        }
        
        public void Connect(string host, int port)
        {
            IPAddress[] hostAddresses = System.Net.Dns.GetHostAddresses(host);
            _socket.Connect(hostAddresses, port);
        }

        public WebResponse GetResponse()
        {
            MemoryStream memoryStream = (MemoryStream) base.GetRequestStream();
            int send = _socket.Send(memoryStream.ToArray());
            Thread.Sleep(2000);
            WebResponse response = new WebResponse(_socket);
            return response;
        }
    }
}
