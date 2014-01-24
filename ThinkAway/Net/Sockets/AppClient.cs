using System;
using System.Net.Sockets;

namespace ThinkAway.Net.Sockets
{
    public sealed class AppClient<TProtocol> where TProtocol : ICustomProtocol, new()
    {
        public event EventHandler<ReceivedEventArgs> Received;

        private void OnReceived(ReceivedEventArgs e)
        {
            EventHandler<ReceivedEventArgs> handler = Received;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<ConnectedEventArgs> Connected;

        private void OnConnected(ConnectedEventArgs e)
        {
            EventHandler<ConnectedEventArgs> handler = Connected;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DisconnectedEventArgs> Disconnected;

        private void OnDisconnected(DisconnectedEventArgs e)
        {
            EventHandler<DisconnectedEventArgs> handler = Disconnected;
            if (handler != null) handler(this, e);
        }

        private readonly AppSocket _appSocket;
        
        private readonly TProtocol _protocol;

        //private readonly ManualResetEvent _manualResetEvent;

        public AppClient()
        {
            _protocol = new TProtocol();

            //_manualResetEvent = new ManualResetEvent(false);
            
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _appSocket = new AppSocket(ref socket);
            _appSocket.Received += appSocket_Received;
            _appSocket.Disconnected += appSocket_Disconnected;
        }

        private void appSocket_Received(object sender, ReceivedEventArgs e)
        {
            OnReceived(e);
        }

        public void Connect(string host, int port)
        {
            
            Socket clientSocket = _appSocket.ClientSocket;
            clientSocket.BeginConnect(host, port, ConnectCallback, clientSocket);
           
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {

                Socket socket = (Socket) ar.AsyncState;
                socket.EndConnect(ar);
                OnConnected(new ConnectedEventArgs());
                _appSocket.ReceiveData();
            }
            catch (SocketException)
            {
                OnDisconnected(new DisconnectedEventArgs());
            }
        }

        private void appSocket_Disconnected(object sender, DisconnectedEventArgs e)
        {
            OnDisconnected(new DisconnectedEventArgs());
        }


        public void Send(TProtocol protocol)
        {
            //_manualResetEvent.WaitOne();
            byte[] data = protocol.ToBytes();
            _appSocket.SendData(data);
        }
    }
}