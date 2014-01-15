
using System;
using System.Xml.Serialization;

namespace ThinkAway.Plus.Update
{
    /// <summary>
    /// 产品
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string ProductVersion { get; set; }
        /// <summary>
        /// 版本名称
        /// </summary>
        public string VersionName { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        public string PublishTime { get; set; }
        /// <summary>
        /// 下载链接
        /// </summary>
        public string DownloadLink { get; set; }
        /// <summary>
        /// 更新说明
        /// </summary>
        public string UpdateDetail { get; set; }
    }
}
