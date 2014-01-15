using System;

namespace ThinkAway.Net.Sockets
{
    public interface IAppSession
    {
        /// <summary>
        /// SessionKey
        /// </summary>
        string SessionKey { get; }

        /// <summary>
        /// AppSocket
        /// </summary>
        AppSocket AppSocket { get;set; }

        /// <summary>
        /// Starts the session.
        /// </summary>
        void StartSession();

        /// <summary>
        /// Closes this session.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets or sets the last active time of the session.
        /// </summary>
        /// <value>
        /// The last active time.
        /// </value>
        DateTime LastActiveTime { get; set; }

        
    }
}
