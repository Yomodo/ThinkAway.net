using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using ThinkAway.Core;

namespace ThinkAway.Plus.Modem
{
    /// <summary>
    /// SMS Controller
    /// </summary>
    public class SMSController
    {
        private readonly Timer _timer;
        /// <summary>
        /// SMS Queue
        /// </summary>
        private readonly ISMSQueue _smsQueue;
        /// <summary>
        /// SMS Pool
        /// </summary>
        private readonly System.Collections.Generic.Dictionary<string,SMSSender> _smsPool;

        private readonly List<string> _ports;

        private void InvokeEventLog(string log)
        {
            MyAction<string> handler = EventLog;
            if (handler != null) handler(log);
        }

        /// <summary>
        /// Event Log
        /// </summary>
        public event MyAction<string> EventLog;

        /// <summary>
        /// COM,Number,Message
        /// </summary>
        public event EventHandler<ReceiveEventArgs> Received;

        public void OnReceived(ReceiveEventArgs arg2)
        {
            EventHandler<ReceiveEventArgs> handler = Received;
            if (handler != null) handler(this, arg2);
        }


        /// <summary>
        /// COM,Number,Message,State
        /// </summary>
        public event EventHandler<SentEventArgs> Sent;

        public void OnSent(SentEventArgs args)
        {
            InvokeEventLog(String.Format("状态:{0}:{1}", args.SmsInfo, args.Result));

            _smsQueue.Set(args.SmsInfo, args.Result ? 500 : 501);

            EventHandler<SentEventArgs> handler = Sent;
            if (handler != null) handler(this, args);

            
            #region 故障处理

            //TODO:16:32 2011-12-07 临时代码 // 重新设置 Modem
            if (args.Result == false)
            {
                //设备重置次数
                const int MAX_RESET = 3;

                string com = args.SmsInfo.Com;
                if (_smsPool.ContainsKey(args.SmsInfo.Com))
                {
                    SMSSender smsSender = _smsPool[args.SmsInfo.Com];
                    if (smsSender.HasError)
                    {
                        int reset = ++smsSender.Reset;
                        if (reset < MAX_RESET)
                        {
                            InvokeEventLog(String.Format("重置:{0}", com));

                            Queue<SMSSendInfo> smsSendInfos = smsSender.Stop();

                            foreach (SMSSendInfo sendInfo in _smsPool[com].Result)
                            {
                                if (sendInfo.State == 1)
                                {
                                    smsSendInfos.Enqueue(sendInfo);
                                }
                            }

                            RemoveModem(com);

                           
                            ////Create
                            smsSender = CreateModem(com, 9600);
                            if (smsSender != null)
                            {
                                foreach (SMSSendInfo sendInfo in smsSendInfos)
                                {
                                    args.SmsInfo.State = 0;
                                    //smsSender.AsyncSend(sendInfo);
                                    AsyncSend(sendInfo);
                                }
                                _smsPool[com].Reset = reset;
                            }
                        }
                        else
                        {
                            //drive has a error .
                            InvokeEventLog(String.Format("设备错误:{0}", com));
                            RemoveModem(com);
                        }
                    }

                }
            }
            //TODO:End

            #endregion
        }


        public SMSController()
        {
            _smsQueue = new DBQueue();
            _timer = new Timer(RequestMessage);
            _smsPool = new System.Collections.Generic.Dictionary<string, SMSSender>();
            //
            _timer.Change(1000, 5000);

            if (_ports == null)
            {
                _ports = new List<string>(SerialPort.GetPortNames());
            }
        }

        /// <summary>
        /// Add SMS Modem to SMS Modem Pool.
        /// 根据指定的 COM 端口标志和波特率创建发送器并返回该对象
        /// </summary>
        /// <param name="port"></param>
        /// <param name="bute"></param>
        public SMSSender CreateModem(string port, int bute)
        {
            SMSSender smsSender = new SMSSender(port,bute);
            if (smsSender.Init())
            {
                smsSender.Sent += smsSender_Sent;
                smsSender.Received += smsSender_Received;
                //
                if(!_smsPool.ContainsKey(port))
                {
                    _smsPool.Add(port, smsSender);
                }

                InvokeEventLog(String.Format("插入设备:{0},{1}",port,bute));

                _ports.Remove(port);
            }
            else
            {
                InvokeEventLog(String.Format("初始化失败:{0}", port));

                return null;
            }
            return smsSender;
        }

        void smsSender_Received(object sender, ReceiveEventArgs args)
        {
            OnReceived(args);
        }

        void smsSender_Sent(object o, SentEventArgs sentEventArgs)
        {
            OnSent(sentEventArgs);
        }

        /// <summary>
        /// Async Send
        /// </summary>
        /// <param name="com"></param>
        /// <param name="phone"></param>
        /// <param name="msg"></param>
        public void AsyncSend(string com, string phone, string msg)
        {
            SMSSendInfo sendInfo = new SMSSendInfo
                                       {
                                           Com = com,
                                           Phone = phone,
                                           Message = msg,
                                           DateTime = DateTime.Now.Ticks
                                       };
            AsyncSend(sendInfo);
        }

        public void AsyncSend(SMSSendInfo smsInfo)
        {
            _smsQueue.Add(smsInfo);

            InvokeEventLog(String.Format("发送:{0}", smsInfo));
        }


        public void RequestMessage(object state)
        {
            System.Collections.Generic.Dictionary<string, SMSSender>.ValueCollection valueCollection = _smsPool.Values;
            SMSSender[] smsSenders = new SMSSender[valueCollection.Count];
            valueCollection.CopyTo(smsSenders,0);
            foreach (SMSSender smsSender in smsSenders)
            {
                InvokeEventLog(String.Format("请求数据:{0}", smsSender.Name));
                List<SMSSendInfo> list = _smsQueue.Get(smsSender.Name);
                foreach (SMSSendInfo smsSendInfo in list)
                {
                    smsSender.AsyncSend(smsSendInfo);

                    InvokeEventLog(String.Format("压入:{0}", smsSendInfo));
                }
            }
        }

        public string[] Ports(int type)
        {
            string[] ports = new string[] {};
            switch (type)
            {
                case 0:
                    ports = _ports.ToArray();
                    break;
                case 1:
                    int i = 0;
                    ports = new string[_smsPool.Count];
                    foreach (var port in _smsPool.Keys)
                    {
                        ports[i] = port;
                        i++;
                    }
                    break;
            }
            return ports;
        }


        //public List<PhoneBook> Receiver(string port)
        //{
        //    List<PhoneBook> list = new List<PhoneBook>();
        //   if(_smsPool.ContainsKey(port))
        //   {
        //       //list.AddRange(_smsPool[port].GetAllNumber());
        //       PhoneBook phoneBook = new PhoneBook();
        //       phoneBook.Name = "Lsong";
        //       phoneBook.Number = "15840171400";
        //       list.Add(phoneBook);
        //   }
        //    return list;
        //}

        public bool RemoveModem(string com)
        {
            bool result = false;
            if (_smsPool.ContainsKey(com))
            {
                InvokeEventLog(String.Format("移除设备:{0}", com));
                result = _smsPool.Remove(com);
                _ports.Add(com);
            }
            return result;
        }
    }
}
