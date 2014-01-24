using System;
using System.Threading;
using NUnit.Framework;
using ThinkAway.IO.Log;

namespace ThinkAway.Test
{
    class LoggerTest
    {
        private Logger _logger;
        public LoggerTest()
        {
            _logger = new Logger();
            _logger.Config.Mode = LogMode.Console | LogMode.Text;

            _logger.Debug("README", README.Name);
            _logger.Debug("README", README.Version.ToString());

            System.Console.WriteLine("=========================================");
        }

        [Test]
        public void Logger()
        {
            _logger.Config = new LogConfig();
            _logger.Config.File = "logger{DATE}.log";
            _logger.Config.Path = "D:\\";
            _logger.Config.MaxSize = 100000;
            _logger.Config.Format = "[{LEVEL}]\t[{DATE}]\t{LOG}";


            _logger.Config.Filter = new LogFilter();
            _logger.Config.Filter.Level = LogLevel.Info;
            _logger.Config.Mode = LogMode.Console | LogMode.Text;
            _logger.Config.Filter.Tag = new[] { "sys", "tag" };

            ThreadStart threadStart = () =>
            {
                for (int i = 0; i < 1000; i++)
                {
                    _logger.Log(LogLevel.Info, "sys", "test");
                    _logger.Log(LogLevel.Warn, "sys", "test");

                    _logger.Debug("tag", "hello");
                    _logger.Error("tag", new Exception("error"));
                }
            };
            Thread thread = new Thread(threadStart);
            Thread thread2 = new Thread(threadStart);

            thread.Start();
            thread2.Start();
        }
    }
}
