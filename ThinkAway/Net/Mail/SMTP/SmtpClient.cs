using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using ThinkAway.Net.Mail.Exceptions;
/******************************************************************************
    Copyright 2001-2005 Ian Stallings
    OpenSmtp.Net is free software; you can redistribute it and/or modify
    it under the terms of the Lesser GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    OpenSmtp.Net is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    Lesser GNU General Public License for more details.

    You should have received a copy of the Lesser GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
/*******************************************************************************/
namespace ThinkAway.Net.Mail.SMTP
{

    /// <summary>
    /// This Type sends a MailMessage using SMTP
    /// <seealso cref="MailAddress"/>
    /// <seealso cref="MailMessage"/>
    /// </summary>
    /// <example>
    /// <code>
    ///		from = new EmailAddress("support@OpenSmtp.com", "Support");
    ///		to = new EmailAddress("recipient@OpenSmtp.com", "Joe Smith");
    ///
    ///		msg = new MailMessage(from, to);
    ///		msg.Subject = "Testing OpenSmtp .Net SMTP component";
    ///		msg.Body = "Hello Joe Smith.";
    /// 
    ///		Smtp smtp = new Smtp("localhost", 25);
    ///		smtp.SendMail(msg);
    /// </code>
    /// </example>
    public class SmtpClient : IDisposable
    {
        private TcpClient _tcpc;
        private int _port = 25;
        private NetworkStream _stream;

        private const int ReceiveBufferSize = 1024;
        private const int SendBufferSize = 1024;

        #region PROPERTIES

        private string _host;

        /// <value>Stores the Host address SMTP server. The default value is "localhost"</value>
        /// <example>"mail.OpenSmtp.com"</example>
        public string Host
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <value>Stores the Port of the host SMTP server. The default value is port 25</value>
        public int Port
        {
            get { return (this._port); }
            set { this._port = value; }
        }

        private int _sendTimeout;

        /// <value>Stores send timeout for the connection to the SMTP server in milliseconds.
        /// The default value is 10000 milliseconds.</value>
        public int SendTimeout
        {
            get { return _sendTimeout; }
            set { _sendTimeout = value; }
        }

        private int _recieveTimeout;

        /// <value>Stores send timeout for the connection to the SMTP server in milliseconds.
        /// The default value is 10000 milliseconds.</value>
        public int RecieveTimeout
        {
            get { return _recieveTimeout; }
            set { _recieveTimeout = value; }
        }

        private string _userName;

        /// <value>Stores the username used to authenticate on the SMTP server.
        /// If no authentication is needed leave this value blank.</value>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _password;

        /// <value>Stores the password used to authenticate on the SMTP server.
        /// If no authentication is needed leave this value blank.</value>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        #endregion

        #region EVENTS

        /// <value>Event that fires when connected with target SMTP server.</value>
        public event EventHandler Connected;

        /// <value>Event that fires when dicconnected with target SMTP server.</value>
        public event EventHandler Disconnected;

        /// <value>Event that fires when authentication is successful.</value>
        public event EventHandler Authenticated;

        /// <value>Event that fires when message transfer has begun.</value>
        public event EventHandler StartedMessageTransfer;

        /// <value>Event that fires when message transfer has ended.</value>
        public event EventHandler EndedMessageTransfer;

        internal void OnConnect(EventArgs e)
        {
            if (Connected != null)
                Connected(this, e);
        }

        internal void OnDisconnect(EventArgs e)
        {
            if (Disconnected != null)
                Disconnected(this, e);
        }

        internal void OnAuthenticated(EventArgs e)
        {
            if (Authenticated != null)
                Authenticated(this, e);
        }

        internal void OnStartedMessageTransfer(EventArgs e)
        {
            if (StartedMessageTransfer != null)
                StartedMessageTransfer(this, e);
        }

        internal void OnEndedMessageTransfer(EventArgs e)
        {
            if (EndedMessageTransfer != null)
                EndedMessageTransfer(this, e);
        }

        #endregion


        /// <summary>Default constructor</summary>
        /// <example>
        /// <code>
        /// 	Smtp smtp = new Smtp();
        /// 	smtp.Host = "mail.OpenSmtp.com";
        /// 	smtp.Port = 25;
        /// </code>
        /// </example>
        public SmtpClient()
        {
            SendTimeout = 50000;
            RecieveTimeout = 50000;
        }


        /// <summary>Constructor specifying a host and port</summary>
        /// <example>
        /// <code>
        /// 	Smtp smtp = new Smtp("mail.OpenSmtp.com", 25);
        /// </code>
        /// </example>
        public SmtpClient(string host) : this(host, 25)
        {
        }

