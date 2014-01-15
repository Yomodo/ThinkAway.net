namespace ThinkAway.Core.Invoker
{
    /// <summary>
    /// 简单工作者的接口约束
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// 执行工作任务
        /// </summary>
        /// <param name="intent"></param>
        void Trigger(Intent intent);
    }
}
