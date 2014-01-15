using System;
using ThinkAway.Core.Invoker;

namespace ThinkAway.Core.Mediator
{
    /// <summary>
    /// 提供了对调停者模式中的同事角色的简单实现 , 无法继承此类
    /// </summary>
    public sealed class ColleagueDelegate : Colleague, IWorker
    {
        private readonly System.Action<Intent> _action;
        /// <summary>
        /// 实现一种简单委托 , 提供调停者到委托的转换
        /// </summary>
        /// <param name="action"></param>
        public ColleagueDelegate(System.Action<Intent> action)
        {
            // TODO: Complete member initialization
            this._action = action;
        }

        #region Implementation of IWorker

        /// <summary>
        /// 执行工作任务
        /// </summary>
        /// <param name="intent"></param>
        public void Trigger(Intent intent)
        {
            Action<Intent> action = _action;
            if (action != null) action(intent);
        }

        #endregion
    }
}
