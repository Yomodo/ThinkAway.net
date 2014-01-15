using System;

namespace ThinkAway.IO.Log
{
    /// <summary>
    /// LogLevel
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        /// <summary>
        /// 输出信息性消息、调试,警告和错误处理消息
        /// </summary>
        Info = 0,
        /// <summary>
        /// 输出调试信息、警告和错误处理消息
        /// </summary>
        Debug,
        /// <summary>
        /// 输出警告和错误处理消息
        /// </summary>
        Warn,
        /// <summary>
        /// 输出错误处理消息
        /// </summary>
        Error
    }
}