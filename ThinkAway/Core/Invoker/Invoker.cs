namespace ThinkAway.Core.Invoker
{
    /// <summary>
    /// 提供了一种对调度着模式的简单实现
    /// </summary>
    public class Invoker
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IWorker _worker;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="worker"></param>
        public Invoker(IWorker worker)
        {
            // TODO: Complete member initialization
            this._worker = worker;
        }

        /// <summary>
        /// 触发
        /// </summary>
        /// <param name="intent"></param>
        public void Trigger(Intent intent)
        {
            _worker.Trigger(intent);
        }
    }
}
