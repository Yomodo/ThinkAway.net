using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using ThinkAway.Core;
using ThinkAway.IO.Log;
/*
 * Lsong
 * i@lsong.org
 * http://lsong.org
 */
namespace ThinkAway.Net.FTP
{
    /// <summary>
	/// Summary description for FTPConnection.
	/// </summary>
	public class FTPClient : TcpClient
	{
	    readonly ILog _logger;
        private FtpMode _mode;

        /// <summary>
        /// 数据传输模式
        /// </summary>
        public FtpMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        private string _host;

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        private int _port;

        /// <summary>
        /// 端口号码
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        private string _userName;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _password;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private const int BLOCK_SIZE = 512;
	    private const int DATA_PORT_RANGE_MIN = 1500;
	    private const int DATA_PORT_RANGE_MAX = 65000;
        
        /// <summary>
        /// FTP Client
        /// </summary>
		public FTPClient()
		{
            _logger = new Logger();
		}

        /// <summary>
        /// FTP Client
        /// </summary>
        /// <param name="host"></param>
        public FTPClient(string host): this(host, 21)
        {
        }

        /// <summary>
        /// FTP Client
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public FTPClient(string host,int port):this()
        {
            Host = host;
            Port = port;
        }

        public void Connect()
        {
            this.Connect(Host, Port);
        }

        /// <summary>
        /// CONNECT
        /// </summary>
        public void Connect(string host)
        {
            Connect(host, 21);
        }

        /// <summary>
        /// CONNECT
        /// </summary>
        public new void Connect(string host, int port)
        {
            Host = host;
            Port = port;
            try
            {
                _logger.Info("ftp","CONNECT:{0}:{1}", Host, Port);
                base.Connect(Host, Port);
                _logger.Info("ftp","CONNECTED:{0}:{1}", Host, Port);
                GetResult();
            }
            catch (Exception exception)
            {
                Disconnect();
                throw new Exception("Couldn't connect to remote server", exception);
            }
        }
        /// <summary>
        /// Disconnect
        /// </summary>
        public void Disconnect()
        {
            SendCommand("QUIT");
            GetResult();
            this.Close();
        }
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void Login(string userName, string password)
        {
            UserName = userName;
            Password = password;


            //SEND USER
            SendCommand(string.Format("USER {0}", UserName));

            Result<string> result = GetResult();
            AssertResult(result);
            
            //SEND PASSWORD
            if (result.Code == 331)
            {
                SendCommand(string.Format("PASS {0}", Password));
                Result<string> read = GetResult();
                AssertResult(read);
            }
        }

        protected void SetTramsferType(FtpType ftpType)
        {
            switch (ftpType)
            {
                case FtpType.ASCII:
                    SendCommand("TYPE A", true);
                    break;
                case FtpType.Binary:
                    SendCommand("TYPE I", true);
                    break;
                default:
                    throw new InvalidEnumArgumentException("Invalid File Transfer Type");
            }
        }

        /// <summary>
        /// SendCommand
        /// </summary>
        /// <param name="command"></param>
        /// <param name="echo"> </param>
        public void SendCommand(String command)
        {
            SendCommand(command, false);
        }

        /// <summary>
        /// SendCommand
        /// </summary>
        /// <param name="command"></param>
        /// <param name="echo"> </param>
        public void SendCommand(String command, bool echo )
        {
            System.Threading.Monitor.Enter(this);
            NetworkStream stream = GetStream();
            command = string.Concat(command, Environment.NewLine);
            byte[] cmdBytes = Encoding.ASCII.GetBytes(command);
            stream.Write(cmdBytes, 0, cmdBytes.Length);
            _logger.Info("ftp", command);
            System.Threading.Monitor.Exit(this);
            if (echo)
            {
                Result<string> result = GetResult();
                AssertResult(result);
            }
        }

        /// <summary>
        /// GetResult
        /// </summary>
        /// <param name="networkStream"></param>
        /// <returns></returns>
        public Result<string> GetResult()
        {
            return GetResult(null);
        }

