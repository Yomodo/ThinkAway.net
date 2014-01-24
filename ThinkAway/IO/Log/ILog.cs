using System;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.IO.Log
{
    /// <summary>
    /// Log API 日志通用访问接口
    /// </summary>
    public interface ILog : IDisposable
    {
        /// <summary>
        /// 日志配置项
        /// </summary>
        LogConfig Config { get; set; }

        /// <summary>
        /// Error Log
        /// </summary>
        /// <param name="log"></param>
        /// <param name="message"></param>
        /// <param name="tag"></param>
        void Error(string tag,string log,params object[] message);
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        void Error(string tag,Exception ex);
        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="log"></param>
        /// <param name="message"></param>
        void Warning(string tag, string log, params object[] message);
        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        void Warning(string tag, Exception ex);
        /// <summary>
        /// Info
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="log"></param>
        /// <param name="message"></param>
        void Info(string tag, string log, params object[] message);
        /// <summary>
        /// Info
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        void Info(string tag, Exception ex);
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="log"></param>
        /// <param name="message"></param>
        void Debug(string tag, string log, params object[] message);
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        void Debug(string tag, Exception ex);

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="tag"></param>
        /// <param name="log"></param>
        /// <param name="message"></param>
        void Log(LogLevel logLevel, string tag, string log, params object[] message);
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        void Log(LogLevel logLevel,string tag,Exception ex);
    }
}