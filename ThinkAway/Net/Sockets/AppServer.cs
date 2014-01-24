using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ThinkAway.IO.Log;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Net.Sockets
{
    public class AppServer : AppServer<AppSession,BinaryProtocol>
    {
        public AppServer(int port) : base(port)
        {
        }
    }

    public class AppServer<TProtocol> : AppServer<AppSession, TProtocol> 
        where TProtocol : ICustomProtocol, new()
    {
        public AppServer(int port) : base(port)
        {
        }
    }

    public class AppServer<TAppSession,TProtocol>  
        where TAppSession : IAppSession, new() 
        where TProtocol : ICustomProtocol, new()
    {
        private readonly ILog log;

        private Socket _listener;

        private bool _listenerRun;

        private readonly Thread _thread;

        private readonly TProtocol _protocol;

        public const int MAX_SESSION_COUNT = 0x64;

        private readonly Queue<object> _messageQueue;

        private readonly IDictionary<string, object> _waitQueue;

        private readonly ManualResetEvent _manualResetEvent;

        private readonly SessionManager<TAppSession> _sessionsManager;

        public TAppSession[] Sessions
        {
            get { return _sessionsManager.Sessions; }
        }

        public event EventHandler<ReceivedEventArgs> Received;

        protected virtual void OnReceived(ReceivedEventArgs e)
        {
            EventHandler<ReceivedEventArgs> handler = Received;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<ConnectedEventArgs> Connected;

        protected virtual void OnConnected(ConnectedEventArgs e)
        {
            EventHandler<ConnectedEventArgs> handler = Connected;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DisconnectedEventArgs> Disconnected;

        protected virtual void OnDisconnected(DisconnectedEventArgs e)
        {
            EventHandler<DisconnectedEventArgs> handler = Disconnected;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// AppServer
        /// </summary>
        /// <param name="port"></param>
        public AppServer(int port)
        {
            log = new Logger();

            log.Config.Mode = LogMode.Console;

            _protocol = new TProtocol();

            _messageQueue = new Queue<object>();

            _waitQueue = new Dictionary<string, object>();

            _thread = new Thread(DequeueCallback, 0);

            _manualResetEvent = new ManualResetEvent(false);

            _sessionsManager = new SessionManager<TAppSession>();
            _sessionsManager.SessionStarted += OnSessionStarted;
            _sessionsManager.SessionStoped += OnSessionStoped;

            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(ipEndPoint);

            log.Debug("log", "AppServer Started.");
        }



        public void StartListener()
        {
            _listenerRun = true;

            _thread.Start();

            _listener.Listen(50);

            log.Debug("log", "AppServer Listening .");

            _listener.BeginAccept(AcceptCallBack, _listener);

            log.Debug("log", "AppServer BeginAccept .");
        }

        public void StopListener()
        {
            _listenerRun = false;
            //_listener.Shutdown(SocketShutdown.Both);
            //_listener.Disconnect(false);
            _listener.Close();
            _listener = null;
        }

        protected virtual void OnSessionStarted(object sender, SessionEventArgs e)
        {
            TAppSession appSession = _sessionsManager[e.SessionKey];
            AppSocket appSocket = appSession.AppSocket;
            appSocket.ReceiveData();

            log.Debug("log", "_sessionsManager_SessionStarted:{0}", e.SessionKey);
        }

        protected virtual void OnSessionStoped(object sender, SessionEventArgs e)
        {
            log.Debug("log", "_sessionsManager_SessionStoped:{0}", e.SessionKey);
        }

        private void AcceptCallBack(IAsyncResult asyncResult)
        {
            if (_listenerRun)
            {
                Socket socket = (Socket)asyncResult.AsyncState;
                Socket client = socket.EndAccept(asyncResult);
                //
                SocketAcceptCallBack(client);
                //
                if (_listenerRun && _sessionsManager.SessionCount < MAX_SESSION_COUNT)
                {
                    _listener.BeginAccept(AcceptCallBack, _listener);
                }
            }
        }

        protected virtual void SocketAcceptCallBack(Socket client)
        {
            AppSocket appSocket = new AppSocket(ref client);
            appSocket.Received += OnAppSocketReceived;
            appSocket.Disconnected += OnAppSocketDisconnected;
            _sessionsManager.Add(appSocket);
        }

        protected virtual void OnAppSocketDisconnected(object sender, DisconnectedEventArgs e)
        {
            SessionEventArgs sessionEventArgs = new SessionEventArgs();
            int hashCode = sender.GetHashCode();
            string key = hashCode.ToString(CultureInfo.InvariantCulture);
            sessionEventArgs.SessionKey = key;
            _sessionsManager.OnSessionStoped(sessionEventArgs);
        }

        protected virtual void OnAppSocketReceived(object sender, ReceivedEventArgs e)
        {
            _messageQueue.Enqueue(e);
            if (_messageQueue.Count == 1)
            {
                _manualResetEvent.Set();
            }
        }

        private void DequeueCallback(object state)
        {
            while (_listenerRun)
            {
                lock (_messageQueue)
                {
                    if (!_listenerRun)
                    {
                        return;
                    }
                    if(_messageQueue.Count > 0)
                    {
                        object dequeue = _messageQueue.Dequeue();
                        ReceivedEventArgs receivedEventArgs = ((ReceivedEventArgs)dequeue);
                        byte[] bytes = receivedEventArgs.Data;
                        receivedEventArgs.Protocol = _protocol.FromBytes(bytes);

                        OnReceived(receivedEventArgs);
                    }
                    else
                    {
                        _manualResetEvent.WaitOne();
                    }
                }
            }
        }
    }
}