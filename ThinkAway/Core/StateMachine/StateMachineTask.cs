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
        /// ��һ��״̬
        /// </summary>
        public String NextStatus
        {
            get { return _nextStatus; }
            set { _nextStatus = value; }
        }

        /// <summary>
        /// ����
        /// </summary>
        public Action<IEventMessage> Action
        {
            get { return _action; }
        }
        /// <summary>
        /// ����
        /// </summary>
        private readonly Action<IEventMessage> _action;
        /// <summary>
        /// ����ָ����ί�� �� ִ�к����һ��״̬��ʼ�� ״̬������
        /// </summary>
        /// <param name="action"></param>
        /// <param name="nextStatus"></param>
        public StateMachineTask(Action<IEventMessage> action, String nextStatus)
        {
            //����
            _action = action;
            //��һ��״̬
            NextStatus = nextStatus;
        }
    }
}