using System;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;

using ThinkAway.Core;
using ThinkAway.Core.Invoker;
using ThinkAway.Core.Mediator;
using ThinkAway.Core.PlanTask;
using ThinkAway.Core.StateMachine;
using ThinkAway.Data;
using ThinkAway.Data.MSSQL;
using ThinkAway.IO.File;
using ThinkAway.IO.File.Log;
using ThinkAway.Net.FTP;
using ThinkAway.Net.Mail;
using ThinkAway.Net.Mail.POP3;
using ThinkAway.Net.Mail.POP3.Header;
using ThinkAway.Net.Mail.SMTP;
using ThinkAway.Net.Sockets;
using ThinkAway.Security;

namespace ThinkAway.Test
{
    class TEST
    {
        private readonly ILog _logger;

        public TEST()
        {
            _logger = new Logger();
            _logger.Config.Format = "{LOG}";
            _logger.Config.Filter.Tag = new[] {"task", "mail", "mediator"};

            _logger.Warning("README",README.Name);
            _logger.Warning("README",README.Version.ToString());
            _logger.Debug("README", "=========================================");
        }

        public void MsSqlTest()
        {
            DbHelper dbHelper = new MsSQL("test");
             using (TransactionScope transactionScope = new TransactionScope())
            {
                for (int i = 0; i < 10; i++)
                {
                    ContentValues contentValues = new ContentValues();
                    contentValues.Add("Name", "lsong");
                    contentValues.Add("Password", "123456");
                    dbHelper.Insert("myTable", contentValues);

                }
                transactionScope.Complete();
            }
            

            using (Cursor cursor = dbHelper.Query("myTable", null, null, null))
            {
                while (cursor.Next())
                {
                    System.Console.Write(cursor[0] + "    ");
                    System.Console.Write(cursor[1] + "    ");
                    System.Console.Write(cursor[2]);
                    System.Console.WriteLine();
                }
            }
           

            System.Console.ReadKey(true);
            
        }

        public void LoggerTest()
        {
           

            _logger.Config = new LogConfig();
            //logger.Config.Load("");
            //logger.Config.FileName = "logger.log";
            //logger.Config.Path = "D:\\";
            //logger.Config.MaxSize = 100000;
            //logger.Config.Format = "[{LEVEL}] [{DATE}] {LOG}";


            //logger.Config.Filter = new LogFilter();
            //logger.Config.Filter.Level = LogLevel.Info;
            _logger.Config.Mode = LogMode.Console;
            //logger.Config.Filter.Tag = new[] { "sys", "tag" };

            ThreadStart threadStart = ()=>
                                {
                                    for (int i = 0; i < 100; i++)
                                    {
                                        _logger.Log(LogLevel.Info, "sys", "test");
                                        _logger.Log(LogLevel.Warn, "sys", "test");
                                        //Exception exception =  new Exception("ggggggggggg");
                                        //exception.Data.Add("test","hhh");
                                        //logger.Log(LogLevel.Debug, "tag", exception);
                                        _logger.Error("tag","aaaaaaaaa{0}||{1}",i,"===");
                                        _logger.Debug("tag","hello");

                                    }
                                };
            Thread thread = new Thread(threadStart);
            Thread thread2 = new Thread(threadStart);

            thread.Start();
            thread2.Start();
        }

        public void CRCTest()
        {
            byte[] bytes = new byte[sizeof(int)];
            Int16 crc16 = CRC.CRC16(bytes);
            Int32 crc32 = CRC.CRC32(bytes);

            _logger.Debug("CRC", crc16.ToString("X2"));
            _logger.Debug("CRC", crc32.ToString("X2"));
        }

        public void FileTest()
        {
            FileHelper fileHelper = new FileHelper("C:\\aa.txt");
            fileHelper.GetIcon();
            fileHelper.Delete();
            fileHelper.MoveTo("d:\\aa.txt");
            fileHelper.CopyTo("d:\\aa.txt");
        }

