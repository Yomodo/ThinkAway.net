using System;
using System.Collections.Generic;
using ThinkAway.Core.Invoker;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Core.StateMachine
{
    /// <summary>
    /// 提供一种有限状态机模型的简单实现
    /// 表示有限个状态以及在这些状态之间的转移和动作等行为的数学模型.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// 状态机的状态
        /// </summary>
        public String Status;
        
        /// <summary>
        /// 状态转移表
        /// </summary>
        private readonly Dictionary<KeyValuePair<string, string>, StateMachineTask> _tTaskTable;

        /// <summary>
        /// 根据指定的状态初始化状态机
        /// </summary>
        /// <param name="initState"></param>
        public StateMachine(String initState)
        {
            Status = initState;
            _tTaskTable = new Dictionary<KeyValuePair<string, string>, StateMachineTask>();
        }
        /// <summary>
        /// 同步触发器
        /// </summary>
        /// <param name="transfer"></param>
        public void SyncTrigger(IEventMessage transfer)
        {
            //执行任务
            RunTask(transfer);
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="state"></param>
        /// <param name="eventMessage"></param>
        /// <param name="action"></param>
        /// <param name="nextState"></param>
        public void AddTask(String state, String eventMessage, Action<IEventMessage> action, String nextState)
        {
            //定义状态-事件的键值对
            KeyValuePair<string, string> stateEventPair = new KeyValuePair<string, string>(state, eventMessage);
            //定义一个任务
            StateMachineTask task = new StateMachineTask(action, nextState);
            //压入任务表（状态转移表）
            _tTaskTable.Add(stateEventPair, task);

        }
        /// <summary>
        /// RunTask
        /// </summary>
        /// <param name="transfer"></param>
        private void RunTask(IEventMessage transfer)
        {
            String eventMessage = transfer.EventMessage;                               //事件消息

            KeyValuePair<string, string> taskEventPair = new KeyValuePair<string, string>(Status, eventMessage);//事件-传输子对

            //判断是否存在任务，如果不存在任务的话，保持现状
            if (_tTaskTable.ContainsKey(taskEventPair))
            {
                //根据事件-传输子对取得一个任务
                StateMachineTask stateMachineTask = _tTaskTable[taskEventPair];
                //判断任务是否存在
                if (stateMachineTask != null)
                {
                    //判断任务中是否有下一个状态
                    if (stateMachineTask.NextStatus != null)
                    {
                        //设定下一个状态
                        Status = stateMachineTask.NextStatus;
                    }
                    //判断动作是否存在
                    if (stateMachineTask.Action != null)
                    {
                        //载入传输子，执行动作
                        stateMachineTask.Action(transfer);
                    }
                }
            }
        }
    }
}