        /// <summary>
        /// GetResult
        /// </summary>
        /// <param name="networkStream"></param>
        /// <returns></returns>
        public Result<string> GetResult(NetworkStream networkStream)
        {
            System.Threading.Monitor.Enter(this);
            Result<string> result = new Result<string>();
            if (networkStream == null)
            {
                networkStream = GetStream();
            }
            Byte[] buffer = new Byte[BLOCK_SIZE];
            StringBuilder stringBuilder = new StringBuilder();
            int i = 0;
            while (networkStream.DataAvailable || i <= 10)
            {
                int bytes = networkStream.Read(buffer, 0, buffer.Length);
                stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));

                if (bytes > 0)
                {
                    i = 100;
                }
                else
                {
                    i++;
                }
            }
            string tempMessage = stringBuilder.ToString();
            string[] message = tempMessage.Split('\n');
            foreach (string msg in message)
            {
                if (!String.IsNullOrEmpty(msg))
                {
                    result.ResultObject.Add(msg.Trim('\r'));
                    _logger.Debug("ftp", msg);
                }
            }
            int length = result.ResultObject.Count;
            if (length == 0)
            {
                throw new Exception("Server does not return message to the message");
            }
            string lastMessage = result.ResultObject[0];
            if (lastMessage.Substring(3, 1) == " " || lastMessage.Substring(3, 1) == "-")
            {
                string lastCode = lastMessage.Substring(0, 3);
                int code;
                Int32.TryParse(lastCode, out code);
                result.Desception = lastMessage;
                result.Code = code;
            }
            System.Threading.Monitor.Exit(this);
            return result;
        }

        /// <summary>
        /// GetTcpClient
        /// </summary>
        /// <param name="ftpMode"></param>
        /// <returns></returns>
        private TcpClient GetTcpClient(FtpMode ftpMode)
        {
            TcpClient tcpClient = null;
            if(ftpMode == FtpMode.Active)
            {
                Random rnd = new Random((int)DateTime.Now.Ticks);
                int  port = rnd.Next(DATA_PORT_RANGE_MIN, DATA_PORT_RANGE_MAX);

                int portHigh = port >> 8;
                int portLow = port & 255;

                IPAddress ipAddress = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[0];
                TcpListener tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start(10);
                SendCommand(string.Format("PORT {0},{1},{2}", ipAddress.ToString().Replace(".", ","), portHigh.ToString(CultureInfo.InvariantCulture), portLow));
                AssertResult(GetResult(), 200);
                SendCommand("MLSD");
                tcpClient = tcpListener.AcceptTcpClient();
                _logger.Debug("ftp", "ftp server AcceptTcpClient");
                tcpListener.Stop();
                
            }
            if(ftpMode == FtpMode.Passive)
            {
                SendCommand("PASV");
                Result<string> result = GetResult();
                string message = result.Desception;
                AssertResult(result);
                //227 Entering Passive Mode (127,0,0,1,148,235)
                int startIndex = message.IndexOf('(');
                int endIndex = message.LastIndexOf(')');
                if (startIndex == -1 || endIndex == -1)
                    throw new Exception(string.Format("{0} Passive Mode not implemented", message));
                startIndex++;
                string str = message.Substring(startIndex, endIndex - startIndex);
                string[] args = str.Split(',');
                string ip = string.Format("{0}.{1}.{2}.{3}", args[0], args[1], args[2],args[3]);
                int port = 256 * int.Parse(args[4]) + int.Parse(args[5]);
                tcpClient = new TcpClient();
                tcpClient.Connect(ip, port);
            }
            return tcpClient;
        }
        /// <summary>
        /// DIR
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        public string[] Dir(string mask)
        {
            SetTramsferType(FtpType.ASCII);
		    
		    TcpClient tcpClient = GetTcpClient(Mode);
            NetworkStream networkStream = tcpClient.GetStream();
            if(Mode == FtpMode.Passive)
            {
                SendCommand("NLST");
            }
            Result<string> result = GetResult(networkStream);
			networkStream.Close();
            tcpClient.Close();
            AssertResult(GetResult());
            if(!String.IsNullOrEmpty(mask))
            {
                List<string> list = result.ResultObject;
                result.ResultObject = list.FindAll(delegate(string x) { return x.IndexOf(mask, System.StringComparison.Ordinal) > -1; });
            }
			return result.ResultObject.ToArray();
		}
        /// <summary>
        /// XDIR
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="onColumnIndex"></param>
        /// <returns></returns>
		public string[] XDir(String mask, int onColumnIndex)
		{
			string[] tmpList = XDir();
            //NOTE:NET2.0
            List<string> list = new List<string>();
            foreach (string tmp in tmpList)
            {
                list.Add(tmp[onColumnIndex].ToString(CultureInfo.InvariantCulture));
            }
            list = list.FindAll(delegate(string x) { return x.IndexOf(mask, System.StringComparison.Ordinal) > -1; });
            //NOTE:NET2.0
            List<string> rList = new List<string>();
            foreach (string s in list)
            {
                rList.Add(s[onColumnIndex].ToString(CultureInfo.InvariantCulture));
            }
            return rList.ToArray();
		}
        /// <summary>
        /// XDIR
        /// </summary>
        /// <returns></returns>
		public string[] XDir()
		{
		    string[] dirs = Dir();
            List<string> list = new List<string>();
            foreach (string dir in dirs)
			{
                list.AddRange(this.GetTokens(dir, " "));
			}
            return list.ToArray();
		}

        private string[] Dir()
        {
            return Dir(String.Empty);
        }

        /// <summary>
        /// SEND FILE
        /// </summary>
        /// <param name="localFileName"></param>
        public void SendFile(string localFileName)
        {
            SendFile(localFileName, Path.GetFileName(localFileName));
        }

        /// <summary>
        /// SEND FILE
        /// </summary>
        /// <param name="localFileName"></param>
        /// <param name="remoteFileName"></param>
        public void SendFile(string localFileName, string remoteFileName)
        {
            FileStream fs = new FileStream(localFileName, FileMode.Open);
            SendStream(remoteFileName,fs);
            fs.Close();
        }

        /// <summary>
        /// SendStream
        /// </summary>
        /// <param name="remoteFileName"></param>
        /// <param name="stream"></param>
        public void SendStream(string remoteFileName, Stream stream)
		{
		    TcpClient tcpClient = GetTcpClient(Mode);
            NetworkStream networkStream = tcpClient.GetStream();
            SendCommand(string.Format("STOR {0}", remoteFileName));
			Byte[] buffer = new Byte[BLOCK_SIZE];
		    int totalBytes = 0;
			while(totalBytes < stream.Length)
			{
				int bytes = stream.Read(buffer, 0, BLOCK_SIZE);
				totalBytes = totalBytes + bytes;
				networkStream.Write(buffer, 0, bytes);
			}

			networkStream.Close();
            tcpClient.Close();
            
		    Result<string> read = GetResult();

            AssertResult(read);
		}

        /// <summary>
        /// GetStream
        /// </summary>
        /// <param name="remoteFileName"></param>
        /// <param name="stream"></param>
        public void GetStream(string remoteFileName, Stream stream)
		{
		    TcpClient tcpClient = GetTcpClient(Mode);

		    SendCommand(string.Format("RETR {0}", remoteFileName),true);
            NetworkStream networkStream = tcpClient.GetStream();

			Byte[] buffer = new Byte[BLOCK_SIZE];
            int bytes;
            while ((bytes = networkStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                stream.Write(buffer, 0, bytes);
            }

            networkStream.Close();
            tcpClient.Close();

            Result<string> readValue = GetResult();
            AssertResult(readValue,226);
        }

        /// <summary>
        /// GetFile
        /// </summary>
        /// <param name="remoteFileName"></param>
        public void GetFile(string remoteFileName)
		{
			GetFile(remoteFileName, Path.GetFileName(remoteFileName));
		}

        /// <summary>
        /// GetFile
        /// </summary>
        /// <param name="remoteFileName"></param>
        /// <param name="localFileName"></param>
        public void GetFile(string remoteFileName, string localFileName)
		{
			FileStream fs = new FileStream(localFileName,FileMode.Create);
			GetStream(remoteFileName, fs);
            fs.Flush();
			fs.Close();
		}
        /// <summary>
        /// DeleteFile
        /// </summary>
        /// <param name="remoteFileName"></param>
		public void DeleteFile(String remoteFileName)
		{
		    SendCommand(string.Format("DELE {0}", remoteFileName),true);
		}
		
		public virtual void MoveFile(string fileName, string to)
		{
            if(to == null)
                throw new ArgumentNullException("to");
		    if (!string.IsNullOrEmpty(to) && !to.EndsWith("/"))
		    {
		        to = string.Format("{0}/", to);
		    }
		    RenameFile(fileName, Path.Combine(to, fileName));
		}
        /// <summary>
        /// RenameFile
        /// </summary>
        /// <param name="fromRemoteFileName"></param>
        /// <param name="toRemoteFileName"></param>
        public void RenameFile(string fromRemoteFileName, string toRemoteFileName)
		{
		    SendCommand(string.Format("RNFR {0}", fromRemoteFileName),true);
			SendCommand(string.Format("RNTO {0}", toRemoteFileName),true);
		}
        /// <summary>
        /// SetCurrentDirectory
        /// </summary>
        /// <param name="remotePath"></param>
		public void SetCurrentDirectory(String remotePath)
		{
		    SendCommand(string.Format("CWD {0}", remotePath),true);
		}
		/// <summary>
        /// MakeDir
		/// </summary>
		/// <param name="directoryName"></param>
		public void MakeDir(string directoryName)
		{
		    SendCommand(string.Format("MKD {0}", directoryName),true);

		}
        /// <summary>
        /// RemoveDir
        /// </summary>
        /// <param name="directoryName"></param>
		public void RemoveDir(string directoryName)
		{
		    SendCommand(string.Format("RMD {0}", directoryName),true);
        }
        /// <summary>
        /// GetTokens
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        protected string[] GetTokens(String text, String delimiter)
        {
            List<string> tokens = new List<string>();
            int next = text.IndexOf(delimiter, System.StringComparison.Ordinal);
            while (next != -1)
            {
                string item = text.Substring(0, next);
                if (!string.IsNullOrEmpty(item))
                {
                    tokens.Add(item);
                }
                text = text.Substring(next + 1);
                next = text.IndexOf(delimiter, System.StringComparison.Ordinal);
            }
            if (!string.IsNullOrEmpty(text))
            {
                tokens.Add(text);
            }
            return tokens.ToArray();
        }

        protected void AssertResult(Result<string> result)
        {
            AssertResult(result, -1);
        }

        protected void AssertResult(Result<string> result, int code)
        {
            if (code != -1 && (result.Code == code))
                return;
            switch (result.Code)
            {
                case 0:
                    return;
                case 211:
                    //211-Features:
                    return;
                case 215:
                    //SYST
                    //215 UNIX emulated by FileZilla
                    //_logger.Warning("ftp",result.Desception);
                    return;
                case 200:
                    //200 Type set to A
                    return;
                case 220:
                    //Hello message
                    return;
                case 226:
                    //226 Transfer OK
                    return;
                case 227:
                    //227 Entering Passive Mode (127,0,0,1,73,188)
                    return;
                case 150:
                    //open dir
                    //Read();
                    //150 Connection accepted
                    return;
                case 125:
                    break;
                case 331:
                    //request password
                    return;
                case 202:
                    //login
                    Disconnect();
                    break;
                case 230:
                    //password
                    //log on
                    return;
                case 257:
                    //make dir 257 "/test" created successfully
                    return;
                case 550:
                    //550 'test': No such file or directory
                    //550 file/directory not found
                    //"550 Directory already exists"
                    break;
                case 553:
                    //553 Filename invalid
                    return;
                case 350:
                    // "350 File exists, ready for destination name."
                    return;
                case 250:
                    //"250 file renamed successfully";
                    return;
                case 421:
                    //timeout
                    break;
                case 500:
                    //500 FTP: command not recognised.
                    break;

            }

            _logger.Error("ftp", result.Desception);
            //throw new Exception(result.Desception);
        }
	}
}
