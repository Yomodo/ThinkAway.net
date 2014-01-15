using System;

namespace ThinkAway.Core.Mediator
{
    /// <summary>
    /// 提供调停者模式的接口约束
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="colleague"></param>
        void AddColleague(String name, IColleague colleague);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        void DeleteColleague(String name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        void Trigger(String name, String method);
    }
}
