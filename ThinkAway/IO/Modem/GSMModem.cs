using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using ThinkAway.Text.PDU;

namespace ThinkAway.IO.Modem
{
    /// <summary>
    ///设备类，完成短信发送 接收等
    /// </summary>
    public class GsmModem : IDisposable
    {
        #region 变量,属性


        private string _portName;
        private string _smsCenter;
        /// <summary>
        /// 串口对象
        /// </summary>
        private readonly SerialPort _sp;                

        private const string _OK = "OK";

        private const string _ERROR = "ERROR";

        private const string _lock_ = "_lock_";


        public bool AutoDelMsg { get; set; }
        /// <summary>
        /// 设备是否打开
        /// 对串口IsOpen属性访问
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return _sp.IsOpen;
            }
        }
        public string SMSCenter
        {
            get { return _smsCenter ?? (_smsCenter = GetMsgCenter()); }
        }

        public string Name
        {
            get { return _portName ?? (_portName = _sp.PortName); }
        }


        #endregion

        #region AT命令

        public struct ATCommand
        {
            /// <summary>
            /// 发送短信
            /// </summary>
            public const string CTRLZ = "\x01a";
            /// <summary>
            /// 换行输入(下一行内容)
            /// </summary>
            public const string LINE = "\r{0}";
            /// <summary>
            /// 短信前缀
            /// </summary>
            public const string CMTI = "+CMTI:";
            /// <summary>
            /// 初始化端口(ATZ)
            /// </summary>
            public const string ATZ = "ATZ";
            /// <summary>
            /// 设置回显(关闭)(ATE0)
            /// </summary>
            public const string ATE = "ATE0";
            /// <summary>
            /// 查询信号质量
            /// </summary>
            public const string CSQ = "AT + CSQ";
            /// <summary>
            /// 获取短信中心地址(CSCA)
            /// </summary>
            public const string CSCA = "AT + CSCA?";
            /// <summary>
            /// 获取机器码
            /// </summary>
            public const string CGMI = "AT + CGMI";
            /// <summary>
            /// PDU编码格式(CMGF=0)
            ///TEXT编码格式(CNGF=1)
            /// </summary>
            public const string CMGF = "AT + CMGF = {0}";
            /// <summary>
            /// 错误报告结果码显示(CMEE=1)
            /// </summary>
            public const string CMEE = "AT + CMEE = 1";
            /// <summary>
            /// 新短信提示(CNMI=2,1)
            /// </summary>
            public const string CNMI = "AT + CNMI = 2,1";
            /// <summary>
            /// PDU设置通信内容长度(PDU编码方式通信内容长度)
            /// TEXT设置通信内容长度(11位电话号码)
            /// 0 REC UNREAD 接收未读
            /// 1 REC READ 接收已读
            /// 2 STO UNSENT 存储未发送
            /// 3 STO SENT 存储已发送
            /// 4 ALL 所有消息
            /// </summary>
            public const string CMGS = "AT + CMGS = {0}";
            /// <summary>
            /// 删除一条短信息(信息序号码)
            /// 参数：短信序号
            /// </summary>
            public const string CMGD = "AT + CMGD = {0}";
            /// <summary>
            /// 读取短信(读取第几条短信)
            /// 参数：短信序号
            /// </summary>
            public const string CMGR = "AT + CMGR = {0}";
            /// <summary>
            /// 获取短信列表
            /// </summary>
            public const string CMGL = "AT + CMGL = {0}";
            /// <summary>
            /// 查询电话本
            /// </summary>
            public const string CPBS = "AT + CPBS = {0}";
            /// <summary>
            /// 读取电话本
            /// </summary>
            public const string CPBR = "AT + CPBR = {0}";
        }

        #endregion

        #region 事件,委托

        /// <summary>
        /// 创建事件收到信息的委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void OnRecievedHandler(object sender, SMSEventArgs e);
        /// <summary>
        /// 收到短信息事件 OnRecieved 
        /// 收到短信将引发此事件
        /// </summary>
        public event OnRecievedHandler OnRecieved;

        public void InvokeOnRecieved(SMSEventArgs args)
        {
            OnRecievedHandler handler = OnRecieved;
            if (handler != null) handler(this, args);
        }


        /// <summary>
        /// 从串口收到数据 串口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                //!!!可能导致IO异常
                string temp = _sp.ReadLine();
                ParseMessage(temp);
                ////TODO: 解析后清空缓冲区
                //_sp.DiscardInBuffer();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            //
        }
        void ParseMessage(string str)
        {
            //判断前缀是否为新短消息
            //"+CMTI: \"SM\",14\r"
            if (str.StartsWith(ATCommand.CMTI))
            {
                str = str.Remove(0, ATCommand.CMTI.Length);
                string[] headArgs = str.Split(',');
                if (headArgs.Length == 2)
                {
                    //string memory = headArgs[0];
                    int index;
                    Int32.TryParse(headArgs[1], out index);
                    if (index != 0)
                    {
                        SMSEnity SMSEnity = ReadMsg(index);
                        if (SMSEnity != null)
                        {
                            InvokeOnRecieved(new SMSEventArgs(SMSEnity));
                        }
                    }
                }
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comPort"></param>
        /// <param name="baudRate"></param>
        /// <param name="readTimeout"></param>
        /// <param name="rtsEnable"></param>
        public GsmModem(string comPort, int baudRate/* = 9600*/, int readTimeout/* = 25000*/, bool rtsEnable/* = true*/)
        {
            _sp = new SerialPort
            {
                PortName = comPort,
                BaudRate = baudRate,
                ReadTimeout = readTimeout,
                RtsEnable = rtsEnable
            };
            _sp.DataReceived += sp_DataReceived;
        }
        /// <summary>
        /// 无参数构造函数 完成有关初始化工作
        /// </summary>
        public GsmModem(string comPort, int baudRate)
        {
            _sp = new SerialPort
            {
                PortName = comPort,
                BaudRate = baudRate,
                ReadTimeout = 3000,
                RtsEnable = true
            };
            _sp.DataReceived += sp_DataReceived;
        }

        public void Dispose()
        {
            if (IsOpen)
            {
                this.Close();
            }
        }
        ~GsmModem()
        {
            Dispose();
        }

        #endregion

        #region 端口方法
        /// <summary>
        /// 设备初始化函数
        /// </summary>
        public bool Init()
        {
            bool retValue = false;
            //检查是否已经打开端口
            if (_sp.IsOpen == false)
            {
                try
                {
                    _sp.Open();  //打开通讯端口
                    //初始化端口
                    SendAT(ATCommand.ATZ);
                    SendAT(ATCommand.ATE);
                    //设置编码格式为 PDU
                    SendAT(ATCommand.CMGF, 0);
                    //新消息产生事件
                    SendAT(ATCommand.CNMI);
                    //没有异常: 初始化成功
                    retValue = true;
                }
                catch
                {
                    _sp.Close();
                    //异常: 初始化失败
                    retValue = false;
                }
            }
            return retValue;
        }

        /// <summary>
        /// 设备关闭函数
        /// </summary>
        /// <returns>是否成功</returns>
        public bool Close()
        {
            bool retValue = false;
            try
            {
                //检查是否已经打开端口
                if (_sp.IsOpen)
                {
                    //关闭端口
                    _sp.Close();
                    //再次检查端口状态
                    retValue = !_sp.IsOpen;
                }
            }
            catch (Exception)
            {
                retValue = false;
            }
            return retValue;
        }
        #endregion

        #region 公开函数
        /// <summary>
        /// 获取机器码
        /// </summary>
        /// <returns>机器码字符串（设备厂商，本机号码）</returns>
        public string GetMachineNo()
        {
            string result = SendAT(ATCommand.CGMI);
            result = result.Substring(result.Length - 4, 3).Trim() == _OK ? result.Substring(0, result.Length - 5).Trim() : string.Empty;
            return result;
        }
        /// <summary>
        /// 获取设备信号强度
        /// </summary>
        /// <returns></returns>
        public int GetRssi()
        {
            int retValue = 99;
            string result = SendAT(ATCommand.CSQ);
            if (result.Substring(result.Length - 4, 3).Trim() == _OK)
            {
                result = result.Substring(7, 2).Trim();
                retValue = Convert.ToInt32(result);
            }
            return retValue;
        }
        /// <summary>
        /// 获取短信中心号码
        /// </summary>
        /// <returns></returns>
        public string GetMsgCenter()
        {
            string tmp = SendAT(ATCommand.CSCA);
            if (tmp.EndsWith(_OK))
            {
                tmp = tmp.Split('\"')[1].Trim();
            }
            return tmp;
        }

        /// <summary>
        /// 发送AT指令 逐条发送AT指令 调用一次发送一条指令
        /// 能返回一个OK或ERROR算一条指令
        /// </summary>
        /// <param name="atCom">AT指令</param>
        /// <returns>发送指令后返回的字符串</returns>
        public string SendAT(string atCom)
        {
            //返回结果
            string result = string.Empty;
            if (String.IsNullOrEmpty(atCom))
            {
                return _ERROR;
            }
            try
            {
                lock (_lock_) //排他锁 ,防止修改AT命令
                {
                    //注销事件关联，为发送做准备
                    _sp.DataReceived -= sp_DataReceived;
                    //忽略接收缓冲区内容，准备发送
                    _sp.DiscardInBuffer();
                    //发送AT指令
                    _sp.Write(atCom + "\r");
                    //接收数据 循环读取数据 直至收到“OK”或“ERROR”
                    string temp = string.Empty;
                    while (!(temp.Equals(_OK) || temp.Equals(_ERROR)))
                    {
                        //读取串口返回结果
                        temp = _sp.ReadLine();
                        //TODO:防止解除事件导致无法受到新信息
                        //ParseMessage(temp);
                        //叠加返回值
                        result += temp;
                        temp = temp.Trim();
                    }
                }
            }
            catch
            {
                result = _ERROR;
            }
            finally
            {
                //事件重新绑定 正常监视串口数据
                _sp.DataReceived += sp_DataReceived;
            }
            return result.Trim();
        }

        /// <summary>
        /// 发送AT命令
        /// </summary>
        /// <param name="atCom"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string SendAT(string atCom, params string[] args)
        {
            //按命令 混合参数
            string comm = string.Format(atCom, args);
            //发送AT命令
            return SendAT(comm);
        }
        /// <summary>
        /// 发送整数参数命令
        /// </summary>
        /// <param name="atCom">AT 指令</param>
        /// <param name="arg">参数</param>
        /// <returns>返回值</returns>
        public string SendAT(string atCom, int arg)
        {
            //发送AT命令
            return SendAT(atCom, arg.ToString());
        }

        #endregion

        #region 短信方法
        /////// <summary>
        /////// 获取电话本资料
        /////// </summary>
        /////// <returns></returns>
        //public List<PhoneBook> GetAllNumber()
        //{
        //    List<PhoneBook> list = new List<PhoneBook>();

        //    string temp = SendAT(ATCommand.CPBR, "?").Split(',')[0];
        //    if (temp != _ERROR)
        //    {
        //        temp = temp.Substring(8, temp.Length - 9).Replace('-', ',');
        //        string[] str = SendAT(ATCommand.CPBR, temp).Split('\r');

        //        list.AddRange(from s in str
        //                      where !String.IsNullOrEmpty(s) && s != _OK && s != _ERROR
        //                      select s.Split(',')
        //                      into strlist select new PhoneBook
        //                                              {
        //                                                  Inedx = Convert.ToInt32(strlist[0].Substring(6, strlist[0].Length - 6).Trim('"')), Number = strlist[1].Trim('"'), NumType = Convert.ToInt32(strlist[2]), Name = strlist[3].Trim('"')
        //                                              });
        //    }
        //    return list;
        //}
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="msg">短信内容</param>
        public bool SendMsg(string phone, string msg)
        {
            int len;
            string pduStr = PduEncoder.Encoding(SMSCenter, phone, msg, out len);   //编码
            string resultStr = SendAT(ATCommand.CMGS, string.Concat(len, "\r", pduStr, ATCommand.CTRLZ));  //26 Ctrl+Z ascii码
            return resultStr.EndsWith(_OK);
        }
        /// <summary>
        /// 删除短信 按索引位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool DelMsg(int index)
        {
            return SendAT(ATCommand.CMGD, index).Equals(_OK);
        }
        /// <summary>
        /// 获取未读信息列表
        /// </summary>
        /// <returns>未读信息列表（中心号码，手机号码，发送时间，短信内容）</returns>
        public List<SMSEnity> GetUnreadMsg()
        {
            const int UnreadMsg = 0;
            List<SMSEnity> result = GetMsgByType(UnreadMsg);
            return result;
        }
        /// <summary>
        /// 获取所有信息 已读,未读
        /// </summary>
        /// <returns></returns>
        public List<SMSEnity> GetAllMsg()
        {
            const int AllMsg = 4;
            List<SMSEnity> result = GetMsgByType(AllMsg);
            return result;
        }
        /// <summary>
        /// 按序号读取短信
        /// 如果没有,返回Null
        /// </summary>
        /// <param name="index">序号</param>
        /// <returns>信息字符串 (中心号码，手机号码，发送时间，短信内容)</returns>
        public SMSEnity ReadMsg(int index)
        {
            SMSEnity SMSEnity = null;
            //
            string resultStr = SendAT(ATCommand.CMGR, index);
            //"+CMGR: 1,,32\r0891683110200405F0240D91683148000148F70008117082419521230C00460061006E006800740031\r\rOK"
            //未找到 返回 ERROR
            if (!resultStr.Equals(_ERROR))
            {
                string[] resultList = resultStr.Split('\r');
                //由于SendAT函数一定会返回结果 所以不用判断数组越界问题
                if (resultList[resultList.Length - 1].Equals(_OK))
                {
                    string head = resultList[0];
                    string pduStr = resultList[1];
                    const string CMGR = "+CMGR:";
                    if (head.StartsWith(CMGR))
                    {
                        head = head.Remove(0, CMGR.Length);
                        string[] headArgs = head.Split(',');
                        //RFC 协议规定必须为3个参数
                        //+CMGR: <stat>, [<alpha>] ,<length> 
                        //<pdu> 
                        if (headArgs.Length == 3)
                        {
                            int stat, alpha, length;
                            Int32.TryParse(headArgs[0], out stat);
                            Int32.TryParse(headArgs[1], out alpha);
                            Int32.TryParse(headArgs[2], out length);
                            if (AutoDelMsg)
                                DelMsg(index);
                        }
                        //0891683110903305F0240D91685148101704F0000811707211110523104ED64EEC768458EB6C146E295EA65047

                        SMSEnity = PduDecoder.Decoding(pduStr);
                    }
                }

            }
            return SMSEnity;
        }
        /// <summary>
        /// 根据类型获取短信
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>信息集合</returns>
        public List<SMSEnity> GetMsgByType(int type)
        {
            List<SMSEnity> result = new List<SMSEnity>();
            string resultStr = SendAT(ATCommand.CMGL, type);
            //"+CMGL: 1,1,,144\r0891683110100305F00005A10110F0000801101021000020805C0A656C76845BA2623760A8597DFF01611F8C2260A8900962E94E2D56FD8054901A7684901A4FE1670D52A1FF0C8054901A516C53F85C067AED8BDA4E3A60A8670D52A130025BA26237670D52A170ED7EBF00310030003000310030FF0C8BDD8D3967E58BE24E137EBF00310030003000310031FF0C6B228FCE4F7F75283002\r+CMGL: 2,1,,146\r0891683110100305F00005A10110F0000801101021000020824E3A4E868BA960A8768456DE94C397F35C3D663E4E2A6027FF0C6211516C53F84E3A60A863D04F9B4E8670AB94C34E1A52A1FF0C6D41884C97F34E50300197F365483001641E602A5E9467095C3D6709FF0C5FEB62E86253003100300031003500360032003000316CE8518C5F00901AFF0C8BA97B495F854E0D518D5BC25BDE3002\r+CMGL: 3,1,,146\r0891683110100305F00005A10110F0000801101021000020824E3A4E867ED960A863D04F9B8D348EAB670D52A1FF0C6211516C53F85F00901A003100300031003900388054901A79D84E66670D52A1FF0C003200345C0F65F64E3A60A863D04F9B6F0F8BDD63D0793A30014EE353D177ED4FE13001673A4E3B75598A007B49670D52A1FF0C62E800310030003100390038537353EF6CE8518C3002\r+CMGL: 4,1,,134\r0891683110100305F00005A10110F0000801101021000020768BDD8D395468546862A54E1A52A16BCF54684E3A60A863D04F9B8BDD8D394FE1606FFF0C8BA960A853CA65F64E8689E38BDD8D3960C551B5FF0C53D10048004652300038003000340031537353EF8BA25236FF0C4E5F53EF62E862538BDD8D3967E58BE270ED7EBF0031003000310039003000313002\r+CMGL: 5,1,,32\r0891683110903305F02405A00110F0000811704051731023100032002D0032FF1A53C85B9E60E00021\r+CMGL: 6,1,,152\r0891683110903305F02405A00110F0000811704051731023880032002D0031FF1A5C0A656C76845BA26237FF1A60A85F53524D751F65484F7F752876848D448D39595799104E3A65B052BF529B7545804A595799100031003851433002767B5F558054901A7F514E0A84254E1A5385007700770077002E00310030003000310030002E0063006F006DFF0C67E58BE230014EA48D3930015145503C300165B94FBF\r+CMGL: 7,1,,157\r0891683110903305F02408A1015695230008117091812170238C8054901A70AB94C3FF0C8BA97B495F85768458F097F366F47F8E5999FF01003100300031003500350035662F70AB94C3670D52A170ED7EBFFF0C73B057285F00901A70AB94C3537353EF83B78D604E009996514D8D3970ED95E894C397F3FF1A628A5E78798F7ED94F60300262E800310030003100350035003563090031952E537353EF63D053D64F7F7528\r+CMGL: 8,1,,30\r0891683110200405F0240D91683103897829F70008117062011375230A653652304E096761FF01\r+CMGL: 9,1,,30\r0891683110903305F0240D91685148101704F00008117072014524230A768458EB6C146E295EA6\r+CMGL: 10,1,,28\r0891683110200405F0040D91683130423215F200081170720165622308548C5E7365F65019\r+CMGL: 11,1,,30\r0891683110200405F0240D91685148101704F00008117072017592230A4E8689E351B35B9A671F\r+CMGL: 12,1,,26\r0891683110900405F0240D91685148101704F0000811707211108023064E8689E351B3\r+CMGL: 13,1,,36\r0891683110903305F0240D91685148101704F0000811707211110523104ED64EEC768458EB6C146E295EA65047\r+CMGL: 14,1,,32\r0891683110200405F0240D91685148101704F00008117072116152230C4E86548C5E7365F650199009\r+CMGL: 15,1,,28\r0891683110903305F0240D91685148101704F0000811707211717223085475768458EB6C14\r+CMGL: 16,1,,32\r0891683110903305F0240D91685148101704F00008117072118193230C548C5E7365F65019900962E9\r+CMGL: 17,1,,34\r0891683110903305F0240D91685148101704F00008117072213393230E628A63E14F4F5740768458EB6C14\r+CMGL: 18,1,,28\r0891683110200405F0040D91683130423215F2000811707221337523084E8689E351B35B9A\r+CMGL: 19,1,,22\r0891683110200405F0040D91683130423215F200081170722163552302548C\r\rOK"
            if (!(String.IsNullOrEmpty(resultStr) || resultStr.Equals(_OK) || resultStr.Equals(_ERROR)))
            {
                //协议头匹配
                const string CMGL = "+CMGL:";
                //"+CMGL: 1,1,,144"
                //"0891683110100305F00005A10110F0000801101021000020824E3A4E868BA960A8768456DE94C397F35C3D663E4E2A6027FF0C6211516C53F84E3A60A863D04F9B4E8670AB94C34E1A52A1FF0C6D41884C97F34E50300197F365483001641E602A5E9467095C3D6709FF0C5FEB62E86253003100300031003500360032003000316CE8518C5F00901AFF0C8BA97B495F854E0D518D5BC25BDE3002"
                string[] resultList = resultStr.Split('\r');
                //由于SendAT函数一定会返回结果 所以不用判断数组越界问题
                if (resultList[resultList.Length - 1].Equals(_OK))
                    for (int i = 0; i < resultList.Length; i += 2)
                    {
                        if (!String.IsNullOrEmpty(resultList[i]))
                        {
                            //Protocol Head
                            string head = resultList[i];
                            if (head.StartsWith(CMGL))
                            {
                                //如果符合协议头标准,移除协议头,以获得参数
                                head = head.Remove(0, CMGL.Length);
                                string[] headArgs = head.Split(',');
                                //RFC 协议规定必须为4个参数
                                //+CMGL : <index>,<stat>, [<alpha>], <length>
                                //<pdu> 
                                if (headArgs.Length == 4)
                                {
                                    int index, stat, alpha, length;
                                    Int32.TryParse(headArgs[0], out index);
                                    Int32.TryParse(headArgs[1], out stat);
                                    Int32.TryParse(headArgs[2], out alpha);
                                    Int32.TryParse(headArgs[3], out length);
                                    if (AutoDelMsg)
                                        DelMsg(index);
                                }
                                //PDU String
                                string pduStr = resultList[i + 1];
                                SMSEnity SMSEnity = PduDecoder.Decoding(pduStr); //PDU解码
                                result.Add(SMSEnity);
                            }
                        }
                    }
            }
            return result;
        }
        #endregion
    }
}
