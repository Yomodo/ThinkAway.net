using System;
using System.Globalization;

namespace ThinkAway.Net.Sockets
{
	/// <summary>
	/// 客户端管理器
	/// </summary>
    public sealed class AppSession : IAppSession
	{
        private AppSocket _appSocket;

        #region Implementation of IAppSession

        /// <summary>
        /// SessionKey
        /// </summary>
        public string SessionKey
        {
            get
            {
                int hashCode = _appSocket.GetHashCode();
                return hashCode.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// 获取或设置服务器通与客户端通信时使用的套接字
        /// </summary>
        public AppSocket AppSocket
        {
            get { return _appSocket; }
            set { _appSocket = value; }
        }

        /// <summary>
        /// Starts the session.
        /// </summary>
        public void StartSession()
        {
            AppSocket.Received += OnAppSocketReceived;
        }

        /// <summary>
        /// Closes this session.
        /// </summary>
        public void Close()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the last active time of the session.
        /// </summary>
        /// <value>
        /// The last active time.
        /// </value>
        public DateTime LastActiveTime { get; set; }

        #endregion

        public AppSession()
        {

        }

	    internal AppSession(AppSocket appSocket)
        {
            AppSocket = appSocket;
        }


	    private void OnAppSocketReceived(object sender, ReceivedEventArgs e)
        {
            LastActiveTime = System.DateTime.Now;
        }
	}
}