        /// <summary>Constructor specifying a host and port</summary>
        /// <example>
        /// <code>
        /// 	Smtp smtp = new Smtp("mail.OpenSmtp.com", 25);
        /// </code>
        /// </example>
        public SmtpClient(string host, int port)
        {
            SendTimeout = 50000;
            RecieveTimeout = 50000;
            this.Host = host;
            this.Port = port;
        }


        #region METHODS
        /// <summary>Sends a mail message using supplied MailMessage properties as string params</summary>
        /// <param name="from">RFC 822 formatted email sender address</param>
        /// <param name="to">RFC 822 formatted email recipient address</param>
        /// <param name="subject">Subject of the email message</param>
        /// <param name="body">Text body of the email message</param>
        /// <example>
        /// <code>
        /// 	Smtp smtp = new Smtp("mail.OpenSmtp.com", 25);
        ///		smtp.SendMail("support@OpenSmtp.com", "recipient@OpenSmtp.com", "Hi", "Hello Joe Smith");
        /// </code>
        /// </example>
        public void SendMail(string from, string to, string subject, string body)
        {
            MailMessage msg = new MailMessage(from, to);
            msg.Subject = subject;
            msg.Body = body;

            SendMail(msg);
        }
        
        /// <summary>Sends a mail message using supplied MailMessage</summary>
        /// <param name="msg">MailMessage instance</param>
        /// <example>
        /// <code>
        ///		MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		msg.Subject = "Hi";
        ///		msg.Body = "Hello Joe Smith."
        /// 	Smtp smtp = new Smtp("mail.OpenSmtp.com", 25);
        ///		smtp.SendMail(msg);
        /// </code>
        /// </example>
        public void SendMail(MailMessage msg)
        {
            WriteToStream(string.Format("MAIL FROM: <{0}>\r\n", msg.From.Address));
            CheckForError(ReadFromStream(), ReplyConstants.OK);

            SendRecipientList(msg.To);
            SendRecipientList(msg.CC);
            SendRecipientList(msg.BCC);

            WriteToStream("DATA\r\n");
            CheckForError(ReadFromStream(), ReplyConstants.START_INPUT);


            OnStartedMessageTransfer(EventArgs.Empty);
            WriteToStream(string.Format("{0}\r\n.\r\n", msg));
            CheckForError(ReadFromStream(), ReplyConstants.OK);
            OnEndedMessageTransfer(EventArgs.Empty);

            WriteToStream( "QUIT\r\n");
            CheckForError(ReadFromStream(), ReplyConstants.QUIT);
        }

        private string ReadFromStream()
        {
            NetworkStream networkStream = _stream;
            return ReadFromStream(ref networkStream);
        }

        private void SendRecipientList(ArrayList arrayList)
        {
            NetworkStream networkStream = _stream;
            SendRecipientList(ref  networkStream,arrayList);
        }

        private void WriteToStream(string p)
        {
            NetworkStream networkStream = _stream;
            WriteToStream(ref  networkStream,p);
        }


        /// <summary>Resets the Smtp instance to it's defaut values; set in the SmtpConfig class</summary>
        /// <example>
        /// <code>
        /// 	Smtp smtp = new Smtp("mail.OpenSmtp.com", 25);
        ///		smtp.Reset();
        /// </code>
        /// </example>
        public void Reset()
        {
            Host = null;
            Port = 25;
            UserName = null;
            Password = null;

            Disconnect();
        }


        // --------------- Helper methods ------------------------------------
        /// <summary>
        /// GetConnection
        /// </summary>
        /// <returns></returns>
        public void Connect()
        {
           Connect(Host,Port);
        }
        public void Connect(string host, int port)
        {
            try
            {
                _tcpc = new TcpClient(Host, Port);

                _tcpc.ReceiveTimeout = RecieveTimeout;
                _tcpc.SendTimeout = SendTimeout;
                _tcpc.ReceiveBufferSize = ReceiveBufferSize;
                _tcpc.SendBufferSize = SendBufferSize;

                LingerOption lingerOption = new LingerOption(true, 10);
                _tcpc.LingerState = lingerOption;
            }
            catch (SocketException e)
            {
                throw new SmtpException(string.Format("Cannot connect to specified smtp host({0}:{1}).", Host, Port), e);
            }

            _stream = _tcpc.GetStream();

            CheckForError(ReadFromStream(ref _stream), ReplyConstants.HELO_REPLY);

            WriteToStream(ref _stream, string.Format("HELO {0}\r\n", System.Net.Dns.GetHostName()));

            CheckForError(ReadFromStream(ref _stream), ReplyConstants.OK);

            OnConnect(EventArgs.Empty);
        }
        /// <summary>
        /// CloseConnection
        /// </summary>
        private void Disconnect()
        {
            // fire disconnect event
            OnDisconnect(EventArgs.Empty);

            // destroy tcp connection if it hasn't already closed
            if (_tcpc != null)
            { _tcpc.Close(); }
        }

