using System;
using System.Collections.Generic;

namespace ThinkAway.Core.Mediator
{
    /// <summary>
    /// 抽象中介者
    /// 用于模块间解耦，通过避免对象互相显式的指向对方从而降低耦合。 
    /// 调停者模式包装了一系列对象相互作用的方式，使得这些对象不必相互明显作用。
    /// 从而使他们可以松散偶合。
    /// 当某些对象之间的作用发生改变时，不会立即影响其他的一些对象之间的作用。
    /// 保证这些作用可以彼此独立的变化。
    /// 调停者模式将多对多的相互作用转化为一对多的相互作用。
    /// 调停者模式将对象的行为和协作抽象化，把对象在小尺度的行为上与其他对象的相互作用分开处理。
    /// </summary>
    public abstract class Mediator : IMediator
    {
        /// <summary>
        /// 中介者肯定需要保持有若干同事的联系方式
        /// </summary>
        protected IDictionary<String, IColleague> Colleagues = new Dictionary<string, IColleague>();

        /// <summary>
        /// 中介者可以动态地与某个同事建立联系  
        /// </summary>
        /// <param name="name"></param>
        /// <param name="colleague"></param>
        public void AddColleague(String name, IColleague colleague)
        {
            //在中介者这里帮助具体同事建立起于中介者的联系
            colleague.SetMediator(this);
            this.Colleagues.Add(name, colleague);
        }

        /// <summary>
        /// 中介者也可以动态地撤销与某个同事的联系  
        /// </summary>
        /// <param name="name"></param>
        public void DeleteColleague(String name)
        {
            this.Colleagues.Remove(name);
        }

        /// <summary>
        /// 中介者必须具备在同事之间处理逻辑、分配任务、促进交流的操作
        /// </summary>
        /// <param name="name"></param>
        /// <param name="method"></param>
        public abstract void Trigger(String name, String method);
    }
}