namespace ThinkAway.IO.Log
{
    /// <summary>
    /// 日志过滤器
    /// </summary>
    public class LogFilter
    {
        private string[] _tag;

        /// <summary>
        /// Tag 标记
        /// </summary>
        public string[] Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        private LogLevel _level;

        /// <summary>
        /// 输出级别
        /// </summary>
        public LogLevel Level
        {
            get { return _level; }
            set { _level = value; }
        }
    }
}
