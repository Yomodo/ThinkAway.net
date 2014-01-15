namespace ThinkAway.Core.Invoker
{
    /// <summary>
    /// 提供了一种基于事件消息模型的接口约束
    /// </summary>
    public interface IEventMessage
    {
        string EventMessage { get; set; }
    }
}