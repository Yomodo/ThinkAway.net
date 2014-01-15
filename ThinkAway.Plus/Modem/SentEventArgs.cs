using System;

namespace ThinkAway.Plus.Modem
{
    public class SentEventArgs : EventArgs
    {
        public bool Result;
        public SMSSendInfo SmsInfo;

        public SentEventArgs()
        {
            Result = new bool();
            SmsInfo = new SMSSendInfo();
        }
    }
}
