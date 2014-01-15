using ThinkAway.Net.Sockets;

namespace ThinkAway.Net.Http
{
    public class WebServer : AppServer
    {
        private int _port = 80;

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public string Host { get; set; }

        public bool Listening { get; set; }

        private readonly WebServerHandler _handler;

        public WebServer(int port, WebServerHandler handler)
            : base(port)
        {
            Host = "127.0.0.1";
            _handler = handler;
        }

    }
}