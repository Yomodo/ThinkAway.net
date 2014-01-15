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
        /// �����Ϣ����Ϣ������,����ʹ�������Ϣ
        /// </summary>
        Info = 0,
        /// <summary>
        /// ���������Ϣ������ʹ�������Ϣ
        /// </summary>
        Debug,
        /// <summary>
        /// �������ʹ�������Ϣ
        /// </summary>
        Warn,
        /// <summary>
        /// �����������Ϣ
        /// </summary>
        Error
    }
}