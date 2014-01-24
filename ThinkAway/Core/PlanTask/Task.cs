using System;
using System.Timers;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Core.PlanTask
{
    /// <summary>
    /// 计划任务 无法继承该类
    /// </summary>
    public sealed class Task
    {
        /// <summary>
        /// 任务计时器
        /// </summary>
        private Timer _timer;
        /// <summary>
        /// 任务时间
        /// </summary>
        public DateTime TaskDateTime;
        /// <summary>
        /// 任务 下一次执行的时间
        /// </summary>
        public DateTime NextDateTime;
        /// <summary>
        /// 任务参数
        /// </summary>
        public object Obj { get; set; }
        /// <summary>
        /// 任务执行周期
        /// </summary>
        public CycleType TaskCycleType { get; set; }
        /// <summary>
        /// 任务执行的回调
        /// </summary>
        public Action<object> TaskAction { get; set; }
        /// <summary>
        /// Execute 当任务被执行
        /// </summary>
        internal event EventHandler<ElapsedEventArgs> Execute;

        internal void OnExecute(ElapsedEventArgs e)
        {
            EventHandler<ElapsedEventArgs> handler = Execute;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// Start 当任务开始
        /// </summary>
        internal event EventHandler Start;

        internal void OnStart(EventArgs e)
        {
            EventHandler handler = Start;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// Stoped 当任务停止
        /// </summary>
        internal event EventHandler Stoped;

        
        internal void OnStoped(EventArgs eventArgs)
        {
            EventHandler handler = Stoped;
            if (handler != null) handler(this, eventArgs);
        }
        /// <summary>
        /// 根据指定的 任务执行时间 和 执行的回调委托创建 任务实例
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="action"></param>
        public Task(DateTime dateTime,Action<object> action)
        {
            TaskAction = action;
            TaskDateTime = dateTime;

            NextDateTime = TaskDateTime;
        }
        /// <summary>
        /// 初始化时钟
        /// </summary>
        private void InitTimer()
        {
            double interval = GetInterval();
            if(interval > 0)
            {
                _timer = new Timer(interval);
                _timer.AutoReset = false;
                _timer.Elapsed += timer_Elapsed;
                _timer.Start();
            }
        }

        /// <summary>
        /// 获得时钟周期
        /// </summary>
        /// <returns></returns>
        private long GetInterval()
        {
            long interval = NextDateTime.Ticks;
            interval -= DateTime.Now.Ticks;
            interval = interval / 10000;
            return interval;
        }

        /// <summary>
        /// 设置下一次执行的时间
        /// </summary>
        /// <param name="taskType"></param>
        private void SetNext(CycleType taskType)
        {
            switch (taskType)
            {
                case CycleType.Once:
                    break;
                case CycleType.Day:
                    NextDateTime = TaskDateTime.AddDays(1);
                    break;
                case CycleType.Week:
                    NextDateTime = TaskDateTime.AddDays(7);
                    break;
                case CycleType.Month:
                    NextDateTime = TaskDateTime.AddMonths(1);
                    break;
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnExecute(e);

            Action<object> action = TaskAction;
            if(action != null)
            {
                action(Obj);
                if (TaskCycleType == CycleType.Once)
                {
                    _timer.Stop();
                    OnStoped(EventArgs.Empty);
                }
                else
                {
                    SetNext(TaskCycleType);
                    InitTimer();
                }
            }
        }
        /// <summary>
        /// 开始执行任务
        /// </summary>
        internal void Run()
        {
            InitTimer();
            OnStart(EventArgs.Empty);
        }
    }
}
