using System;
using System.Globalization;
using System.Net.Sockets;

namespace ThinkAway.Net.Sockets
{
    /// <summary>
    /// 用来封装套接字
    /// 提供给clientmanager一个完备的套接字来调用
    /// </summary>
    public sealed class AppSocket : IDisposable
    {
        #region 私有变量
        /// <summary>
        /// 表示客户端的套接字
        /// </summary>
        private Socket _clientSocket;

        public Socket ClientSocket
        {
            get { return _clientSocket; }
        }

        private int _bufferSie = 16384;

        public int BufferSie 
        {
            get { return _bufferSie; }
            set { _bufferSie = value; }
        }

        /// <summary>
        /// 接受缓冲区
        /// </summary>
        private byte[] _receiveBuffer;

        #endregion 私有变量
        
        /// <summary>
        /// 接受到用户发送来的包
        /// </summary>
        public event EventHandler<ReceivedEventArgs> Received;

        private void OnReceived(ReceivedEventArgs e)
        {
            int hashCode = this.GetHashCode();
            EventHandler<ReceivedEventArgs> handler = Received;
            e.ClientKey = hashCode.ToString(CultureInfo.InvariantCulture);
            if (handler != null) handler(this, e);
        }

        public event EventHandler<DisconnectedEventArgs> Disconnected;

        private void OnDisconnected(DisconnectedEventArgs e)
        {
            EventHandler<DisconnectedEventArgs> handler = Disconnected;
            if (handler != null) handler(this, e);
        }

        public AppSocket(ref Socket clientSocket)
        {
            _clientSocket = clientSocket;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void SendData(byte[] data)
        {
            if (_clientSocket.Connected)
            {
                _clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, AsyncSend, null);
            }
            else
            {
                OnDisconnected(new DisconnectedEventArgs());
            }
        }

        /// <summary>
        /// 接受数据
        /// </summary>
        public void ReceiveData()
        {
            _receiveBuffer = new byte[BufferSie];
            _clientSocket.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, AsyncReceive, null);
        }


        /// <summary>
        /// 异步接受数据
        /// </summary>
        /// <param name="result"></param>
        private void AsyncReceive(IAsyncResult result)
        {
            try
            {
                if (!_clientSocket.Connected)
                {
                    throw new SocketException(10054);
                }

                int readbyte = _clientSocket.EndReceive(result);
                if (readbyte == 0) //未收到数据
                    return;
                byte[] data = new byte[readbyte];
                for (int i = 0; i < readbyte; i++)
                {
                    data[i] = _receiveBuffer[i];
                }
                ReceivedEventArgs receivedEventArgs = new ReceivedEventArgs();
                receivedEventArgs.Data = data;
                OnReceived(receivedEventArgs);

                ReceiveData();
            }
            catch (SocketException exception)
            {
                switch (exception.ErrorCode)
                {
                        //客户端断开
                    case 10054:
                        Dispose();
                        break;
                    default:
                        System.Console.WriteLine("Socket错误:{0}", exception);
                        break;
                }
            }
        }

        /// <summary>
        /// 发送命令完成
        /// </summary>
        /// <param name="result"></param>
        private void AsyncSend(IAsyncResult result)
        {
            _clientSocket.EndSend(result);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (_clientSocket.Connected)
            {
                _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();
                _clientSocket = null;
            }
            DisconnectedEventArgs disconnectedEventArgs = new DisconnectedEventArgs();
            OnDisconnected(disconnectedEventArgs);
        }

    }
}
