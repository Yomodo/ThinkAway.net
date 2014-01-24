using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

namespace ThinkAway.Net.Sockets
{
    public sealed class SessionManager<TAppSession> 
        where TAppSession : IAppSession, new()
    {
        private const int Interval = 5000;

        private const int SessionTime = 5;

        private readonly Timer _timer;
        
        private readonly IDictionary _dictionary;

        /// <summary>
        /// 当用户成功登录时发生。
        /// </summary>
        public event EventHandler<SessionEventArgs> SessionStarted;

        private void OnSessionStarted(SessionEventArgs e)
        {
            EventHandler<SessionEventArgs> handler = SessionStarted;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// 当用户成功注销时发生。
        /// </summary>
        public event EventHandler<SessionEventArgs> SessionStoped;

        public void OnSessionStoped(SessionEventArgs e)
        {
            EventHandler<SessionEventArgs> handler = SessionStoped;
            if (handler != null) handler(this, e);
        }

        public SessionManager()
        {
            _dictionary = new Dictionary<string, AppSession>();

            _timer = new Timer(Interval);
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            ArrayList arrayList = new ArrayList(_dictionary.Keys);
            foreach (string key in arrayList)
            {
                TAppSession appSession = (TAppSession)_dictionary[key];
                TimeSpan timeSpan = DateTime.Now - appSession.LastActiveTime;

                if (timeSpan.Seconds > SessionTime)
                {
                    _dictionary.Remove(key);

                    SessionEventArgs sessionEventArgs = new SessionEventArgs();
                    sessionEventArgs.SessionKey = appSession.SessionKey;
                    OnSessionStoped(sessionEventArgs);
                }
            }
        }

        public TAppSession this[string sessionKey]
        {
            get { return (TAppSession)_dictionary[sessionKey]; }
        }

        public TAppSession[] Sessions
        {
            get
            {
                TAppSession[] sessions = new TAppSession[_dictionary.Count];
                _dictionary.Values.CopyTo(sessions, 0);
                return sessions;
            }
        }

        public int SessionCount
        {
            get { return _dictionary.Count; }
        }

        internal void Add(AppSocket appSocket)
        {
            TAppSession appSession = new TAppSession();
            appSession.AppSocket = appSocket;
            appSession.StartSession();
            _dictionary.Add(appSession.SessionKey,appSession);
            //
            SessionEventArgs sessionEventArgs = new SessionEventArgs();
            sessionEventArgs.SessionKey = appSession.SessionKey;
            OnSessionStarted(sessionEventArgs);
        }
    }
}
