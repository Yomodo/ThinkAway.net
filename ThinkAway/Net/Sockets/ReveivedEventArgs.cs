using System;

namespace ThinkAway.Net.Sockets
{
    /// <summary>
    /// 
    /// </summary>
    public class ReceivedEventArgs : EventArgs
    {
        private byte[] data;

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public ICustomProtocol Protocol;

        public string ClientKey { get; set; }
    }
}