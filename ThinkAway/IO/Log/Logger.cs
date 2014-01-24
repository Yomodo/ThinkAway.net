using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace ThinkAway.IO.Log
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    public class Logger : ILog
    {
        /// <summary>
        /// 
        /// </summary>
        private TextWriter _textWriter;
        /// <summary>
        /// 
        /// </summary>
        private EventLog _eventLog;

        /// <summary>
        /// LogCat
        /// </summary>
        public Logger()
        {
            //LogConfig
            Config = new LogConfig();
        }
        
        #region Implementation of ILog

 
        /// <summary>
        /// 
        /// </summary>
        public LogConfig Config { get; set; }

        /// <summary>
        /// Error Log
        /// </summary>
        /// <param name="log"></param>
        /// <param name="message"></param>
        /// <param name="tag"></param>
        public void Error(string tag, string log, params object[] message)
        {
            Log(LogLevel.Error, tag, log, message);
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        public void Error(string tag, Exception ex)
        {
            Log(LogLevel.Error, tag,  ex);
        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="log"></param>
        /// <param name="message"></param>
        public void Warning(string tag, string log, params object[] message)
        {
            Log(LogLevel.Warn, tag, log, message);

        }

        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        public void Warning(string tag, Exception ex)
        {
            Log(LogLevel.Warn, tag,  ex);
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="log"></param>
        /// <param name="message"></param>
        public void Info(string tag, string log, params object[] message)
        {
            Log(LogLevel.Info, tag, log, message);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        public void Info(string tag, Exception ex)
        {
            Log(LogLevel.Info, tag,ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="log"></param>
        /// <param name="message"></param>
        public void Debug(string tag, string log, params object[] message)
        {
            Log(LogLevel.Debug, tag, log, message);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        public void Debug(string tag, Exception ex)
        {
            Log(LogLevel.Debug, tag, ex);
        }

        /// <summary>
        /// log
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        public void Log(LogLevel logLevel, string tag, Exception ex)
        {
            string exceptionStr = ParseException(ex);
            Log(logLevel, tag, "{0}\r\nException:{1}", ex.Message, exceptionStr);
        }

        /// <summary>
        /// log
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="tag"></param>
        /// <param name="log"></param>
        /// <param name="message"></param>
        public void Log(LogLevel logLevel, string tag, string log, params object[] message)
        {
            if (logLevel < Config.Filter.Level)
            {
                return;
            }
            if (Config.Filter.Tag != null && Config.Filter.Tag.Length > 0)
            {
                bool hasTag = false;
                foreach (string t in Config.Filter.Tag)
                {
                    if(Equals(t,tag))
                    {
                        hasTag = true;
                        break;
                    }
                }
                if(!hasTag)return;
            }
            string text = string.Format(log, message);

            string format = ParseFormat(Config.Format);
            
            format = format.Replace("{LOG}", text);
            format = format.Replace("{TAG}", tag);
            format = format.Replace("{LEVEL}", logLevel.ToString());
            text = format;
            switch (Config.Mode)
            {
                case LogMode.Text:
                    TextOut(text);
                    break;
                case LogMode.Console:
                    ConsoleOut(logLevel,text);
                    break;
                case LogMode.EventLog:
                    EventOut(text);
                    break;
            }
        }
        /// <summary>
        /// TextOut
        /// </summary>
        /// <param name="text"></param>
        protected virtual void TextOut(string text)
        {
            if(_textWriter == null)
            {
                string path = ParseFormat(Config.Path);
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string file = ParseFormat(Config.File);
                string fileName = Path.Combine(path, file);
                _textWriter =  new StreamWriter(fileName,Config.Append);
            }
            _textWriter.WriteLine(text);
            _textWriter.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        protected virtual void EventOut(string text)
        {
            _eventLog = _eventLog ?? new EventLog(Config.File);
            _eventLog.WriteEntry(text);
        }

        /// <summary>
        /// ConsoleOut
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="log"></param>
        protected virtual void ConsoleOut(LogLevel logLevel, string log)
        {
            switch (logLevel)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogLevel.Warn:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Console.WriteLine(log);
            Console.ResetColor();
        }

        /// <summary>
        /// ParseFormat
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        protected virtual string ParseFormat(string format)
        {
            DateTime now = DateTime.Now;
            string temp = format;
            temp = temp.Replace("{YEAR}", now.Year.ToString(CultureInfo.InvariantCulture));
            temp = temp.Replace("{TIME}", now.ToString("HH:mm:ss"));
            temp = temp.Replace("{DATE}", now.ToString("yyyy-MM-dd"));
            
            return temp;
        }
        /// <summary>
        /// ParseException
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected virtual string ParseException(Exception ex)
        {
            StringBuilder stringBuilder = new StringBuilder(ex.ToString());
            stringBuilder.AppendLine();
            if (!String.IsNullOrEmpty(ex.StackTrace))
            {
                stringBuilder.AppendFormat("StackTrace:{0}", ex.StackTrace);
                stringBuilder.AppendLine();
            }
            if(ex.Data.Count > 0)
            {
                stringBuilder.AppendFormat("Data:{0}", ex.StackTrace);
                stringBuilder.AppendLine();
                foreach (DictionaryEntry dictionaryEntry in ex.Data)
                {
                    stringBuilder.AppendFormat("Key:{0};Value:{1}", dictionaryEntry.Key, dictionaryEntry.Value);
                    stringBuilder.AppendLine();
                }
            }
            return stringBuilder.ToString();
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            if(_textWriter != null)
            {
                _textWriter.Close();
                _textWriter.Dispose();
            }
        }

        #endregion
    }
    
}