        public void Md5Test()
        {
            Console.WriteLine(MD5.MDString(""));
            Console.WriteLine();
            Console.WriteLine(MD5.MDString("a"));
            Console.WriteLine();
            Console.WriteLine(MD5.MDString("abc"));
            Console.WriteLine();
            Console.WriteLine(MD5.MDString("message digest"));
            Console.WriteLine();
            Console.WriteLine(MD5.MDString("abcdefghijklmnopqrstuvwxyz"));
            Console.WriteLine();
            Console.WriteLine(MD5.MDString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"));
            Console.WriteLine();
            Console.WriteLine(MD5.MDString("12345678901234567890123456789012345678901234567890123456789012345678901234567890"));
            Console.ReadLine();
        }


        public void MediatorTest()
        {
            ConcreteMediator concreteMediator = new ConcreteMediator();

            Action<Intent> action = x => _logger.Debug("mediator", x.EventMessage);

            concreteMediator.AddColleague("testA", new ColleagueDelegate(action));
            concreteMediator.AddColleague("testB", new ColleagueDelegate(action));

            concreteMediator.Trigger("testA","test");

            concreteMediator.Execute(new Intent("aaaaaaaaaaa"));
        }


        public void StateMachineTest()
        {
            StateMachine stateMachine = new StateMachine("INIT");
            stateMachine.AddTask("INIT","",x=>_logger.Debug("test",x.EventMessage),"NEXT");
            stateMachine.SyncTrigger(null);
        }

        public void TaskTest()
        {
            TaskScheduler taskScheduler = new TaskScheduler();
            taskScheduler.Start += (x, y) => _logger.Info("task", "Start:" + y.Task.Key);
            taskScheduler.Execute += (x, y) => _logger.Info("task", "Execute:" + y.Task.Key);
            taskScheduler.Stoped += (x, y) => _logger.Info("task", "Stoped:" + y.Task.Key);
            
            for (int i = 0; i < 100; i++)
            {
                int i1 = i;
                Action<object> action = x => _logger.Debug("task", i1+"-->"+(System.DateTime.Now - (DateTime)x).ToString());
                DateTime dateTime = System.DateTime.Now.AddSeconds(i);
                Task task = new Task(dateTime, action);
                task.Obj = System.DateTime.Now;
                task.TaskCycleType = CycleType.Once;
                taskScheduler.Add("test" + i, task);
                //Thread.Sleep(300);
            }
            //taskScheduler.Run("test");
            taskScheduler.RunAllTask();
        }

        public void ResultTest()
        {
            Result<string> result = new Result<string>();
            result.Code = 100;
            result.Desception = "test";
            result.ResultObject = new List<string>();


            _logger.Info("result","",result.Code);
        }

        public void MailTest()
        {

            using (SmtpClient smtpClient = new SmtpClient("smtp.163.com"))
            {
                smtpClient.Connected += (x, y) => _logger.Debug("mail", "Connected:{0}", y);
                smtpClient.Authenticated += (x, y) => _logger.Debug("mail", "Authenticated:{0}", y);
                smtpClient.StartedMessageTransfer += (x, y) => _logger.Debug("mail", "StartedMessageTransfer:{0}", y);
                smtpClient.EndedMessageTransfer += (x, y) => _logger.Debug("mail", "EndedMessageTransfer:{0}", y);
                smtpClient.Disconnected += (x, y) => _logger.Debug("mail", "Disconnected:{0}", y);

                smtpClient.Connect();

                smtpClient.UserName = "song940@163.com";
                smtpClient.Password = "song940@163.com";

                smtpClient.Authenticate("song940@163.com", "song940@163.com");

                MailAddress from = new MailAddress("Lsong", "song940@163.com");
                MailAddress to = new MailAddress("admin@lsong.org");
                MailAddress cc = new MailAddress("Test<cc@lsong.org>");

                MailMessage mailMessage = new MailMessage(from, to);
                mailMessage.AddRecipient(cc, AddressType.Cc);
                mailMessage.AddRecipient("test@lsong.org", AddressType.Bcc);

                mailMessage.Charset = "UTF-8";
                mailMessage.Priority = MailPriority.High;
                mailMessage.Notification = true;

                mailMessage.AddCustomHeader("X-CustomHeader", "Value");
                mailMessage.AddCustomHeader("X-CompanyName", "Value");

                //string testCid = mailMessage.AddImage("C:\\test.bmp");

                //mailMessage.AddAttachment("C:\\test.zip");

                mailMessage.Subject = "This's a test Mail.";
                mailMessage.Body = "hello everybody .";
                mailMessage.HtmlBody =
                    string.Format("<html><body>hello everybody .<br /><img src='cid:{0}' /></body></html>", "");

                smtpClient.SendMail(mailMessage);
            }

            using (PopClient popClient = new PopClient("pop.163.com"))
            {
                popClient.UserName = "";
                popClient.Password = "";
                popClient.Connect("pop.163.com", 110, false);
                popClient.Authenticate("song940@163.com", "song940@163.com");

                int messageCount = popClient.GetMessageCount();
                for (int i = messageCount; i > 1; i--)
                {
                    //try
                    //{
                    MessageHeader messageHeader = popClient.GetMessageHeaders(i);
                    MailAddress sender = messageHeader.Sender;
                    MailAddress from = messageHeader.From;
                    List<MailAddress> to = messageHeader.To;
                    string subject = messageHeader.Subject;

                    _logger.Debug("mail", subject);
                    if (sender != null)
                    {
                        _logger.Info("mail", "Sender:{0}", sender);
                    }
                    if (from != null)
                    {
                        _logger.Info("mail", "From:{0}", from);
                    }
                    if (to != null)
                    {
                        foreach (MailAddress mailAddress in to)
                        {
                            _logger.Info("mail", "TO:{0}", mailAddress);
                        }
                    }
                    Message message = popClient.GetMessage(i);
                    MessagePart textBody = message.FindFirstPlainTextVersion();
                    MessagePart htmlBody = message.FindFirstHtmlVersion();


                    if (textBody != null)
                    {
                        string text = textBody.GetBodyAsText();
                        System.Console.WriteLine(text);
                    }else if (htmlBody != null)
                    {
                        string html = htmlBody.GetBodyAsText();
                        System.Console.WriteLine(html);
                    }
                    System.Console.ReadKey();
                    //}
                    //catch (Exception exception)
                    //{
                    //    _logger.Error("mail", exception);
                    //}
                }
            }

        }

        public void SocketTest()
        {
			string portStr =  System.Console.ReadLine();
			int port = System.Convert.ToInt32(portStr);
            AppServer<TextMessage> appServer = new AppServer<TextMessage>(port);
            appServer.Connected += (x, y) => _logger.Debug("socket", y.ToString());
            appServer.Disconnected += (x, y) => _logger.Debug("socket", y.ToString());
            appServer.Received += (x, y) => _logger.Debug("socket", y.ToString());

            appServer.StartListener();

            while (true)
            {
                string readLine = System.Console.ReadLine();
                if(string.IsNullOrEmpty(readLine))
                {
                    break;
                }
                TextMessage textMessage = new TextMessage();
                textMessage.Message = readLine;
                appServer.SendMessage(textMessage);
            }

            System.Console.ReadKey(true);
            appServer.StopListener();
        }


        public void FtpTest()
        {
            FTPClient ftpClient = new FTPClient("127.0.0.1");
            ftpClient.Mode = FtpMode.Passive;
            ftpClient.Connect();
            ftpClient.Login("admin","123");
            ftpClient.SendCommand("SYST",true);
            ftpClient.SendCommand("FEAT",true);
            ftpClient.SendCommand("CLNT 1.0.0.0",true);
            ftpClient.SendCommand("OPTS UTF8 ON",true);
            ftpClient.SendCommand("PWD",true);
            ftpClient.SetCurrentDirectory("test");
            ftpClient.Dir();
            ftpClient.SendFile(@"C:\test.zip", "aa.zip", FtpType.Binary);
            ftpClient.GetFile("aa.zip", @"D:\test.zip", FtpType.Binary);
            ftpClient.MoveFile("aa.zip", "test");

        }
    }
}
