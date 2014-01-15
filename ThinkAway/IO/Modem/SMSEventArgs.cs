using System;

namespace ThinkAway.IO.Modem
{
    /// <summary>
    /// SMSEventArgs
    /// </summary>
    public class SMSEventArgs : EventArgs
    {
        /// <summary>
        /// SMSEventArgs
        /// </summary>
        /// <param name="smsEnity"></param>
        public SMSEventArgs(SMSEnity smsEnity)
        {
            SmsInfo = smsEnity;
        }
        /// <summary>
        /// SMSEnity
        /// </summary>
        public SMSEnity SmsInfo { get; private set; }
    }
}
