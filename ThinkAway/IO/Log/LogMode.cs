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
        /// 控制台输出
        /// </summary>
        Console = 1,
        /// <summary>
        /// 文本输出
        /// </summary>
        Text = 2,
        /// <summary>
        /// 事件日志
        /// </summary>
        EventLog = 4,
        
    }
}