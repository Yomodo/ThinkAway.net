using System;

namespace ThinkAway.IO.Log
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum LogMode
    {
        /// <summary>
        /// ����̨���
        /// </summary>
        Console = 1,
        /// <summary>
        /// �ı����
        /// </summary>
        Text = 2,
        /// <summary>
        /// �¼���־
        /// </summary>
        EventLog = 4,
        
    }
}