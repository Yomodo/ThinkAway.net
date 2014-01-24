using System;
using System.Timers;

/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Core.TaskTimer
{
    /// <summary>
    /// 计划任务 无法继承该类
    /// </summary>
    public sealed class TaskTimer : Timer
    {
        /// <summary>
        /// 任务时间
        /// </summary>
        public DateTime TaskDateTime;
        /// <summary>
        /// 任务 下一次执行的时间
        /// </summary>
        public DateTime NextDateTime;

        private object _obj;

        /// <summary>
        /// 任务参数
        /// </summary>
        public object Obj
        {
            get { return _obj; }
            set { _obj = value; }
        }

        private CycleType _taskCycleType;

        /// <summary>
        /// 任务执行周期
        /// </summary>
        public CycleType TaskCycleType
        {
            get { return _taskCycleType; }
            set { _taskCycleType = value; }
        }

        private Action<object> _taskAction;

        /// <summary>
        /// 任务执行的回调
        /// </summary>
        public Action<object> TaskAction
        {
            get { return _taskAction; }
            set { _taskAction = value; }
        }

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
        public TaskTimer(DateTime dateTime,Action<object> action)
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
                Interval = interval;
                AutoReset = false;
                Elapsed += timer_Elapsed;
                base.Start();
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
                    Stop();
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
        internal new void Start()
        {
            InitTimer();
            //OnStart(EventArgs.Empty);
        }
    }
}
