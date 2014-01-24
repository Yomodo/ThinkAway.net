using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;

namespace ThinkAway.IO.Log
{
    /// <summary>
    /// LogConfig
    /// </summary>
    public class LogConfig
    {
        private string _path = "./";
        /// <summary>
        /// 日志路径
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private int _maxSize = 10000;
        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxSize
        {
            get { return  _maxSize;}
            set { _maxSize = value; }
        }

        private string _file = "logger.log";
        /// <summary>
        /// 日志文件名
        /// </summary>                      
        public string File
        {
            get { return _file; }
            set { _file = value; }
        }

        private bool _append;

        /// <summary>
        /// 是否追加日志
        /// </summary>
        public bool Append
        {
            get { return _append; }
            set { _append = value; }
        }

        private Encoding _encoding = Encoding.UTF8;

        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        private LogFilter _filter;

        /// <summary>
        /// 日志过滤器
        /// </summary>
        public LogFilter Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private LogMode _logMode = LogMode.Console;
        /// <summary>
        /// 日志模式
        /// </summary>
        public LogMode Mode
        {
            get { return _logMode; }
            set { _logMode = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        private string _format = "[{LEVEL}]\t [{DATE}] [{TIME}] {LOG}";
        /// <summary>
        /// 日志输出格式
        /// </summary>
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public LogConfig()
        {
            Filter = new LogFilter();
        }
        /// <summary>
        /// LogConfig
        /// </summary>
        /// <param name="sectionName"></param>
        public LogConfig(string sectionName)
        {
            Filter = new LogFilter();

            object section = ConfigurationManager.GetSection(sectionName);
            NameValueCollection nameValue = section as NameValueCollection;
            if (nameValue != null)
            {
                foreach (string key in nameValue)
                {
                    string value = nameValue[key];
                    switch (key)
                    {
                        case "Path":
                            Path = value;
                            break;
                        case "File":
                            File = value;
                            break;
                        case "Format":
                            Format = value;
                            break;
                        case "MaxSize":
                            MaxSize = Convert.ToInt32(value);
                            break;
                        case "Mode":
                            string[] modes = value.Split('|');
                            foreach (string mode in modes)
                            {
                                Mode |= (LogMode)Enum.Parse(typeof(LogMode), mode); 
                            }
                            break;
                        case "Level":
                            Filter.Level = (LogLevel)Enum.Parse(typeof(LogLevel), value);
                            break;
                        case "Append":
                            Append = Convert.ToBoolean(value);
                            break;
                        case "Tag":
                            Filter.Tag = value.Split('|');
                            break;
                        
                    }
                }
            }
                
        }
    }
}