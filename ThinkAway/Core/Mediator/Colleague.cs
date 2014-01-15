namespace ThinkAway.Core.Mediator
{
    /// <summary>
    /// 提供了一种对 调停者模式中 同事 的抽象 , 无法创建该类的实例
    /// </summary>
    public abstract class Colleague : IColleague
    {
        /// <summary>
        /// 
        /// </summary>
        protected IMediator Mediator;

        /// <summary>
        /// 在抽象同事类中添加用于与中介者取得联系（即注册）的方法
        /// </summary>
        /// <param name="mediator"></param>
        public void SetMediator(IMediator mediator)
        {
            this.Mediator = mediator;
        }
    }
}