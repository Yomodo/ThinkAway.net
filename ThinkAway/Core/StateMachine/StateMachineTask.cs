using System;
using ThinkAway.Core.Invoker;

namespace ThinkAway.Core.StateMachine
{
    /// <summary>
    /// 
    /// </summary>
    public class StateMachineTask
    {
        private string _nextStatus;

        /// <summary>
        /// 下一个状态
        /// </summary>
        public String NextStatus
        {
            get { return _nextStatus; }
            set { _nextStatus = value; }
        }

        /// <summary>
        /// 动作
        /// </summary>
        public Action<IEventMessage> Action
        {
            get { return _action; }
        }
        /// <summary>
        /// 动作
        /// </summary>
        private readonly Action<IEventMessage> _action;
        /// <summary>
        /// 根据指定的委托 和 执行后的下一个状态初始化 状态机任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="nextStatus"></param>
        public StateMachineTask(Action<IEventMessage> action, String nextStatus)
        {
            //动作
            _action = action;
            //下一个状态
            NextStatus = nextStatus;
        }
    }
}