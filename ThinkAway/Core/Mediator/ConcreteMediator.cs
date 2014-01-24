
using System.Threading;
using ThinkAway.Core.Invoker;

namespace ThinkAway.Core.Mediator
{
    /// <summary>
    /// 封装了一种调停者模式的简单实现 , 无法继承此类
    /// 通过使用 ColleagueDelegate 转换的委托方式实现单一入口函数并利用事件消息的方式进行数据传递
    /// </summary>
    public sealed class ConcreteMediator : Mediator
    {
        #region Overrides of Mediator

        /// <summary>
        /// 中介者必须具备在同事之间处理逻辑、分配任务、促进交流的操作
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="eventMessage"></param>
        public override void Trigger(string receiver, string eventMessage)
        {
            Intent intent = new Intent(receiver,eventMessage);
            Execute(intent);
        }

        /// <summary>
        /// 根据指定的 <c>Intent</c> 对象传递并执行任务
        /// </summary>
        /// <param name="intent"></param>
        public void Execute(Intent intent)
        {
            ThreadPool.QueueUserWorkItem(RunTask, intent);
        }
        /// <summary>
        /// PostData
        /// </summary>
        /// <param name="obj"></param>
        private void RunTask(object obj)
        {
            Intent intent = obj as Intent;
            if (intent != null)
            {
                if (intent.Receiver == null)
                {
                    foreach (System.Collections.Generic.KeyValuePair<string, IColleague> colleague in Colleagues)
                    {
                        Execute(colleague.Key, intent);
                    }
                }
                else
                {
                    Execute(intent.Receiver, intent);
                }
            }
        }
        /// <summary>
        /// 根据指定的接收者和 <c>Intent</c> 对象传递并执行任务
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="intent"></param>
        private void Execute(string receiver, Intent intent)
        {
            if (Colleagues.ContainsKey(receiver))
            {
                IWorker worker = Colleagues[receiver] as IWorker;
                if (worker != null)
                {
                    worker.Trigger(intent);
                }
            }
        }
        #endregion
    }
}
