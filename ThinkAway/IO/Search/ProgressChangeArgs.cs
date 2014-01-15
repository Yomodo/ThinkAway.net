namespace ThinkAway.IO.Search
{
    /// <summary>
    /// 文件搜索进度发生改变
    /// </summary>
    public class ProgressChangeArgs
    {
        /// <summary>
        /// %
        /// </summary>
        public int Percent;
        /// <summary>
        /// Value
        /// </summary>
        public long Value;
        /// <summary>
        /// Length
        /// </summary>
        public long Length;
    }
}