        /// <summary>
        /// SendRecipientList
        /// </summary>
        /// <param name="nwstream"></param>
        /// <param name="recipients"></param>
        private void SendRecipientList(ref NetworkStream nwstream, ArrayList recipients)
        {
            //	Iterate through all addresses and send them:
            for (IEnumerator i = recipients.GetEnumerator(); i.MoveNext(); )
            {
                MailAddress recipient = (MailAddress)i.Current;
                if (recipient != null)
                    WriteToStream(ref nwstream, string.Format("RCPT TO: <{0}>\r\n", recipient.Address));

                // potential 501 error (not valid sender, bad email address) below:
                CheckForError(ReadFromStream(ref nwstream), ReplyConstants.OK);
            }
        }
        /// <summary>
        /// CheckMailMessage
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected bool CheckMailMessage(MailMessage message)
        {
            const string returnMessage = "Mail Message is missing ";

            if (message.To == null || message.To.Count <= 0)
            {
                throw new SmtpException(string.Format("{0}'To:' field", returnMessage));
            }
            return true;
        }

        /// <summary>
        /// NetworkStream Helper methods
        /// </summary>
        /// <param name="nw"></param>
        /// <param name="line"></param>
        private void WriteToStream(ref NetworkStream nw, string line)
        {
            try
            {
                byte[] arrToSend = Encoding.ASCII.GetBytes(line);
                nw.Write(arrToSend, 0, arrToSend.Length);
                //Console.WriteLine("[client]:" + line);
            }
            catch (System.Exception)
            {
                throw new SmtpException("Write to Stream threw an System.Exception");
            }
        }

        private string ReadFromStream(ref NetworkStream nw)
        {
            try
            {

                byte[] readBuffer = new byte[4096];

                int length = nw.Read(readBuffer, 0, readBuffer.Length);
                string returnMsg = Encoding.ASCII.GetString(readBuffer, 0, length);

                return returnMsg;
            }
            catch (System.Exception e)
            {
                throw new SmtpException(string.Format("Read from Stream threw an System.Exception: {0}", e));
            }
        }

        /// <summary>
        /// Checks stream returned from SMTP server for success code
        /// If the success code is not present it will throw an error.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="successCode"></param>
        protected void CheckForError(string s, string successCode)
        {
            if (s.IndexOf(successCode, System.StringComparison.Ordinal) == -1)
                throw new SmtpException(string.Format("ERROR - Expecting: {0}. Recieved: {1}", successCode, s));
        }

        /// <summary>
        /// Check to see if the command sent returns a Unknown Command Error
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected bool IsUnknownCommand(string s)
        {
            return s.IndexOf(ReplyConstants.UNKNOWN, System.StringComparison.Ordinal) != -1;
        }

        /// <summary>
        /// Check to see if AUTH command returns valid challenge
        /// A valid AUTH string must be passed into this method.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected bool AuthImplemented(string s)
        {
            return s.IndexOf(ReplyConstants.SERVER_CHALLENGE, System.StringComparison.Ordinal) != -1;
        }

        #endregion

        public void Dispose()
        {
            Disconnect();
        }
        /// <summary>
        /// AuthLogin
        /// </summary>
        /// <returns></returns>
        public void Authenticate()
        {
            Authenticate(UserName, Password);
        }
        /// <summary>
        /// Authentication is used if the u/p are supplied
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void Authenticate(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;

            WriteToStream(ref _stream, "AUTH LOGIN\r\n");
            if (AuthImplemented(ReadFromStream()))
            {
                WriteToStream(string.Format("{0}\r\n",Convert.ToBase64String(Encoding.ASCII.GetBytes(this.UserName.ToCharArray()))));

                CheckForError(ReadFromStream(), ReplyConstants.SERVER_CHALLENGE);

                WriteToStream(string.Format("{0}\r\n",Convert.ToBase64String(Encoding.ASCII.GetBytes(this.Password.ToCharArray()))));
                CheckForError(ReadFromStream(), ReplyConstants.AUTH_SUCCESSFUL);

                OnAuthenticated(EventArgs.Empty);
            }
        }

    }

}