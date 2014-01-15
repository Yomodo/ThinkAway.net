using System;

namespace ThinkAway.Plus.Modem
{
    public class ReceiveEventArgs : EventArgs
    {
        public SMSSendInfo SmsInfo { get; set; }

        public ReceiveEventArgs()
        {
            SmsInfo = new SMSSendInfo();
        }
    }
}
