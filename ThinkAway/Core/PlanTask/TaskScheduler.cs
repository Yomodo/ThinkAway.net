using System;
using System.Collections.Generic;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Core.PlanTask
{
    /// <summary>
    /// 计划任务
    /// </summary>
    public class TaskScheduler
    {
        /// <summary>
        /// 当任务开始执行
        /// </summary>
        public event EventHandler<TaskArgs> Start;

        public void OnStart(TaskArgs e)
        {
            EventHandler<TaskArgs> handler = Start;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// 当任务停止
        /// </summary>
        public event EventHandler<TaskArgs> Stoped;

        public void OnStoped(TaskArgs e)
        {
            EventHandler<TaskArgs> handler = Stoped;
            if (handler != null) handler(this, e);
        }
        /// <summary>
        /// 当任务执行
        /// </summary>
        public event EventHandler<TaskArgs> Execute;

        public void OnExecute(TaskArgs e)
        {
            EventHandler<TaskArgs> handler = Execute;
            if (handler != null) handler(this, e);
        }

        private readonly IDictionary<string,Task> _dictionary;

        public TaskScheduler()
        {
            _dictionary = new Dictionary<string, Task>();
        }
        /// <summary>
        /// 将指定的任务 添加到计划任务列表
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="task"></param>
        public void Add(string taskName, Task task)
        {
            task.Start += task_Start;
            task.Stoped += task_Stoped;
            task.Execute += task_Execute;
            _dictionary.Add(taskName,task);
        }
        /// <summary>
        /// GetTaskInfo 检索实例
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private KeyValuePair<string, Task> GetTaskInfo(object obj)
        {
            KeyValuePair<string, Task> result = new KeyValuePair<string, Task>();
            foreach (KeyValuePair<string, Task> keyValuePair in _dictionary)
            {
                Task task = keyValuePair.Value;
                if(Equals(task,obj))
                {
                    result = keyValuePair;
                    break;
                }
            }
            return result;
        }

        void task_Start(object sender, System.EventArgs e)
        {
            KeyValuePair<string, Task> keyValue = GetTaskInfo(sender);
            OnStart(new TaskArgs(keyValue));
        }

        void task_Stoped(object sender, EventArgs e)
        {
            KeyValuePair<string, Task> keyValue = GetTaskInfo(sender);
            _dictionary.Remove(keyValue.Key);
            OnStoped(new TaskArgs(keyValue));
        }

        void task_Execute(object sender, System.Timers.ElapsedEventArgs e)
        {
            KeyValuePair<string, Task> keyValue = GetTaskInfo(sender);
            OnExecute(new TaskArgs(keyValue));
        }
        /// <summary>
        /// 启动具有指定名称的任务实例
        /// </summary>
        /// <param name="taskName"></param>
        public void Run(string taskName)
        {
            Task task =  _dictionary[taskName];
            task.Run();
        }
        /// <summary>
        /// 启动任务列表中的所有任务实例
        /// </summary>
        public void RunAllTask()
        {
            foreach (string key in _dictionary.Keys)
            {
                Run(key);
            }
        }
    }
}
