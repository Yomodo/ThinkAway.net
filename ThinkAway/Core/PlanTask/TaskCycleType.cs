namespace ThinkAway.Core.PlanTask
{
    /// <summary>
    /// 任务执行周期
    /// </summary>
    public enum CycleType
    {
        /// <summary>
        /// 仅一次
        /// </summary>
        Once = 0,
        /// <summary>
        /// 每天
        /// </summary>
        Day,
        /// <summary>
        /// 每周
        /// </summary>
        Week,
        /// <summary>
        /// 每月
        /// </summary>
        Month
    }
}
