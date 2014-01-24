using System;
using System.IO;
using Microsoft.Win32;
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
namespace ThinkAway.Net.Mail.SMTP {


    /// <summary>
	/// This Type stores a file attachment. There can be many attachments in each MailMessage
	/// <seealso cref="MailMessage"/>
	/// </summary>
	/// <example>
	/// <code>
	/// MailMessage msg = new MailMessage();
	/// Attachment a = new Attachment("C:\\file.jpg");
	/// msg.AddAttachment(a);
	/// </code>
	/// </example>
    public class Attachment : IComparable
	{
        private string _encoding = "base64";

        private string _name;

        /// <summary>
        /// Stores the Attachment Name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _contentId;

        /// <summary>
        /// Stores the MIME ContentId of the attachment
        /// </summary>
        public string ContentId
        {
            get { return _contentId; }
            set { _contentId = value; }
        }

        private string _mimeType;

        /// <summary>
        /// Stores the MIME content-type of the attachment
        /// </summary>
        public string MimeType
        {
            get { return _mimeType; }
            set { _mimeType = value; }
        }

        /// <summary>
        /// Returns the MIME content-encoding type of the attachment
		/// </summary>
		public string Encoding
		{
			get { return(_encoding); }
            set { _encoding = value; }
		}

        private string _filePath;

        /// <summary>
        /// Returns the path of an attached file
		/// </summary>
        public string FilePath
        {
            get { return _filePath; }
            private set { _filePath = value; }
        }

        private int _size;

        /// <summary>
        /// Returns the attachment size in bytes
        /// </summary>
        public int Size
        {
            get { return _size; }
            private set { _size = value; }
        }

        private string _encodedFilePath;

        /// <summary>
        /// When the file is encoded it is stored in temp directory until sendMail() method is called.
        /// This property retrieves the path to that temp file.
        /// </summary>
        internal string EncodedFilePath
        {
            get { return _encodedFilePath; }
            private set { _encodedFilePath = value; }
        }


        /// <summary>Constructor using a file path</summary>
        /// <example>
        /// <code>Attachment a = new Attachment("C:\\file.jpg");</code>
        /// </example>
        public Attachment(string filePath)
        {
            FilePath = filePath;
            if(File.Exists(filePath))
            {
                this.MimeType = GetMimeType(Path.GetExtension(filePath));
                FileStream fileStream = new FileStream(filePath,FileMode.Open);
                InitAttachment(fileStream, Path.GetFileName(filePath));
            }
            else
            {
                throw new SmtpException("Attachment file does not exist or path is incorrect.");
            }
        }

        /// <summary>Constructor using a provided Stream</summary>
        /// <example>
        /// <code>Attachment a = new Attachment(new FileStrema(@"C:\file.jpg", FileMode.Open, FileAccess.Read), "file.jpg");</code>
        /// </example>
        public Attachment(Stream stream, string fileName)
        {
            InitAttachment(stream, fileName);
        }

        private void InitAttachment(Stream stream, string fileName)
        {
            try
            {
                this.MimeType = "unknown/application";
                this.Name = fileName;
                Size = (int) stream.Length;

                string encodedTempFile = Path.GetTempFileName();
                MailEncoder.ConvertToBase64(stream, encodedTempFile);
                EncodedFilePath = encodedTempFile;
            }
            catch (Exception exception)
            {
                throw new SmtpException("Attachment file does not exist or path is incorrect.", exception);
            }
        }


        ~Attachment()
        {
            // delete temp file used for temp encoding of large files
            try
            {
                if (this.EncodedFilePath != null)
                {
                    FileInfo fileInfo = new FileInfo(this.EncodedFilePath);
                    if (fileInfo.Exists) { fileInfo.Delete(); }
                }
            }
            catch (ArgumentNullException)
            { }
        }

        /// <summary>Returns the MIME content-type for the supplied file extension</summary>
		/// <returns>String MIME type (Example: \"text/plain\")</returns>
		private string GetMimeType(string fileExtension)
		{
		    try
            {
                RegistryKey extKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtension);
                if (extKey != null)
                {
                    string contentType = (string)extKey.GetValue("Content Type");

                    return contentType;
                }
            }
            catch (System.Exception)
            {
                return "application/octet-stream";
            }
		    return null;
		}
		
		private string Line(string str)
		{
			return string.Format("{0}{1}",str,Environment.NewLine);
		}

        public int CompareTo(object attachment)        
		{ 
			// Order instances based on the Date         
			return (this.Name.CompareTo(((Attachment)(attachment)).Name));        
		}
		/// <summary>
		/// Encode the file for inclusion as a mime attachment.
		/// The boundaries are not provided.
		/// </summary>
		/// <returns></returns>

        public String ToMime()
		{
		    StringBuilder sb = new StringBuilder();
		    if (ContentId != null)
		    {
		    	sb.AppendFormat(Line("Content-ID: <{0}>"), ContentId);
		    }
		    sb.AppendFormat(Line("Content-Type: {0};"), MimeType);
		    sb.AppendFormat(Line(" name=\"{0}\""), MailEncoder.ConvertToQP(Name));
		    sb.AppendFormat(Line("Content-Transfer-Encoding: {0}"), Encoding);
		    sb.AppendFormat(Line("Content-Disposition: attachment;"));
		    sb.AppendFormat(Line(Line(" filename=\"{0}\"")), MailEncoder.ConvertToQP(Name));

		    FileStream fin = new FileStream(EncodedFilePath, FileMode.Open, FileAccess.Read);

		    while (fin.Position != fin.Length)
		    {
		        byte[] bin = new byte[76];
		        int len = fin.Read(bin, 0, 76);
		        sb.AppendFormat(Line("{0}"), System.Text.Encoding.UTF8.GetString(bin, 0, len));
		    }

		    fin.Close();
		    return sb.ToString();
		}

	}
}
