using System.Collections.Generic;
using System.Linq;

namespace ThinkAway.Plus.Modem
{
    /// <summary>
    /// SMS Queue
    /// </summary>
    class SMSQueue
    {
        private static SMSQueue _instance;

        /// <summary>
        /// SMS Queue
        /// </summary>
        readonly Dictionary<string, List<SMSSendInfo>> _dictionary;

        /// <summary>
        /// Instance
        /// </summary>
        public static SMSQueue Instance
        {
            get { return _instance ?? (_instance = new SMSQueue()); }
        }


        SMSQueue()
        {
            _dictionary = new Dictionary<string, List<SMSSendInfo>>();
        }
        internal static SMSSendInfo GetSendMessage(string Com)
        {
            return SMSQueue.Instance.GetSend(Com);
        }

        private SMSSendInfo GetSend(string Com)
        {
            SMSSendInfo smsInfo = null;
            if (_dictionary.ContainsKey(Com))
            {
                smsInfo = _dictionary[Com].FirstOrDefault();
                if (smsInfo != null)
                    _dictionary[Com].Remove(smsInfo);
            }
            return smsInfo;
        }

        internal static void AddSendMessage(SMSSendInfo sendInfo)
        {
            SMSQueue.Instance.AddSend(sendInfo);
        }

        private void AddSend(SMSSendInfo sendInfo)
        {
            if (_dictionary.ContainsKey(sendInfo.Com))
            {
                _dictionary[sendInfo.Com].Add(sendInfo);
            }
            else
            {
                _dictionary.Add(sendInfo.Com, new List<SMSSendInfo> { sendInfo });
            }
        }
        private bool CheckQueueHasSMS(string com)
        {
            return _dictionary[com].Count > 0;
        }

        internal static void AddReceivedMessage(SMSSendInfo smsInfo)
        {
            //throw new System.NotImplementedException();
        }

        internal static bool CheckQueue(string Com)
        {
            return SMSQueue.Instance.CheckQueueHasSMS(Com);
        }
    }
}
