using System;
using System.Collections;
using System.IO;
using System.Text;
using ThinkAway.Net.Mail.Exceptions;
using ThinkAway.Text.MIME.Encode;
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
    /// This Type stores the addresses, attachments, body, subject,
    /// and properties of the email message. There can be many
    /// attachments and email addresses in each MailMessage.
    /// <seealso cref="MailAddress"/>
    /// <seealso cref="Attachment"/>
    /// <example>
    /// <code>
    /// MailAddress from = new MailAddress("Lsong","song940@163.com");
	///	MailAddress to	 = new MailAddress("admin@lsong.org");
	///	MailAddress cc	 = new MailAddress("Test<cc@lsong.org>");
	///       
	/// MailMessage mailMessage = new MailMessage(from,to);
	/// mailMessage.AddRecipient(cc,AddressType.Cc);
	/// mailMessage.AddRecipient("test@lsong.org",AddressType.Bcc); 
	///                  
	///       
	/// mailMessage.Charset = "UTF-8";
	/// mailMessage.Priority = MailPriority.High;
	/// mailMessage.Notification = true;
	///       
	/// mailMessage.AddCustomHeader("X-CustomHeader", "Value");
	/// mailMessage.AddCustomHeader("X-CompanyName", "Value"); 
	/// 
	/// string testCid =  mailMessage.AddImage("C:\\test.bmp");
	///       
	///	mailMessage.AddAttachment("C:\\test.zip");
	///       
	///	mailMessage.Subject = "This's a test Mail.";
	/// mailMessage.Body = "hello everybody .";
	/// mailMessage.HtmlBody = "<html><body>hello everybody .<br /><img src='cid:"+ testCid +"' /></body></html>"; 
    /// </code>
    /// </example>
    ///  </summary>
    public class MailMessage
    {
        private MailAddress _from;
        private MailAddress _replyTo;  
              
        private ArrayList _ccList;
        private ArrayList _bccList;
        private readonly ArrayList _images;
        
        private ArrayList _attachments;
        private ArrayList _recipientList;
        
        private ArrayList _customHeaders;
        
        private string _subject;
        private string _body;
        private string _htmlBody;
        private string _mixedBoundary;
        private string _altBoundary;
        private string _priority;
        private string _charset = "ISO-8859-1";
        
        private bool _notification;        
        
        private readonly string _relatedBoundary;

        public MailMessage()
        {
            _recipientList = new ArrayList();
            _ccList = new ArrayList();
            _bccList = new ArrayList();
            _attachments = new ArrayList();
            _images = new ArrayList();
            _customHeaders = new ArrayList();
            _mixedBoundary = GenerateMixedMimeBoundary();
            _altBoundary = GenerateAltMimeBoundary();
            _relatedBoundary = GenerateRelatedMimeBoundary();
        }

        /// <summary>Constructor using EmailAddress type</summary>
        /// <example>
        /// <code>
        /// 	EmailAddress from 	= new EmailAddress("support@OpenSmtp.com", "Support");
        /// 	EmailAddress to 	= new EmailAddress("recipient@OpenSmtp.com", "Joe Smith");
        /// 	MailMessage msg 	= new MailMessage(from, to);
        /// </code>
        /// </example>
        public MailMessage(MailAddress sender, MailAddress recipient)
            : this()
        {
            _from = sender;
            _recipientList.Add(recipient);
        }

        /// <summary>Constructor using string email addresses</summary>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        /// </code>
        /// </example>
        public MailMessage(string senderAddress, string recipientAddress)
            : this(new MailAddress(senderAddress), new MailAddress(recipientAddress))
        { }


        // -------------------------- Properties --------------------------

        /// <value>Stores the EmailAddress to Reply-To.
        /// If no EmailAddress is specified the From address is used.</value>
        public MailAddress ReplyTo
        {
            get { return _replyTo ?? _from; }
            set { _replyTo = value; }
        }

        /// <value>Stores the EmailAddress of the sender</value>
        public MailAddress From
        {
            get { return _from; }
            set { _from = value; }
        }

        /// <value>Stores the EmailAddress of the recipient</value>
        public ArrayList To
        {
            get { return _recipientList; }
            set { _recipientList = value; }
        }

        /// <value>Stores the subject of the MailMessage</value>
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        /// <value>Stores the text body of the MailMessage</value>
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        /// <value>Stores the HTML body of the MailMessage</value>
        public string HtmlBody
        {
            get { return _htmlBody; }
            set { _htmlBody = value; }
        }

        /// <value>Stores Mail Priority value</value>
        /// <seealso>MailPriority</seealso>
        public string Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }


        /// <value>Stores the Read Confirmation Reciept</value>
        public bool Notification
        {
            get { return _notification; }
            set { _notification = value; }
        }

        /// <value>Stores an ArrayList of CC EmailAddresses</value>
        public ArrayList CC
        {
            get { return _ccList; }
            set { _ccList = value; }
        }

        /// <value>Stores an ArrayList of BCC EmailAddresses</value>
        public ArrayList BCC
        {
            get { return _bccList; }
            set { _bccList = value; }
        }

        /// <value>Stores the character set of the MailMessage</value>
        public string Charset
        {
            get { return _charset; }
            set { _charset = value; }
        }

        /// <value>Stores a list of Attachments</value>
        public ArrayList Attachments
        {
            get { return _attachments; }
            set { _attachments = value; }
        }

        /// <value>Stores a NameValueCollection of custom headers</value>
        public ArrayList CustomHeaders
        {
            get { return _customHeaders; }
            set { _customHeaders = value; }
        }

        /// <value>Stores the string boundary used between MIME parts</value>
        internal string AltBoundary
        {
            get { return _altBoundary; }
            set { _altBoundary = value; }
        }

        /// <value>Stores the string boundary used between MIME parts</value>
        internal string MixedBoundary
        {
            get { return _mixedBoundary; }
            set { _mixedBoundary = value; }
        }
        
        
        /// <summary>
        /// Generate a content id
        /// </summary>
        /// <returns></returns>
        private string NewCid()
        {
            int attachmentid = _attachments.Count + _images.Count + 1;
            return string.Format("att{0}", attachmentid);
        }

        /// <summary>Adds a recipient EmailAddress to the MailMessage</summary>
        /// <param name="address">EmailAddress that you want to add to the MailMessage</param>
        /// <param name="type">AddressType of the address</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		MailAddress cc = new MailAddress("cc@OpenSmtp.com");
        ///		msg.AddRecipient(cc, AddressType.Cc);
        /// </code>
        /// </example>
        public void AddRecipient(MailAddress address, AddressType type)
        {
            try
            {
                switch (type)
                {
                    case AddressType.To:
                        To.Add(address);
                        break;
                    case AddressType.Cc:
                        CC.Add(address);
                        break;
                    case AddressType.Bcc:
                        BCC.Add(address);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new SmtpException(string.Format("Exception in AddRecipient: {0}", e));
            }
        }

        /// <summary>Adds a recipient RFC 822 formatted email address to the MailMessage</summary>
        /// <param name="address">RFC 822 formatted email address that you want to add to the MailMessage</param>
        /// <param name="type">AddressType of the email address</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		msg.AddRecipient("cc@OpenSmtp.com", AddressType.Cc);
        /// </code>
        /// </example>
        public void AddRecipient(string address, AddressType type)
        {
            MailAddress email = new MailAddress(address);
            AddRecipient(email, type);
        }        

        /// <summary>Adds an included image to the MailMessage using a file path</summary>
        /// <param name="filepath">File path to the file you want to attach to the MailMessage</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		msg.AddImage(@"C:\file.jpg");
        /// </code>
        /// </example>
        // start added/modified by mb
        public string AddImage(string filepath)
        {
        	string cid = NewCid();
            AddImage(filepath, cid);
            return cid;
        }

        public string AddImage(string filepath, string cid)
        {
            if (filepath != null)
            {
                Attachment image = new Attachment(filepath);
                image.ContentId = cid;
                _images.Add(image);
            }
            return cid;
        }
        
        /// <summary>Adds an Attachment to the MailMessage using a file path</summary>
        /// <param name="filepath">File path to the file you want to attach to the MailMessage</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		msg.AddAttachment(@"C:\file.jpg");
        /// </code>
        /// </example>
        // start added/modified by mb
        public void AddAttachment(string filepath)
        {
            AddAttachment(filepath, NewCid());
        }

        public void AddAttachment(string filepath, string cid)
        {
            if (filepath != null)
            {
                Attachment attachment = new Attachment(filepath);
                attachment.ContentId = cid;
                Attachments.Add(attachment);
            }
        }
        // end added by mb

        /// <summary>Adds an Attachment to the MailMessage using an Attachment instance</summary>
        /// <param name="attachment">Attachment you want to attach to the MailMessage</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		Attachment att = new Attachment(@"C:\file.jpg");
        ///		msg.AddAttachment(att);
        /// </code>
        /// </example>
        public void AddAttachment(Attachment attachment)
        {
            if (attachment != null)
            {
                Attachments.Add(attachment);
            }
        }

        /// <summary>Adds an Attachment to the MailMessage using a provided Stream</summary>
        /// <param name="stream">stream you want to attach to the MailMessage</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		Attachment att = new Attachment(new FileStream(@"C:\File.jpg", FileMode.Open, FileAccess.Read), "Test Name");
        ///		msg.AddAttachment(att);
        /// </code>
        /// </example>
        public void AddAttachment(Stream stream)
        {
            if (stream != null)
            {
                Attachments.Add(stream);
            }
        }


        /// <summary>
        /// Adds an custom header to the MailMessage.
        ///	NOTE: some SMTP servers will reject improper custom headers
        ///</summary>
        /// <param name="name">Name of the custom header</param>
        /// <param name="body">Value of the custom header</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		msg.AddCustomHeader("X-Something", "HeaderValue");
        /// </code>
        /// </example>
        public void AddCustomHeader(string name, string body)
        {
            if (name != null && body != null)
            {
                AddCustomHeader(new MailHeader(name, body));
            }
        }

        /// <summary>
        /// Adds an custom header to the MailMessage.
        ///	NOTE: some SMTP servers will reject improper custom headers
        ///</summary>
        /// <param name="mailheader">MailHeader instance</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		MailHeader header = new MailHeader("X-Something", "HeaderValue");		
        ///		msg.AddCustomHeader(header);
        /// </code>
        /// </example>
        public void AddCustomHeader(MailHeader mailheader)
        {
            if (mailheader.Name != null && mailheader.Body != null)
            {
                CustomHeaders.Add(mailheader);
            }
        }

        /// <summary>Populates the Body property of the MailMessage from a text file</summary>
        /// <param name="filePath">Path to the file containing the MailMessage body</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		msg.GetBodyFromFile(@"C:\body.txt");
        /// </code>
        /// </example>
        public void GetBodyFromFile(string filePath)
        {
            this.Body = GetFileAsString(filePath);
        }

        /// <summary>Populates the HtmlBody property of the MailMessage from a HTML file</summary>
        /// <param name="filePath">Path to the file containing the MailMessage html body</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		msg.GetHtmlBodyFromFile(@"C:\htmlbody.html");
        /// </code>
        /// </example>
        public void GetHtmlBodyFromFile(string filePath)
        {
            // add extension
            this.HtmlBody = GetFileAsString(filePath);
        }

        /// <summary>Resets all of the MailMessage properties</summary>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		msg.Reset();
        /// </code>
        /// </example>
        public void Reset()
        {
            _from = null;
            _replyTo = null;
            _recipientList.Clear();
            _ccList.Clear();
            _bccList.Clear();
            _customHeaders.Clear();
            _attachments.Clear();
            _subject = null;
            _body = null;
            _htmlBody = null;
            _priority = null;
            _mixedBoundary = null;
            _altBoundary = null;
            _charset = null;
            _notification = false;
        }

        /// <summary>Saves the MailMessage as a RFC 822 formatted message</summary>
        /// <param name="filePath">Specifies the file path to save the message</param>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", recipient@OpenSmtp.com");
        ///		msg.Body = "body";
        ///		msg.Subject = "subject";
        ///		msg.Save(@"C:\email.txt");
        /// </code>
        /// </example>
        public void Save(string filePath)
        {
            StreamWriter sw = File.CreateText(filePath);
            sw.Write(this.ToString());
            sw.Flush();
            sw.Close();
        }

        /// <summary>Returns the MailMessage as a RFC 822 formatted message</summary>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		msg.Body = "body";
        ///		msg.Subject = "subject";
        ///		string message = msg.ToString();
        /// </code>
        /// </example>
        /// <returns>Mailmessage as RFC 822 formatted string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(ReplyTo.DisplayName))
            {
                sb.AppendFormat("Reply-To: \"{0}\" <{1}>\r\n", MailEncoder.ConvertHeaderToQP(ReplyTo.DisplayName, Charset), ReplyTo.Address);
            }
            else
            {
                sb.AppendFormat("Reply-To: <{0}>\r\n", ReplyTo.Address);
            }

            if (!string.IsNullOrEmpty(From.DisplayName))
            {
                sb.AppendFormat("From: \"{0}\" <{1}>\r\n", MailEncoder.ConvertHeaderToQP(From.DisplayName, Charset), From.Address);
            }
            else
            {
                sb.AppendFormat("From: <{0}>\r\n", From.Address);
            }

            sb.AppendFormat("To: {0}\r\n", CreateAddressList(To));

            if (_ccList.Count != 0)
            {
                sb.AppendFormat("CC: {0}\r\n", CreateAddressList(CC));
            }

            if (!string.IsNullOrEmpty(Subject))
            {
                StringBuilder cleanSubject = new StringBuilder(Subject);
                cleanSubject.Replace("\r\n", null);
                cleanSubject.Replace("\n", null);

                sb.AppendFormat("Subject: {0}\r\n", MailEncoder.ConvertHeaderToQP(cleanSubject.ToString(), Charset));
            }

            sb.AppendFormat("Date: {0}\r\n", DateTime.Now.ToUniversalTime().ToString("R"));
            sb.AppendFormat("{0}\r\n", X_MAILER_HEADER);

            if (Notification)
            {
                if (string.IsNullOrEmpty(ReplyTo.DisplayName))
                {
                    sb.AppendFormat("Disposition-Notification-To: <{0}>\r\n", ReplyTo.Address);
                }
                else
                {
                    sb.AppendFormat("Disposition-Notification-To: {0} <{1}>\r\n", MailEncoder.ConvertHeaderToQP(ReplyTo.DisplayName, Charset), ReplyTo.Address);
                }
            }

            if (Priority != null)
            {
                sb.AppendFormat("X-Priority: {0}\r\n", Priority);
            }

            if (CustomHeaders != null)
            {
                for (IEnumerator i = _customHeaders.GetEnumerator(); i.MoveNext(); )
                {
                    MailHeader mailHeader = (MailHeader)i.Current;

                    if (mailHeader != null)
                    {
                        if (mailHeader.Name.Length >= 0 && mailHeader.Body.Length >= 0)
                        {
                            sb.AppendFormat("{0}:{1}\r\n", mailHeader.Name, MailEncoder.ConvertHeaderToQP(mailHeader.Body, Charset));
                        }
                        else
                        {
                            // TODO: Check if below is within RFC spec.
                            sb.AppendFormat("{0}:\r\n", mailHeader.Name);
                        }
                    }

                }
            }

            sb.Append("MIME-Version: 1.0\r\n");
            sb.Append(GetMessageBody());

            return sb.ToString();
        }

        /// <summary>Returns a clone of this message</summary>
        /// <example>
        /// <code>
        /// 	MailMessage msg = new MailMessage("support@OpenSmtp.com", "recipient@OpenSmtp.com");
        ///		msg.Body = "body";
        ///		msg.Subject = "subject";
        ///
        ///		msg2 = msg.Copy();
        /// </code>
        /// </example>
        /// <returns>Mailmessage</returns>
        public MailMessage Copy()
        {
            return (MailMessage)this.MemberwiseClone();
        }

        /// Internal/Private methods below
        // ------------------------------------------------------

        private string GetFileAsString(string filePath)
        {
            FileStream fin = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] bin = new byte[fin.Length];
            long rdlen = 0;

            StringBuilder sb = new StringBuilder();

            while (rdlen < fin.Length)
            {
                int len = fin.Read(bin, 0, (int)fin.Length);
                sb.Append(Encoding.UTF7.GetString(bin, 0, len));
                rdlen = (rdlen + len);
            }

            fin.Close();
            return sb.ToString();
        }


        /// <summary>
        /// Determines the format of the message and adds the
        /// appropriate mime containers.
        /// 
        /// This will return the html and/or text and/or 
        /// embedded images and/or attachments.
        /// </summary>
        /// <returns></returns>
        private String GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();

            if (_attachments.Count > 0)
            {
                sb.AppendFormat("Content-Type: multipart/mixed;");
                sb.AppendFormat("boundary=\"{0}\"", MixedBoundary);
                sb.AppendFormat("\r\n\r\nThis is a multi-part message in MIME format.");
                sb.AppendFormat("\r\n\r\n--{0}\r\n", MixedBoundary);
            }

            sb.Append(GetInnerMessageBody());

            if (_attachments.Count > 0)
            {
                foreach (Attachment attachment in _attachments)
                {
                    sb.AppendFormat("\r\n\r\n--{0}\r\n", MixedBoundary);
                    sb.AppendFormat(attachment.ToMime());
                }
                sb.AppendFormat("\r\n\r\n--{0}--\r\n", MixedBoundary);
            }
            return sb.ToString();

        }

        /// <summary>
        /// Get the html and/or text and/or images.
        /// </summary>
        /// <returns></returns>
        private string GetInnerMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            if (_images.Count > 0)
            {
                sb.AppendFormat("Content-Type: multipart/related;");
                sb.AppendFormat("boundary=\"{0}\"", _relatedBoundary);
                sb.AppendFormat("\r\n\r\n--{0}\r\n", _relatedBoundary);
            }

            sb.Append(GetReadableMessageBody());

            if (_images.Count > 0)
            {
                foreach (Attachment image in _images)
                {
                    sb.AppendFormat("\r\n\r\n--{0}\r\n", _relatedBoundary);
                    sb.AppendFormat(image.ToMime());
                }
                sb.AppendFormat("\r\n\r\n--{0}--\r\n", _relatedBoundary);
            }
            return sb.ToString();
        }

        private String GetReadableMessageBody()
        {

            StringBuilder sb = new StringBuilder();

            if (HtmlBody == null)
            {
                sb.Append(GetTextMessageBody(Body, "text/plain"));
            }
            else if (Body == null)
            {
                sb.Append(GetTextMessageBody(HtmlBody, "text/html"));
            }
            else
            {
                sb.Append(GetAltMessageBody(
                    GetTextMessageBody(Body, "text/plain"),
                    GetTextMessageBody(HtmlBody, "text/html")));
            }
            return sb.ToString();

        }


        private string GetTextMessageBody(string messageBody, string textType)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Content-Type: {0};\r\n", textType);
            sb.AppendFormat(" charset=\"{0}\"\r\n", Charset);
            sb.AppendFormat("Content-Transfer-Encoding: quoted-printable\r\n\r\n");
            sb.AppendFormat(MailEncoder.ConvertToQP(messageBody, Charset));

            return sb.ToString();
        }

        private string GetAltMessageBody(string messageBody1, string messageBody2)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Content-Type: multipart/alternative;");
            sb.AppendFormat("boundary=\"{0}\"", AltBoundary);

            sb.AppendFormat("\r\n\r\nThis is a multi-part message in MIME format.");

            sb.AppendFormat("\r\n\r\n--{0}\r\n", AltBoundary);
            sb.AppendFormat(messageBody1);
            sb.AppendFormat("\r\n\r\n--{0}\r\n", AltBoundary);
            sb.AppendFormat(messageBody2);
            sb.AppendFormat("\r\n\r\n--{0}--\r\n", AltBoundary);

            return sb.ToString();
        }


        // creates comma separated address list from to: and cc:
        private string CreateAddressList(ArrayList msgList)
        {
            StringBuilder sb = new StringBuilder();

            int index = 1;
            int msgCount = msgList.Count;

            for (IEnumerator i = msgList.GetEnumerator(); i.MoveNext(); index++)
            {
                MailAddress a = (MailAddress)i.Current;

                // if the personal name exists, add it to the address sent. IE: "Ian Stallings" <istallings@mail.com>
                if (a != null)
                {
                    if (!string.IsNullOrEmpty(a.DisplayName))
                    {
                        sb.AppendFormat("\"{0}\" <{1}>", MailEncoder.ConvertHeaderToQP(a.DisplayName, _charset), a.Address);
                    }
                    else
                    {
                        sb.AppendFormat("<{0}>", a.Address);
                    }

                    // if it's not the last address add a semi-colon:
                    if (msgCount != index)
                    {
                        sb.Append(",");
                    }
                }
            }

            return sb.ToString();
        }

	    private static string GenerateMixedMimeBoundary()
        {
            // Below generates uniqe boundary for each message. These are used to separate mime parts in a message.
            return string.Format("Part.{0}.{1}", Convert.ToString(new Random(unchecked((int)DateTime.Now.Ticks)).Next()), Convert.ToString(new Random(~unchecked((int)DateTime.Now.Ticks)).Next()));
        }

        private static string GenerateAltMimeBoundary()
        {
            // Below generates uniqe boundary for each message. These are used to separate mime parts in a message.
            return string.Format("Part.{0}.{1}", Convert.ToString(new Random(~unchecked((int)DateTime.Now.AddDays(1).Ticks)).Next()), Convert.ToString(new Random(unchecked((int)DateTime.Now.AddDays(1).Ticks)).Next()));
        }

        private static string GenerateRelatedMimeBoundary()
        {
            // Below generates uniqe boundary for each message. These are used to separate mime parts in a message.
            return string.Format("Part.{0}.{1}", Convert.ToString(new Random(~unchecked((int)DateTime.Now.AddDays(2).Ticks)).Next()), Convert.ToString(new Random(unchecked((int)DateTime.Now.AddDays(1).Ticks)).Next()));
        }


        internal const string X_MAILER_HEADER = "X-Mailer: mail.lsong.org";
    }
}
