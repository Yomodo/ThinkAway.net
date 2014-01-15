using System;
using System.Net.Sockets;

namespace ThinkAway.Net.Sockets
{
	/// <summary>
	/// 用户事件的数据
	/// </summary>
	public class SessionEventArgs : EventArgs
	{
        public string SessionKey { get; set; }
    }
}
