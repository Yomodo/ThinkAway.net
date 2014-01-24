using System.IO;
using System.Net.Sockets;
using System.Text;
using ThinkAway.Net.Sockets;

namespace ThinkAway.Net.Http
{
    public abstract class WebServerHandler : BinaryProtocol
    {

        public Encoding Encoding { get; set; }

        protected WebServerRequest Request { get; set; }

        protected WebServerResponse Response { get; set; }

        protected WebServerHandler()
        {
            Encoding = System.Text.Encoding.Default;
        }

        protected virtual void Post()
        {
            Response.Headers.ContentType = "text/html";
            Response.Headers.StatusCode = 200;
            //
            Response.Write("Hello World");
        }

        protected virtual void Get()
        {
            Response.Headers.ContentType = "text/html";
            Response.Headers.StatusCode = 200;
            //
            Response.Write("Hello World");
        }

        protected internal virtual void Callback(object obj)
        {
            Socket socket = obj as Socket;
            if (socket != null && socket.Connected)
            {
                Request = new WebServerRequest(socket);

                Response = new WebServerResponse(socket);

                switch (Request.Headers.Method)
                {
                    case "GET":
                        Get();
                        break;
                    case "POST":
                        Post();
                        break;
                }
                if (!socket.Connected)
                {
                    throw new SocketException();
                }
                byte[] bytes = ((MemoryStream)Response.GetResponseStream()).ToArray();
                int send = socket.Send(bytes, bytes.Length, SocketFlags.None);

                socket.Close();
            }
        }
    }
}
