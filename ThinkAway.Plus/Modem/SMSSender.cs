using System;
using System.Collections.Generic;
using System.Threading;
using ThinkAway.IO.Modem;

namespace ThinkAway.Plus.Modem
{
    /// <summary>
    /// SMS Sender
    /// </summary>
    public class SMSSender : GsmModem
    {
        private Thread _thread;

        private bool _runing;

        private int _maxReply = 0x3;

        public int MaxReply
        {
            get { return _maxReply; }
            set { _maxReply = value; }
        }

        /// <summary>
        /// 是否存在连续错误
        /// </summary>
        public bool HasError
        {
            get
            {
                return (Error + 1) > _maxReply;
            }
        }

        public IEnumerable<SMSSendInfo> Result
        {
            get { return _result; }
        }

        /// <summary>
        /// reset
        /// </summary>
        public int Reset { get; set; }

        public int Error { get; private set; }

        private readonly List<SMSSendInfo> _result;

        private readonly Queue<SMSSendInfo> _smsQueue;

        /// <summary>
        /// COM,Number,Message
        /// </summary>
        public event EventHandler<ReceiveEventArgs> Received;

        public void OnReceived(ReceiveEventArgs e)
        {
            EventHandler<ReceiveEventArgs> handler = Received;
            if (handler != null) handler(this, e);
        }


        /// <summary>
        /// COM,Number,Message,State
        /// </summary>
        public event EventHandler<SentEventArgs> Sent;

        public void OnSent(SentEventArgs args)
        {
            Error = args.Result ? 0 : ++Error;
            args.SmsInfo.State = args.Result ? 0 : 1;
            _result.Add(args.SmsInfo);


            EventHandler<SentEventArgs> handler = Sent;
            if (handler != null) handler(this, args);
        }

        public SMSSender(string port, int bute) : base(port, bute)
        {
            _result = new List<SMSSendInfo>();
            _smsQueue = new Queue<SMSSendInfo>();
        }

        public new bool Init()
        {
            bool result = base.Init();
            if (result)
            {
                AutoDelMsg = true;
                OnRecieved += _gsmModem_OnRecieved;
            }
            return result;
        }

        /// <summary>
        /// modem received event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _gsmModem_OnRecieved(object sender, SMSEventArgs e)
        {
            SMSSendInfo smsInfo = new SMSSendInfo();
            GsmModem gsmModem = sender as GsmModem;
            if (gsmModem != null)
            {
                smsInfo.Com = gsmModem.Name;
                smsInfo.Phone = e.SmsInfo.Number;
                smsInfo.Message = e.SmsInfo.Message;
                ReceiveEventArgs args = new ReceiveEventArgs();
                args.SmsInfo = smsInfo;
                OnReceived(args);
            }
        }

        /// <summary>
        /// recursion get message to send.
        /// </summary>
        private void RecursionSend()
        {
            while (_runing && _smsQueue.Count > 0)
            {
                SMSSendInfo smsInfo = _smsQueue.Dequeue();
                //
                System.Console.WriteLine("弹出:{0}", smsInfo);

                string phone = smsInfo.Phone;
                string message = smsInfo.Message;
                bool result = SendMsg(phone, message);
                SentEventArgs sentEventArgs = new SentEventArgs();
                sentEventArgs.SmsInfo = smsInfo;
                if (result)
                {
                    sentEventArgs.Result = true;
                    OnSent(sentEventArgs);
                }
                else
                {
                    if ((++smsInfo.Reply) < _maxReply)
                    {
                        _smsQueue.Enqueue(smsInfo);
                    }
                    else
                    {
                        sentEventArgs.Result = false;
                        OnSent(sentEventArgs);
                    }
                }
                //检查SIM卡信息
                if (_smsQueue.Count == 0)
                {
                    PublishMessage();
                    //wait time set 3000
                    Thread.Sleep(3000);
                }
            }
            _runing = false;
        }

        private void PublishMessage()
        {
            List<SMSEnity> list = GetAllMsg();
            foreach (SMSEnity info in list)
            {
                SMSSendInfo smsSendInfo = new SMSSendInfo
                                              {
                                                  Com = Name,
                                                  Phone = info.Number,
                                                  Message = info.Message
                                              };
                ReceiveEventArgs receiveEventArgs = new ReceiveEventArgs();
                receiveEventArgs.SmsInfo = smsSendInfo;
                OnReceived(receiveEventArgs);
            }
        }

        /// <summary>
        /// Async send message .
        /// </summary>
        /// <param name="sendInfo"></param>
        internal void AsyncSend(SMSSendInfo sendInfo)
        {
            //添加到发送队列
            _smsQueue.Enqueue(sendInfo);
            if (!_runing)
            {
                
                _thread = new Thread(RecursionSend);

                _runing = true;

                _thread.Start();
            }
        }
        /// <summary>
        /// 立即停止发送 并返回未处理的队列
        /// </summary>
        /// <returns></returns>
        public Queue<SMSSendInfo> Stop()
        {
            if (_runing)
            {
                _runing = false;
                Dispose();
            }
            return _smsQueue;
        }
    }
}
