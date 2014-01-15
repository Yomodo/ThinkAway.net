using System.Collections.Generic;

namespace ThinkAway.Plus.Modem
{
    interface ISMSQueue
    {
        /// <summary>
        /// check com has message.
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        bool CheckQueue(string com);
        /// <summary>
        /// add send message to queue.
        /// </summary>
        /// <param name="smsInfo"></param>
        long Add(SMSSendInfo smsInfo);

        /// <summary>
        /// Change State
        /// </summary>
        /// <param name="sendInfo"></param>
        /// <param name="state"></param>
        void Set(SMSSendInfo sendInfo, int state);
        /// <summary>
        /// get send message from com with queue.
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        List<SMSSendInfo> Get(string com);
    }
}
