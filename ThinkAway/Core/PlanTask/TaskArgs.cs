using System;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Core.PlanTask
{
    /// <summary>
    /// 封装了与 任务计划 相关的事件参数
    /// </summary>
    public class TaskArgs : EventArgs
    {
        public System.Collections.Generic.KeyValuePair<string, Task> Task;

        public TaskArgs(System.Collections.Generic.KeyValuePair<string, Task> keyValue)
        {
            // TODO: Complete member initialization
            this.Task = keyValue;
        }
    }
}
